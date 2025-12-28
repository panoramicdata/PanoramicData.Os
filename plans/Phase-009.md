# Phase 9: Object Streaming Infrastructure

## Overview

Implement the typed object streaming infrastructure that transforms the shell from text-based I/O to a strongly-typed pipeline model similar to PowerShell but with C# LINQ syntax.

## Goals

- Commands yield `IAsyncEnumerable<T>` instead of writing to console
- Pipeline type checking at parse time (before execution)
- Full C# lambda expression support via Roslyn
- Backpressure-aware buffered streaming
- Implicit console formatter for unpiped output

## Dependencies

- Phase 1 (Foundation & Boot) - Shell basics
- Roslyn NuGet packages for expression parsing

## Duration

Estimated: 4 weeks

---

## Architecture

### Stream Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                         Pipeline                                │
├─────────────────────────────────────────────────────────────────┤
│  ls --as=FileEntry                                              │
│    │                                                            │
│    ├──▶ IAsyncEnumerable<FileEntry>                            │
│    │                                                            │
│  where x => x.Size > 1000                                       │
│    │                                                            │
│    ├──▶ IAsyncEnumerable<FileEntry>  (filtered)                │
│    │                                                            │
│  select x => { x.Name, x.Size }                                 │
│    │                                                            │
│    ├──▶ IAsyncEnumerable<Anonymous>  (projected)               │
│    │                                                            │
│  take 10                                                        │
│    │                                                            │
│    └──▶ DefaultFormatter ──▶ Console                           │
└─────────────────────────────────────────────────────────────────┘
```

### Logging Architecture

```
┌─────────────────┐
│    Command      │──▶ ILogger ──▶ Central Log Aggregator
├─────────────────┤                     │
│ Data Stream     │──▶ Pipeline         ├──▶ Console (if verbose)
│ Log Stream      │──▶ LogChannel       ├──▶ File
│ Trace Stream    │──▶ TraceChannel     └──▶ Telemetry
└─────────────────┘
```

---

## Deliverables

### 9.1 Core Stream Types

**Files**: `src/PanoramicData.Os.CommandLine/Streaming/`

```csharp
// Base interface for all stream objects
public interface IStreamObject
{
    StreamMetadata Metadata { get; }
}

public record StreamMetadata
{
    public string? Source { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public int? SequenceNumber { get; init; }
}

// Text output types
public record TextLine : IStreamObject
{
    public string Content { get; init; }
    public int LineNumber { get; init; }
    public string? SourceFile { get; init; }
    public StreamMetadata Metadata { get; init; }
}

public record TextChunk : IStreamObject
{
    public string Content { get; init; }
    public int StartLine { get; init; }
    public int EndLine { get; init; }
    public string? SourceFile { get; init; }
    public StreamMetadata Metadata { get; init; }
}

// File system types
public record FileSystemEntry : IStreamObject
{
    public string Name { get; init; }
    public string FullPath { get; init; }
    public long Size { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset Modified { get; init; }
    public string? Extension { get; init; }
    public bool IsDirectory { get; init; }
    public FileAttributes Attributes { get; init; }
    public StreamMetadata Metadata { get; init; }
}
```

**Acceptance Criteria**:
- [ ] All stream types implement `IStreamObject`
- [ ] Metadata includes source and timestamp
- [ ] Types are records for immutability

---

### 9.2 Command Execution with Streaming

**Files**: `src/PanoramicData.Os.CommandLine/`

Update `ShellCommandSpecification`:

```csharp
public class ShellCommandSpecification
{
    // Existing properties...
    
    /// <summary>
    /// Supported output types for --as flag. First is default.
    /// </summary>
    public Type[] OutputTypes { get; init; } = [];
    
