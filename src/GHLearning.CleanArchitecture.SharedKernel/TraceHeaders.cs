namespace GHLearning.CleanArchitecture.SharedKernel;

/// <summary>
/// Trace Headers
/// https://www.w3.org/TR/trace-context/
/// </summary>
public static class TraceHeaders
{
	public const string TraceParent = "traceparent";
	public const string TraceId = "trace-id";
	public const string ParentId = "parent-id";
	public const string TraceFlag = "trace-flags";
}

