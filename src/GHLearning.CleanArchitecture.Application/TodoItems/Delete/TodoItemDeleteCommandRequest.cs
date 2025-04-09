using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Delete;

public record TodoItemDeleteCommandRequest(
	Guid Id) : ICommandRequest;
