namespace GHLearning.CleanArchitecture.Application.Users.GetByEmail;

public record UserGetByEmailQueryResponse(
	Guid Id,
	string Email,
	string FirstName,
	string LastName);
