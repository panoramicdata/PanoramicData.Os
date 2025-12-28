using Spectre.Console;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Default console implementation using Spectre.Console for rich output.
/// </summary>
/// <remarks>
/// Create a new console with a specific Spectre.Console instance.
/// </remarks>
public class PanConsole(IAnsiConsole ansiConsole) : IConsole
{

	/// <summary>
	/// Create a new console using the default Spectre.Console instance.
	/// </summary>
	public PanConsole() : this(Spectre.Console.AnsiConsole.Console)
	{
	}

	/// <inheritdoc />
	public IAnsiConsole AnsiConsole => ansiConsole;

	/// <inheritdoc />
	public void Write(string text)
	{
		ansiConsole.Write(new Text(text));
	}

	/// <inheritdoc />
	public void WriteLine(string text = "")
	{
		ansiConsole.WriteLine(text);
	}

	/// <inheritdoc />
	public void WriteColored(string text, Color color)
	{
		ansiConsole.Write(new Text(text, new Style(color)));
	}

	/// <inheritdoc />
	public void WriteLineColored(string text, Color color)
	{
		ansiConsole.MarkupLine($"[{color.ToMarkup()}]{Markup.Escape(text)}[/]");
	}

	/// <inheritdoc />
	public void WriteError(string message)
	{
		ansiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public void WriteWarning(string message)
	{
		ansiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public void WriteSuccess(string message)
	{
		ansiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public void WriteInfo(string message)
	{
		ansiConsole.MarkupLine($"[blue]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public string? ReadLine()
	{
		return System.Console.ReadLine();
	}

	/// <inheritdoc />
	public ConsoleKeyInfo ReadKey(bool intercept = false)
	{
		return System.Console.ReadKey(intercept);
	}

	/// <inheritdoc />
	public void Clear()
	{
		ansiConsole.Clear();
	}
}
