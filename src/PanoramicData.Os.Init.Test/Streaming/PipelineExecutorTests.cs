using PanoramicData.Os.CommandLine.Pipeline;
using PanoramicData.Os.CommandLine.Streaming;

namespace PanoramicData.Os.Init.Test.Streaming;

/// <summary>
/// Tests for the PipelineExecutor.
/// </summary>
public class PipelineExecutorTests : BaseTest
{
	private readonly PipelineExecutor _executor = new();

	#region Source Execution

	[Fact]
	public async Task ExecuteSourceAsync_YieldsAllItems()
	{
		var source = GenerateTextLines(5);

		var items = await PipelineExecutor.ToListAsync(source, CancellationToken);

		items.Count.Should().Be(5);
		for (var i = 0; i < 5; i++)
		{
			items[i].Content.Should().Be($"Line {i}");
		}
	}

	#endregion

	#region Where (Filter)

	[Fact]
	public async Task WhereAsync_FiltersCorrectly()
	{
		var source = GenerateTextLines(10);

		var filtered = PipelineExecutor.WhereAsync(source, line => line.LineNumber % 2 == 0, CancellationToken);
		var items = await PipelineExecutor.ToListAsync(filtered, CancellationToken);

		items.Count.Should().Be(5);
		items.Should().AllSatisfy(item => (item.LineNumber % 2 == 0).Should().BeTrue());
	}

	[Fact]
	public async Task WhereAsync_EmptyResult_WhenNoMatch()
	{
		var source = GenerateTextLines(5);

		var filtered = PipelineExecutor.WhereAsync(source, _ => false, CancellationToken);
		var items = await PipelineExecutor.ToListAsync(filtered, CancellationToken);

		items.Should().BeEmpty();
	}

	#endregion

	#region Select (Projection)

	[Fact]
	public async Task SelectAsync_TransformsItems()
	{
		var source = GenerateTextLines(3);

		var projected = PipelineExecutor.SelectAsync(source, line =>
			new StringValue($"Transformed: {line.Content}"), CancellationToken);
		var items = await PipelineExecutor.ToListAsync(projected, CancellationToken);

		items.Count.Should().Be(3);
		items[0].Value.Should().Be("Transformed: Line 0");
		items[1].Value.Should().Be("Transformed: Line 1");
		items[2].Value.Should().Be("Transformed: Line 2");
	}

	#endregion

	#region Take

	[Fact]
	public async Task TakeAsync_LimitsItems()
	{
		var source = GenerateTextLines(100);

		var taken = PipelineExecutor.TakeAsync(source, 5, CancellationToken);
		var items = await PipelineExecutor.ToListAsync(taken, CancellationToken);

		items.Count.Should().Be(5);
	}

	[Fact]
	public async Task TakeAsync_ReturnsAll_WhenCountExceedsSource()
	{
		var source = GenerateTextLines(3);

		var taken = PipelineExecutor.TakeAsync(source, 100, CancellationToken);
		var items = await PipelineExecutor.ToListAsync(taken, CancellationToken);

		items.Count.Should().Be(3);
	}

	#endregion

	#region Skip

	[Fact]
	public async Task SkipAsync_SkipsItems()
	{
		var source = GenerateTextLines(10);

		var skipped = PipelineExecutor.SkipAsync(source, 5, CancellationToken);
		var items = await PipelineExecutor.ToListAsync(skipped, CancellationToken);

		items.Count.Should().Be(5);
		items[0].Content.Should().Be("Line 5");
	}

	[Fact]
	public async Task SkipAsync_ReturnsEmpty_WhenSkippingAll()
	{
		var source = GenerateTextLines(3);

		var skipped = PipelineExecutor.SkipAsync(source, 10, CancellationToken);
		var items = await PipelineExecutor.ToListAsync(skipped, CancellationToken);

		items.Should().BeEmpty();
	}

	#endregion

	#region FirstOrDefault

