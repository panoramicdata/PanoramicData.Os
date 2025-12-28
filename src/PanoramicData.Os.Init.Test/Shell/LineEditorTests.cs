using PanoramicData.Os.Init.Test.Mocks;

namespace PanoramicData.Os.Init.Test.Shell;

/// <summary>
/// Tests for the LineEditor class.
/// </summary>
public class LineEditorTests : IDisposable
{
	private readonly MockTerminalIO _mockIO;
	private readonly LineEditor _editor;

	public LineEditorTests()
	{
		_mockIO = new MockTerminalIO();
		_editor = new LineEditor(_mockIO);
	}

	public void Dispose()
	{
		_editor.Dispose();
		_mockIO.Dispose();
		GC.SuppressFinalize(this);
	}

	[Fact]
	public void ReadLine_WithSimpleText_ReturnsText()
	{
		// Arrange
		_mockIO.QueueInputLine("hello");

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().Be("hello");
	}

	[Fact]
	public void ReadLine_WithCtrlC_ReturnsNull()
	{
		// Arrange
		_mockIO.QueueCtrlC();

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().BeNull();
	}

	[Fact]
	public void ReadLine_WithCtrlD_OnEmptyLine_ReturnsNull()
	{
		// Arrange
		_mockIO.QueueCtrlD();

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().BeNull();
	}

	[Fact]
	public void ReadLine_WithBackspace_DeletesCharacter()
	{
		// Arrange
		_mockIO.QueueInput("hell");
		_mockIO.QueueBackspace();
		_mockIO.QueueInput("p\n");

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().Be("help");
	}

	[Fact]
	public void ReadLine_WithHistory_NavigatesUp()
	{
		// First command
		_mockIO.QueueInputLine("first");
		var result1 = _editor.ReadLine(0);
		_ = result1.Should().Be("first");

		// Second command with up arrow to recall first
		_mockIO.QueueUpArrow();
		_mockIO.QueueInput("\n");
		var result2 = _editor.ReadLine(0);

		// Assert
		_ = result2.Should().Be("first");
	}

	[Fact]
	public void ReadLine_WithLeftArrow_MovesCursor()
	{
		// Arrange - type "ab", go left, insert "c", should be "acb"
		_mockIO.QueueInput("ab");
		_mockIO.QueueLeftArrow();
		_mockIO.QueueInput("c\n");

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().Be("acb");
	}

	[Fact]
	public void ReadLine_WithDelete_DeletesCharacterAtCursor()
	{
		// Arrange - type "abc", go to start, delete first char
		_mockIO.QueueInput("abc");
		_mockIO.QueueHome();
		_mockIO.QueueDelete();
		_mockIO.QueueInput("\n");

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().Be("bc");
	}

	[Fact]
	public void History_RespectsMaxSize()
	{
		// Arrange
		_editor.MaxHistorySize = 3;

		// Add 5 commands
		for (var i = 1; i <= 5; i++)
		{
			_mockIO.QueueInputLine($"cmd{i}");
			_ = _editor.ReadLine(0);
		}

		// Assert - only last 3 should remain
		_ = _editor.History.Count.Should().Be(3);
		_ = _editor.History[0].Should().Be("cmd3");
		_ = _editor.History[1].Should().Be("cmd4");
		_ = _editor.History[2].Should().Be("cmd5");
	}

	[Fact]
	public void ReadLine_WithEmptyInput_NotAddedToHistory()
	{
		// Arrange - press enter on empty line
		_mockIO.QueueInput("\n");

		// Act
		var result = _editor.ReadLine(0);

		// Assert
		_ = result.Should().Be("");
		_ = _editor.History.Should().BeEmpty();
	}

	[Fact]
	public void ReadLine_DuplicateCommand_NotAddedTwice()
	{
		// First command
		_mockIO.QueueInputLine("test");
		_ = _editor.ReadLine(0);

		// Same command again
		_mockIO.QueueInputLine("test");
		_ = _editor.ReadLine(0);

		// Assert
		_ = _editor.History.Count.Should().Be(1);
	}
}
