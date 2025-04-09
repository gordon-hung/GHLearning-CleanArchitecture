using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.Core.TodoItems;

public record TodoItemCreated(
	Guid Id,
	Guid UserId,
	string Description,
	DateTimeOffset? DueDate,
	IEnumerable<string> Labels,
	Priority Priority);
