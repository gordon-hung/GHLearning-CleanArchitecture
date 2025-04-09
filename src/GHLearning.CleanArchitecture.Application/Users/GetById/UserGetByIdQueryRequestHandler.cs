using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;
using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Application.Users.GetById;

internal sealed class UserGetByIdQueryRequestHandler(
	IUserRepository userRepository) : IQueryRequestHandler<UserGetByIdQueryRequest, UserGetByIdQueryResponse>
{
	public async Task<Result<UserGetByIdQueryResponse>> Handle(UserGetByIdQueryRequest request, CancellationToken cancellationToken)
	{
		var info = await userRepository.GetAsync(request.Id, cancellationToken).ConfigureAwait(false);

		return info is null
			? Result.Failure<UserGetByIdQueryResponse>(UserErrors.NotFound(request.Id))
			: new UserGetByIdQueryResponse(
				Id: info.Id,
				Email: info.Email,
				FirstName: info.FirstName,
				LastName: info.LastName);
	}
}
