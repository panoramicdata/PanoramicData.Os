namespace PanoramicData.Os.Init.Test;

/// <summary>
/// Base class for all tests providing common DRY patterns.
/// </summary>
public abstract class BaseTest
{
	/// <summary>
	/// Gets the cancellation token from xUnit 3's TestContext.
	/// Use this in all async test methods for proper test cancellation support.
	/// </summary>
	protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;
}
