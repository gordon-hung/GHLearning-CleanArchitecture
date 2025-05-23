using System.Diagnostics;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.WebApi.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
	public Task InvokeAsync(HttpContext context)
	{
		using var activity = Activity.Current;
		var traceId = activity?.TraceId.ToString();
		var spanId = activity?.SpanId.ToString();
		var traceFlags = ((int)(activity?.ActivityTraceFlags ?? ActivityTraceFlags.None)).ToString("D2");
		var traceParentHeader = $"00-{traceId}-{spanId}-{traceFlags}";

		context.Response.OnStarting(() =>
		{
			context.Response.Headers[TraceHeaders.TraceParent] = traceParentHeader;
			context.Response.Headers[TraceHeaders.TraceId] = traceId;
			context.Response.Headers[TraceHeaders.ParentId] = spanId;
			context.Response.Headers[TraceHeaders.TraceFlag] = traceFlags;
			return Task.CompletedTask;
		});

		return next(context);
	}
}