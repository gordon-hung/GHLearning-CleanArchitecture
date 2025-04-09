using GHLearning.CleanArchitecture.SharedKernel;
using GHLearning.CleanArchitecture.WebApi.ViewModels;

namespace GHLearning.CleanArchitecture.WebApi.Extensions;

public static class CustomResultExtensions
{
	public static CustomResultViewModel Match(
		this Result result,
		Func<Result, CustomResultViewModel> onSuccess,
		Func<Result, CustomResultViewModel> onFailure)
	{
		return result.IsSuccess ? onSuccess(result) : onFailure(result);
	}

	public static CustomResultViewModel<TOut> Match<TIn, TOut>(
		this Result<TIn> result,
		Func<Result<TIn>, CustomResultViewModel<TOut>> onSuccess,
		Func<Result<TIn>, CustomResultViewModel<TOut>> onFailure)
	{
		return result.IsSuccess ? onSuccess(result) : onFailure(result);
	}
}
