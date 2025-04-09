using FluentValidation;

namespace GHLearning.CleanArchitecture.Application.TodoItems.Delete;

internal sealed class TodoItemDeleteCommandRequestValidator : AbstractValidator<TodoItemDeleteCommandRequest>
{
	public TodoItemDeleteCommandRequestValidator()
	{
		RuleFor(c => c.Id).NotEmpty();
	}
}
