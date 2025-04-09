using FluentValidation;

namespace GHLearning.CleanArchitecture.Application.Users.Register;

internal sealed class UserRegisterCommandRequestValidator : AbstractValidator<UserRegisterCommandRequest>
{
	public UserRegisterCommandRequestValidator()
	{
		RuleFor(c => c.FirstName).NotEmpty();
		RuleFor(c => c.LastName).NotEmpty();
		RuleFor(c => c.Email).NotEmpty().EmailAddress();
		RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
	}
}
