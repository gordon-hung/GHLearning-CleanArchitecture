using System.Runtime.CompilerServices;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.Infrastructure.Entities;
using GHLearning.CleanArchitecture.Infrastructure.Entities.Models;

using Microsoft.EntityFrameworkCore;

namespace GHLearning.CleanArchitecture.Infrastructure.TodoItems;

internal sealed class TodoItemRepository(
	TimeProvider timeProvider,
	SampleContext context) : ITodoItemRepository
{
	public async Task CompletedAsync(TodoItemCompleted source, CancellationToken cancellationToken = default)
	{
		var entity = await context.TodoItems.SingleOrDefaultAsync(t => t.Id == source.Id && t.UserId == source.UserId, cancellationToken).ConfigureAwait(false)
			?? throw new ArgumentNullException(nameof(source));

		entity.IsCompleted = 1;
		entity.CompletedAt = timeProvider.GetUtcNow().UtcDateTime;

		context.TodoItems.Update(entity);

		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	public async Task CreatedAsync(TodoItemCreated source, CancellationToken cancellationToken = default)
	{
		var entity = new TodoItem
		{
			Id = source.Id,
			UserId = source.UserId,
			Description = source.Description,
			DueDate = source.DueDate?.UtcDateTime,
			Labels = string.Join(',', source.Labels),
			Priority = (int)source.Priority,
			CreatedAt = timeProvider.GetUtcNow().UtcDateTime
		};

		await context.TodoItems.AddAsync(entity, cancellationToken).ConfigureAwait(false);
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	public async Task DeleteAsync(TodoItemDelete source, CancellationToken cancellationToken = default)
	{
		var entity = await context.TodoItems.SingleOrDefaultAsync(t => t.Id == source.Id && t.UserId == source.UserId, cancellationToken).ConfigureAwait(false)
			?? throw new ArgumentNullException(nameof(source));

		context.TodoItems.Remove(entity);

		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	public async Task<TodoItemInfo?> GetAsync(TodoItemGet source, CancellationToken cancellationToken = default)
	{
		var entity = await context.TodoItems.FirstOrDefaultAsync(
			predicate: t => t.Id == source.Id && t.UserId == source.UserId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return entity is null
			? null
			: new TodoItemInfo(
				Id: entity.Id,
				UserId: entity.UserId,
				Description: entity.Description,
				DueDate: entity.DueDate.HasValue ? new DateTimeOffset(entity.DueDate.Value, TimeSpan.Zero) : null,
				Labels: entity.Labels.Split(','),
				IsCompleted: entity.IsCompleted,
				CreatedAt: new DateTimeOffset(entity.CreatedAt, TimeSpan.Zero),
				CompletedAt: entity.CompletedAt.HasValue ? new DateTimeOffset(entity.CompletedAt.Value, TimeSpan.Zero) : null,
				Priority: (Priority)entity.Priority);
	}

	public async IAsyncEnumerable<TodoItemInfo> QueryAsync(TodoItemQuery source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var todoItems = await context.TodoItems
			.Where(t => t.UserId == source.UserId)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		foreach (var item in todoItems.OrderBy(t => t.CreatedAt).ThenBy(t => t.Id))
		{
			yield return new TodoItemInfo(
				Id: item.Id,
				UserId: item.UserId,
				Description: item.Description,
				DueDate: item.DueDate.HasValue ? new DateTimeOffset(item.DueDate.Value, TimeSpan.Zero) : null,
				Labels: item.Labels.Split(','),
				IsCompleted: item.IsCompleted,
				CreatedAt: new DateTimeOffset(item.CreatedAt, TimeSpan.Zero),
				CompletedAt: item.CompletedAt.HasValue ? new DateTimeOffset(item.CompletedAt.Value, TimeSpan.Zero) : null,
				Priority: (Priority)item.Priority);
		}
	}
}
