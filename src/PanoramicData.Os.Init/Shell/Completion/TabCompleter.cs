using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Completion;

/// <summary>
/// Provides tab completion for file and directory paths.
/// </summary>
/// <remarks>
/// Create a new tab completer.
/// </remarks>
/// <param name="pathProvider">Path provider for file system access.</param>
/// <param name="commandProvider">Optional command specification provider for context-aware completion.</param>
public class TabCompleter(IPathProvider pathProvider, ICommandSpecificationProvider? commandProvider = null)
{

	// Completion state for rotation
	private string _lastCompletionPrefix = string.Empty;
	private int _lastCursorPosition = -1;
	private List<string> _completions = [];
	private int _completionIndex = -1;
	private string _originalText = string.Empty;
	private int _originalCursorPosition;
	private CompletionType _lastCompletionType;

	/// <summary>
	/// Result of a tab completion attempt.
	/// </summary>
	public readonly record struct CompletionResult
	{
		/// <summary>
		/// The new line text after completion.
		/// </summary>
		public string NewText { get; init; }

		/// <summary>
		/// The new cursor position.
		/// </summary>
		public int NewCursorPosition { get; init; }

		/// <summary>
		/// Whether a completion was applied.
		/// </summary>
		public bool Applied { get; init; }

		/// <summary>
		/// All available completions (for display if multiple).
		/// </summary>
		public IReadOnlyList<string> AllCompletions { get; init; }
	}

	/// <summary>
	/// Attempt tab completion on the current line.
	/// </summary>
	/// <param name="currentLine">The current line text.</param>
	/// <param name="cursorPosition">The cursor position.</param>
	/// <returns>The completion result.</returns>
	public CompletionResult Complete(string currentLine, int cursorPosition)
	{
		// Extract the word being typed at cursor
		var (wordStart, word) = ExtractWordAtCursor(currentLine, cursorPosition);

		// Check if this is a continuation of the previous completion (rotation)
		if (IsRotation(currentLine, cursorPosition))
		{
			return RotateCompletion(currentLine, wordStart);
		}

		// Start a new completion
		_originalText = currentLine;
		_originalCursorPosition = cursorPosition;
		_lastCompletionPrefix = word;
		_lastCursorPosition = cursorPosition;

		// Determine the completion type based on command context
		_lastCompletionType = DetermineCompletionType(currentLine, wordStart);

		// Get completions
		_completions = GetCompletions(currentLine, wordStart, word, _lastCompletionType);

		if (_completions.Count == 0)
		{
			_completionIndex = -1;
			return new CompletionResult
			{
				NewText = currentLine,
				NewCursorPosition = cursorPosition,
				Applied = false,
				AllCompletions = []
			};
		}

		// Apply first completion
		_completionIndex = 0;
		return ApplyCompletion(currentLine, wordStart, _completions[0]);
	}

	/// <summary>
	/// Reset the completion state.
	/// </summary>
	public void Reset()
	{
		_lastCompletionPrefix = string.Empty;
		_lastCursorPosition = -1;
		_completions = [];
		_completionIndex = -1;
		_originalText = string.Empty;
	}

	/// <summary>
	/// Check if the current tab is a rotation of existing completions.
	/// </summary>
	private bool IsRotation(string currentLine, int cursorPosition)
	{
		// If we have active completions and the line matches what we expect after previous completion
		if (_completions.Count > 1 && _completionIndex >= 0)
		{
			// The line should match the last applied completion
			var (wordStart, _) = ExtractWordAtCursor(_originalText, _originalCursorPosition);
			var expectedLine = ApplyCompletionText(_originalText, wordStart, _completions[_completionIndex]);
			return currentLine == expectedLine;
		}

		return false;
	}

	/// <summary>
	/// Rotate to the next completion.
	/// </summary>
	private CompletionResult RotateCompletion(string currentLine, int wordStart)
	{
		_completionIndex = (_completionIndex + 1) % _completions.Count;

		var (originalWordStart, _) = ExtractWordAtCursor(_originalText, _originalCursorPosition);
		return ApplyCompletion(_originalText, originalWordStart, _completions[_completionIndex]);
	}

	/// <summary>
	/// Extract the word at the cursor position.
	/// </summary>
	private static (int wordStart, string word) ExtractWordAtCursor(string line, int cursorPosition)
	{
		if (string.IsNullOrEmpty(line) || cursorPosition == 0)
		{
			return (0, string.Empty);
		}

		// Find word boundaries - work backwards from cursor
		var wordStart = cursorPosition;
		while (wordStart > 0 && !IsWordBoundary(line[wordStart - 1]))
		{
			wordStart--;
		}

		var word = line[wordStart..cursorPosition];
		return (wordStart, word);
	}

