using CorrelationId;

using CorrelationId.Abstractions;
using GHLearning.CleanArchitecture.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace GHLearning.CleanArchitecture.Infrastructure.Correlations;

internal sealed class CustomCorrelationIdProvider(ISequentialGuidGenerator sequentialGuidGenerator) : ICorrelationIdProvider
{
	public string GenerateCorrelationId(HttpContext context)
		=> context.Request.Headers[CorrelationIdOptions.DefaultHeader].FirstOrDefault()
		?? context.Items[CorrelationIdOptions.DefaultHeader]?.ToString()
		?? sequentialGuidGenerator.NewId().ToString();
}