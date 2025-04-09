using FluentValidation;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Completed;

internal sealed class TodoItemCompletedCommandRequestValidator : AbstractValidator<TodoItemCompletedCommandRequest>
{
	public TodoItemCompletedCommandRequestValidator()
	{
		RuleFor(c => c.Id).NotEmpty();
	}
}
