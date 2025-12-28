namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Represents a key input.
/// </summary>
public readonly record struct KeyInput
{
	public KeyType Type { get; init; }
	public char Character { get; init; }

	public static readonly KeyInput Unknown = new() { Type = KeyType.Unknown };
	public static readonly KeyInput Enter = new() { Type = KeyType.Enter };
	public static readonly KeyInput Backspace = new() { Type = KeyType.Backspace };
	public static readonly KeyInput Delete = new() { Type = KeyType.Delete };
	public static readonly KeyInput Tab = new() { Type = KeyType.Tab };
	public static readonly KeyInput Escape = new() { Type = KeyType.Escape };
	public static readonly KeyInput Up = new() { Type = KeyType.Up };
	public static readonly KeyInput Down = new() { Type = KeyType.Down };
	public static readonly KeyInput Left = new() { Type = KeyType.Left };
	public static readonly KeyInput Right = new() { Type = KeyType.Right };
	public static readonly KeyInput Home = new() { Type = KeyType.Home };
	public static readonly KeyInput End = new() { Type = KeyType.End };
	public static readonly KeyInput Ctrl_A = new() { Type = KeyType.Ctrl_A };
	public static readonly KeyInput Ctrl_C = new() { Type = KeyType.Ctrl_C };
	public static readonly KeyInput Ctrl_D = new() { Type = KeyType.Ctrl_D };
	public static readonly KeyInput Ctrl_E = new() { Type = KeyType.Ctrl_E };
	public static readonly KeyInput Ctrl_K = new() { Type = KeyType.Ctrl_K };
	public static readonly KeyInput Ctrl_L = new() { Type = KeyType.Ctrl_L };
	public static readonly KeyInput Ctrl_U = new() { Type = KeyType.Ctrl_U };
	public static readonly KeyInput Ctrl_W = new() { Type = KeyType.Ctrl_W };
	public static readonly KeyInput Ctrl_Left = new() { Type = KeyType.Ctrl_Left };
	public static readonly KeyInput Ctrl_Right = new() { Type = KeyType.Ctrl_Right };
	public static readonly KeyInput Char = new() { Type = KeyType.Char };
}
