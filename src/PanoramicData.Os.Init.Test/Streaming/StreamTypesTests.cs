using PanoramicData.Os.CommandLine.Streaming;

namespace PanoramicData.Os.Init.Test.Streaming;

/// <summary>
/// Tests for core stream types.
/// </summary>
public class StreamTypesTests : BaseTest
{
	[Fact]
	public void TextLine_CreatesWithContent()
	{
		var line = new TextLine("Hello, World!", 1, "test.txt");

		line.Content.Should().Be("Hello, World!");
		line.LineNumber.Should().Be(1);
		line.SourceFile.Should().Be("test.txt");
	}

	[Fact]
	public void TextLine_ToStringReturnsContent()
	{
		var line = new TextLine("Test content", 1, "file.txt");

		// TextLine converts FROM string (implicit), but uses ToString for TO string
		string content = line.ToString();

		content.Should().Be("Test content");
	}

	[Fact]
	public void TextLine_ImplicitConversionFromString()
	{
		TextLine line = "Test content";

		line.Content.Should().Be("Test content");
	}

	[Fact]
	public void TextLine_ToStringMethod()
	{
		var line = new TextLine("My content", 1, "file.txt");

		line.ToString().Should().Be("My content");
	}

	[Fact]
	public void TextChunk_CreatesWithContent()
	{
		var chunk = new TextChunk("Line 1\nLine 2\nLine 3", 1, 3, "test.txt");

		chunk.Content.Should().Be("Line 1\nLine 2\nLine 3");
		chunk.LineCount.Should().Be(3);
		chunk.SourceFile.Should().Be("test.txt");
	}

	[Fact]
	public void TextChunk_FromFactory()
	{
		var chunk = TextChunk.From("Single line", 1, 1, "test.txt");

		chunk.LineCount.Should().Be(1);
		chunk.Content.Should().Be("Single line");
	}

	[Fact]
	public void TextChunk_ToLines_SplitsCorrectly()
	{
		var chunk = new TextChunk("Line 1\nLine 2\nLine 3", 1, 3, "test.txt");

		var lines = chunk.ToLines().ToList();

		lines.Count.Should().Be(3);
		lines[0].Content.Should().Be("Line 1");
		lines[1].Content.Should().Be("Line 2");
		lines[2].Content.Should().Be("Line 3");
	}

	[Fact]
	public void FileEntry_FromFile_CreatesCorrectEntry()
	{
		// Create a temp file to test with
		var tempFile = Path.GetTempFileName();
		try
		{
			File.WriteAllText(tempFile, "test content");
			var info = new FileInfo(tempFile);

			var entry = FileEntry.FromFile(info);

			entry.Name.Should().Be(Path.GetFileName(tempFile));
			entry.FullPath.Should().Be(tempFile);
			entry.Size.Should().Be(12); // "test content" = 12 bytes
			entry.IsDirectory.Should().BeFalse();
			entry.Extension.Should().Be(".tmp");
		}
		finally
		{
			File.Delete(tempFile);
		}
	}

	[Fact]
	public void FileEntry_FromDirectory_CreatesCorrectEntry()
	{
		var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		Directory.CreateDirectory(tempDir);
		try
		{
			var info = new DirectoryInfo(tempDir);

			var entry = FileEntry.FromDirectory(info);

			entry.Name.Should().Be(Path.GetFileName(tempDir));
			entry.FullPath.Should().Be(tempDir);
			entry.Size.Should().Be(0);
			entry.IsDirectory.Should().BeTrue();
		}
		finally
		{
			Directory.Delete(tempDir);
		}
	}

	[Fact]
	public void FileEntry_FromPath_DetectsFileVsDirectory()
	{
		var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		Directory.CreateDirectory(tempDir);
		var tempFile = Path.Combine(tempDir, "test.txt");
		File.WriteAllText(tempFile, "content");

		try
		{
			var fileEntry = FileEntry.FromPath(tempFile);
			var dirEntry = FileEntry.FromPath(tempDir);

			fileEntry.IsDirectory.Should().BeFalse();
			dirEntry.IsDirectory.Should().BeTrue();
		}
		finally
		{
			File.Delete(tempFile);
			Directory.Delete(tempDir);
		}
	}

	[Fact]
	public void StreamChar_CreatesWithCharacter()
	{
		var streamChar = new StreamChar('A', 0, "input");

		streamChar.Value.Should().Be('A');
		streamChar.Position.Should().Be(0);
		streamChar.SourceFile.Should().Be("input");
	}

	[Fact]
	public void StreamChar_ImplicitConversions()
	{
		StreamChar fromChar = 'X';
		char toChar = fromChar;

		toChar.Should().Be('X');
	}

	[Fact]
	public void StreamByte_CreatesWithByte()
	{
		var streamByte = new StreamByte(0xFF, 0, "binary");

		streamByte.Value.Should().Be(0xFF);
		streamByte.Position.Should().Be(0);
		streamByte.SourceFile.Should().Be("binary");
	}

	[Fact]
	public void StreamByte_ImplicitConversions()
	{
		StreamByte fromByte = 0xAB;
		byte toByte = fromByte;

		toByte.Should().Be(0xAB);
	}

	[Fact]
	public void StringValue_CreatesWithValue()
	{
		var stringValue = new StringValue("test value");

		stringValue.Value.Should().Be("test value");
	}

	[Fact]
	public void StringValue_ImplicitStringConversion()
	{
		var stringValue = new StringValue("convertible");

		string value = stringValue;

		value.Should().Be("convertible");
	}

	[Fact]
	public void StreamMetadata_Now_CreatesWithCurrentTimestamp()
	{
		var before = DateTimeOffset.UtcNow;
		var metadata = StreamMetadata.Now("source", 42);
		var after = DateTimeOffset.UtcNow;

		metadata.Source.Should().Be("source");
		metadata.SequenceNumber.Should().Be(42);
		metadata.Timestamp.Should().NotBeNull();
		(metadata.Timestamp >= before && metadata.Timestamp <= after).Should().BeTrue();
	}

	[Fact]
	public void StreamMetadata_Empty_HasNullValues()
	{
		var empty = StreamMetadata.Empty;

		empty.Source.Should().BeNull();
		empty.Timestamp.Should().BeNull();
		empty.SequenceNumber.Should().BeNull();
	}
}
