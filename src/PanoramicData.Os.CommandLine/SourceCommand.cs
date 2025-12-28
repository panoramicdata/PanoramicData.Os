using PanoramicData.Os.CommandLine.Streaming;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Base class for commands that produce an output stream without consuming input.
/// This is the base for "source" commands like ls, cat, echo.
/// </summary>
/// <typeparam name="TOutput">The type of objects this command produces.</typeparam>
public abstract class SourceCommand<TOutput> : PanCommand
	where TOutput : IStreamObject
{
	/// <summary>
	/// Execute the command and yield output objects.
	/// </summary>
	/// <param name="context">The execution context containing parsed options and environment.</param>
	/// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
	/// <returns>An async enumerable of output objects.</returns>
	protected abstract IAsyncEnumerable<TOutput> ExecuteStreamAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Execute the command, yielding objects to the provided output action.
	/// This bridges the streaming model with the traditional execution model.
	/// </summary>
	/// <param name="context">The execution context.</param>
	/// <param name="outputAction">Action to receive each output object.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The exit code.</returns>
	protected async Task<int> ExecuteWithOutputAsync(
		CommandExecutionContext context,
		Func<TOutput, ValueTask> outputAction,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await foreach (var item in ExecuteStreamAsync(context, cancellationToken).ConfigureAwait(false))
			{
				await outputAction(item).ConfigureAwait(false);
			}

			return 0;
		}
		catch (OperationCanceledException)
		{
			return 130; // Standard cancellation exit code
		}
	}

	/// <summary>
	/// Get the async enumerable for pipeline execution.
	/// </summary>
	public IAsyncEnumerable<TOutput> GetOutputStream(
		CommandExecutionContext context,
		CancellationToken cancellationToken = default)
		=> ExecuteStreamAsync(context, cancellationToken);
}
