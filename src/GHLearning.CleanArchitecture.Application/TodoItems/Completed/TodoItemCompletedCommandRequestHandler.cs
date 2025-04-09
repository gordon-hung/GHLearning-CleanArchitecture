using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Completed;

internal class TodoItemCompletedCommandRequestHandler(
	IUserContext userContext,
	ITodoItemRepository todoItemRepository) : ICommandRequestHandler<TodoItemCompletedCommandRequest>
{
	public async Task<Result> Handle(TodoItemCompletedCommandRequest request, CancellationToken cancellationToken)
	{
		var todoItem = await todoItemRepository.GetAsync(
			source: new TodoItemGet(
			request.Id,
			userContext.UserId),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (todoItem is null)
			return Result.Failure(TodoItemErrors.NotFound(request.Id));

		if (todoItem.IsCompleted == 1)
			return Result.Failure(TodoItemErrors.AlreadyCompleted(request.Id));

		await todoItemRepository.CompletedAsync(new TodoItemCompleted(request.Id, todoItem.UserId), cancellationToken).ConfigureAwait(false);

		return Result.Success();
	}
}
