using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.TodoItems.Get;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace GHLearning.CleanArchitecture.ApplicationTests.TodoItems.Get;

public class TodoItemGetQueryRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemGetQueryRequest(
			Id: Guid.NewGuid());

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		var todoItem = new TodoItemInfo(
			Id: request.Id,
			UserId: userId,
			Description: "description",
			DueDate: null,
			Labels: ["LabelA", "LabelB"],
			IsCompleted: 0,
			CreatedAt: DateTimeOffset.UtcNow,
			CompletedAt: null,
			Priority: Priority.Normal);
		_ = fakeTodoItemRepository.GetAsync(
			Arg.Is<TodoItemGet>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.Returns(todoItem);

		var sut = new TodoItemGetQueryRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Equal(todoItem.Id, actual.Value.Id);
		Assert.Equal(todoItem.UserId, actual.Value.UserId);
		Assert.Equal(todoItem.Description, actual.Value.Description);
		Assert.Null(actual.Value.DueDate);
		Assert.True(actual.Value.Labels.Count > 0);
		Assert.Equal(todoItem.IsCompleted, actual.Value.IsCompleted);
		Assert.Equal(todoItem.CreatedAt, actual.Value.CreatedAt);
		Assert.Null(actual.Value.CompletedAt);
		Assert.Equal(todoItem.Priority, actual.Value.Priority);
	}

	[Fact]
	public async Task Handle_NotFound()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemGetQueryRequest(
			Id: Guid.NewGuid());

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		_ = fakeTodoItemRepository.GetAsync(
			Arg.Is<TodoItemGet>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.ReturnsNull();

		var sut = new TodoItemGetQueryRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);
	}
}
