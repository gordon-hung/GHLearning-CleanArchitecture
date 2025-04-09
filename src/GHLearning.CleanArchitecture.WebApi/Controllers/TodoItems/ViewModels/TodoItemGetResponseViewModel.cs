using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.WebApi.Controllers.TodoItems.ViewModels;

public record TodoItemGetResponseViewModel(
	Guid Id,
	Guid UserId,
	string Description,
	DateTimeOffset? DueDate,
	IReadOnlyCollection<string> Labels,
	sbyte IsCompleted,
	DateTimeOffset CreatedAt,
	DateTimeOffset? CompletedAt,
	Priority Priority);
