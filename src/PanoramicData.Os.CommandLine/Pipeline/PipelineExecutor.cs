using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using PanoramicData.Os.CommandLine.Streaming;

namespace PanoramicData.Os.CommandLine.Pipeline;

/// <summary>
/// Executes pipelines by wiring up stages with channels and managing backpressure.
/// </summary>
public class PipelineExecutor
{
	private readonly PipelineOptions _options;

	/// <summary>
	/// Create a new pipeline executor with the specified options.
	/// </summary>
	public PipelineExecutor(PipelineOptions? options = null)
	{
		_options = options ?? PipelineOptions.Default;
	}

	/// <summary>
	/// Execute a simple single-source pipeline that yields objects.
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="source">The source async enumerable.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The async enumerable of objects.</returns>
	public static async IAsyncEnumerable<T> ExecuteSourceAsync<T>(
		IAsyncEnumerable<T> source,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where T : IStreamObject
	{
		await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			yield return item;
		}
	}

	/// <summary>
	/// Execute a filter stage that transforms input to output.
	/// </summary>
	/// <typeparam name="TIn">Input type.</typeparam>
	/// <typeparam name="TOut">Output type.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="transform">The transformation function.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The transformed stream.</returns>
	public static async IAsyncEnumerable<TOut> ExecuteTransformAsync<TIn, TOut>(
		IAsyncEnumerable<TIn> input,
		Func<TIn, TOut?> transform,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where TIn : IStreamObject
		where TOut : IStreamObject
	{
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			var result = transform(item);
			if (result is not null)
			{
				yield return result;
			}
		}
	}

	/// <summary>
	/// Execute a filter stage with an async transformation.
	/// </summary>
	public static async IAsyncEnumerable<TOut> ExecuteTransformAsync<TIn, TOut>(
		IAsyncEnumerable<TIn> input,
		Func<TIn, ValueTask<TOut?>> transform,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where TIn : IStreamObject
		where TOut : IStreamObject
	{
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			var result = await transform(item).ConfigureAwait(false);
			if (result is not null)
			{
				yield return result;
			}
		}
	}

	/// <summary>
	/// Execute a where (filter) operation.
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="predicate">The filter predicate.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The filtered stream.</returns>
	public static async IAsyncEnumerable<T> WhereAsync<T>(
		IAsyncEnumerable<T> input,
		Func<T, bool> predicate,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where T : IStreamObject
	{
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			if (predicate(item))
			{
				yield return item;
			}
		}
	}

	/// <summary>
	/// Execute a select (projection) operation.
	/// </summary>
	/// <typeparam name="TIn">Input type.</typeparam>
	/// <typeparam name="TOut">Output type.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="selector">The projection function.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The projected stream.</returns>
	public static async IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(
		IAsyncEnumerable<TIn> input,
		Func<TIn, TOut> selector,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where TIn : IStreamObject
		where TOut : IStreamObject
	{
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			yield return selector(item);
		}
	}

	/// <summary>
	/// Execute a take operation (limit number of items).
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="count">Maximum number of items to take.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The limited stream.</returns>
	public static async IAsyncEnumerable<T> TakeAsync<T>(
		IAsyncEnumerable<T> input,
		int count,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where T : IStreamObject
	{
		var taken = 0;
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			if (taken >= count)
			{
				yield break;
			}

			yield return item;
			taken++;
		}
	}

	/// <summary>
	/// Execute a skip operation.
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="count">Number of items to skip.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The stream after skipping.</returns>
	public static async IAsyncEnumerable<T> SkipAsync<T>(
		IAsyncEnumerable<T> input,
		int count,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where T : IStreamObject
	{
		var skipped = 0;
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			if (skipped < count)
			{
				skipped++;
				continue;
			}

			yield return item;
		}
	}

