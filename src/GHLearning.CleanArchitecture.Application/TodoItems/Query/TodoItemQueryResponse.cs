using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Query;

public record TodoItemQueryResponse(
	Guid Id,
	Guid UserId,
	string Description,
	DateTimeOffset? DueDate,
	IReadOnlyCollection<string> Labels,
	sbyte IsCompleted,
	DateTimeOffset CreatedAt,
	DateTimeOffset? CompletedAt,
	Priority Priority);
