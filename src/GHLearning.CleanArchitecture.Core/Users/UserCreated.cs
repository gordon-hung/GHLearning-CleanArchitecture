namespace GHLearning.CleanArchitecture.Core.Users;

public record UserCreated(
	Guid Id,
	string Email,
	string FirstName,
	string LastName,
	string PasswordHash);
