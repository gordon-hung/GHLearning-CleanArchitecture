using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Query;

internal sealed class TodoItemQueryRequestHandler(
	IUserContext userContext,
	ITodoItemRepository todoItemRepository) : IQueryRequestHandler<TodoItemQueryRequest, IReadOnlyCollection<TodoItemQueryResponse>>
{
	public async Task<Result<IReadOnlyCollection<TodoItemQueryResponse>>> Handle(TodoItemQueryRequest request, CancellationToken cancellationToken)
	{
		var infos = await todoItemRepository.QueryAsync(
			new TodoItemQuery(
				UserId: userContext.UserId),
			cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return infos.Select(info => new TodoItemQueryResponse(
			Id: info.Id,
			UserId: info.UserId,
			Description: info.Description,
			DueDate: info.DueDate,
			Labels: info.Labels,
			IsCompleted: info.IsCompleted,
			CreatedAt: info.CreatedAt,
			CompletedAt: info.CompletedAt,
			Priority: info.Priority))
			.ToArray()
			.AsReadOnly();
		;
	}
}
