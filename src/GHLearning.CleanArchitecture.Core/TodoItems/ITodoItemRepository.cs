using GHLearning.CleanArchitecture.Core.TodoItems;

namespace GHLearning.CleanArchitecture.Core.TodoItems;

public interface ITodoItemRepository
{
	public Task CompletedAsync(TodoItemCompleted source, CancellationToken cancellationToken = default);

	public Task CreatedAsync(TodoItemCreated source, CancellationToken cancellationToken = default);

	public Task DeleteAsync(TodoItemDelete source, CancellationToken cancellationToken = default);

	public Task<TodoItemInfo?> GetAsync(TodoItemGet source, CancellationToken cancellationToken = default);

	public IAsyncEnumerable<TodoItemInfo> QueryAsync(TodoItemQuery source, CancellationToken cancellationToken = default);
}
