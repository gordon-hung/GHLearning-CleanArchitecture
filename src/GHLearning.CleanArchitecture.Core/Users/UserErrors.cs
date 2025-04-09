using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Core.Users;

public static class UserErrors
{
	public static readonly Error NotFoundByEmail = Error.NotFound(
		"Users.NotFoundByEmail",
		"The user with the specified email was not found");

	public static readonly Error EmailNotUnique = Error.Conflict(
		"Users.EmailNotUnique",
		"The provided email is not unique");

	public static Error NotFound(Guid userId) => Error.NotFound(
				"Users.NotFound",
		$"The user with the Id = '{userId}' was not found");

	public static Error Unauthorized() => Error.Unauthorized(
		"Users.Unauthorized",
		"You are not authorized to perform this action.");
}
