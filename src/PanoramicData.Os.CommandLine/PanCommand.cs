using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Result of command execution using HTTP status codes.
/// </summary>
public class CommandResult
{
	/// <summary>
	/// The exit code (HTTP status code: 2xx for success, 4xx for client errors, 5xx for server errors).
	/// </summary>
	public int ExitCode { get; init; }

	/// <summary>
	/// Whether the command executed successfully (2xx status code).
	/// </summary>
	public bool Success => ExitCode >= 200 && ExitCode <= 299;

	/// <summary>
	/// Optional error message if the command failed.
	/// </summary>
	public string? ErrorMessage { get; init; }

	// Standard HTTP success codes
	/// <summary>Create a successful result (200 OK).</summary>
	public static CommandResult Ok() => new() { ExitCode = 200 };

	/// <summary>Create a successful result indicating resource creation (201 Created).</summary>
	public static CommandResult Created() => new() { ExitCode = 201 };

	/// <summary>Create a successful result with no content (204 No Content).</summary>
	public static CommandResult NoContent() => new() { ExitCode = 204 };

	// Standard HTTP client error codes (4xx)
	/// <summary>Create an error for invalid arguments (400 Bad Request).</summary>
	public static CommandResult BadRequest(string? message = null) =>
		new() { ExitCode = 400, ErrorMessage = message };

	/// <summary>Create an error for permission issues (403 Forbidden).</summary>
	public static CommandResult Forbidden(string? message = null) =>
		new() { ExitCode = 403, ErrorMessage = message };

	/// <summary>Create an error for missing resources (404 Not Found).</summary>
	public static CommandResult NotFound(string? message = null) =>
		new() { ExitCode = 404, ErrorMessage = message };

	/// <summary>Create an error for resource conflicts (409 Conflict).</summary>
	public static CommandResult Conflict(string? message = null) =>
		new() { ExitCode = 409, ErrorMessage = message };

	// Standard HTTP server error codes (5xx)
	/// <summary>Create an error for internal failures (500 Internal Server Error).</summary>
	public static CommandResult InternalError(string? message = null) =>
		new() { ExitCode = 500, ErrorMessage = message };

	/// <summary>Create an error for network issues (502 Bad Gateway).</summary>
	public static CommandResult NetworkError(string? message = null) =>
		new() { ExitCode = 502, ErrorMessage = message };

	/// <summary>Create an error for unavailable services (503 Service Unavailable).</summary>
	public static CommandResult Unavailable(string? message = null) =>
		new() { ExitCode = 503, ErrorMessage = message };

	/// <summary>
	/// Create a custom error result with a specific HTTP status code.
	/// </summary>
	public static CommandResult Error(int exitCode, string? message = null) =>
		new() { ExitCode = exitCode, ErrorMessage = message };

	/// <summary>
	/// Create an error result with a generic failure (400 Bad Request).
	/// </summary>
	[Obsolete("Use BadRequest(), NotFound(), InternalError(), or other specific methods instead.")]
	public static CommandResult Fail(string message) =>
		new() { ExitCode = 400, ErrorMessage = message };
}

/// <summary>
/// Base class for all PanoramicData.Os commands.
/// All applications must inherit from this class and implement the required methods.
/// </summary>
public abstract class PanCommand
{
	/// <summary>
	/// The name of the command (what the user types).
	/// </summary>
	public abstract string Name { get; }

	/// <summary>
	/// Short description of the command for help text.
	/// </summary>
	public abstract string Description { get; }

	/// <summary>
	/// Detailed usage information.
	/// </summary>
	public virtual string Usage => Name;

	/// <summary>
	/// Examples of how to use the command.
	/// </summary>
	public virtual IEnumerable<string> Examples => [];

	/// <summary>
	/// Default execution mode for this command.
	/// </summary>
	public virtual ExecutionMode DefaultExecutionMode => ExecutionMode.Blocking;

	/// <summary>
	/// Whether this command supports non-blocking execution.
	/// </summary>
	public virtual bool SupportsNonBlocking => false;

	/// <summary>
	/// Configure the System.CommandLine Command with options and arguments.
	/// Override this to add custom options and arguments.
	/// </summary>
	/// <param name="command">The command to configure.</param>
	protected virtual void ConfigureCommand(Command command)
	{
		// Base implementation does nothing - override to add options/arguments
	}

	/// <summary>
	/// Execute the command asynchronously.
	/// This is the main entry point for command execution.
	/// </summary>
	/// <param name="context">The execution context containing all services and parameters.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>The command result.</returns>
	protected abstract Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken);

	/// <summary>
	/// Called when the command starts executing in non-blocking mode.
	/// </summary>
	protected virtual Task OnStartedAsync(CommandExecutionContext context, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	/// <summary>
	/// Called when the command is stopping in non-blocking mode.
	/// </summary>
	protected virtual Task OnStoppingAsync(CommandExecutionContext context, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	/// <summary>
	/// Build the System.CommandLine Command.
	/// </summary>
	internal Command BuildCommand()
	{
		var command = new Command(Name, Description);
		ConfigureCommand(command);
		return command;
	}

	/// <summary>
	/// Run the command with the given execution context.
	/// Called by the command runner.
	/// </summary>
	public async Task<int> RunAsync(CommandExecutionContext context, ExecutionMode mode)
	{
		try
		{
			context.Logger.LogDebug("Executing command {Command}", Name);

			if (mode == ExecutionMode.NonBlocking && SupportsNonBlocking)
			{
				return await RunNonBlockingAsync(context);
			}

			var result = await ExecuteAsync(context, context.CancellationToken);

			if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
			{
				context.Console.WriteError(result.ErrorMessage);
			}

			context.Logger.LogDebug("Command {Command} completed with exit code {ExitCode}", Name, result.ExitCode);
			return result.ExitCode;
		}
		catch (OperationCanceledException)
		{
			context.Logger.LogInformation("Command {Command} was cancelled", Name);
			return 503; // Service Unavailable (operation cancelled)
		}
		catch (Exception ex)
		{
			context.Logger.LogError(ex, "Command {Command} failed with exception", Name);
			context.Console.WriteError($"{Name}: {ex.Message}");
			return 500; // Internal Server Error
		}
	}

	private async Task<int> RunNonBlockingAsync(CommandExecutionContext context)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

		await OnStartedAsync(context, cts.Token);

		try
		{
			var result = await ExecuteAsync(context, cts.Token);
			return result.ExitCode;
		}
		finally
		{
			await OnStoppingAsync(context, cts.Token);
		}
	}
}
