using GHLearning.CleanArchitecture.Application.Users.GetById;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace GHLearning.CleanArchitecture.ApplicationTests.Users.GetById;

public class UserGetByIdQueryRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();

		var request = new UserGetByIdQueryRequest(
			Id: Guid.NewGuid());

		var userInfo = new UserInfo(
			Id: request.Id,
			Email: "email@gmail.com",
			FirstName: "FirstName",
			LastName: "LastName",
			PasswordHash: "PasswordHash");
		_ = fakeUserRepository.GetAsync(
			Arg.Is(request.Id),
			Arg.Any<CancellationToken>())
			.Returns(userInfo);

		var sut = new UserGetByIdQueryRequestHandler(
			fakeUserRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Equal(userInfo.Id, actual.Value.Id);
		Assert.Equal(userInfo.Email, actual.Value.Email);
		Assert.Equal(userInfo.FirstName, actual.Value.FirstName);
		Assert.Equal(userInfo.LastName, actual.Value.LastName);
	}

	[Fact]
	public async Task Handle_NotFound()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();

		var request = new UserGetByIdQueryRequest(
			Id: Guid.NewGuid());

		_ = fakeUserRepository.GetAsync(
			Arg.Is(request.Id),
			Arg.Any<CancellationToken>())
			.ReturnsNull();

		var sut = new UserGetByIdQueryRequestHandler(
			fakeUserRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);
	}
}
