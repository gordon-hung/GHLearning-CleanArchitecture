using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.TodoItems.Completed;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace GHLearning.CleanArchitecture.ApplicationTests.TodoItems.Completed;

public class TodoItemCompletedCommandRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemCompletedCommandRequest(
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

		var sut = new TodoItemCompletedCommandRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);

		_ = fakeTodoItemRepository
			.Received()
			.CompletedAsync(
			Arg.Is<TodoItemCompleted>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Handle_NotFound()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemCompletedCommandRequest(
			Id: Guid.NewGuid());

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		_ = fakeTodoItemRepository.GetAsync(
			Arg.Is<TodoItemGet>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.ReturnsNull();

		var sut = new TodoItemCompletedCommandRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);

		_ = fakeTodoItemRepository
			.DidNotReceive()
			.CompletedAsync(
			Arg.Is<TodoItemCompleted>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Handle_AlreadyCompleted()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemCompletedCommandRequest(
			Id: Guid.NewGuid());

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		var todoItem = new TodoItemInfo(
			Id: request.Id,
			UserId: userId,
			Description: "description",
			DueDate: null,
			Labels: ["LabelA", "LabelB"],
			IsCompleted: 1,
			CreatedAt: DateTimeOffset.UtcNow,
			CompletedAt: DateTimeOffset.UtcNow,
			Priority: Priority.Normal);
		_ = fakeTodoItemRepository.GetAsync(
			Arg.Is<TodoItemGet>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.Returns(todoItem);

		var sut = new TodoItemCompletedCommandRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.Conflict, actual.Error.Type);

		_ = fakeTodoItemRepository
			.DidNotReceive()
			.CompletedAsync(
			Arg.Is<TodoItemCompleted>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>());
	}
}
