using GHLearning.CleanArchitecture.SharedKernel;
using MediatR;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

public interface IQueryRequestHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
	where TQuery : IQueryRequest<TResponse>;
