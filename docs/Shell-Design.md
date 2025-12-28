# PanoramicData.Os Shell Design

## Overview

PanoramicData.Os implements a novel shell architecture where **commands are fully introspectable, type-safe, and validatable before execution**. Unlike traditional Unix shells that treat everything as text streams, this shell operates on **typed object streams** with compile-time validation using C# and Roslyn.

The shell acts as a **type-safe query engine** over object streams, similar to LINQ at the command line.

---

## Core Principles

1. **Pre-execution Validation**: The shell validates command syntax, types, and stream compatibility *before* the user presses Enter
2. **Self-describing Commands**: Every command declares its specification (options, streams, exit codes) statically
3. **Typed Object Streams**: Commands emit and consume `ObjectStream<T>` rather than raw bytes
4. **Live Feedback**: Visual indicators show validity state in real-time (red/green/flashing)
5. **No Implicit Streams**: No stdout/stderr/stdin - all streams are explicitly named

---

## Command Specification

Every command that inherits from `ShellCommand` must provide a `ShellCommandSpecification`:

```csharp
public class LsCommand : ShellCommand
{
    public static readonly ShellCommandSpecification Specification = new()
    {
        Name = "ls",
        Description = "List directory contents",
        Usage = "ls [options] [path]",
        
        Options = [
            new OptionSpec<DirectoryInfo>("path", 
                shortName: null,
                longName: null,
                description: "Directory to list",
                isPositional: true,
                required: false,
                defaultValue: "."),
            new OptionSpec<bool>("all",
                shortName: "a",
                longName: "all", 
                description: "Show hidden files",
                required: false,
                defaultValue: false)
        ],
        
        InputStreams = [],
        
        OutputStreams = [
            new StreamSpec<FileSystemEntry>("entries", 
                StreamRequirement.Required,
                description: "File and directory entries")
        ],
        
        ExitCodes = [
            new ExitCodeSpec(0, "Success", "Directory listed successfully"),
            new ExitCodeSpec(1, "NotFound", "Directory does not exist"),
            new ExitCodeSpec(2, "PermissionDenied", "Cannot read directory")
        ],
        
        ExecutionMode = ExecutionMode.Blocking
    };
}
```

---

## Option Specifications

### Supported Types

Options are strongly typed with the following supported types:

| Type | Description | Example |
|------|-------------|---------|
| `bool` | Boolean flag | `--verbose` |
| `int`, `long` | Integers | `--count 10` |
| `float`, `double`, `decimal` | Floating point | `--threshold 0.5` |
| `string` | Text | `--name "test"` |
| `char` | Single character | `--delimiter ','` |
| `DateTime`, `DateTimeOffset` | Date/time | `--after 2024-01-01` |
| `TimeSpan` | Duration | `--timeout 30s` |
| `Guid` | Unique identifier | `--id abc123...` |
| `FileInfo` | File reference | `--input file.txt` |
| `DirectoryInfo` | Directory reference | `--output /tmp/` |
| `Uri` | URI/URL | `--endpoint https://...` |
| `IPAddress` | IP address | `--host 192.168.1.1` |
| `Stream` | Binary stream | (for piped data) |

### File/Directory Validation

FileInfo and DirectoryInfo options support validation constraints:

```csharp
new OptionSpec<FileInfo>("input",
    mustExist: true,        // File must exist
    mustBeReadable: true,   // Must have read permission
    mustBeWritable: false)  // Write permission not required

new OptionSpec<DirectoryInfo>("output",
    mustExist: false,       // Will be created if needed
    mustBeWritable: true)   // Must be writable
```

**Validation Timing**:
- **Parse time**: Warn if constraints not met (visual indicator)
- **Execution time**: Error if constraints not met
- **Live updates**: Visual state changes as file system changes (e.g., red → green when file appears)

### Option Constraints

```csharp
new OptionSpec<int>("count",
    min: 1,
    max: 100,
    required: true)

new OptionSpec<string>("format",
    allowedValues: ["json", "xml", "csv"],
    required: false,
    defaultValue: "json")
```

---

## Stream Specifications

### Stream Requirements

Streams can be marked with three requirement levels:

```csharp
public enum StreamRequirement
{
    Required,       // Must be connected or command cannot execute
    Optional,       // May be connected
    Conditional     // Required based on runtime condition (lambda)
}
```

### Conditional Streams

```csharp
new StreamSpec<ErrorInfo>("errors",
    StreamRequirement.Conditional,
    condition: context => context.Options.Get<bool>("includeErrors"))
```

### Input and Output Streams

Commands declare both input and output streams:

