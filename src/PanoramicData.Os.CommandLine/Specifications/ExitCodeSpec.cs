namespace PanoramicData.Os.CommandLine.Specifications;

/// <summary>
/// Specifies an exit code and its meaning for a command.
/// Uses HTTP status codes for familiar, well-defined semantics.
/// </summary>
/// <param name="Code">The exit code value (HTTP status code).</param>
/// <param name="Name">Short name for the exit code.</param>
/// <param name="Description">Description of what this exit code means.</param>
public sealed record ExitCodeSpec(int Code, string Name, string Description)
{
	/// <summary>
	/// Validates that the exit code is in a valid HTTP status code range.
	/// </summary>
	public bool IsValid => Code >= 100 && Code <= 599;

	/// <summary>
	/// Whether this is a success code (2xx).
	/// </summary>
	public bool IsSuccess => Code >= 200 && Code <= 299;

	/// <summary>
	/// Whether this is a client error code (4xx).
	/// </summary>
	public bool IsClientError => Code >= 400 && Code <= 499;

	/// <summary>
	/// Whether this is a server/system error code (5xx).
	/// </summary>
	public bool IsServerError => Code >= 500 && Code <= 599;
}

/// <summary>
/// Standard exit codes using HTTP status codes for familiar semantics.
/// </summary>
public static class StandardExitCodes
{
	// 2xx Success
	/// <summary>Command completed successfully (HTTP 200 OK).</summary>
	public static ExitCodeSpec Success { get; } = new(200, "OK", "Command completed successfully");

	/// <summary>Resource created successfully (HTTP 201 Created).</summary>
	public static ExitCodeSpec Created { get; } = new(201, "Created", "Resource created successfully");

	/// <summary>Request accepted for processing (HTTP 202 Accepted).</summary>
	public static ExitCodeSpec Accepted { get; } = new(202, "Accepted", "Request accepted for processing");

	/// <summary>Command completed with no content to return (HTTP 204 No Content).</summary>
	public static ExitCodeSpec NoContent { get; } = new(204, "NoContent", "Command completed with no content to return");

	// 4xx Client Errors
	/// <summary>Invalid command-line arguments (HTTP 400 Bad Request).</summary>
	public static ExitCodeSpec BadRequest { get; } = new(400, "BadRequest", "Invalid command-line arguments");

	/// <summary>Authentication required (HTTP 401 Unauthorized).</summary>
	public static ExitCodeSpec Unauthorized { get; } = new(401, "Unauthorized", "Authentication required");

	/// <summary>Insufficient permissions (HTTP 403 Forbidden).</summary>
	public static ExitCodeSpec Forbidden { get; } = new(403, "Forbidden", "Insufficient permissions");

	/// <summary>Resource not found (HTTP 404 Not Found).</summary>
	public static ExitCodeSpec NotFound { get; } = new(404, "NotFound", "Resource not found");

	/// <summary>Operation not allowed (HTTP 405 Method Not Allowed).</summary>
	public static ExitCodeSpec NotAllowed { get; } = new(405, "NotAllowed", "Operation not allowed");

	/// <summary>Operation timed out (HTTP 408 Request Timeout).</summary>
	public static ExitCodeSpec Timeout { get; } = new(408, "Timeout", "Operation timed out");

	/// <summary>Resource already exists or conflict (HTTP 409 Conflict).</summary>
	public static ExitCodeSpec Conflict { get; } = new(409, "Conflict", "Resource already exists or conflict");

	/// <summary>Resource no longer available (HTTP 410 Gone).</summary>
	public static ExitCodeSpec Gone { get; } = new(410, "Gone", "Resource no longer available");

	/// <summary>Precondition failed (HTTP 412 Precondition Failed).</summary>
	public static ExitCodeSpec PreconditionFailed { get; } = new(412, "PreconditionFailed", "Precondition failed");

	/// <summary>Request entity too large (HTTP 413 Payload Too Large).</summary>
	public static ExitCodeSpec TooLarge { get; } = new(413, "TooLarge", "Request entity too large");

	/// <summary>Unsupported format or type (HTTP 415 Unsupported Media Type).</summary>
	public static ExitCodeSpec UnsupportedType { get; } = new(415, "UnsupportedType", "Unsupported format or type");

	/// <summary>Too many requests (HTTP 429 Too Many Requests).</summary>
	public static ExitCodeSpec TooManyRequests { get; } = new(429, "TooManyRequests", "Too many requests");

	// 5xx Server/System Errors
	/// <summary>Internal error (HTTP 500 Internal Server Error).</summary>
	public static ExitCodeSpec InternalError { get; } = new(500, "InternalError", "Internal error");

	/// <summary>Not implemented (HTTP 501 Not Implemented).</summary>
	public static ExitCodeSpec NotImplemented { get; } = new(501, "NotImplemented", "Not implemented");

	/// <summary>Network or upstream error (HTTP 502 Bad Gateway).</summary>
	public static ExitCodeSpec NetworkError { get; } = new(502, "NetworkError", "Network or upstream error");

	/// <summary>Service unavailable (HTTP 503 Service Unavailable).</summary>
	public static ExitCodeSpec Unavailable { get; } = new(503, "Unavailable", "Service unavailable");

	/// <summary>Gateway timeout (HTTP 504 Gateway Timeout).</summary>
	public static ExitCodeSpec GatewayTimeout { get; } = new(504, "GatewayTimeout", "Gateway timeout");

	/// <summary>Insufficient storage (HTTP 507 Insufficient Storage).</summary>
	public static ExitCodeSpec InsufficientStorage { get; } = new(507, "InsufficientStorage", "Insufficient storage");

	// Aliases for backward compatibility and convenience
	/// <summary>Alias for BadRequest - Invalid arguments.</summary>
	public static ExitCodeSpec InvalidArguments => BadRequest;

	/// <summary>Alias for NotFound - File not found.</summary>
	public static ExitCodeSpec FileNotFound => NotFound;

	/// <summary>Alias for NotFound - Directory not found.</summary>
	public static ExitCodeSpec DirectoryNotFound => NotFound;

	/// <summary>Alias for Forbidden - Permission denied.</summary>
	public static ExitCodeSpec PermissionDenied => Forbidden;

	/// <summary>Alias for InternalError - I/O error.</summary>
	public static ExitCodeSpec IoError => InternalError;

	/// <summary>Alias for Conflict - Resource already exists.</summary>
	public static ExitCodeSpec AlreadyExists => Conflict;

	/// <summary>Alias for InternalError - General error.</summary>
	public static ExitCodeSpec GeneralError => InternalError;

	/// <summary>Alias for Unavailable - Invalid state.</summary>
	public static ExitCodeSpec InvalidState => Unavailable;

	/// <summary>Alias for Unavailable - Operation cancelled.</summary>
	public static ExitCodeSpec Cancelled => Unavailable;

	/// <summary>Gets all standard exit codes.</summary>
	public static IReadOnlyList<ExitCodeSpec> All { get; } =
	[
		// 2xx
		Success,
		Created,
		Accepted,
		NoContent,
		// 4xx
		BadRequest,
		Unauthorized,
		Forbidden,
		NotFound,
		NotAllowed,
		Timeout,
		Conflict,
		Gone,
		PreconditionFailed,
		TooLarge,
		UnsupportedType,
		TooManyRequests,
		// 5xx
		InternalError,
		NotImplemented,
		NetworkError,
		Unavailable,
		GatewayTimeout,
		InsufficientStorage
	];
}
