using System.ComponentModel;

namespace GHLearning.CleanArchitecture.WebApi.ViewModels;

public record CustomResultViewModel
{
	public int Status { get; set; }

	public CustomError? Error { get; set; }
}

public record CustomResultViewModel<TResponse> : CustomResultViewModel
{
	public TResponse Data { get; set; } = default!;
}

public enum CustomResultStatus
{
	[Description("成功")]
	Success = 1,

	[Description("非預期性錯誤")]
	UnexpectedError = 999,

	[Description("預期性錯誤")]
	ExpectedError = 0
}

public record CustomError
{
	public int Code { get; set; }
	public string Title { get; set; } = default!;
	public string Type { get; set; } = default!;
	public string Description { get; set; } = default!;
	public IDictionary<string, object?>? Extensions { get; init; }
	public string Exception { get; set; } = default!;
}
