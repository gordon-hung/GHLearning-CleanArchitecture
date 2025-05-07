using GHLearning.CleanArchitecture.Application.Users.GetByEmail;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace GHLearning.CleanArchitecture.ApplicationTests.Users.GetByEmail;

public class UserGetByEmailQueryRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();

		var request = new UserGetByEmailQueryRequest(
			Email: "email@gmail.com");

		var userInfo = new UserInfo(
			Id: Guid.NewGuid(),
			Email: request.Email,
			FirstName: "FirstName",
			LastName: "LastName",
			PasswordHash: "PasswordHash");
		_ = fakeUserRepository.GetAsync(
			Arg.Is(request.Email),
			Arg.Any<CancellationToken>())
			.Returns(userInfo);

		var sut = new UserGetByEmailQueryRequestHandler(
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

		var request = new UserGetByEmailQueryRequest(
			Email: "email@gmail.com");

		_ = fakeUserRepository.GetAsync(
			Arg.Is(request.Email),
			Arg.Any<CancellationToken>())
			.ReturnsNull();

		var sut = new UserGetByEmailQueryRequestHandler(
			fakeUserRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);
	}
}
