using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Completed;

public record TodoItemCompletedCommandRequest(
	Guid Id) : ICommandRequest;
