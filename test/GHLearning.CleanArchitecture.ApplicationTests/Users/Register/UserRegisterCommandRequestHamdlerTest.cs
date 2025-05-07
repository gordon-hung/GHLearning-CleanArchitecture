using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Users.Register;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;
using NSubstitute;

namespace GHLearning.CleanArchitecture.ApplicationTests.Users.Register;

public class UserRegisterCommandRequestHamdlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();
		var fakePasswordHasher = Substitute.For<IPasswordHasher>();
		var fakeSequentialGuidGenerator = Substitute.For<ISequentialGuidGenerator>();

		var request = new UserRegisterCommandRequest(
			Email: "email@gmail.com",
			FirstName: "FirstName",
			LastName: "LastName",
			Password: "Password");

		_ = fakeUserRepository.IsEmailUniqueAsync(
			Arg.Is(request.Email),
			Arg.Any<CancellationToken>())
			.Returns(true);

		var id = Guid.NewGuid();
		_ = fakeSequentialGuidGenerator.NewIdAsync(Arg.Any<CancellationToken>()).Returns(id);

		var passwordHash = "passwordHash";
		_ = fakePasswordHasher.Hash(Arg.Is(request.Password)).Returns(passwordHash);

		var sut = new UserRegisterCommandRequestHamdler(
			fakeUserRepository,
			fakePasswordHasher,
			fakeSequentialGuidGenerator);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.True(actual.IsSuccess);
		Assert.Equal(Error.None, actual.Error);
		Assert.Equal(id, actual.Value);
	}

	[Fact]
	public async Task Handle_EmailNotUnique()
	{
		var fakeUserRepository = Substitute.For<IUserRepository>();
		var fakePasswordHasher = Substitute.For<IPasswordHasher>();
		var fakeSequentialGuidGenerator = Substitute.For<ISequentialGuidGenerator>();

		var request = new UserRegisterCommandRequest(
			Email: "email@gmail.com",
			FirstName: "FirstName",
			LastName: "LastName",
			Password: "Password");

		_ = fakeUserRepository.IsEmailUniqueAsync(
			Arg.Is(request.Email),
			Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new UserRegisterCommandRequestHamdler(
			fakeUserRepository,
			fakePasswordHasher,
			fakeSequentialGuidGenerator);

		var cancellationTokenSource = new CancellationTokenSource();
		var actual = await sut.Handle(request, cancellationTokenSource.Token);

		Assert.False(actual.IsSuccess);
		Assert.Equal(ErrorType.Conflict, actual.Error.Type);
	}
}
