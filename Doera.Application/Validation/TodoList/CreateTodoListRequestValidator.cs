using Doera.Application.DTOs.TodoList.Requests;
using FluentValidation;

namespace Doera.Application.Validation.TodoList;

public sealed class CreateTodoListRequestValidator : AbstractValidator<CreateTodoListRequest>
{
    public CreateTodoListRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}