```csharp
// A filter command
InputStreams = [
    new StreamSpec<T>("input", StreamRequirement.Required)
],
OutputStreams = [
    new StreamSpec<T>("output", StreamRequirement.Required)
]

// A command with multiple outputs
OutputStreams = [
    new StreamSpec<FileSystemEntry>("files", StreamRequirement.Required),
    new StreamSpec<ErrorInfo>("errors", StreamRequirement.Optional)
]
```

### Unconnected Required Streams

If a required output stream is not connected:
- Command **cannot execute** (Enter key is ignored)
- User receives feedback (audible beep and/or visual flash)
- User must either:
  - Pipe to another command: `ls | tabulate`
  - Explicitly discard: `ls | blackhole`

---

## Object Streams

### Concept

Commands communicate via `ObjectStream<T>` - typed, serialized object streams:

```
ls | filter "x => x.Size > 1024" | sort "x => x.Name" | tabulate
```

- `ls` emits `ObjectStream<FileSystemEntry>`
- `filter` receives and emits `ObjectStream<FileSystemEntry>`
- `sort` receives and emits `ObjectStream<FileSystemEntry>`
- `tabulate` receives `ObjectStream<T>` and renders to display

### Serialization

Object streams are serialized using ProtoBuf (or similar) for:
- Efficient binary encoding
- Schema preservation
- Cross-process communication

### Auto-Tabulation

When an `ObjectStream<T>` reaches the terminal (no downstream command), it is automatically rendered via `tabulate` which:
- Detects properties of `T`
- Formats as a table or appropriate display
- Handles pagination for large streams

---

## Type Propagation and Validation

### Generic Commands

Commands like `filter`, `sort`, `select` are generic and bind their type parameter from upstream:

```csharp
public class FilterCommand<T> : ShellCommand
{
    // T is bound at parse time from upstream command's output type
}
```

### Pipeline Validation

At parse time, the shell:
1. Resolves each command's specification
2. Propagates output types through the pipeline
3. Validates stream compatibility between adjacent commands
4. Compiles C# expressions against the known types
5. Reports errors before execution

### C# Expressions

Filter, sort, and projection expressions use C# syntax validated by Roslyn:

```bash
ls | filter "x => x.Created > DateTime.UtcNow.AddDays(-1)"
ls | sort "x => x.Size" descending
ls | select "x => new { x.Name, SizeKb = x.Size / 1024 }"
```

Benefits:
- Full C# expression power
- Roslyn validates at parse time
- IntelliSense/tab-completion knows the schema
- No custom DSL to learn

---

## Exit Codes

Exit codes use **HTTP status codes** for familiar, well-defined semantics that developers already know.

### Success Codes (2xx)

| Code | Name | Description |
|------|------|-------------|
| 200 | OK | Command completed successfully |
| 201 | Created | Resource created successfully |
| 202 | Accepted | Request accepted for processing |
| 204 | NoContent | Command completed with no content to return |

### Client Error Codes (4xx)

| Code | Name | Description |
|------|------|-------------|
| 400 | BadRequest | Invalid command-line arguments |
| 401 | Unauthorized | Authentication required |
| 403 | Forbidden | Insufficient permissions |
| 404 | NotFound | Resource not found |
| 405 | NotAllowed | Operation not allowed |
| 408 | Timeout | Operation timed out |
| 409 | Conflict | Resource already exists or conflict |
| 410 | Gone | Resource no longer available |
| 412 | PreconditionFailed | Precondition failed |
| 413 | TooLarge | Request entity too large |
| 415 | UnsupportedType | Unsupported format or type |
| 429 | TooManyRequests | Too many requests |

### Server/System Error Codes (5xx)

| Code | Name | Description |
|------|------|-------------|
| 500 | InternalError | Internal error |
| 501 | NotImplemented | Not implemented |
| 502 | NetworkError | Network or upstream error |
| 503 | Unavailable | Service unavailable |
| 504 | GatewayTimeout | Gateway timeout |
| 507 | InsufficientStorage | Insufficient storage |

### Convenience Aliases

For backward compatibility and semantic clarity, aliases are provided:

```csharp
StandardExitCodes.FileNotFound      // → 404 NotFound
StandardExitCodes.DirectoryNotFound // → 404 NotFound
StandardExitCodes.PermissionDenied  // → 403 Forbidden
StandardExitCodes.InvalidArguments  // → 400 BadRequest
StandardExitCodes.AlreadyExists     // → 409 Conflict
StandardExitCodes.IoError           // → 500 InternalError
StandardExitCodes.GeneralError      // → 500 InternalError
```

### Command-Specific Exit Codes

