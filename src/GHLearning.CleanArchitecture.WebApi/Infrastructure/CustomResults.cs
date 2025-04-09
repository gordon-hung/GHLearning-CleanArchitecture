using GHLearning.CleanArchitecture.SharedKernel;
using GHLearning.CleanArchitecture.WebApi.ViewModels;

namespace GHLearning.CleanArchitecture.WebApi.Infrastructure;

public static class CustomResults
{
	public static CustomResultViewModel<TViewModel> Failure<TResponse, TViewModel>(Result<TResponse> result)
	{
		return result.IsSuccess ? throw new InvalidOperationException() : CreateErrorResponse<TViewModel>(result.Error, GetErrors(result));
	}

	public static CustomResultViewModel Failure(Result result)
	{
		return result.IsSuccess ? throw new InvalidOperationException() : CreateErrorResponse(result.Error, GetErrors(result));
	}

	public static CustomResultViewModel<TViewModel> Success<TResponse, TViewModel>(Result<TResponse> result, TViewModel viewModel)
		=> !result.IsSuccess
		? throw new InvalidOperationException()
		: new CustomResultViewModel<TViewModel>
		{
			Status = (int)CustomResultStatus.Success,
			Data = viewModel
		};

	public static CustomResultViewModel Success(Result result)
		=> !result.IsSuccess
		? throw new InvalidOperationException()
		: new CustomResultViewModel
		{
			Status = (int)CustomResultStatus.Success
		};

	private static CustomResultViewModel<TViewModel> CreateErrorResponse<TViewModel>(Error error, Dictionary<string, object?>? errors)
	{
		return new CustomResultViewModel<TViewModel>
		{
			Status = (int)CustomResultStatus.ExpectedError,
			Error = new CustomError
			{
				Code = (int)error.Type,
				Title = GetTitle(error),
				Type = GetType(error.Type),
				Description = GetDetail(error),
				Extensions = errors
			}
		};
	}

	private static CustomResultViewModel CreateErrorResponse(Error error, Dictionary<string, object?>? errors)
	{
		return new CustomResultViewModel
		{
			Status = (int)CustomResultStatus.ExpectedError,
			Error = new CustomError
			{
				Code = (int)error.Type,
				Title = GetTitle(error),
				Type = GetType(error.Type),
				Description = GetDetail(error),
				Extensions = errors
			}
		};
	}

	private static string GetTitle(Error error) =>
		error.Type switch
		{
			ErrorType.Validation => error.Code.ToString(),
			ErrorType.Unauthorized => error.Code.ToString(),
			ErrorType.Forbidden => error.Code.ToString(),
			ErrorType.NotFound => error.Code.ToString(),
			ErrorType.Conflict => error.Code.ToString(),
			ErrorType.InternalServerError => error.Code.ToString(),
			ErrorType.NotImplemented => error.Code.ToString(),
			_ => "Server failure"
		};

	private static string GetDetail(Error error) =>
		error.Type switch
		{
			ErrorType.Validation => error.Description,
			ErrorType.Unauthorized => error.Description,
			ErrorType.Forbidden => error.Description,
			ErrorType.NotFound => error.Description,
			ErrorType.Conflict => error.Description,
			ErrorType.InternalServerError => error.Description,
			ErrorType.NotImplemented => error.Description,
			_ => "An unexpected error occurred"
		};

	private static string GetType(ErrorType errorType) =>
		errorType switch
		{
			ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7231#section-6.5",
			ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
			ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
			ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
			ErrorType.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
			ErrorType.NotImplemented => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
			_ => "https://tools.ietf.org/html/rfc7231#section-6"
		};

	private static Dictionary<string, object?>? GetErrors(Result result)
	{
		return result.Error is not ValidationError validationError
			? null
			: new Dictionary<string, object?>
			{
				{ "errors", validationError.Errors }
			};
	}
}