	/// <summary>
	/// Check if a character is a word boundary.
	/// </summary>
	private static bool IsWordBoundary(char c)
	{
		return char.IsWhiteSpace(c) || c == '|' || c == '>' || c == '<' || c == ';' || c == '&';
	}

	/// <summary>
	/// Get completions for the current word.
	/// </summary>
	private List<string> GetCompletions(string line, int wordStart, string word, CompletionType completionType)
	{
		var completions = new List<string>();

		// Determine if this is a command position (first word or after pipe)
		var isCommandPosition = IsCommandPosition(line, wordStart);

		if (isCommandPosition && commandProvider != null)
		{
			// Could add command completion here in the future
			// For now, just do path completion
		}

		// Path completion based on completion type
		completions.AddRange(GetPathCompletions(word, completionType));

		return completions;
	}

	/// <summary>
	/// Determine the completion type based on command context.
	/// </summary>
	private CompletionType DetermineCompletionType(string line, int wordStart)
	{
		if (commandProvider == null)
		{
			return CompletionType.Any;
		}

		// Parse the line to find the command and current argument position
		var tokens = TokenizeLine(line[..wordStart]);
		if (tokens.Count == 0)
		{
			return CompletionType.Command;
		}

		var commandName = tokens[0];
		var spec = commandProvider.GetSpecification(commandName);
		if (spec == null)
		{
			return CompletionType.Any;
		}

		var positionalIndex = CountPositionalArguments(tokens);
		return GetCompletionTypeForPosition(spec, positionalIndex);
	}

	/// <summary>
	/// Count the number of positional (non-flag) arguments after the command.
	/// </summary>
	private static int CountPositionalArguments(List<string> tokens)
	{
		var count = 0;
		for (var i = 1; i < tokens.Count; i++)
		{
			if (!tokens[i].StartsWith('-'))
			{
				count++;
			}
		}

		return count;
	}

	/// <summary>
	/// Get the completion type for a specific positional argument index.
	/// </summary>
	private static CompletionType GetCompletionTypeForPosition(ShellCommandSpecification spec, int positionalIndex)
	{
		var positionalOptions = spec.Options?
			.Where(o => o.IsPositional)
			.OrderBy(o => o.Position)
			.ToList();

		if (positionalOptions == null || positionalIndex >= positionalOptions.Count)
		{
			return CompletionType.Any;
		}

		var optionSpec = positionalOptions[positionalIndex];
		return OptionSpecToCompletionType(optionSpec);
	}

	/// <summary>
	/// Convert an option spec to its corresponding completion type.
	/// </summary>
	private static CompletionType OptionSpecToCompletionType(OptionSpec optionSpec) => optionSpec switch
	{
		DirectoryOptionSpec => CompletionType.DirectoryOnly,
		FileOptionSpec => CompletionType.FileOnly,
		_ => CompletionType.Any
	};

	/// <summary>
	/// Tokenize a line into words for analysis.
	/// </summary>
	private static List<string> TokenizeLine(string line)
	{
		var tokens = new List<string>();
		var current = new System.Text.StringBuilder();
		var inQuote = false;
		var quoteChar = '"';

		foreach (var c in line)
		{
			if (inQuote)
			{
				if (c == quoteChar)
				{
					inQuote = false;
				}
				else
				{
					current.Append(c);
				}
			}
			else if (c == '"' || c == '\'')
			{
				inQuote = true;
				quoteChar = c;
			}
			else if (char.IsWhiteSpace(c))
			{
				if (current.Length > 0)
				{
					tokens.Add(current.ToString());
					current.Clear();
				}
			}
			else
			{
				current.Append(c);
			}
		}

		if (current.Length > 0)
		{
			tokens.Add(current.ToString());
		}

		return tokens;
	}

	/// <summary>
	/// Check if the word at wordStart is in a command position.
	/// </summary>
	private static bool IsCommandPosition(string line, int wordStart)
	{
		if (wordStart == 0)
		{
			return true;
		}

		// Check if preceded by pipe or at start (after skipping whitespace)
		var i = wordStart - 1;
		while (i >= 0 && char.IsWhiteSpace(line[i]))
		{
			i--;
		}

		return i < 0 || line[i] == '|' || line[i] == ';' || line[i] == '&';
	}

