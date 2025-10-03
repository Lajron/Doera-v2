using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoList;

namespace Doera.Web.Features.TodoList.ViewModels {
    public record TodoListPageVM(
        TodoListDto List,
        TodoItemFilter Filter
    );
}