	/// <summary>
	/// Execute a first operation (take first item or default).
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The first item or default.</returns>
	public static async Task<T?> FirstOrDefaultAsync<T>(
		IAsyncEnumerable<T> input,
		CancellationToken cancellationToken)
		where T : IStreamObject
	{
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			return item;
		}

		return default;
	}

	/// <summary>
	/// Execute a count operation.
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The count of items.</returns>
	public static async Task<long> CountAsync<T>(
		IAsyncEnumerable<T> input,
		CancellationToken cancellationToken)
		where T : IStreamObject
	{
		long count = 0;
		await foreach (var _ in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			count++;
		}

		return count;
	}

	/// <summary>
	/// Collect all items from a stream into a list.
	/// </summary>
	/// <typeparam name="T">The type of objects in the stream.</typeparam>
	/// <param name="input">The input stream.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A list of all items.</returns>
	public static async Task<List<T>> ToListAsync<T>(
		IAsyncEnumerable<T> input,
		CancellationToken cancellationToken)
		where T : IStreamObject
	{
		var list = new List<T>();
		await foreach (var item in input.WithCancellation(cancellationToken).ConfigureAwait(false))
		{
			list.Add(item);
		}

		return list;
	}

	/// <summary>
	/// Create a bounded channel for passing objects between stages.
	/// </summary>
	/// <typeparam name="T">The type of objects in the channel.</typeparam>
	/// <returns>A channel with bounded capacity.</returns>
	public Channel<T> CreateChannel<T>() where T : IStreamObject
	{
		return Channel.CreateBounded<T>(new BoundedChannelOptions(_options.ChannelCapacity)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = true,
			SingleWriter = true
		});
	}

	/// <summary>
	/// Pipe a source into a channel, completing the channel when done.
	/// </summary>
	/// <typeparam name="T">The type of objects.</typeparam>
	/// <param name="source">The source stream.</param>
	/// <param name="channel">The target channel.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The number of items written.</returns>
	public static async Task<long> PipeToChannelAsync<T>(
		IAsyncEnumerable<T> source,
		ChannelWriter<T> channel,
		CancellationToken cancellationToken)
		where T : IStreamObject
	{
		long count = 0;
		try
		{
			await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
			{
				await channel.WriteAsync(item, cancellationToken).ConfigureAwait(false);
				count++;
			}
		}
		finally
		{
			channel.Complete();
		}

		return count;
	}

	/// <summary>
	/// Read all items from a channel as an async enumerable.
	/// </summary>
	/// <typeparam name="T">The type of objects.</typeparam>
	/// <param name="channel">The source channel.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>An async enumerable of items from the channel.</returns>
	public static async IAsyncEnumerable<T> ReadFromChannelAsync<T>(
		ChannelReader<T> channel,
		[EnumeratorCancellation] CancellationToken cancellationToken)
		where T : IStreamObject
	{
		await foreach (var item in channel.ReadAllAsync(cancellationToken).ConfigureAwait(false))
		{
			yield return item;
		}
	}

	/// <summary>
	/// Execute a full pipeline and collect results.
	/// </summary>
	/// <typeparam name="T">The output type of the pipeline.</typeparam>
	/// <param name="source">The source stream.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The execution result with collected items.</returns>
	public static async Task<(PipelineExecutionResult Result, List<T> Items)> ExecuteAndCollectAsync<T>(
		IAsyncEnumerable<T> source,
		CancellationToken cancellationToken)
		where T : IStreamObject
	{
		var stopwatch = Stopwatch.StartNew();
		var items = new List<T>();

		try
		{
			await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
			{
				items.Add(item);
			}

			stopwatch.Stop();
			return (PipelineExecutionResult.Ok(items.Count, stopwatch.Elapsed), items);
		}
		catch (OperationCanceledException)
		{
			stopwatch.Stop();
			return (PipelineExecutionResult.Cancelled(), items);
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			return (PipelineExecutionResult.Failed(1, ex.Message), items);
		}
	}
}
