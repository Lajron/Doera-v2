using Doera.Application.DTOs.TodoItem;
using Doera.Core.Entities;
using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Extensions {
    internal static class TodoItemQueryExtensions {
        public static IQueryable<TodoItem> ApplyFilter(this IQueryable<TodoItem> query, Guid userId, TodoItemFilter filter) {
            query = query.Where(i => i.UserId == userId);

            if (filter.TodoListId is Guid listId)
                query = query.Where(i => i.TodoListId == listId);

            if (filter.Status is TodoStatus status)
                query = query.Where(i => i.Status == status);

            if (filter.Priority is TodoPriority priority)
                query = query.Where(i => i.Priority == priority);

            if (filter.DueAfter is DateTimeOffset dueAfter)
                query = query.Where(i => i.DueDate >= dueAfter);

            if (filter.DueBefore is DateTimeOffset dueBefore)
                query = query.Where(i => i.DueDate <= dueBefore);

            if (filter.TagIds is not null) {
                var tagIdsList = filter.TagIds.ToArray();
                if (tagIdsList.Length != 0)
                    query = query.Where(i => i.TodoItemTags.Any(t => tagIdsList.Contains(t.TagId)));
            }
                

            if (!string.IsNullOrWhiteSpace(filter.Search)) {
                var search = filter.Search.Trim();
                query = query.Where(i => i.Title.Contains(search) || (i.Description != null && i.Description.Contains(search)));
            }

            if (filter.IsArchived is not null) {
                if (filter.IsArchived.Value) query = query.Where(i => i.ArchivedAt != null);
                else query = query.Where(i => i.ArchivedAt == null);
            }

            return query;
        }

        public static IQueryable<TodoItem> ApplySort(this IQueryable<TodoItem> query, TodoSort sort = TodoSort.Order, SortDirection dir = SortDirection.Asc) {
            var isAsc = (dir == SortDirection.Asc);

            return sort switch {
                TodoSort.Order => isAsc ? query.OrderBy(i => i.Order).ThenBy(i => i.CreatedAt) : query.OrderByDescending(i => i.Order).ThenByDescending(i => i.CreatedAt),
                TodoSort.DueDate => isAsc ? query.OrderBy(i => i.DueDate).ThenBy(i => i.Order) : query.OrderByDescending(i => i.DueDate).ThenByDescending(i => i.Order),
                TodoSort.CreatedAt => isAsc ? query.OrderBy(i => i.CreatedAt).ThenBy(i => i.Order) : query.OrderByDescending(i => i.CreatedAt).ThenByDescending(i => i.Order),
                _ => throw new ArgumentOutOfRangeException(nameof(sort), "Invalid sort option")
            };
        }
    }
}
