using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.TodoItems.Delete;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace GHLearning.CleanArchitecture.ApplicationTests.TodoItems.Delete;

public class TodoItemDeleteCommandRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemDeleteCommandRequest(
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

		var sut = new TodoItemDeleteCommandRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);

		_ = fakeTodoItemRepository
			.Received()
			.DeleteAsync(
			Arg.Is<TodoItemDelete>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Handle_NotFound()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemDeleteCommandRequest(
			Id: Guid.NewGuid());

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		_ = fakeTodoItemRepository.GetAsync(
			Arg.Is<TodoItemGet>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>())
			.ReturnsNull();

		var sut = new TodoItemDeleteCommandRequestHandler(
			fakeUserContext,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);

		_ = fakeTodoItemRepository
			.DidNotReceive()
			.DeleteAsync(
			Arg.Is<TodoItemDelete>(compare => compare.Id == request.Id && compare.UserId == userId),
			Arg.Any<CancellationToken>());
	}
}
