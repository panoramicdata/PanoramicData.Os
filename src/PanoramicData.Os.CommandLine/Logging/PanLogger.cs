using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine.Logging;

/// <summary>
/// Default logger implementation that wraps an ILogger.
/// </summary>
public class PanLogger : IPanLogger
{
	private readonly ILogger _logger;
	private readonly PanLogLevel _minimumLevel;

	/// <summary>
	/// Create a new PanLogger wrapping an ILogger.
	/// </summary>
	public PanLogger(ILogger logger, PanLogLevel minimumLevel = PanLogLevel.Information)
	{
		_logger = logger;
		_minimumLevel = minimumLevel;
	}

	/// <inheritdoc />
	public void Trace(string message)
	{
		if (IsEnabled(PanLogLevel.Trace))
		{
			_logger.LogTrace("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Debug(string message)
	{
		if (IsEnabled(PanLogLevel.Debug))
		{
			_logger.LogDebug("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Info(string message)
	{
		if (IsEnabled(PanLogLevel.Information))
		{
			_logger.LogInformation("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Warn(string message)
	{
		if (IsEnabled(PanLogLevel.Warning))
		{
			_logger.LogWarning("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Error(string message)
	{
		if (IsEnabled(PanLogLevel.Error))
		{
			_logger.LogError("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Error(Exception exception, string message)
	{
		if (IsEnabled(PanLogLevel.Error))
		{
			_logger.LogError(exception, "{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Critical(string message)
	{
		if (IsEnabled(PanLogLevel.Critical))
		{
			_logger.LogCritical("{Message}", message);
		}
	}

	/// <inheritdoc />
	public void Critical(Exception exception, string message)
	{
		if (IsEnabled(PanLogLevel.Critical))
		{
			_logger.LogCritical(exception, "{Message}", message);
		}
	}

	/// <inheritdoc />
	public bool IsEnabled(PanLogLevel level)
	{
		return level >= _minimumLevel && _logger.IsEnabled(level.ToLogLevel());
	}
}
