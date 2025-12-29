using PanoramicData.Os.CommandLine.Pipeline;
using PanoramicData.Os.CommandLine.Streaming;
using System.Threading.Channels;

namespace PanoramicData.Os.Init.Test.Streaming;

/// <summary>
/// Tests for pipeline definition types.
/// </summary>
public class PipelineTypesTests : BaseTest
{
	#region PipelineStage Tests

	[Fact]
	public void PipelineStage_CreatesWithCommandName()
	{
		var stage = new PipelineStage
		{
			CommandName = "ls",
			Arguments = new Dictionary<string, object?> { ["l"] = true }
		};

		stage.CommandName.Should().Be("ls");
		((bool?)stage.Arguments["l"]).Should().BeTrue();
		stage.InputType.Should().BeNull();
		stage.OutputType.Should().BeNull();
	}

	[Fact]
	public void PipelineStage_IsFirst_WhenIndexZero()
	{
		var stage = new PipelineStage
		{
			CommandName = "echo",
			Arguments = new Dictionary<string, object?>(),
			Index = 0
		};

		stage.IsFirst.Should().BeTrue();
	}

	[Fact]
	public void PipelineStage_IsNotFirst_WhenIndexNonZero()
	{
		var stage = new PipelineStage
		{
			CommandName = "grep",
			Arguments = new Dictionary<string, object?>(),
			Index = 1
		};

		stage.IsFirst.Should().BeFalse();
	}

	[Fact]
	public void PipelineStage_ConsumesInput_WhenHasInputType()
	{
		var stage = new PipelineStage
		{
			CommandName = "where",
			Arguments = new Dictionary<string, object?>(),
			InputType = typeof(TextLine)
		};

		stage.ConsumesInput.Should().BeTrue();
	}

	[Fact]
	public void PipelineStage_ProducesOutput_WhenHasOutputType()
	{
		var stage = new PipelineStage
		{
			CommandName = "ls",
			Arguments = new Dictionary<string, object?>(),
			OutputType = typeof(FileEntry)
		};

		stage.ProducesOutput.Should().BeTrue();
	}

	[Fact]
	public void PipelineStage_WithInputAndOutputTypes()
	{
		var stage = new PipelineStage
		{
			CommandName = "select",
			Arguments = new Dictionary<string, object?>(),
			InputType = typeof(FileEntry),
			OutputType = typeof(StringValue)
		};

		stage.InputType.Should().Be(typeof(FileEntry));
		stage.OutputType.Should().Be(typeof(StringValue));
	}

	[Fact]
	public void PipelineStage_CanSetChannels()
	{
		var channel = Channel.CreateUnbounded<IStreamObject>();
		var stage = new PipelineStage
		{
			CommandName = "cat",
			Arguments = new Dictionary<string, object?>()
		};

		stage.InputChannel = channel.Reader;
		stage.OutputChannel = channel.Writer;

		stage.InputChannel.Should().NotBeNull();
		stage.OutputChannel.Should().NotBeNull();
	}

	#endregion

	#region PipelineDefinition Tests

	[Fact]
	public void PipelineDefinition_HasStages_WhenNotEmpty()
	{
		var stages = new List<PipelineStage>
		{
			new() { CommandName = "ls", Arguments = new Dictionary<string, object?>() },
			new() { CommandName = "where", Arguments = new Dictionary<string, object?>() }
		};
		var definition = new PipelineDefinition(stages);

		definition.HasStages.Should().BeTrue();
		definition.Stages.Count.Should().Be(2);
	}

	[Fact]
	public void PipelineDefinition_HasStages_FalseWhenEmpty()
	{
		var definition = new PipelineDefinition([]);

		definition.HasStages.Should().BeFalse();
	}

