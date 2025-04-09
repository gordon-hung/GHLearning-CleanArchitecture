using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Delete;

internal class TodoItemDeleteCommandRequestHandler(
	IUserContext userContext,
	ITodoItemRepository todoItemRepository) : ICommandRequestHandler<TodoItemDeleteCommandRequest>
{
	public async Task<Result> Handle(TodoItemDeleteCommandRequest request, CancellationToken cancellationToken)
	{
		var todoItem = await todoItemRepository.GetAsync(
			source: new TodoItemGet(
				request.Id,
				userContext.UserId),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (todoItem is null)
			return Result.Failure(TodoItemErrors.NotFound(request.Id));

		await todoItemRepository.DeleteAsync(new TodoItemDelete(request.Id, todoItem.UserId), cancellationToken).ConfigureAwait(false);

		return Result.Success();
	}
}
