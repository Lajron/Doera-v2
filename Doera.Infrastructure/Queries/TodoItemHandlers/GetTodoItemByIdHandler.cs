using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.DTOs.TodoItem.Responses;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.DTOs.TodoList.Responses;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Queries.TodoItemHandlers {
    internal class GetTodoItemByIdHandler(
            ApplicationDbContext _db,
            ICurrentUser _currentUser
        ) : IQueryHandler<GetTodoItemByIdRequest, GetTodoItemByIdResponse?> {
        public async Task<Result<GetTodoItemByIdResponse?>> HandleAsync(GetTodoItemByIdRequest query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();

            var dto = await _db.TodoItems
                .Where(i => i.Id == query.Id && i.UserId == userId)
                .Select(i => new TodoItemDto {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Order = i.Order,

                })
                .FirstOrDefaultAsync(cancellationToken);

            return Result<GetTodoItemByIdResponse?>.Success(null);
        }
    }
}
