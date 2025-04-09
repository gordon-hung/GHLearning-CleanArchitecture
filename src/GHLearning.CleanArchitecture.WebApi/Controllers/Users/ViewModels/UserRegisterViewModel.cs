namespace GHLearning.CleanArchitecture.WebApi.Controllers.Users.ViewModels;

public record UserRegisterViewModel(
	string Email,
	string FirstName,
	string LastName,
	string Password);
