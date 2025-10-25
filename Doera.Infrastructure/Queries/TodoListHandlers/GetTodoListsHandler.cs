using Doera.Application.Abstractions.Results;
using Doera.Application.Caching;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Caching;
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
            ICurrentUser _currentUser,
            ICacheService _cache
        ) : IQueryHandler<GetTodoListsRequest, IEnumerable<TodoListDto>> {
        public async Task<Result<IEnumerable<TodoListDto>>> HandleAsync(GetTodoListsRequest query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();
            var key = CacheKeys.TodoLists.ForUser(userId);

            var lists = await _cache.GetOrCreateAsync(
                key,
                async () => await _db.TodoLists
                    .Where(x => x.UserId == userId)
                    .OrderBy(x => x.Order)
                    .Select(l => new TodoListDto {
                        Id = l.Id,
                        Name = l.Name,
                        Order = l.Order
                    })
                    .ToListAsync(cancellationToken),
                ttl: TimeSpan.FromSeconds(120),
                cancellationToken: cancellationToken
            );

            return lists ?? [];
        }
    }
}