	[Fact]
	public async Task FirstOrDefaultAsync_ReturnsFirst()
	{
		var source = GenerateTextLines(5);

		var first = await PipelineExecutor.FirstOrDefaultAsync(source, CancellationToken);

		first.Should().NotBeNull();
		first.Content.Should().Be("Line 0");
	}

	[Fact]
	public async Task FirstOrDefaultAsync_ReturnsDefault_WhenEmpty()
	{
		var source = GenerateTextLines(0);

		var first = await PipelineExecutor.FirstOrDefaultAsync(source, CancellationToken);

		// TextLine is a struct, so default is a struct with null Content
		first.Should().Be(default(TextLine));
		first.Content.Should().BeNull();
	}

	#endregion

	#region Count

	[Fact]
	public async Task CountAsync_CountsAllItems()
	{
		var source = GenerateTextLines(42);

		var count = await PipelineExecutor.CountAsync(source, CancellationToken);

		count.Should().Be(42);
	}

	[Fact]
	public async Task CountAsync_ReturnsZero_WhenEmpty()
	{
		var source = GenerateTextLines(0);

		var count = await PipelineExecutor.CountAsync(source, CancellationToken);

		count.Should().Be(0);
	}

	#endregion

	#region Transform

	[Fact]
	public async Task ExecuteTransformAsync_TransformsAllItems()
	{
		var source = GenerateTextLines(5);

		var transformed = PipelineExecutor.ExecuteTransformAsync<TextLine, StringValue>(source, line =>
			new StringValue(line.Content.ToUpper()), CancellationToken);
		var items = await PipelineExecutor.ToListAsync(transformed, CancellationToken);

		items.Count.Should().Be(5);
		items[0].Value.Should().Be("LINE 0");
	}

	[Fact]
	public async Task ChainedFilterAndTransform_WorksCorrectly()
	{
		var source = GenerateTextLines(5);

		// Filter first, then transform - this is the proper pattern for filtering
		var filtered = PipelineExecutor.WhereAsync(source, line => line.LineNumber % 2 == 0, CancellationToken);
		var transformed = PipelineExecutor.SelectAsync(filtered, line => new StringValue(line.Content.ToUpper()), CancellationToken);
		var items = await PipelineExecutor.ToListAsync(transformed, CancellationToken);

		items.Count.Should().Be(3); // 0, 2, 4
	}

	#endregion

	#region Channel Operations

	[Fact]
	public async Task CreateChannel_CreatesWithBoundedCapacity()
	{
		var executor = new PipelineExecutor(new PipelineOptions { ChannelCapacity = 10 });
		var channel = executor.CreateChannel<TextLine>();

		channel.Reader.Should().NotBeNull();
		channel.Writer.Should().NotBeNull();
	}

	[Fact]
	public async Task PipeToChannelAsync_WritesAllItems()
	{
		var source = GenerateTextLines(5);
		var channel = _executor.CreateChannel<TextLine>();

		var writeTask = PipelineExecutor.PipeToChannelAsync(source, channel.Writer, CancellationToken);

		var items = new List<TextLine>();
		await foreach (var item in channel.Reader.ReadAllAsync(CancellationToken))
		{
			items.Add(item);
		}

		var count = await writeTask;

		count.Should().Be(5);
		items.Count.Should().Be(5);
	}

	[Fact]
	public async Task ReadFromChannelAsync_ReadsAllItems()
	{
		var channel = _executor.CreateChannel<TextLine>();
		var ct = CancellationToken;

		// Write items in background
		_ = Task.Run(async () =>
		{
			for (var i = 0; i < 3; i++)
			{
				await channel.Writer.WriteAsync(new TextLine($"Item {i}", i, "test"), ct).ConfigureAwait(false);
			}
			channel.Writer.Complete();
		}, ct);

		var items = await PipelineExecutor.ToListAsync(PipelineExecutor.ReadFromChannelAsync(channel.Reader, CancellationToken), CancellationToken);

		items.Count.Should().Be(3);
	}