    /// <summary>
    /// Supported input types. Empty for commands that don't accept piped input.
    /// </summary>
    public Type[] InputTypes { get; init; } = [];
}
```

Update command base class:

```csharp
public abstract class StreamCommand<TInput, TOutput> : PanCommand
    where TInput : IStreamObject
    where TOutput : IStreamObject
{
    protected abstract IAsyncEnumerable<TOutput> ExecuteStreamAsync(
        CommandExecutionContext context,
        IAsyncEnumerable<TInput>? input,
        CancellationToken cancellationToken);
}

// For commands with no input
public abstract class SourceCommand<TOutput> : PanCommand
    where TOutput : IStreamObject
{
    protected abstract IAsyncEnumerable<TOutput> ExecuteStreamAsync(
        CommandExecutionContext context,
        CancellationToken cancellationToken);
}
```

**Acceptance Criteria**:
- [ ] `StreamCommand<TIn, TOut>` base class implemented
- [ ] `SourceCommand<TOut>` base class for source commands
- [ ] Commands can declare output types
- [ ] `--as` flag parsed and validated

---

### 9.3 Pipeline Executor

**Files**: `src/PanoramicData.Os.CommandLine/Pipeline/`

```csharp
public class PipelineExecutor
{
    public async Task<int> ExecuteAsync(
        Pipeline pipeline,
        CancellationToken cancellationToken)
    {
        // Create bounded channels between stages
        // Wire up producers and consumers
        // Handle backpressure
        // Return combined exit code
    }
}

public class Pipeline
{
    public IReadOnlyList<PipelineStage> Stages { get; }
    public IReadOnlyDictionary<string, PipelineStage> NamedStreams { get; }
}

public class PipelineStage
{
    public ICommand Command { get; }
    public Type InputType { get; }
    public Type OutputType { get; }
    public Channel<IStreamObject>? InputChannel { get; set; }
    public Channel<IStreamObject>? OutputChannel { get; set; }
}
```

**Acceptance Criteria**:
- [ ] Channel-based pipeline execution
- [ ] Backpressure via bounded channels
- [ ] Cancellation propagation
- [ ] Early termination (e.g., `take 10` stops upstream)

---

### 9.4 Default Formatter

**Files**: `src/PanoramicData.Os.Init/Shell/Formatters/`

```csharp
public interface IStreamFormatter<T> where T : IStreamObject
{
    void Format(T item, Terminal terminal);
}

public class DefaultFormatter
{
    private readonly Dictionary<Type, object> _formatters = new();
    
    public void Register<T>(IStreamFormatter<T> formatter) where T : IStreamObject;
    
    public async Task FormatAsync<T>(
        IAsyncEnumerable<T> stream,
        Terminal terminal,
        CancellationToken cancellationToken) where T : IStreamObject;
}

// Built-in formatters
public class FileSystemEntryFormatter : IStreamFormatter<FileSystemEntry>
{
    // Formats as ls -l style output
}

public class TextLineFormatter : IStreamFormatter<TextLine>
{
    // Just prints the line content
}
```

**Acceptance Criteria**:
- [ ] Formatter registry with type-based lookup
- [ ] `FileSystemEntry` formatter (ls style)
- [ ] `TextLine` formatter (simple print)
- [ ] Fallback to `ToString()` for unknown types

---

### 9.5 Update ls Command

**Files**: `src/PanoramicData.Os.Init/Shell/Commands/LsCommand.cs`

```csharp
public class LsCommand : SourceCommand<FileSystemEntry>
{
    private static readonly ShellCommandSpecification _specification = new()
    {
        // Existing spec...
        OutputTypes = [typeof(FileSystemEntry), typeof(string)],
    };
    
    protected override async IAsyncEnumerable<FileSystemEntry> ExecuteStreamAsync(
        CommandExecutionContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var entries = GetDirectoryEntries(context);
        
        foreach (var entry in entries)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
                
            yield return entry;
        }
    }
}
```

**Acceptance Criteria**:
- [ ] `ls` yields `FileSystemEntry` objects
- [ ] Supports `--as=string` for just names
- [ ] Works with pipeline (e.g., `ls | where x => x.Size > 1000`)
- [ ] Existing tests pass

---

### 9.6 Update cat Command

**Files**: `src/PanoramicData.Os.Init/Shell/Commands/CatCommand.cs`

```csharp
public class CatCommand : SourceCommand<TextLine>  // Or TextChunk or char
{
    private static readonly ShellCommandSpecification _specification = new()
    {
        // Existing spec...
        OutputTypes = [typeof(TextLine), typeof(TextChunk), typeof(char)],
    };
    
    protected override async IAsyncEnumerable<TextLine> ExecuteStreamAsync(
        CommandExecutionContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var files = context.GetParameter<string[]>("positional", []);
        var lineNumber = 0;
        
        foreach (var file in files)
        {
            var path = context.ResolvePath(file);
            await foreach (var line in File.ReadLinesAsync(path, cancellationToken))
            {
                yield return new TextLine
                {
                    Content = line,
                    LineNumber = ++lineNumber,
                    SourceFile = file,
                    Metadata = new StreamMetadata { Source = file }
                };
            }
        }
    }
}
```

**Acceptance Criteria**:
- [ ] `cat` yields `TextLine` by default
- [ ] Supports `--as=char` for character stream
- [ ] Supports `--as=TextChunk` for chunked output
- [ ] Works with pipeline (e.g., `cat file.txt | where x => x.Content.Contains("TODO")`)

---

### 9.7 LINQ where Command

**Files**: `src/PanoramicData.Os.Init/Shell/Commands/WhereCommand.cs`

```csharp
public class WhereCommand<T> : StreamCommand<T, T> where T : IStreamObject
{
    private readonly Expression<Func<T, bool>> _predicate;
    private readonly Func<T, bool> _compiled;
    
    public WhereCommand(Expression<Func<T, bool>> predicate)
    {
        _predicate = predicate;
        _compiled = predicate.Compile();
    }
    
