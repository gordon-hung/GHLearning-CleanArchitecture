using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.TodoItems.Query;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;

namespace GHLearning.CleanArchitecture.ApplicationTests.TodoItems.Query;

public class TodoItemQueryRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemQueryRequest();

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		var todoItem = new TodoItemInfo(
			Id: Guid.NewGuid(),
			UserId: userId,
			Description: "description",
			DueDate: null,
			Labels: ["LabelA", "LabelB"],
			IsCompleted: 0,
			CreatedAt: DateTimeOffset.UtcNow,
			CompletedAt: null,
			Priority: Priority.Normal);
		_ = fakeTodoItemRepository.QueryAsync(
			Arg.Is<TodoItemQuery>(compare => compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.Returns(new[] { todoItem }.ToAsyncEnumerable());

		var sut = new TodoItemQueryRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Equal(todoItem.Id, actual.Value.ElementAt(0).Id);
		Assert.Equal(todoItem.UserId, actual.Value.ElementAt(0).UserId);
		Assert.Equal(todoItem.Description, actual.Value.ElementAt(0).Description);
		Assert.Null(actual.Value.ElementAt(0).DueDate);
		Assert.True(actual.Value.ElementAt(0).Labels.Count > 0);
		Assert.Equal(todoItem.IsCompleted, actual.Value.ElementAt(0).IsCompleted);
		Assert.Equal(todoItem.CreatedAt, actual.Value.ElementAt(0).CreatedAt);
		Assert.Null(actual.Value.ElementAt(0).CompletedAt);
		Assert.Equal(todoItem.Priority, actual.Value.ElementAt(0).Priority);
	}

	[Fact]
	public async Task Handle_ReturnsNull()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemQueryRequest();

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		_ = fakeTodoItemRepository.QueryAsync(
			Arg.Is<TodoItemQuery>(compare => compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<TodoItemInfo>().ToAsyncEnumerable());

		var sut = new TodoItemQueryRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Empty(actual.Value);
	}
}
