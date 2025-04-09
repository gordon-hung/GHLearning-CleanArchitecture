namespace GHLearning.CleanArchitecture.Core.Users;

public record UserUpdatePassword(
	Guid Id,
	string Email,
	string PasswordHash);
