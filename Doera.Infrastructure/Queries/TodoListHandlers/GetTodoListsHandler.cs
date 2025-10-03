using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Queries.TodoListHandlers {
    internal class GetTodoListsHandler(
            ApplicationDbContext _db,
            ICurrentUser _currentUser
        ) : IQueryHandler<GetTodoListsRequest, IEnumerable<TodoListDto>> {
        public async Task<Result<IEnumerable<TodoListDto>>> HandleAsync(GetTodoListsRequest query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();

            return await _db.TodoLists
                .Where(l => l.UserId == userId)
                .OrderBy(l => l.Order)
                .Select(l => new TodoListDto {
                    Id = l.Id,
                    Name = l.Name,
                    Order = l.Order
                })
                .ToListAsync(cancellationToken);
        }
    }
}
