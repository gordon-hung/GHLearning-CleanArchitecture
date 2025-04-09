using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Query;

public sealed record TodoItemQueryRequest : IQueryRequest<IReadOnlyCollection<TodoItemQueryResponse>>;
