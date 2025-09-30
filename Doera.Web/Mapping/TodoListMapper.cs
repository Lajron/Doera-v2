using Doera.Application.DTOs.TodoList.Requests;
using Doera.Web.Features.TodoList.ViewModels;

namespace Doera.Web.Mapping {
    public static class TodoListMapper {

        public static CreateTodoListRequest ToDto(this CreateTodoListVM model) {
            return new CreateTodoListRequest {
                Title = model.Title
            };
        }

    }
}
