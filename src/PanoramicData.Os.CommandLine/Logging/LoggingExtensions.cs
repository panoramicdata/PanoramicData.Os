using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine.Logging;

/// <summary>
/// Extension methods for adding logging to service collections.
/// </summary>
public static class LoggingExtensions
{
	/// <summary>
	/// Add PanoramicData.Os console logging to the service collection.
	/// </summary>
	public static IServiceCollection AddPanLogging(this IServiceCollection services, LogLevel minimumLevel = LogLevel.Information)
	{
		services.AddSingleton<ILoggerProvider>(new ConsoleLoggerProvider(minimumLevel));
		services.AddSingleton<ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>();
		services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
		return services;
	}

	/// <summary>
	/// Add PanoramicData.Os console logging with a custom write action.
	/// </summary>
	public static IServiceCollection AddPanLogging(this IServiceCollection services, Action<string> writeAction, LogLevel minimumLevel = LogLevel.Information)
	{
		services.AddSingleton<ILoggerProvider>(new ConsoleLoggerProvider(minimumLevel, writeAction));
		services.AddSingleton<ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>();
		services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
		return services;
	}

	/// <summary>
	/// Create a PanLogger for a specific category.
	/// </summary>
	public static IPanLogger CreatePanLogger(this ILoggerFactory factory, string categoryName, PanLogLevel minimumLevel = PanLogLevel.Information)
	{
		return new PanLogger(factory.CreateLogger(categoryName), minimumLevel);
	}

	/// <summary>
	/// Create a PanLogger for a specific type.
	/// </summary>
	public static IPanLogger CreatePanLogger<T>(this ILoggerFactory factory, PanLogLevel minimumLevel = PanLogLevel.Information)
	{
		return new PanLogger(factory.CreateLogger<T>(), minimumLevel);
	}
}

/// <summary>
/// Simple logger factory for standalone use.
/// </summary>
public static class PanLoggerFactory
{
	private static readonly Lazy<ILoggerFactory> _factory = new(() =>
	{
		var factory = new Microsoft.Extensions.Logging.LoggerFactory();
		factory.AddProvider(new ConsoleLoggerProvider(LogLevel.Information));
		return factory;
	});

	/// <summary>
	/// Get the default logger factory.
	/// </summary>
	public static ILoggerFactory Default => _factory.Value;

	/// <summary>
	/// Create a logger for a specific category.
	/// </summary>
	public static ILogger CreateLogger(string categoryName)
	{
		return Default.CreateLogger(categoryName);
	}

	/// <summary>
	/// Create a logger for a specific type.
	/// </summary>
	public static ILogger<T> CreateLogger<T>()
	{
		return Default.CreateLogger<T>();
	}

	/// <summary>
	/// Create a PanLogger for a specific category.
	/// </summary>
	public static IPanLogger CreatePanLogger(string categoryName, PanLogLevel minimumLevel = PanLogLevel.Information)
	{
		return new PanLogger(CreateLogger(categoryName), minimumLevel);
	}

	/// <summary>
	/// Create a PanLogger for a specific type.
	/// </summary>
	public static IPanLogger CreatePanLogger<T>(PanLogLevel minimumLevel = PanLogLevel.Information)
	{
		return new PanLogger(CreateLogger<T>(), minimumLevel);
	}
}
