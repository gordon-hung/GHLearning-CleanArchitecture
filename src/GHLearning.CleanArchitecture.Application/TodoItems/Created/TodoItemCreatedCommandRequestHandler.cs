using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Created;

internal class TodoItemCreatedCommandRequestHandler(
	IUserContext userContext,
	ISequentialGuidGenerator sequentialGuidGenerator,
	ITodoItemRepository todoItemRepository) : ICommandRequestHandler<TodoItemCreatedCommandRequest, Guid>
{
	public async Task<Result<Guid>> Handle(TodoItemCreatedCommandRequest request, CancellationToken cancellationToken)
	{
		Guid id = sequentialGuidGenerator.NewId();

		var created = new TodoItemCreated(
			id,
			userContext.UserId,
			request.Description,
			request.DueDate,
			request.Labels,
			request.Priority);

		await todoItemRepository.CreatedAsync(created, cancellationToken).ConfigureAwait(false);

		return id;
	}
}
