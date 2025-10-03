using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Tags;
using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.DTOs.TodoList.Responses;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Doera.Infrastructure.Queries.TodoListHandlers {
    // Don't need this one anymore
    internal class GetTodoListByIdHandler(
            ApplicationDbContext _db,
            ICurrentUser _currentUser
        ) : IQueryHandler<GetTodoListByIdRequest, TodoListDto> {

        public async Task<Result<TodoListDto>> HandleAsync(GetTodoListByIdRequest query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();

            var dto = await _db.TodoLists
                .Where(l => l.Id == query.Id && l.UserId == userId)
                .Select(l => new TodoListDto {
                    Id = l.Id,
                    Name = l.Name,
                    Order = l.Order
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return Errors.TodoList.NotFound();

            return dto;
        }
    }
}
