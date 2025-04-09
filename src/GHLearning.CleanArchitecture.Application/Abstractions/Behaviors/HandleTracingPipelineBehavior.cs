using System.Diagnostics;
using MediatR;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Behaviors;

public class HandleTracingPipelineBehavior<TRequest, TResponse>(
	ActivitySource activitySource) : IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		using var activity = activitySource.StartActivity($"MediatR Handle");

		activity?.SetTag("TRequest", typeof(TRequest).Name);
		//activity?.SetTag("TRequest Content", JsonSerializer.Serialize(request));
		var response = await next(cancellationToken).ConfigureAwait(false);
		//activity?.SetTag("TResponse", typeof(TResponse).Name);
		//activity?.SetTag("TResponse Content", JsonSerializer.Serialize(response));
		return response;
	}
}
