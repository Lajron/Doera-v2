using Doera.Application.DTOs.Tags;
using Doera.Application.DTOs.TodoItem;
using Doera.Core.Entities;
using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Extensions {
    internal static class TagQueryExtensions {

        public static IQueryable<Tag> ApplyFilter(this IQueryable<Tag> query, Guid userId, TagFilter filter) {
            query = query.Where(t => t.UserId == userId);

            if (filter.TodoListId is Guid listId) {
                query = query.Where(t => t.TodoItemTags.Any(it => it.TodoItem!.TodoListId == listId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search)) {
                var search = filter.Search.Trim();
                query = query.Where(t => t.NormalizedName.Contains(search));
            }

            return query;
        }

        public static IQueryable<Tag> ApplySort(this IQueryable<Tag> query, SortDirection dir = SortDirection.Asc) {
            var isAsc = (dir == SortDirection.Asc);

            return isAsc ? query.OrderBy(t => t.TodoItemTags.Count).ThenBy(t => t.NormalizedName) 
                         : query.OrderByDescending(t => t.TodoItemTags.Count).ThenByDescending(t => t.NormalizedName);
        }
    }
}
