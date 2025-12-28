namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Represents a token in the command line for syntax highlighting.
/// </summary>
public readonly struct LineToken
{
	public string Text { get; init; }
	public TokenType Type { get; init; }
	public int StartIndex { get; init; }
	public int Length => Text.Length;
}
