using Spectre.Console;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Defines the console I/O interface for commands.
/// </summary>
public interface IConsole
{
	/// <summary>
	/// Write text to the console.
	/// </summary>
	void Write(string text);

	/// <summary>
	/// Write a line to the console.
	/// </summary>
	void WriteLine(string text = "");

	/// <summary>
	/// Write text with a specific color.
	/// </summary>
	void WriteColored(string text, Color color);

	/// <summary>
	/// Write a line with a specific color.
	/// </summary>
	void WriteLineColored(string text, Color color);

	/// <summary>
	/// Write an error message.
	/// </summary>
	void WriteError(string message);

	/// <summary>
	/// Write a warning message.
	/// </summary>
	void WriteWarning(string message);

	/// <summary>
	/// Write a success message.
	/// </summary>
	void WriteSuccess(string message);

	/// <summary>
	/// Write an info message.
	/// </summary>
	void WriteInfo(string message);

	/// <summary>
	/// Read a line from the console.
	/// </summary>
	string? ReadLine();

	/// <summary>
	/// Read a key from the console.
	/// </summary>
	ConsoleKeyInfo ReadKey(bool intercept = false);

	/// <summary>
	/// Clear the console.
	/// </summary>
	void Clear();

	/// <summary>
	/// Gets the Spectre.Console instance for advanced rendering.
	/// </summary>
	IAnsiConsole AnsiConsole { get; }
}
