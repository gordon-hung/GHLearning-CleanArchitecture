using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.WebApi.Controllers.TodoItems.ViewModels;

public record TodoItemCreatedViewModel(
	string Description,
	DateTimeOffset? DueDate,
	IEnumerable<string> Labels,
	Priority Priority);
