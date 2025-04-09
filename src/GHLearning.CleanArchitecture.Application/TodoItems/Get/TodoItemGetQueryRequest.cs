using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Get;

public record TodoItemGetQueryRequest(
	Guid Id) : IQueryRequest<TodoItemGetResponse>;