	[Fact]
	public void PipelineDefinition_FirstStage_ReturnsFirst()
	{
		var stages = new List<PipelineStage>
		{
			new() { CommandName = "ls", Arguments = new Dictionary<string, object?>(), Index = 0 },
			new() { CommandName = "where", Arguments = new Dictionary<string, object?>(), Index = 1 }
		};
		var definition = new PipelineDefinition(stages);

		definition.FirstStage.Should().NotBeNull();
		definition.FirstStage.CommandName.Should().Be("ls");
	}

	[Fact]
	public void PipelineDefinition_LastStage_ReturnsLast()
	{
		var stages = new List<PipelineStage>
		{
			new() { CommandName = "ls", Arguments = new Dictionary<string, object?>() },
			new() { CommandName = "select", Arguments = new Dictionary<string, object?>() }
		};
		var definition = new PipelineDefinition(stages);

		definition.LastStage.Should().NotBeNull();
		definition.LastStage.CommandName.Should().Be("select");
	}

	[Fact]
	public void PipelineDefinition_OutputType_FromLastStage()
	{
		var stages = new List<PipelineStage>
		{
			new() { CommandName = "ls", Arguments = new Dictionary<string, object?>(), OutputType = typeof(FileEntry) },
			new() { CommandName = "select", Arguments = new Dictionary<string, object?>(), OutputType = typeof(StringValue) }
		};
		var definition = new PipelineDefinition(stages);

		definition.OutputType.Should().Be(typeof(StringValue));
	}

	[Fact]
	public void PipelineDefinition_NamedStreams_CanBeProvided()
	{
		var stages = new List<PipelineStage>
		{
			new() { CommandName = "ls", Arguments = new Dictionary<string, object?>() }
		};
		var namedStage = new PipelineStage { CommandName = "tee", Arguments = new Dictionary<string, object?>() };
		var namedStreams = new Dictionary<string, PipelineStage> { ["output"] = namedStage };
		var definition = new PipelineDefinition(stages, namedStreams);

		definition.NamedStreams.Should().HaveCount(1);
		definition.NamedStreams["output"].Should().BeSameAs(namedStage);
	}

	#endregion

	#region PipelineExecutionResult Tests

	[Fact]
	public void PipelineExecutionResult_Ok_CreatesSuccessResult()
	{
		var result = PipelineExecutionResult.Ok(100, TimeSpan.FromMilliseconds(50));

		result.ExitCode.Should().Be(0);
		result.Success.Should().BeTrue();
		result.ObjectCount.Should().Be(100);
		result.Duration.TotalMilliseconds.Should().Be(50);
		result.Error.Should().BeNull();
	}

	[Fact]
	public void PipelineExecutionResult_Failed_CreatesFailedResult()
	{
		var result = PipelineExecutionResult.Failed(1, "Command not found");

		result.ExitCode.Should().Be(1);
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Command not found");
	}

	[Fact]
	public void PipelineExecutionResult_Cancelled_CreatesSpecialCode()
	{
		var result = PipelineExecutionResult.Cancelled();

		result.ExitCode.Should().Be(130); // Standard Unix signal for SIGINT cancellation
		result.Success.Should().BeFalse();
	}

	[Fact]
	public void PipelineExecutionResult_StageExitCodes_TracksAllStages()
	{
		var result = new PipelineExecutionResult
		{
			ExitCode = 0,
			StageExitCodes = [0, 0, 1, 0]
		};

		result.StageExitCodes.Count.Should().Be(4);
		result.StageExitCodes[2].Should().Be(1);
	}

	#endregion

	#region PipelineOptions Tests

	[Fact]
	public void PipelineOptions_Default_HasReasonableDefaults()
	{
		var options = PipelineOptions.Default;

		options.ChannelCapacity.Should().Be(100);
		options.ContinueOnError.Should().BeFalse();
	}

	[Fact]
	public void PipelineOptions_CustomCapacity()
	{
		var options = new PipelineOptions { ChannelCapacity = 50 };

		options.ChannelCapacity.Should().Be(50);
	}

	#endregion
}
