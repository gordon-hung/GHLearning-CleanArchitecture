using GHLearning.CleanArchitecture.SharedKernel;
using MediatR;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

public interface IQueryRequest<TResponse> : IRequest<Result<TResponse>>;