Commands may use any valid HTTP status code:

```csharp
ExitCodes = [
    StandardExitCodes.Success,        // 200
    StandardExitCodes.NotFound,       // 404
    new ExitCodeSpec(422, "ValidationFailed", "Input validation failed")
]
```

---

## Execution Mode

### Modes

```csharp
public enum ExecutionMode
{
    Blocking,       // Command runs to completion before returning
    Background      // Command runs in background, shell returns immediately
}
```

### Declaration

Commands declare their default execution mode:

```csharp
ExecutionMode = ExecutionMode.Blocking  // Default for most commands
ExecutionMode = ExecutionMode.Background // For servers, daemons
```

### User Override

Users can override for the terminal command in a pipeline:

```bash
server &              # Run in background (explicit)
ls | filter ...       # Blocking (default)
```

Background execution only applies to the final command in a pipeline.

---

## Live Shell Feedback

### Visual Indicators

The shell provides real-time visual feedback:

| State | Visual | Meaning |
|-------|--------|---------|
| Valid, ready | Normal/green text | Press Enter to execute |
| Syntax error | Red text | Cannot parse |
| Type mismatch | Red text | Stream types incompatible |
| File missing | Red text → green | Waiting for file to exist |
| Stream unconnected | Flashing | Required output has no receiver |

### Audible Feedback

- **Beep**: Attempted to execute invalid command
- **(Future)**: Configurable sounds for different states

### Tab Completion

The shell knows:
- Available commands and their options
- Properties of `T` in `ObjectStream<T>` for expressions
- File/directory paths with validation state

---

## Logging Architecture

### Three Output Channels

Commands have three distinct output channels:

| Channel | Purpose | Destination |
|---------|---------|-------------|
| **Tracing** | Debug/diagnostic info | OS-level trace sink (future) |
| **Logging** | Structured logs via `ILogger` | OS-level log sink(s) |
| **Streams** | Command data output | Named `ObjectStream<T>` |

### No stdout/stderr

Traditional stdout/stderr do not exist. Instead:
- **Data output** → Named object streams
- **Error messages** → Logging channel
- **Debug info** → Tracing channel

---

## Built-in Commands

### LINQ Query Operators

| Command | Description | Example |
|---------|-------------|---------|
| `where "predicate"` | Filter stream by C# predicate | `where x => x.Size > 1024` |
| `select "projection"` | Project to new type | `select x => { x.Name, x.Size }` |
| `orderBy "keySelector"` | Sort ascending | `orderBy x => x.Name` |
| `orderBy "keySelector" desc` | Sort descending | `orderBy x => x.Size desc` |
| `take n` | Take first n items | `take 10` |
| `skip n` | Skip first n items | `skip 5` |
| `distinct` | Remove duplicates | `distinct` |
| `groupBy "keySelector"` | Group by key | `groupBy x => x.Extension` |
| `first` | Take first item | `first` |
| `last` | Take last item | `last` |
| `count` | Count items | `count` |
| `any "predicate"` | Check if any match | `any x => x.Size > 0` |
| `all "predicate"` | Check if all match | `all x => x.IsReadable` |

### Stream Utilities

| Command | Description |
|---------|-------------|
| `tabulate` | Render `ObjectStream<T>` as formatted table |
| `blackhole` | Consume and discard `ObjectStream<T>` |
| `tee @streamName` | Split stream to named destination |
| `merge @stream1 @stream2` | Combine multiple streams into one |

### Output Type Selection

Commands can specify their output type using `--as`:

```bash
cat file.txt                    # Default: TextLine
cat --as=char file.txt          # Character stream
cat --as=TextChunk file.txt     # Chunked with metadata

ls                              # Default: FileSystemEntry
ls --as=string                  # Just names as strings
```

Commands declare supported output types in their specification:

```csharp
OutputTypes = [
    typeof(TextLine),      // Default (first in list)
    typeof(TextChunk),
    typeof(char)
],
```

---

## Stream Splitting with Tee

The `tee` command splits a stream to a named destination while continuing the main pipeline:

```bash
# Single line - split to visualizer, continue to main output
http --as=AudioStream url | tee @audio | mixer --volume=50%

# Named stream can be consumed elsewhere
@audio | visualizer --type=spectrum

# Multi-line for readability
http --as=AudioStream url 
    | tee @forRecording
    | mixer --volume=50%;
@forRecording | save recording.wav
```

Named streams use `@name` syntax and are scoped to the current command block.

---

## LINQ Pipeline with IAsyncQueryable

The shell supports full C# LINQ method syntax with `IAsyncQueryable` propagation:

