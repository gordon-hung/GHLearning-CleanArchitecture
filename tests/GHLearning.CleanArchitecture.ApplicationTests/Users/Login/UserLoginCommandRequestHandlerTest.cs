using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Users.Login;
using GHLearning.CleanArchitecture.Application.Users.Register;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace GHLearning.CleanArchitecture.ApplicationTests.Users.Login;

public class UserLoginCommandRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();
		var fakePasswordHasher = Substitute.For<IPasswordHasher>();
		var fakeTokenProvider = Substitute.For<ITokenProvider>();

		var request = new UserLoginCommandRequest(
			Email: "email@gmail.com",
			Password: "Password");

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

		_ = fakePasswordHasher.Verify(request.Password, userInfo.PasswordHash).Returns(true);

		var token = "token";
		_ = fakeTokenProvider.Create(Arg.Is<UserInfo>(compare =>
			compare.Id == userInfo.Id &&
			compare.Email == userInfo.Email))
			.Returns(token);

		var sut = new LoginUserCommandHandler(
			fakeUserRepository,
			fakePasswordHasher,
			fakeTokenProvider);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Equal(token, actual.Value);
	}

	[Fact]
	public async Task Handle_NotFoundByEmail()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();
		var fakePasswordHasher = Substitute.For<IPasswordHasher>();
		var fakeTokenProvider = Substitute.For<ITokenProvider>();

		var request = new UserLoginCommandRequest(
			Email: "email@gmail.com",
			Password: "Password");

		_ = fakeUserRepository.GetAsync(
			Arg.Is(request.Email),
			Arg.Any<CancellationToken>())
			.ReturnsNull();

		var sut = new LoginUserCommandHandler(
			fakeUserRepository,
			fakePasswordHasher,
			fakeTokenProvider);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);
	}

	[Fact]
	public async Task Handle_Password_Verified()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();
		var fakePasswordHasher = Substitute.For<IPasswordHasher>();
		var fakeTokenProvider = Substitute.For<ITokenProvider>();

		var request = new UserLoginCommandRequest(
			Email: "email@gmail.com",
			Password: "Password");

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

		_ = fakePasswordHasher.Verify(request.Password, userInfo.PasswordHash).Returns(false);

		var sut = new LoginUserCommandHandler(
			fakeUserRepository,
			fakePasswordHasher,
			fakeTokenProvider);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.NotFound, actual.Error.Type);
	}
}
