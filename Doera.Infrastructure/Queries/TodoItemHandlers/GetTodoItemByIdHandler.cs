using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Tags;
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
        ) : IQueryHandler<GetTodoItemByIdRequest, TodoItemDto> {
        public async Task<Result<TodoItemDto>> HandleAsync(GetTodoItemByIdRequest query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();

            var dto = await _db.TodoItems
                .Where(i => i.Id == query.Id && i.UserId == userId)
                .Select(i => new TodoItemDto {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Order = i.Order,
                    Status = i.Status,
                    Priority = i.Priority,
                    StartDate = i.StartDate,
                    DueDate = i.DueDate,
                    ArchivedAt = i.ArchivedAt,
                    Tags = i.TodoItemTags.Select(t => new TagDto {
                        Id = t.TagId,
                        DisplayName = t.Tag!.DisplayName
                    })

                })
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return Errors.TodoItem.NotFound();

            return dto;
        }
    }
}
