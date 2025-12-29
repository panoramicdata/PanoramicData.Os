using PanoramicData.Os.Init.Shell.Completion;
using PanoramicData.Os.Init.Test.Mocks;

namespace PanoramicData.Os.Init.Test.Shell;

/// <summary>
/// Tests for the TabCompleter class.
/// </summary>
public class TabCompleterTests : BaseTest
{
	[Fact]
	public void Complete_WithNoMatches_ReturnsNotApplied()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("cat nonexistent", 15);

		// Assert
		result.Applied.Should().BeFalse();
		result.AllCompletions.Should().BeEmpty();
	}

	[Fact]
	public void Complete_WithSingleFileMatch_CompletesFile()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "readme.txt");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("cat read", 8);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("cat readme.txt");
		result.NewCursorPosition.Should().Be(14);
	}

	[Fact]
	public void Complete_WithSingleDirectoryMatch_CompletesWithTrailingSeparator()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/");
		pathProvider.AddDirectory("/", "home");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("cd ho", 5);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("cd home/");
		result.NewCursorPosition.Should().Be(8);
	}

	[Fact]
	public void Complete_WithMultipleMatches_ReturnsFirstMatch()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "file1.txt");
		pathProvider.AddFile("/home", "file2.txt");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("cat fi", 6);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
		result.NewText.Should().Be("cat file1.txt");
	}

	[Fact]
	public void Complete_PressingTabAgain_RotatesToNextCompletion()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "file1.txt");
		pathProvider.AddFile("/home", "file2.txt");
		var completer = new TabCompleter(pathProvider);

		// Act - First tab
		var result1 = completer.Complete("cat fi", 6);

		// Act - Second tab (simulating user pressing tab on completed line)
		var result2 = completer.Complete(result1.NewText, result1.NewCursorPosition);

		// Assert
		result2.Applied.Should().BeTrue();
		result2.NewText.Should().Be("cat file2.txt");
	}

	[Fact]
	public void Complete_RotatesBackToFirst_AfterAllCompletions()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "file1.txt");
		pathProvider.AddFile("/home", "file2.txt");
		var completer = new TabCompleter(pathProvider);

		// Act - Tab three times to wrap around
		var result1 = completer.Complete("cat fi", 6);
		var result2 = completer.Complete(result1.NewText, result1.NewCursorPosition);
		var result3 = completer.Complete(result2.NewText, result2.NewCursorPosition);

		// Assert - Should wrap back to first
		result3.Applied.Should().BeTrue();
		result3.NewText.Should().Be("cat file1.txt");
	}

	[Fact]
	public void Complete_WithEmptyPrefix_ListsAllInDirectory()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddDirectory("/home", "Documents");
		pathProvider.AddFile("/home", "readme.txt");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("ls ", 3);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
		// Directories come first
		result.NewText.Should().Be("ls Documents/");
	}

	[Fact]
	public void Complete_WithPathPrefix_CompletesInSubdirectory()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/");
		pathProvider.AddDirectory("/", "home");
		pathProvider.AddFile("/home", "file.txt");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("cat home/fi", 11);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("cat home/file.txt");
	}

	[Fact]
	public void Complete_DirectoryEndsWithSlash_ListsContents()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/");
		pathProvider.AddDirectory("/", "home");
		pathProvider.AddFile("/home", "file.txt");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("ls home/", 8);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("ls home/file.txt");
	}

	[Fact]
	public void Reset_ClearsCompletionState()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "file1.txt");
		pathProvider.AddFile("/home", "file2.txt");
		var completer = new TabCompleter(pathProvider);

		// Start completion
		var result1 = completer.Complete("cat fi", 6);

		// Reset
		completer.Reset();

		// Complete again - should start fresh
		var result2 = completer.Complete("cat fi", 6);

		// Assert - Should get first match again, not second
		result2.NewText.Should().Be(result1.NewText);
	}

	[Fact]
	public void Complete_DirectoriesBeforeFiles_InSort()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "abc.txt");
		pathProvider.AddDirectory("/home", "aaa_folder");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("ls a", 4);

		// Assert - Directory should come first even though file is alphabetically first
		result.NewText.Should().Be("ls aaa_folder/");
	}

	[Fact]
	public void Complete_CaseInsensitive_MatchesFiles()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "README.txt");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("cat read", 8);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("cat README.txt");
	}

	[Fact]
	public void Complete_PreservesTextAfterCursor()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "file.txt");
		var completer = new TabCompleter(pathProvider);

		// Act - Cursor is in the middle, there's text after
		var result = completer.Complete("cat fi | grep test", 6);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("cat file.txt | grep test");
	}

	[Fact]
	public void Complete_AtStartOfLine_CompletesPath()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddFile("/home", "script.sh");
		var completer = new TabCompleter(pathProvider);

		// Act
		var result = completer.Complete("scr", 3);

		// Assert
		result.Applied.Should().BeTrue();
		result.NewText.Should().Be("script.sh");
	}

	[Fact]
	public void Complete_DirectoryCommand_OnlyShowsDirectories()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddDirectory("/home", "documents");
		pathProvider.AddDirectory("/home", "downloads");
		pathProvider.AddFile("/home", "data.txt");
		pathProvider.AddFile("/home", "diary.txt");

		var commandProvider = new MockCommandSpecificationProvider();
		commandProvider.AddDirectoryCommand("cd");

		var completer = new TabCompleter(pathProvider, commandProvider);

		// Act
		var result = completer.Complete("cd d", 4);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
		result.AllCompletions.Should().Contain(c => c.Contains("documents"));
		result.AllCompletions.Should().Contain(c => c.Contains("downloads"));
		result.AllCompletions.Should().NotContain(c => c.Contains("data.txt"));
		result.AllCompletions.Should().NotContain(c => c.Contains("diary.txt"));
	}

	[Fact]
	public void Complete_FileCommand_OnlyShowsFiles()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddDirectory("/home", "documents");
		pathProvider.AddDirectory("/home", "downloads");
		pathProvider.AddFile("/home", "data.txt");
		pathProvider.AddFile("/home", "diary.txt");

		var commandProvider = new MockCommandSpecificationProvider();
		commandProvider.AddFileCommand("cat");

		var completer = new TabCompleter(pathProvider, commandProvider);

		// Act
		var result = completer.Complete("cat d", 5);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
		result.AllCompletions.Should().Contain(c => c.Contains("data.txt"));
		result.AllCompletions.Should().Contain(c => c.Contains("diary.txt"));
		result.AllCompletions.Should().NotContain(c => c.Contains("documents"));
		result.AllCompletions.Should().NotContain(c => c.Contains("downloads"));
	}

	[Fact]
	public void Complete_UnknownCommand_ShowsBothFilesAndDirectories()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddDirectory("/home", "docs");
		pathProvider.AddFile("/home", "data.txt");

		var commandProvider = new MockCommandSpecificationProvider();
		// Don't register any commands

		var completer = new TabCompleter(pathProvider, commandProvider);

		// Act
		var result = completer.Complete("unknown d", 9);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
	}

	[Fact]
	public void Complete_NoCommandProvider_ShowsBothFilesAndDirectories()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/home");
		pathProvider.AddDirectory("/home", "docs");
		pathProvider.AddFile("/home", "data.txt");

		var completer = new TabCompleter(pathProvider); // No command provider

		// Act
		var result = completer.Complete("cd d", 4);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
	}

	[Fact]
	public void Complete_DirectoryCommand_WithPath_OnlyShowsDirectories()
	{
		// Arrange
		var pathProvider = new MockPathProvider();
		pathProvider.SetCurrentDirectory("/");
		pathProvider.AddDirectory("/", "home");
		pathProvider.AddDirectory("/home", "user");
		pathProvider.AddDirectory("/home", "ubuntu");
		pathProvider.AddFile("/home", "test.txt");

		var commandProvider = new MockCommandSpecificationProvider();
		commandProvider.AddDirectoryCommand("cd");

		var completer = new TabCompleter(pathProvider, commandProvider);

		// Act
		var result = completer.Complete("cd home/u", 9);

		// Assert
		result.Applied.Should().BeTrue();
		result.AllCompletions.Should().HaveCount(2);
		result.AllCompletions.Should().AllSatisfy(c => c.Should().NotContain("test.txt"));
	}
}
