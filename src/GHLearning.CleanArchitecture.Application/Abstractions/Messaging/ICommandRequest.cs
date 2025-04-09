using GHLearning.CleanArchitecture.SharedKernel;
using MediatR;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

public interface ICommandRequest : IRequest<Result>, IBaseCommandRequest;

public interface ICommandRequest<TResponse> : IRequest<Result<TResponse>>, IBaseCommandRequest;

public interface IBaseCommandRequest;
