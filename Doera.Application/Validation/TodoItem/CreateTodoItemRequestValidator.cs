using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Core.Enums;
using FluentValidation;

namespace Doera.Application.Validation.TodoItem;

public sealed class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemRequestValidator()
    {
        RuleFor(x => x.TodoListId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status != default);

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority != default);

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue)
            .WithMessage("Due date must be on or after Start date.");

        RuleForEach(x => x.TagNames!)
            .NotEmpty()
            .MaximumLength(50)
            .OverridePropertyName("TagNames")
            .When(x => x.TagNames != null);
    }
}