using System.Diagnostics.CodeAnalysis;

[assembly: CLSCompliant(false)]

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test naming convention")]
[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait", Justification = "Test project")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Test project exception handling")]