```bash
# Simple projection
ls | select x => x.Name

# Anonymous type projection  
ls | select x => { x.Name, x.Size, IsLarge: x.Size > 1000000 }

# Full query pipeline
ls | where x => x.Extension == ".cs" 
   | select x => { x.Name, x.Size } 
   | orderBy x => x.Size desc 
   | take 10

# Aggregation
ls | groupBy x => x.Extension 
   | select g => { Extension: g.Key, Count: g.Count(), TotalSize: g.Sum(f => f.Size) }
```

### Type Inference

Type inference flows through the pipeline:
1. Command output type determined by `--as` flag or default
2. Each LINQ operator infers `x` type from upstream
3. Tab-completion works inside lambdas based on inferred type

```bash
ls | select x => x.<TAB>
#                  ↑ shows: Name, Size, Extension, Created, Modified, etc.
```

### Query Provider Benefits

For data sources implementing `IAsyncQueryable`, expressions can be pushed to the source:

```bash
# Could translate to SQL/OData query on server
db.Users | where x => x.Age > 21 | select x => x.Name | take 100
```

---

## Built-in Commands

---

## Example Pipelines

```bash
# List large files modified recently
ls | filter "x => x.Size > 1_000_000 && x.Modified > DateTime.UtcNow.AddDays(-7)"
   | sort "x => x.Size" desc
   | select "x => new { x.Name, SizeMb = x.Size / 1_000_000.0, x.Modified }"
   | tabulate

# Route compiler output to different destinations
compile src/*.cs | route {
    [artifacts] > /build/,
    [warnings] | filter "w => w.Level >= WarningLevel.Medium" > warnings.log,
    [errors] > errors.log
}

# Process with explicit stream discard
analyze data.csv | [summary] > report.txt, [details] | blackhole
```

---

## Future Considerations

- **Roslyn integration** for expression compilation and IntelliSense
- **Remote execution** - serialize commands across machines
- **Persistent pipelines** - save and replay pipeline definitions
- **GUI generation** - auto-generate forms from command specifications
- **API exposure** - expose commands as HTTP/gRPC endpoints

---

## Implementation Status

| Component | Status |
|-----------|--------|
| `ShellCommandSpecification` | 🟢 Implemented |
| `OptionSpec<T>` | 🟢 Implemented |
| `StreamSpec<T>` | 🟢 Implemented |
| `ExitCodeSpec` | 🟢 Implemented |
| Standard exit codes | 🟢 Implemented |
| Command specs on all commands | 🟢 Implemented |
| Tab completion with rotation | 🟢 Implemented |
| Context-aware path completion | 🟢 Implemented |
| Invalid path highlighting | 🟢 Implemented |
| Line validation with flash | 🟢 Implemented |
| Cross-platform path resolution | 🟢 Implemented |
| Pipeline type validation | 🟡 In Progress |
| `IAsyncEnumerable<T>` streaming | 🟡 In Progress |
| C# expression parsing (Roslyn) | 🔴 Not started |
| Live visual feedback | 🟡 Partial |
| `ObjectStream<T>` serialization | 🔴 Not started |
| Built-in LINQ commands | 🔴 Not started |
| Tee/named streams | 🔴 Not started |
| Formatter registry | 🔴 Not started |

---

## Streaming Implementation Plan

### Phase 1: Core Infrastructure (Current)

**Goal**: Commands yield typed objects instead of writing to console.

1. Add `IStreamObject` base interface to `PanoramicData.Os.CommandLine`
2. Create core stream types: `TextLine`, `TextChunk`, `FileSystemEntry`
3. Update `CommandExecutionContext` to support object output
4. Add `OutputTypes` property to `ShellCommandSpecification`
5. Implement channel-based pipeline executor with backpressure
6. Create default console formatter

**Commands to update**:
- `ls` → yields `IAsyncEnumerable<FileSystemEntry>`
- `cat` → yields `IAsyncEnumerable<TextLine>` (default), supports `--as=char|TextChunk`

### Phase 2: LINQ Operators

**Goal**: Basic query operators with type checking.

1. Add Roslyn NuGet packages for expression parsing
2. Implement expression parser for lambdas
3. Create `where`, `select`, `orderBy`, `take`, `skip` commands
4. Pipeline type validation at parse time
5. Tab completion inside lambda expressions

### Phase 3: Advanced Features

**Goal**: Full streaming capabilities.

1. `tee` command with named streams (`@name` syntax)
2. `groupBy` and aggregation operators
3. Formatter/visualizer registry
4. Terminal capability detection

---

*Document version: 1.2*  
*Last updated: December 28, 2025*
