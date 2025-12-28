namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Token types for syntax highlighting.
/// </summary>
public enum TokenType
{
	/// <summary>Default text color.</summary>
	Default,

	/// <summary>Command name (first token).</summary>
	Command,

	/// <summary>Valid command that exists.</summary>
	ValidCommand,

	/// <summary>Invalid/unknown command.</summary>
	InvalidCommand,

	/// <summary>Command argument.</summary>
	Argument,

	/// <summary>Flag/option starting with - or --.</summary>
	Flag,

	/// <summary>String literal in quotes.</summary>
	String,

	/// <summary>Path (contains / or \).</summary>
	Path,

	/// <summary>Path that doesn't exist when it should.</summary>
	InvalidPath,

	/// <summary>Number.</summary>
	Number,

	/// <summary>Pipe operator |.</summary>
	Pipe,

	/// <summary>Redirect operators > >> <.</summary>
	Redirect,

	/// <summary>Environment variable $VAR.</summary>
	Variable,

	/// <summary>Comment starting with #.</summary>
	Comment,

	/// <summary>Error indicator.</summary>
	Error,

	/// <summary>Warning indicator.</summary>
	Warning,

	/// <summary>Success indicator.</summary>
	Success,

	/// <summary>Prompt username.</summary>
	PromptUser,

	/// <summary>Prompt hostname.</summary>
	PromptHost,

	/// <summary>Prompt separator characters.</summary>
	PromptSeparator,

	/// <summary>Prompt path.</summary>
	PromptPath,

	/// <summary>Prompt symbol ($, #, etc).</summary>
	PromptSymbol
}
