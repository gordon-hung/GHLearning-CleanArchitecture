using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.Infrastructure.Entities;
using GHLearning.CleanArchitecture.Infrastructure.Entities.Models;

using GHLearning.CleanArchitecture.Infrastructure.TodoItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace GHLearning.CleanArchitecture.InfrastructureTests.TodoItems;

public class TodoItemRepositoryTest
{
	[Fact]
	public async Task Created()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Created)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemCreated(
			Id: Guid.NewGuid(),
			UserId: user.Id,
			Description: "description",
			null,
			Labels: ["label1", "label2"],
			Priority: Priority.Normal);

		var createdAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(createdAt);

		var sut = new TodoItemRepository(timeProvider, context);

		await sut.CreatedAsync(source);

		var actual = await context.TodoItems
		.Where(x => x.Id == source.Id)
		.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.Id, actual.Id);
		Assert.Equal(source.UserId, actual.UserId);
		Assert.Equal(source.Description, actual.Description);
		Assert.Null(actual.DueDate);
		Assert.Equal(string.Join(',', source.Labels), actual.Labels);
		Assert.Equal(Priority.Normal, (Priority)actual.Priority);
		Assert.Equal(createdAt, actual.CreatedAt);
		Assert.Null(actual.CompletedAt);
		Assert.Equal(0, actual.IsCompleted);
	}

	[Fact]
	public async Task Created_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Created_InvalidOperationException)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var todoItem = new TodoItem
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			Description = "description",
			DueDate = null,
			Labels = "label1,label2",
			Priority = (int)Priority.Normal,
			CreatedAt = DateTimeOffset.UtcNow.UtcDateTime
		};
		_ = await context.TodoItems.AddAsync(todoItem);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemCreated(
			Id: todoItem.Id,
			UserId: user.Id,
			Description: "description",
			null,
			Labels: ["label1", "label2"],
			Priority: Priority.Normal);

		var createdAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(createdAt);

		var sut = new TodoItemRepository(timeProvider, context);

		await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreatedAsync(source));
	}

	[Fact]
	public async Task Completed()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Completed)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();
		var todoItem = new TodoItem
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			Description = "description",
			DueDate = null,
			Labels = "label1,label2",
			Priority = (int)Priority.Normal,
			CreatedAt = DateTimeOffset.UtcNow.UtcDateTime
		};
		_ = await context.TodoItems.AddAsync(todoItem);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemCompleted(
			Id: todoItem.Id,
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		await sut.CompletedAsync(source);

		var actual = await context.TodoItems
		.Where(x => x.Id == source.Id)
		.SingleAsync();

		Assert.Equal(1, actual.IsCompleted);
		Assert.Equal(completedAt, actual.CompletedAt!.Value);
	}

	[Fact]
	public async Task Completed_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Completed_ArgumentNullException)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemCompleted(
			Id: Guid.NewGuid(),
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CompletedAsync(source));
	}

	[Fact]
	public async Task Delete()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Delete)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();
		var todoItem = new TodoItem
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			Description = "description",
			DueDate = null,
			Labels = "label1,label2",
			Priority = (int)Priority.Normal,
			CreatedAt = DateTimeOffset.UtcNow.UtcDateTime
		};
		_ = await context.TodoItems.AddAsync(todoItem);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemDelete(
			Id: todoItem.Id,
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		await sut.DeleteAsync(source);

		var actual = await context.TodoItems
		.Where(x => x.Id == source.Id)
		.FirstOrDefaultAsync();

		Assert.Null(actual);
	}

	[Fact]
	public async Task Delete_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Delete_ArgumentNullException)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemDelete(
			Id: Guid.NewGuid(),
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		await Assert.ThrowsAsync<ArgumentNullException>(() => sut.DeleteAsync(source));
	}

	[Fact]
	public async Task Get()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Get)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();
		var todoItem = new TodoItem
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			Description = "description",
			DueDate = null,
			Labels = "label1,label2",
			Priority = (int)Priority.Normal,
			CreatedAt = DateTimeOffset.UtcNow.UtcDateTime
		};
		_ = await context.TodoItems.AddAsync(todoItem);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemGet(
			Id: todoItem.Id,
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		var actual = await sut.GetAsync(source);

		Assert.NotNull(actual);
		Assert.Equal(todoItem.Id, actual.Id);
		Assert.Equal(todoItem.UserId, actual.UserId);
		Assert.Equal(todoItem.Description, actual.Description);
		Assert.Null(actual.DueDate);
		Assert.Equal(todoItem.Labels.Split(','), actual.Labels);
		Assert.Equal(todoItem.Priority, (int)actual.Priority);
		Assert.Equal(todoItem.CreatedAt, actual.CreatedAt);
		Assert.Null(actual.CompletedAt);
		Assert.Equal(0, actual.IsCompleted);
	}

	[Fact]
	public async Task Get_Null()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Get_Null)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemGet(
			Id: Guid.NewGuid(),
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		var actual = await sut.GetAsync(source);

		Assert.Null(actual);
	}

	[Fact]
	public async Task Query()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Query)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();
		var todoItem = new TodoItem
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			Description = "description",
			DueDate = null,
			Labels = "label1,label2",
			Priority = (int)Priority.Normal,
			CreatedAt = DateTimeOffset.UtcNow.UtcDateTime
		};
		_ = await context.TodoItems.AddAsync(todoItem);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemQuery(
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		var actual = await sut.QueryAsync(source).ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal(todoItem.Id, actual.ElementAt(0).Id);
		Assert.Equal(todoItem.UserId, actual.ElementAt(0).UserId);
		Assert.Equal(todoItem.Description, actual.ElementAt(0).Description);
		Assert.Null(actual.ElementAt(0).DueDate);
		Assert.Equal(todoItem.Labels.Split(','), actual.ElementAt(0).Labels);
		Assert.Equal(todoItem.Priority, (int)actual.ElementAt(0).Priority);
		Assert.Equal(todoItem.CreatedAt, actual.ElementAt(0).CreatedAt);
		Assert.Null(actual.ElementAt(0).CompletedAt);
		Assert.Equal(0, actual.ElementAt(0).IsCompleted);
	}

	[Fact]
	public async Task Query_Empty()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Query_Empty)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var timeProvider = Substitute.For<TimeProvider>();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var source = new TodoItemQuery(
			UserId: user.Id);

		var completedAt = DateTimeOffset.UtcNow;
		_ = timeProvider.GetUtcNow().Returns(completedAt);

		var sut = new TodoItemRepository(timeProvider, context);

		var actual = await sut.QueryAsync(source).ToArrayAsync();

		Assert.Empty(actual);
	}
}
