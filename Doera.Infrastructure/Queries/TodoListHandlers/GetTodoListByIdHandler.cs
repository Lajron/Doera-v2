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
                        Order = l.Order,
                        TodoItems = l.TodoItems
                            .OrderBy(i => i.Order)
                            .Select(i => new TodoItemSummaryDto {
                                Id = i.Id,
                                Title = i.Title,
                                Description = i.Description,
                                Order = i.Order,
                                Status = i.Status,
                                Priority = i.Priority,
                                StartDate = i.StartDate,
                                DueDate = i.DueDate,
                                IsArchived = i.ArchivedAt.HasValue,
                                Tags = i.TodoItemTags.Select(t => new TagDto {
                                    Id = t.Tag!.Id,
                                    DisplayName = t.Tag!.DisplayName
                                })
                            })
                })
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return Errors.TodoList.NotFound();

            return dto;
        }
    }
}
