using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Tags;
using Doera.Application.DTOs.TodoItem;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Infrastructure.Data;
using Doera.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Doera.Infrastructure.Queries.TodoItemHandlers {
    internal class GetTodoItemByFilterHandler(
            ApplicationDbContext _db,
            ICurrentUser _currentUser
        ) : IQueryHandler<TodoItemFilter, IEnumerable<TodoItemDto>> {

        public async Task<Result<IEnumerable<TodoItemDto>>> HandleAsync(TodoItemFilter query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();

            var dto = await _db.TodoItems
                .ApplyFilter(userId, query)
                .ApplySort(query.SortBy, query.Direction)
                .Select(i => new TodoItemDto {
                    Id = i.Id,
                    TodoListId = i.TodoListId,
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
                .ToListAsync(cancellationToken);

            return dto;
        }
    }
}
