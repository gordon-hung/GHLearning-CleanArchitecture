using GHLearning.CleanArchitecture.SharedKernel;
using MediatR;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

public interface ICommandRequestHandler<in TCommand>
	: IRequestHandler<TCommand, Result>
	where TCommand : ICommandRequest;

public interface ICommandRequestHandler<in TCommand, TResponse>
	: IRequestHandler<TCommand, Result<TResponse>>
	where TCommand : ICommandRequest<TResponse>;
