using Spectre.Console;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Default console implementation using Spectre.Console for rich output.
/// </summary>
public class PanConsole : IConsole
{
	private readonly IAnsiConsole _ansiConsole;

	/// <summary>
	/// Create a new console using the default Spectre.Console instance.
	/// </summary>
	public PanConsole() : this(Spectre.Console.AnsiConsole.Console)
	{
	}

	/// <summary>
	/// Create a new console with a specific Spectre.Console instance.
	/// </summary>
	public PanConsole(IAnsiConsole ansiConsole)
	{
		_ansiConsole = ansiConsole;
	}

	/// <inheritdoc />
	public IAnsiConsole AnsiConsole => _ansiConsole;

	/// <inheritdoc />
	public void Write(string text)
	{
		_ansiConsole.Write(new Text(text));
	}

	/// <inheritdoc />
	public void WriteLine(string text = "")
	{
		_ansiConsole.WriteLine(text);
	}

	/// <inheritdoc />
	public void WriteColored(string text, Color color)
	{
		_ansiConsole.Write(new Text(text, new Style(color)));
	}

	/// <inheritdoc />
	public void WriteLineColored(string text, Color color)
	{
		_ansiConsole.MarkupLine($"[{color.ToMarkup()}]{Markup.Escape(text)}[/]");
	}

	/// <inheritdoc />
	public void WriteError(string message)
	{
		_ansiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public void WriteWarning(string message)
	{
		_ansiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public void WriteSuccess(string message)
	{
		_ansiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
	}

	/// <inheritdoc />
	public void WriteInfo(string message)
	{
		_ansiConsole.MarkupLine($"[blue]{Markup.Escape(message)}[/]");
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
		_ansiConsole.Clear();
	}
}
