using System.Text.Json;
using System.Text.Json.Serialization;

namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// JSON serialization context for ColorPalette.
/// </summary>
[JsonSerializable(typeof(ColorPalette))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class ColorPaletteJsonContext : JsonSerializerContext
{
}

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

/// <summary>
/// Color palette for shell syntax highlighting.
/// </summary>
public class ColorPalette
{
	/// <summary>
	/// Map of token types to ANSI color codes.
	/// </summary>
	[JsonPropertyName("colors")]
	public Dictionary<string, string> Colors { get; set; } = new();

	/// <summary>
	/// Get the color code for a token type.
	/// </summary>
	public string GetColor(TokenType tokenType)
	{
		var key = tokenType.ToString();
		return Colors.TryGetValue(key, out var color) ? color : GetDefaultColor(tokenType);
	}

	/// <summary>
	/// Set the color for a token type.
	/// </summary>
	public void SetColor(TokenType tokenType, string ansiColor)
	{
		Colors[tokenType.ToString()] = ansiColor;
	}

	/// <summary>
	/// Get the default color for a token type (fallback).
	/// </summary>
	private static string GetDefaultColor(TokenType tokenType)
	{
		return tokenType switch
		{
			TokenType.Default => AnsiColors.White,
			TokenType.Command => AnsiColors.BrightCyan,
			TokenType.ValidCommand => AnsiColors.BrightCyan,
			TokenType.InvalidCommand => AnsiColors.Red + AnsiColors.Underline,
			TokenType.Argument => AnsiColors.White,
			TokenType.Flag => AnsiColors.BrightYellow,
			TokenType.String => AnsiColors.BrightGreen,
			TokenType.Path => AnsiColors.BrightBlue,
			TokenType.Number => AnsiColors.BrightMagenta,
			TokenType.Pipe => AnsiColors.BrightWhite + AnsiColors.Bold,
			TokenType.Redirect => AnsiColors.BrightWhite + AnsiColors.Bold,
			TokenType.Variable => AnsiColors.Cyan,
			TokenType.Comment => AnsiColors.BrightBlack,
			TokenType.Error => AnsiColors.Red + AnsiColors.Bold,
			TokenType.Warning => AnsiColors.Yellow + AnsiColors.Bold,
			TokenType.Success => AnsiColors.Green + AnsiColors.Bold,
			TokenType.PromptUser => AnsiColors.BrightGreen,
			TokenType.PromptHost => AnsiColors.BrightGreen,
			TokenType.PromptSeparator => AnsiColors.White,
			TokenType.PromptPath => AnsiColors.BrightBlue,
			TokenType.PromptSymbol => AnsiColors.White,
			_ => AnsiColors.White
		};
	}

	/// <summary>
	/// Creates the default dark mode palette with pastel colors.
	/// </summary>
	public static ColorPalette CreateDefaultDark()
	{
		var palette = new ColorPalette();

		// Using 256-color mode for pastel colors
		// Format: \x1b[38;5;{n}m for foreground

		// Pastel colors for tokens (dark mode friendly)
		palette.SetColor(TokenType.Default, "\x1b[38;5;252m");           // Light gray
		palette.SetColor(TokenType.Command, "\x1b[38;5;117m");           // Pastel cyan
		palette.SetColor(TokenType.ValidCommand, "\x1b[38;5;117m");      // Pastel cyan
		palette.SetColor(TokenType.InvalidCommand, "\x1b[38;5;196m\x1b[4m"); // Solid red + underline
		palette.SetColor(TokenType.Argument, "\x1b[38;5;188m");          // Light cream
		palette.SetColor(TokenType.Flag, "\x1b[38;5;222m");              // Pastel yellow
		palette.SetColor(TokenType.String, "\x1b[38;5;150m");            // Pastel green
		palette.SetColor(TokenType.Path, "\x1b[38;5;111m");              // Pastel blue
		palette.SetColor(TokenType.Number, "\x1b[38;5;183m");            // Pastel purple
		palette.SetColor(TokenType.Pipe, "\x1b[38;5;255m\x1b[1m");       // Bright white bold
		palette.SetColor(TokenType.Redirect, "\x1b[38;5;255m\x1b[1m");   // Bright white bold
		palette.SetColor(TokenType.Variable, "\x1b[38;5;152m");          // Pastel teal
		palette.SetColor(TokenType.Comment, "\x1b[38;5;245m");           // Gray

		// Solid colors for status indicators
		palette.SetColor(TokenType.Error, "\x1b[38;5;196m\x1b[1m");      // Solid red bold
		palette.SetColor(TokenType.Warning, "\x1b[38;5;226m\x1b[1m");    // Solid yellow bold
		palette.SetColor(TokenType.Success, "\x1b[38;5;46m\x1b[1m");     // Solid green bold

		// Prompt colors
		palette.SetColor(TokenType.PromptUser, "\x1b[38;5;114m");        // Pastel green
		palette.SetColor(TokenType.PromptHost, "\x1b[38;5;114m");        // Pastel green
		palette.SetColor(TokenType.PromptSeparator, "\x1b[38;5;250m");   // Light gray
		palette.SetColor(TokenType.PromptPath, "\x1b[38;5;111m");        // Pastel blue
		palette.SetColor(TokenType.PromptSymbol, "\x1b[38;5;250m");      // Light gray

		return palette;
	}

	/// <summary>
	/// Load a palette from a JSON file.
	/// </summary>
	public static ColorPalette? LoadFromFile(string path)
	{
		try
		{
			if (!File.Exists(path))
			{
				return null;
			}

			var json = File.ReadAllText(path);
			return JsonSerializer.Deserialize(json, ColorPaletteJsonContext.Default.ColorPalette);
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Save the palette to a JSON file.
	/// </summary>
	public void SaveToFile(string path)
	{
		var json = JsonSerializer.Serialize(this, ColorPaletteJsonContext.Default.ColorPalette);
		File.WriteAllText(path, json);
	}

	/// <summary>
	/// Colorize text with the specified token type.
	/// </summary>
	public string Colorize(string text, TokenType tokenType)
	{
		return $"{GetColor(tokenType)}{text}{AnsiColors.Reset}";
	}
}
