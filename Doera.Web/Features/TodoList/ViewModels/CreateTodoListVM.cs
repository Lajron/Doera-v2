using System.ComponentModel.DataAnnotations;

namespace Doera.Web.Features.TodoList.ViewModels {
    public record CreateTodoListVM {
        [Required]
        [StringLength(64, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
    }
}
