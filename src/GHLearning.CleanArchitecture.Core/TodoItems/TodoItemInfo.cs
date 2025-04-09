using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.Core.TodoItems;

public record TodoItemInfo(
	Guid Id,
	Guid UserId,
	string Description,
	DateTimeOffset? DueDate,
	IReadOnlyCollection<string> Labels,
	sbyte IsCompleted,
	DateTimeOffset CreatedAt,
	DateTimeOffset? CompletedAt,
	Priority Priority);
