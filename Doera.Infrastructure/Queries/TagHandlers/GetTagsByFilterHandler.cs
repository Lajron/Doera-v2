using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Tags;
using Doera.Application.DTOs.TodoItem;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Infrastructure.Data;
using Doera.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Queries.TagHandlers {
    internal class GetTagsByFilterHandler(
            ApplicationDbContext _db,
            ICurrentUser _currentUser
        ) : IQueryHandler<TagFilter, IEnumerable<TagDto>> {
        public async Task<Result<IEnumerable<TagDto>>> HandleAsync(TagFilter query, CancellationToken cancellationToken = default) {
            var userId = _currentUser.RequireUserId();

            var dto = await _db.Tags
                .ApplyFilter(userId, query)
                .ApplySort(query.Direction)
                .Distinct()
                .Select(t => new TagDto {
                    Id = t.Id,
                    DisplayName = t.DisplayName
                })
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            return dto;
        }
    }
}
