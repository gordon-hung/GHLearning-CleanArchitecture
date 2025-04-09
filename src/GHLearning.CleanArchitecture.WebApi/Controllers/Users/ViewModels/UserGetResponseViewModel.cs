namespace GHLearning.CleanArchitecture.WebApi.Controllers.Users.ViewModels;

public record UserGetResponseViewModel(
	Guid Id,
	string Email,
	string FirstName,
	string LastName);
