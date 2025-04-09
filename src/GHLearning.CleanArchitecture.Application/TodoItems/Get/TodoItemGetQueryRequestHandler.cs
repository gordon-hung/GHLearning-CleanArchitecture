using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Get;

internal sealed class TodoItemGetQueryRequestHandler(
	IUserContext userContext,
	ITodoItemRepository todoItemRepository) : IQueryRequestHandler<TodoItemGetQueryRequest, TodoItemGetResponse>
{
	public async Task<Result<TodoItemGetResponse>> Handle(TodoItemGetQueryRequest request, CancellationToken cancellationToken)
	{
		var info = await todoItemRepository.GetAsync(
			new TodoItemGet(
				Id: request.Id,
				UserId: userContext.UserId),
			cancellationToken)
			.ConfigureAwait(false);

		return info is null
			? Result.Failure<TodoItemGetResponse>(TodoItemErrors.NotFound(request.Id))
			: new TodoItemGetResponse(
			Id: info.Id,
			UserId: info.UserId,
			Description: info.Description,
			DueDate: info.DueDate,
			Labels: info.Labels,
			IsCompleted: info.IsCompleted,
			CreatedAt: info.CreatedAt,
			CompletedAt: info.CompletedAt,
			Priority: info.Priority);
	}
}
