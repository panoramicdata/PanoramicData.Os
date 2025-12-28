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