    protected override async IAsyncEnumerable<T> ExecuteStreamAsync(
        CommandExecutionContext context,
        IAsyncEnumerable<T>? input,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (input is null) yield break;
        
        await foreach (var item in input.WithCancellation(cancellationToken))
        {
            if (_compiled(item))
                yield return item;
        }
    }
}
```

**Acceptance Criteria**:
- [ ] `where` filters stream by predicate
- [ ] Predicate parsed from C# lambda expression
- [ ] Type checking at parse time
- [ ] `where x => x.Size > 1000` works with `ls`

---

### 9.8 LINQ select Command

**Files**: `src/PanoramicData.Os.Init/Shell/Commands/SelectCommand.cs`

```csharp
public class SelectCommand<TInput, TOutput> : StreamCommand<TInput, TOutput>
    where TInput : IStreamObject
    where TOutput : IStreamObject
{
    private readonly Expression<Func<TInput, TOutput>> _selector;
    private readonly Func<TInput, TOutput> _compiled;
    
    // Implementation similar to where
}
```

**Acceptance Criteria**:
- [ ] `select` projects to new type
- [ ] Supports property selection: `select x => x.Name`
- [ ] Supports anonymous types: `select x => { x.Name, x.Size }`
- [ ] Output type correctly inferred

---

### 9.9 Roslyn Expression Parser

**Files**: `src/PanoramicData.Os.CommandLine/Expressions/`

```csharp
public class ExpressionParser
{
    /// <summary>
    /// Parse a C# lambda expression string into an Expression tree.
    /// </summary>
    public Expression<Func<T, TResult>> Parse<T, TResult>(string expression);
    
    /// <summary>
    /// Get completions for a partial expression.
    /// </summary>
    public IEnumerable<CompletionItem> GetCompletions<T>(
        string partialExpression,
        int cursorPosition);
}
```

**Acceptance Criteria**:
- [ ] Parses simple lambdas: `x => x.Size > 1000`
- [ ] Parses projections: `x => x.Name`
- [ ] Parses anonymous types: `x => { x.Name, x.Size }`
- [ ] Reports syntax errors with position
- [ ] Tab completion for properties

---

### 9.10 Pipeline Type Validation

**Files**: `src/PanoramicData.Os.Init/Shell/`

Update `PanShell` and `LineEditor` to validate pipeline types:

```csharp
public class PipelineValidator
{
    public ValidationResult Validate(string commandLine)
    {
        // Parse commands
        // Check output type of each stage matches input type of next
        // Return errors with positions for highlighting
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
}

public class ValidationError
{
    public int StartPosition { get; }
    public int EndPosition { get; }
    public string Message { get; }
}
```

**Acceptance Criteria**:
- [ ] Type mismatches detected at parse time
- [ ] Error positions for syntax highlighting
- [ ] Invalid pipelines cannot be submitted
- [ ] Error message shown in line editor

---

## Testing

All tests in `src/PanoramicData.Os.Init.Test/`:

### Unit Tests

- `Streaming/StreamTypesTests.cs` - Stream object creation and metadata
- `Streaming/PipelineExecutorTests.cs` - Pipeline execution and backpressure
- `Streaming/FormatterTests.cs` - Output formatting
- `Commands/LsStreamingTests.cs` - ls with streaming
- `Commands/CatStreamingTests.cs` - cat with streaming
- `Expressions/ExpressionParserTests.cs` - Lambda parsing
- `Pipeline/TypeValidationTests.cs` - Pipeline type checking

### Integration Tests

- `PipelineIntegrationTests.cs` - Full pipeline execution
- `LsWhereSelectTests.cs` - `ls | where | select` chains

---

## Milestones

| Milestone | Deliverables | Target |
|-----------|--------------|--------|
| M1 | 9.1, 9.2, 9.3 - Core infrastructure | Week 1 |
| M2 | 9.4, 9.5, 9.6 - ls/cat streaming | Week 2 |
| M3 | 9.7, 9.8, 9.9 - LINQ operators | Week 3 |
| M4 | 9.10 - Validation, polish, testing | Week 4 |

---

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Roslyn package size (~20MB) | Larger deployment | Acceptable for NativeAOT scenario |
| Expression parsing complexity | Development time | Start with subset of C# expressions |
| Anonymous type handling | Runtime complexity | Use dynamic or ExpandoObject initially |
| Backpressure tuning | Performance | Make buffer sizes configurable |

---

## Success Criteria

- [ ] `ls | where x => x.Size > 1000 | take 10` executes correctly
- [ ] `cat file.txt | where x => x.Content.Contains("TODO")` works
- [ ] Type mismatches show error before Enter is pressed
- [ ] Tab completion works inside lambda expressions
- [ ] All existing tests pass
- [ ] Backpressure prevents memory exhaustion on large streams

---

*Document version: 1.0*  
*Created: December 28, 2025*
