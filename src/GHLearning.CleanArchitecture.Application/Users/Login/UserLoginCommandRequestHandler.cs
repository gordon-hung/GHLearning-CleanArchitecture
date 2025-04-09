using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
	IUserRepository userRepository,
	IPasswordHasher passwordHasher,
	ITokenProvider tokenProvider) : ICommandRequestHandler<UserLoginCommandRequest, string>
{
	public async Task<Result<string>> Handle(UserLoginCommandRequest command, CancellationToken cancellationToken)
	{
		var user = await userRepository.GetAsync(command.Email, cancellationToken).ConfigureAwait(false);

		if (user is null)
		{
			return Result.Failure<string>(UserErrors.NotFoundByEmail);
		}

		bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

		if (!verified)
		{
			return Result.Failure<string>(UserErrors.NotFoundByEmail);
		}

		string token = tokenProvider.Create(user);

		return token;
	}
}