	/// <summary>
	/// Get path completions for a partial path.
	/// </summary>
	private List<string> GetPathCompletions(string partial, CompletionType completionType)
	{
		var completions = new List<string>();

		try
		{
			var (searchDir, prefix, searchPattern) = ParsePartialPath(partial);

			if (!pathProvider.DirectoryExists(searchDir))
			{
				return completions;
			}

			// Collect matching directories (unless we only want files)
			if (completionType != CompletionType.FileOnly)
			{
				CollectMatchingDirectories(searchDir, prefix, searchPattern, completions);
			}

			// Collect matching files (unless we only want directories)
			if (completionType != CompletionType.DirectoryOnly)
			{
				CollectMatchingFiles(searchDir, prefix, searchPattern, completions);
			}

			// Sort: directories first, then files, both alphabetically
			SortCompletions(completions);
		}
		catch
		{
			// Ignore errors during completion
		}

		return completions;
	}

	/// <summary>
	/// Parse a partial path into search directory, prefix, and pattern.
	/// </summary>
	private (string searchDir, string prefix, string searchPattern) ParsePartialPath(string partial)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return (pathProvider.GetCurrentDirectory(), string.Empty, string.Empty);
		}

		if (partial.EndsWith(pathProvider.DirectorySeparator) || partial.EndsWith('/'))
		{
			return (ResolvePath(partial), partial, string.Empty);
		}

		var dirPart = pathProvider.GetDirectoryName(partial);
		var filePart = pathProvider.GetFileName(partial);

		if (string.IsNullOrEmpty(dirPart))
		{
			return (pathProvider.GetCurrentDirectory(), string.Empty, filePart);
		}

		var searchDir = ResolvePath(dirPart);
		var prefix = dirPart.EndsWith(pathProvider.DirectorySeparator) || dirPart.EndsWith('/')
			? dirPart
			: dirPart + pathProvider.DirectorySeparator;

		return (searchDir, prefix, filePart);
	}

	/// <summary>
	/// Collect matching directories into the completions list.
	/// </summary>
	private void CollectMatchingDirectories(string searchDir, string prefix, string searchPattern, List<string> completions)
	{
		foreach (var dir in pathProvider.GetDirectories(searchDir))
		{
			var name = pathProvider.GetFileName(dir);
			if (MatchesPattern(name, searchPattern))
			{
				completions.Add(prefix + name + pathProvider.DirectorySeparator);
			}
		}
	}

	/// <summary>
	/// Collect matching files into the completions list.
	/// </summary>
	private void CollectMatchingFiles(string searchDir, string prefix, string searchPattern, List<string> completions)
	{
		foreach (var file in pathProvider.GetFiles(searchDir))
		{
			var name = pathProvider.GetFileName(file);
			if (MatchesPattern(name, searchPattern))
			{
				completions.Add(prefix + name);
			}
		}
	}

	/// <summary>
	/// Check if a name matches the search pattern.
	/// </summary>
	private static bool MatchesPattern(string name, string searchPattern)
	{
		return string.IsNullOrEmpty(searchPattern) ||
			   name.StartsWith(searchPattern, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Sort completions with directories first, then files, both alphabetically.
	/// </summary>
	private void SortCompletions(List<string> completions)
	{
		completions.Sort((a, b) =>
		{
			var aIsDir = a.EndsWith(pathProvider.DirectorySeparator) || a.EndsWith('/');
			var bIsDir = b.EndsWith(pathProvider.DirectorySeparator) || b.EndsWith('/');

			if (aIsDir && !bIsDir) return -1;
			if (!aIsDir && bIsDir) return 1;

			return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
		});
	}

	/// <summary>
	/// Resolve a path relative to the current directory.
	/// </summary>
	private string ResolvePath(string path)
	{
		if (Path.IsPathRooted(path))
		{
			return path;
		}

		return pathProvider.Combine(pathProvider.GetCurrentDirectory(), path);
	}

	/// <summary>
	/// Apply a completion to the line.
	/// </summary>
	private CompletionResult ApplyCompletion(string line, int wordStart, string completion)
	{
		var newText = ApplyCompletionText(line, wordStart, completion);
		var newCursorPosition = wordStart + completion.Length;

		return new CompletionResult
		{
			NewText = newText,
			NewCursorPosition = newCursorPosition,
			Applied = true,
			AllCompletions = _completions
		};
	}

	/// <summary>
	/// Apply completion text to a line.
	/// </summary>
	private string ApplyCompletionText(string line, int wordStart, string completion)
	{
		// Find where the current word ends
		var wordEnd = wordStart;
		while (wordEnd < line.Length && !IsWordBoundary(line[wordEnd]))
		{
			wordEnd++;
		}

		// Build new line
		var before = line[..wordStart];
		var after = line[wordEnd..];

		return before + completion + after;
	}
}
