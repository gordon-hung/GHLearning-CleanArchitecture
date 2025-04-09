using System.Net;
using GHLearning.CleanArchitecture.SharedKernel;
using GHLearning.CleanArchitecture.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GHLearning.CleanArchitecture.WebApi.Filters;

public class FormatResponseResultFilterAttribute : ResultFilterAttribute
{
	public override void OnResultExecuting(ResultExecutingContext context)
	{
		// 如果 ModelState 無效，則設定錯誤回應
		if (!context.ModelState.IsValid)
		{
			context.Result = CreateBadRequestResult(context.ModelState);
			context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			return; // 跳過後續處理
		}

		// 檢查是否有錯誤訊息
		if (context.Result is ObjectResult result && result.Value is CustomResultViewModel response)
		{
			if (response.Status != (int)CustomResultStatus.Success)
			{
				if (response.Error is not null)
				{
					context.Result = SetErrorResult(response);
					context.HttpContext.Response.StatusCode = response.Error.Code;
				}
			}
		}
	}

	private static JsonResult CreateBadRequestResult(ModelStateDictionary modelState)
	{
		var customError = new CustomError
		{
			Code = (int)ErrorType.Validation,
			Title = "Bad Request",
			Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			Description = "The request contains invalid data.",
			Extensions = modelState
				.Where(e => e.Value?.Errors.Count > 0)
				.ToDictionary(
					e => e.Key,
					e => (object?)e.Value?.Errors.Select(err => err.ErrorMessage).ToArray())
		};

		return new JsonResult(new CustomResultViewModel<object>
		{
			Status = (int)CustomResultStatus.ExpectedError,
			Error = customError
		});
	}

	private static JsonResult SetErrorResult(CustomResultViewModel result)
	{
		var customError = new CustomError
		{
			Code = result.Error!.Code,
			Title = result.Error!.Title,
			Type = result.Error!.Type,
			Description = result.Error!.Description,
			Extensions = result.Error!.Extensions
		};

		return new JsonResult(new CustomResultViewModel<object>
		{
			Status = result.Status,
			Error = customError
		});
	}
}
