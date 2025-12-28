namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// ANSI escape codes for terminal colors and formatting.
/// </summary>
public static class AnsiColors
{
    // Reset
    public const string Reset = "\x1b[0m";

    // Text attributes
    public const string Bold = "\x1b[1m";
    public const string Dim = "\x1b[2m";
    public const string Italic = "\x1b[3m";
    public const string Underline = "\x1b[4m";
    public const string Blink = "\x1b[5m";
    public const string Reverse = "\x1b[7m";

    // Foreground colors
    public const string Black = "\x1b[30m";
    public const string Red = "\x1b[31m";
    public const string Green = "\x1b[32m";
    public const string Yellow = "\x1b[33m";
    public const string Blue = "\x1b[34m";
    public const string Magenta = "\x1b[35m";
    public const string Cyan = "\x1b[36m";
    public const string White = "\x1b[37m";

    // Bright foreground colors
    public const string BrightBlack = "\x1b[90m";
    public const string BrightRed = "\x1b[91m";
    public const string BrightGreen = "\x1b[92m";
    public const string BrightYellow = "\x1b[93m";
    public const string BrightBlue = "\x1b[94m";
    public const string BrightMagenta = "\x1b[95m";
    public const string BrightCyan = "\x1b[96m";
    public const string BrightWhite = "\x1b[97m";

    // Background colors
    public const string BgBlack = "\x1b[40m";
    public const string BgRed = "\x1b[41m";
    public const string BgGreen = "\x1b[42m";
    public const string BgYellow = "\x1b[43m";
    public const string BgBlue = "\x1b[44m";
    public const string BgMagenta = "\x1b[45m";
    public const string BgCyan = "\x1b[46m";
    public const string BgWhite = "\x1b[47m";

    // Cursor control
    public const string ClearScreen = "\x1b[2J";
    public const string ClearLine = "\x1b[2K";
    public const string CursorHome = "\x1b[H";
    public const string SaveCursor = "\x1b[s";
    public const string RestoreCursor = "\x1b[u";

    /// <summary>
    /// Move cursor to specific position.
    /// </summary>
    public static string MoveCursor(int row, int col) => $"\x1b[{row};{col}H";

    /// <summary>
    /// Move cursor up N lines.
    /// </summary>
    public static string CursorUp(int n = 1) => $"\x1b[{n}A";

    /// <summary>
    /// Move cursor down N lines.
    /// </summary>
    public static string CursorDown(int n = 1) => $"\x1b[{n}B";

    /// <summary>
    /// Move cursor forward N columns.
    /// </summary>
    public static string CursorForward(int n = 1) => $"\x1b[{n}C";

    /// <summary>
    /// Move cursor back N columns.
    /// </summary>
    public static string CursorBack(int n = 1) => $"\x1b[{n}D";

    /// <summary>
    /// Colorize text with the specified color and reset after.
    /// </summary>
    public static string Colorize(string text, string color) => $"{color}{text}{Reset}";

    /// <summary>
    /// Create a colored and bold string.
    /// </summary>
    public static string BoldColor(string text, string color) => $"{Bold}{color}{text}{Reset}";
}
