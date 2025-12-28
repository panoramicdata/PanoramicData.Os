using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine.Logging;

/// <summary>
/// Console logger that writes to standard output with colored output.
/// Designed for PanoramicData.Os where we may not have a full logging infrastructure.
/// </summary>
public class ConsoleLogger : ILogger
{
	private readonly string _categoryName;
	private readonly LogLevel _minimumLevel;
	private readonly Action<string>? _writeAction;
	private static readonly object _lock = new();

	/// <summary>
	/// Create a new console logger.
	/// </summary>
	public ConsoleLogger(string categoryName, LogLevel minimumLevel = LogLevel.Information, Action<string>? writeAction = null)
	{
		_categoryName = categoryName;
		_minimumLevel = minimumLevel;
		_writeAction = writeAction;
	}

	/// <inheritdoc />
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

	/// <inheritdoc />
	public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		var message = formatter(state, exception);
		var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
		var levelStr = GetLevelString(logLevel);
		var colorCode = GetLevelColor(logLevel);
		var resetCode = "\x1b[0m";

		var formattedMessage = $"[{timestamp}] {colorCode}[{levelStr}]{resetCode} [{GetShortCategoryName(_categoryName)}] {message}";

		if (exception != null)
		{
			formattedMessage += Environment.NewLine + exception;
		}

		lock (_lock)
		{
			if (_writeAction != null)
			{
				_writeAction(formattedMessage);
			}
			else
			{
				Console.WriteLine(formattedMessage);
			}
		}
	}

	private static string GetLevelString(LogLevel level) => level switch
	{
		LogLevel.Trace => "TRACE",
		LogLevel.Debug => "DEBUG",
		LogLevel.Information => "INFO",
		LogLevel.Warning => "WARN",
		LogLevel.Error => "ERROR",
		LogLevel.Critical => "CRIT",
		_ => "UNKN"
	};

	private static string GetLevelColor(LogLevel level) => level switch
	{
		LogLevel.Trace => "\x1b[90m",    // Dark gray
		LogLevel.Debug => "\x1b[36m",    // Cyan
		LogLevel.Information => "\x1b[32m", // Green
		LogLevel.Warning => "\x1b[33m",  // Yellow
		LogLevel.Error => "\x1b[31m",    // Red
		LogLevel.Critical => "\x1b[35m", // Magenta
		_ => "\x1b[0m"                   // Reset
	};

	private static string GetShortCategoryName(string categoryName)
	{
		var lastDot = categoryName.LastIndexOf('.');
		return lastDot >= 0 ? categoryName[(lastDot + 1)..] : categoryName;
	}

	private sealed class NullScope : IDisposable
	{
		public static NullScope Instance { get; } = new();
		public void Dispose() { }
	}
}

/// <summary>
/// Factory for creating console loggers.
/// </summary>
public class ConsoleLoggerProvider : ILoggerProvider
{
	private readonly LogLevel _minimumLevel;
	private readonly Action<string>? _writeAction;

	/// <summary>
	/// Create a new console logger provider.
	/// </summary>
	public ConsoleLoggerProvider(LogLevel minimumLevel = LogLevel.Information, Action<string>? writeAction = null)
	{
		_minimumLevel = minimumLevel;
		_writeAction = writeAction;
	}

	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName)
	{
		return new ConsoleLogger(categoryName, _minimumLevel, _writeAction);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
