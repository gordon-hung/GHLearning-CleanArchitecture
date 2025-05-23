using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.TodoItems.Created;
using GHLearning.CleanArchitecture.Core.TodoItems;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;

namespace GHLearning.CleanArchitecture.ApplicationTests.TodoItems.Created;

public class TodoItemCreatedCommandRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserContext = Substitute.For<IUserContext>();
		var fakeSequentialGuidGenerator = Substitute.For<ISequentialGuidGenerator>();
		var fakeTodoItemRepository = Substitute.For<ITodoItemRepository>();

		var request = new TodoItemCreatedCommandRequest(
			Description: "description",
			DueDate: null,
			Labels: ["LabelA", "LabelB"],
			Priority: Priority.Normal);

		var userId = Guid.NewGuid();
		_ = fakeUserContext.UserId.Returns(userId);

		var id = Guid.NewGuid();
		_ = fakeSequentialGuidGenerator.NewId().Returns(id);

		var sut = new TodoItemCreatedCommandRequestHandler(
			fakeUserContext,
			fakeSequentialGuidGenerator,
			fakeTodoItemRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Equal(id, actual.Value);

		_ = fakeTodoItemRepository
			.Received()
			.CreatedAsync(
			Arg.Is<TodoItemCreated>(compare =>
			compare.Id == id &&
			compare.UserId == userId &&
			compare.Description == request.Description &&
			compare.DueDate == request.DueDate &&
			compare.Labels.Any() &&
			compare.Priority == request.Priority),
			Arg.Any<CancellationToken>());
	}
}
