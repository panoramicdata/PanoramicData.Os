using PanoramicData.Os.CommandLine.Streaming;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Base class for commands that both consume input and produce output.
/// This is the base for "filter" commands like where, select, grep.
/// </summary>
/// <typeparam name="TInput">The type of objects this command consumes.</typeparam>
/// <typeparam name="TOutput">The type of objects this command produces.</typeparam>
public abstract class StreamCommand<TInput, TOutput> : PanCommand
	where TInput : IStreamObject
	where TOutput : IStreamObject
{
	/// <summary>
	/// Execute the command, transforming input objects to output objects.
	/// </summary>
	/// <param name="context">The execution context containing parsed options and environment.</param>
	/// <param name="input">The input stream to process (null if no piped input).</param>
	/// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
	/// <returns>An async enumerable of output objects.</returns>
	protected abstract IAsyncEnumerable<TOutput> ExecuteStreamAsync(
		CommandExecutionContext context,
		IAsyncEnumerable<TInput>? input,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Execute the command, transforming input to output with the provided action.
	/// </summary>
	/// <param name="context">The execution context.</param>
	/// <param name="input">The input stream.</param>
	/// <param name="outputAction">Action to receive each output object.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The exit code.</returns>
	protected async Task<int> ExecuteWithOutputAsync(
		CommandExecutionContext context,
		IAsyncEnumerable<TInput>? input,
		Func<TOutput, ValueTask> outputAction,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await foreach (var item in ExecuteStreamAsync(context, input, cancellationToken).ConfigureAwait(false))
			{
				await outputAction(item).ConfigureAwait(false);
			}

			return 0;
		}
		catch (OperationCanceledException)
		{
			return 130;
		}
	}

	/// <summary>
	/// Get the async enumerable for pipeline execution.
	/// </summary>
	public IAsyncEnumerable<TOutput> GetOutputStream(
		CommandExecutionContext context,
		IAsyncEnumerable<TInput>? input,
		CancellationToken cancellationToken = default)
		=> ExecuteStreamAsync(context, input, cancellationToken);
}

/// <summary>
/// Base class for commands that consume input but don't produce output.
/// This is the base for "sink" commands that terminate a pipeline.
/// </summary>
/// <typeparam name="TInput">The type of objects this command consumes.</typeparam>
public abstract class SinkCommand<TInput> : PanCommand
	where TInput : IStreamObject
{
	/// <summary>
	/// Execute the command, consuming all input objects.
	/// </summary>
	/// <param name="context">The execution context containing parsed options and environment.</param>
	/// <param name="input">The input stream to consume.</param>
	/// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
	/// <returns>The exit code.</returns>
	protected abstract Task<int> ExecuteStreamAsync(
		CommandExecutionContext context,
		IAsyncEnumerable<TInput> input,
		CancellationToken cancellationToken = default);
}
