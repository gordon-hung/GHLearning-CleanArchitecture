using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.Users.Register;

internal sealed class UserRegisterCommandRequestHamdler(
	IUserRepository userRepository,
	IPasswordHasher passwordHasher,
	ISequentialGuidGenerator sequentialGuidGenerator) : ICommandRequestHandler<UserRegisterCommandRequest, Guid>
{
	public async Task<Result<Guid>> Handle(UserRegisterCommandRequest command, CancellationToken cancellationToken)
	{
		if (!await userRepository.IsEmailUniqueAsync(command.Email, cancellationToken).ConfigureAwait(false))
		{
			return Result.Failure<Guid>(UserErrors.EmailNotUnique);
		}

		var user = new UserCreated
		(
			Id: sequentialGuidGenerator.NewId(),
			Email: command.Email,
			FirstName: command.FirstName,
			LastName: command.LastName,
			PasswordHash: passwordHasher.Hash(command.Password)
		);

		await userRepository.CreatedAsync(user, cancellationToken).ConfigureAwait(false);

		return user.Id;
	}
}
