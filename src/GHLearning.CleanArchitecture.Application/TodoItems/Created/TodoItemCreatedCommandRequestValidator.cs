using FluentValidation;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Created;

public class TodoItemCreatedCommandRequestValidator : AbstractValidator<TodoItemCreatedCommandRequest>
{
	public TodoItemCreatedCommandRequestValidator()
	{
		RuleFor(c => c.Priority).IsInEnum();
		RuleFor(c => c.Description).NotEmpty().MaximumLength(255);
		RuleFor(c => c.DueDate).GreaterThanOrEqualTo(DateTime.Today).When(x => x.DueDate.HasValue);
	}
}
