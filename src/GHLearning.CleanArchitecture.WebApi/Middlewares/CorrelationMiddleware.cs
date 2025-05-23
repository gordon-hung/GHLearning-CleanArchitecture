using System.Diagnostics;
using CorrelationId;
using CorrelationId.Abstractions;

namespace GHLearning.CleanArchitecture.WebApi.Middlewares;


public class CorrelationMiddleware(ICorrelationIdProvider correlationIdProvider, RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	public async Task InvokeAsync(HttpContext context)
	{
		var correlationId = correlationIdProvider.GenerateCorrelationId(context);
		var activity = Activity.Current;
		// 設置 correlation-id 作為標籤
		activity?.SetTag(CorrelationIdOptions.DefaultHeader, correlationId);
		context.Request.Headers[CorrelationIdOptions.DefaultHeader] = correlationId;
		context.Items[CorrelationIdOptions.DefaultHeader] = correlationId;
		context.Response.Headers[CorrelationIdOptions.DefaultHeader] = correlationId;

		await _next(context).ConfigureAwait(false);
	}
}