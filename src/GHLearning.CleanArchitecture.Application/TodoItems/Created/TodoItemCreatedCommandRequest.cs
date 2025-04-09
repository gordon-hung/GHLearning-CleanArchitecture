using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Created;

public record TodoItemCreatedCommandRequest(
	string Description,
	DateTimeOffset? DueDate,
	IEnumerable<string> Labels,
	Priority Priority) : ICommandRequest<Guid>;
