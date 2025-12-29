namespace PanoramicData.Os.Init.Test.Shell;

/// <summary>
/// Tests for the ColorPalette class.
/// </summary>
public class ColorPaletteTests : BaseTest
{
	[Fact]
	public void CreateDefaultDark_ReturnsValidPalette()
	{
		// Act
		var palette = ColorPalette.CreateDefaultDark();

		// Assert
		_ = palette.Should().NotBeNull();
		_ = palette.Colors.Should().NotBeEmpty();
	}

	[Fact]
	public void CreateDefaultDark_ContainsRequiredTokenTypes()
	{
		// Act
		var palette = ColorPalette.CreateDefaultDark();

		// Assert - check key token types have colors
		_ = palette.GetColor(TokenType.Command).Should().NotBeNullOrEmpty();
		_ = palette.GetColor(TokenType.ValidCommand).Should().NotBeNullOrEmpty();
		_ = palette.GetColor(TokenType.InvalidCommand).Should().NotBeNullOrEmpty();
		_ = palette.GetColor(TokenType.Argument).Should().NotBeNullOrEmpty();
		_ = palette.GetColor(TokenType.Flag).Should().NotBeNullOrEmpty();
		_ = palette.GetColor(TokenType.String).Should().NotBeNullOrEmpty();
		_ = palette.GetColor(TokenType.Path).Should().NotBeNullOrEmpty();
	}

	[Fact]
	public void GetColor_UnknownToken_ReturnsDefaultColor()
	{
		// Arrange
		var palette = ColorPalette.CreateDefaultDark();

		// Act
		var color = palette.GetColor(TokenType.Default);

		// Assert
		_ = color.Should().NotBeNull();
	}

	[Fact]
	public void SetColor_UpdatesColorForTokenType()
	{
		// Arrange
		var palette = ColorPalette.CreateDefaultDark();
		var newColor = AnsiColors.Magenta;

		// Act
		palette.SetColor(TokenType.Command, newColor);
		var result = palette.GetColor(TokenType.Command);

		// Assert
		_ = result.Should().Be(newColor);
	}
}
