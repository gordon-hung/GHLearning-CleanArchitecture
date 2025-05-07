using CorrelationId;

using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GHLearning.CleanArchitecture.Infrastructure.Correlations;

internal sealed class CustomCorrelationIdProvider : ICorrelationIdProvider
{
	public string GenerateCorrelationId(HttpContext context)
	{
		var correlationId = context.Request.Headers[CorrelationIdOptions.DefaultHeader].FirstOrDefault()
			?? context.Items[CorrelationIdOptions.DefaultHeader]?.ToString();

		if (string.IsNullOrEmpty(correlationId))
		{
			correlationId = Guid.NewGuid().ToString();
			context.Request.Headers[CorrelationIdOptions.DefaultHeader] = correlationId;
		}

		context.Items[CorrelationIdOptions.DefaultHeader] = correlationId;
		context.Response.Headers[CorrelationIdOptions.DefaultHeader] = correlationId;

		return correlationId;
	}
}
