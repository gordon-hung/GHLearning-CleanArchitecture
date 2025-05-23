using System.Diagnostics;
using MediatR;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Behaviors;

public class HandleTracingPipelineBehavior<TRequest, TResponse>(
	ActivitySource activitySource) : IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
{
	public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		using var activity = activitySource.StartActivity("MediatR Handle");
		activity?.SetTag("mediatr.request.namespace", typeof(TRequest).Namespace);
		activity?.SetTag("mediatr.request.name", typeof(TRequest).Name);
		return next(cancellationToken);
	}
}
