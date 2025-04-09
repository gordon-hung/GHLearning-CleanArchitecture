namespace GHLearning.CleanArchitecture.Application.Users.GetById;

public record UserGetByIdQueryResponse(
	Guid Id,
	string Email,
	string FirstName,
	string LastName);
