using System;
using System.ComponentModel.DataAnnotations;

namespace Doera.Web.Features.TodoList.ViewModels {
    public record EditTodoListVM {
        [Required] public Guid Id { get; init; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; init; } = string.Empty;
    }
}