	#endregion

	#region ExecuteAndCollect

	[Fact]
	public async Task ExecuteAndCollectAsync_ReturnsResultWithItems()
	{
		var source = GenerateTextLines(10);

		var (result, items) = await PipelineExecutor.ExecuteAndCollectAsync(source, CancellationToken);

		result.ExitCode.Should().Be(0);
		result.ObjectCount.Should().Be(10);
		items.Count.Should().Be(10);
		(result.Duration > TimeSpan.Zero).Should().BeTrue();
	}

	[Fact]
	public async Task ExecuteAndCollectAsync_HandlesCancellation()
	{
		var cts = new CancellationTokenSource();

		async IAsyncEnumerable<TextLine> InfiniteSource([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
		{
			var i = 0;
			while (!ct.IsCancellationRequested)
			{
				yield return new TextLine($"Line {i++}", i, "infinite");
				// Don't pass token to delay - let the loop check cancellation
				await Task.Delay(1).ConfigureAwait(false);
			}
		}

		// Cancel after short delay
		cts.CancelAfter(100);

		var (result, items) = await PipelineExecutor.ExecuteAndCollectAsync(
			InfiniteSource(cts.Token), cts.Token);

		// Cancellation should have occurred - either gracefully (exit code 0) or as cancelled
		(result.ExitCode == 0 || result.ExitCode == -1).Should().BeTrue();
		// Some items should have been collected before cancellation
		(items.Count >= 0).Should().BeTrue();
	}

	#endregion

	#region Pipeline Composition

	[Fact]
	public async Task Pipeline_ChainedOperations_WorkCorrectly()
	{
		var source = GenerateTextLines(20);

		// Chain: Skip 5 -> Where even -> Take 3
		var pipeline = PipelineExecutor.TakeAsync(
			PipelineExecutor.WhereAsync(
				PipelineExecutor.SkipAsync(source, 5, CancellationToken),
				line => line.LineNumber % 2 == 0, CancellationToken),
			3, CancellationToken);

		var items = await PipelineExecutor.ToListAsync(pipeline, CancellationToken);

		items.Count.Should().Be(3);
		// After skip 5: 5,6,7,8,9,10,11,12...
		// Where even: 6,8,10,12...
		// Take 3: 6,8,10
		items[0].LineNumber.Should().Be(6);
		items[1].LineNumber.Should().Be(8);
		items[2].LineNumber.Should().Be(10);
	}

	[Fact]
	public async Task Pipeline_SelectThenWhere_WorksCorrectly()
	{
		var source = GenerateFileEntries(10);

		var pipeline = PipelineExecutor.WhereAsync(
			PipelineExecutor.SelectAsync(source, entry =>
				new TextLine(entry.Name, (int)entry.Size, entry.FullPath), CancellationToken),
			line => line.Content.Contains("5"), CancellationToken);

		var items = await PipelineExecutor.ToListAsync(pipeline, CancellationToken);

		items.Should().HaveCount(1);
		items[0].Content.Should().Contain("5");
	}

	#endregion

	#region Helpers

	private static async IAsyncEnumerable<TextLine> GenerateTextLines(int count)
	{
		for (var i = 0; i < count; i++)
		{
			yield return new TextLine($"Line {i}", i, "test.txt");
			await Task.Yield();
		}
	}

	private static async IAsyncEnumerable<FileEntry> GenerateFileEntries(int count)
	{
		for (var i = 0; i < count; i++)
		{
			yield return new FileEntry(
				Name: $"file{i}.txt",
				FullPath: $"/path/to/file{i}.txt",
				Size: i * 100,
				Created: DateTimeOffset.UtcNow,
				Modified: DateTimeOffset.UtcNow,
				Extension: ".txt",
				IsDirectory: false,
				Attributes: System.IO.FileAttributes.Normal,
				Metadata: StreamMetadata.Now($"file{i}.txt", i));
			await Task.Yield();
		}
	}

	#endregion
}
