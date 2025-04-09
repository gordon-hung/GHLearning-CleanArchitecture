namespace GHLearning.CleanArchitecture.Core.Users;

public record UserInfo(
	Guid Id,
	string Email,
	string FirstName,
	string LastName,
	string PasswordHash);
