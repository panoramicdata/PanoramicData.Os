using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine.Logging;

/// <summary>
/// Default logger implementation that wraps an ILogger.
/// </summary>
/// <remarks>
/// Create a new PanLogger wrapping an ILogger.
/// </remarks>
public class PanLogger(ILogger logger, PanLogLevel minimumLevel = PanLogLevel.Information) : IPanLogger
{

	/// <inheritdoc />
	public void Trace(string message)
	{
		if (IsEnabled(PanLogLevel.Trace))
		{
			logger.LogTrace("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Debug(string message)
	{
		if (IsEnabled(PanLogLevel.Debug))
		{
			logger.LogDebug("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Info(string message)
	{
		if (IsEnabled(PanLogLevel.Information))
		{
			logger.LogInformation("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Warn(string message)
	{
		if (IsEnabled(PanLogLevel.Warning))
		{
			logger.LogWarning("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Error(string message)
	{
		if (IsEnabled(PanLogLevel.Error))
		{
			logger.LogError("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Error(Exception exception, string message)
	{
		if (IsEnabled(PanLogLevel.Error))
		{
			logger.LogError(exception, "{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Critical(string message)
	{
		if (IsEnabled(PanLogLevel.Critical))
		{
			logger.LogCritical("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Critical(Exception exception, string message)
	{
		if (IsEnabled(PanLogLevel.Critical))
		{
			logger.LogCritical(exception, "{Message}", message);
		}
	}

	/// <inheritdoc />
	public bool IsEnabled(PanLogLevel level)
	{
		return level >= minimumLevel && logger.IsEnabled(level.ToLogLevel());
	}
}
