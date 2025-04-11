using CorrelationId;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace GHLearning.CleanArchitecture.WebApi.Filters;

public class HandleCorrelationActionFilter(
	ICorrelationIdProvider correlationIdProvider) : IActionFilter
{
	public void OnActionExecuted(ActionExecutedContext context)
	{
		var activity = Activity.Current;
		activity?.Stop();
	}

	public void OnActionExecuting(ActionExecutingContext context)
	{
		// 從請求的 header 中獲取 correlation-id
		var correlationId = correlationIdProvider.GenerateCorrelationId(context.HttpContext);

		var activity = Activity.Current;

		// 設置 correlation-id 作為標籤
		activity?.SetTag(CorrelationIdOptions.DefaultHeader, correlationId);
	}
}
