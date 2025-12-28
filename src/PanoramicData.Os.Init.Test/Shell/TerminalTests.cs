using PanoramicData.Os.Init.Test.Mocks;

namespace PanoramicData.Os.Init.Test.Shell;

/// <summary>
/// Tests for the Terminal class.
/// </summary>
public class TerminalTests : IDisposable
{
	private readonly MockTerminalIO _mockIO;
	private readonly Terminal _terminal;

	public TerminalTests()
	{
		_mockIO = new MockTerminalIO();
		_terminal = new Terminal(_mockIO);
	}

	public void Dispose()
	{
		_terminal.Dispose();
		_mockIO.Dispose();
		GC.SuppressFinalize(this);
	}

	[Fact]
	public void Write_OutputsText()
	{
		// Arrange
		_mockIO.ClearOutput(); // Clear any initialization output

		// Act
		_terminal.Write("Hello");

		// Assert
		_ = _mockIO.Output.Should().Contain("Hello");
	}

	[Fact]
	public void WriteLine_OutputsTextWithNewline()
	{
		// Arrange
		_mockIO.ClearOutput();

		// Act
		_terminal.WriteLine("Hello");

		// Assert
		_ = _mockIO.Output.Should().Contain("Hello\n");
	}

	[Fact]
	public void WriteColored_IncludesAnsiCodes()
	{
		// Arrange
		_mockIO.ClearOutput();

		// Act
		_terminal.WriteColored("test", AnsiColors.Red);

		// Assert
		_ = _mockIO.Output.Should().Contain(AnsiColors.Red);
		_ = _mockIO.Output.Should().Contain("test");
		_ = _mockIO.Output.Should().Contain(AnsiColors.Reset);
	}

	[Fact]
	public void ReadLine_ReturnsInputText()
	{
		// Arrange
		_mockIO.QueueInputLine("test input");

		// Act
		var result = _terminal.ReadLine();

		// Assert
		_ = result.Should().Be("test input");
	}

	[Fact]
	public void ReadLine_WithBackspace_DeletesCharacter()
	{
		// Arrange
		_mockIO.QueueInput("hell");
		_mockIO.QueueBackspace();
		_mockIO.QueueInputLine("p");

		// Act
		var result = _terminal.ReadLine();

		// Assert
		_ = result.Should().Be("help");
	}

	[Fact]
	public void ReadLine_WithCtrlC_ReturnsNull()
	{
		// Arrange
		_mockIO.QueueCtrlC();

		// Act
		var result = _terminal.ReadLine();

		// Assert
		_ = result.Should().BeNull();
	}

	[Fact]
	public void Clear_SendsClearSequence()
	{
		// Arrange
		_mockIO.ClearOutput();

		// Act
		_terminal.Clear();

		// Assert
		_ = _mockIO.Output.Should().Contain(AnsiColors.ClearScreen);
		_ = _mockIO.Output.Should().Contain(AnsiColors.CursorHome);
	}
}
