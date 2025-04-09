using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.Users.GetByEmail;

internal sealed class UserGetByEmailQueryRequestHandler(
	IUserRepository userRepository) : IQueryRequestHandler<UserGetByEmailQueryRequest, UserGetByEmailQueryResponse>
{
	public async Task<Result<UserGetByEmailQueryResponse>> Handle(UserGetByEmailQueryRequest request, CancellationToken cancellationToken)
	{
		var info = await userRepository.GetAsync(request.Email, cancellationToken).ConfigureAwait(false);

		return info is null
			? Result.Failure<UserGetByEmailQueryResponse>(UserErrors.NotFoundByEmail)
			: new UserGetByEmailQueryResponse(
				Id: info.Id,
				Email: info.Email,
				FirstName: info.FirstName,
				LastName: info.LastName);
	}
}
