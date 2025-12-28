using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine.Logging;

/// <summary>
/// Log levels for PanoramicData.Os.
/// </summary>
public enum PanLogLevel
{
	Trace,
	Debug,
	Information,
	Warning,
	Error,
	Critical,
	None
}

/// <summary>
/// Simple logger interface for PanoramicData.Os applications.
/// </summary>
public interface IPanLogger
{
	/// <summary>
	/// Log a trace message.
	/// </summary>
	void Trace(string message);

	/// <summary>
	/// Log a debug message.
	/// </summary>
	void Debug(string message);

	/// <summary>
	/// Log an information message.
	/// </summary>
	void Info(string message);

	/// <summary>
	/// Log a warning message.
	/// </summary>
	void Warn(string message);

	/// <summary>
	/// Log an error message.
	/// </summary>
	void Error(string message);

	/// <summary>
	/// Log an error with exception.
	/// </summary>
	void Error(Exception exception, string message);

	/// <summary>
	/// Log a critical error.
	/// </summary>
	void Critical(string message);

	/// <summary>
	/// Log a critical error with exception.
	/// </summary>
	void Critical(Exception exception, string message);

	/// <summary>
	/// Check if a log level is enabled.
	/// </summary>
	bool IsEnabled(PanLogLevel level);
}

/// <summary>
/// Converts PanLogLevel to Microsoft.Extensions.Logging.LogLevel.
/// </summary>
public static class LogLevelExtensions
{
	/// <summary>
	/// Convert PanLogLevel to LogLevel.
	/// </summary>
	public static LogLevel ToLogLevel(this PanLogLevel level) => level switch
	{
		PanLogLevel.Trace => LogLevel.Trace,
		PanLogLevel.Debug => LogLevel.Debug,
		PanLogLevel.Information => LogLevel.Information,
		PanLogLevel.Warning => LogLevel.Warning,
		PanLogLevel.Error => LogLevel.Error,
		PanLogLevel.Critical => LogLevel.Critical,
		PanLogLevel.None => LogLevel.None,
		_ => LogLevel.Information
	};

	/// <summary>
	/// Convert LogLevel to PanLogLevel.
	/// </summary>
	public static PanLogLevel ToPanLogLevel(this LogLevel level) => level switch
	{
		LogLevel.Trace => PanLogLevel.Trace,
		LogLevel.Debug => PanLogLevel.Debug,
		LogLevel.Information => PanLogLevel.Information,
		LogLevel.Warning => PanLogLevel.Warning,
		LogLevel.Error => PanLogLevel.Error,
		LogLevel.Critical => PanLogLevel.Critical,
		LogLevel.None => PanLogLevel.None,
		_ => PanLogLevel.Information
	};
}
