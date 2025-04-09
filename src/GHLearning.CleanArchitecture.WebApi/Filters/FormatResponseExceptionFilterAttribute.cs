using GHLearning.CleanArchitecture.SharedKernel;
using GHLearning.CleanArchitecture.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GHLearning.CleanArchitecture.WebApi.Filters;

public class FormatResponseExceptionFilterAttribute(
	ILogger<FormatResponseExceptionFilterAttribute> logger,
	IWebHostEnvironment env) : ExceptionFilterAttribute
{
	public override void OnException(ExceptionContext context)
	{
		// 根據不同的異常類型，設置錯誤訊息
		var errorMessage = GetErrorMessageForException(context.Exception);

		// 建立回應內容
		var result = new CustomResultViewModel<CustomError>
		{
			Status = (int)CustomResultStatus.UnexpectedError,
			Error = errorMessage
		};

		// 設定回應內容及狀態碼
		context.Result = new JsonResult(result)
		{
			StatusCode = errorMessage.Code
		};
	}

	private CustomError GetErrorMessageForException(Exception exception)
	{
		logger.LogError(exception, $"{nameof(FormatResponseExceptionFilterAttribute)} 攔截例外");
		// 預設的錯誤訊息
		var errorMessage = new CustomError
		{
			Code = (int)ErrorType.InternalServerError,
			Title = "Server failure",
			Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
			Description = "An unexpected error occurred",
			Exception = env.IsDevelopment() ? exception.ToString() : "An issue has occurred, please get in touch with the administrator."
		};

		// 處理 NotImplementedException 類型的錯誤
		if (exception is NotImplementedException)
		{
			errorMessage = new CustomError
			{
				Code = (int)ErrorType.NotImplemented,
				Title = "Not Implemented",
				Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.2",
				Description = "Invalid action: Rubbing not supported.",
				Exception = env.IsDevelopment() ? exception.ToString() : "An issue has occurred, please get in touch with the administrator."
			};
		}

		return errorMessage;
	}
}
