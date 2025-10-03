using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Common;
using Doera.Application.DTOs.TodoList.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doera.Application.Interfaces.Services {
    public interface ITodoListService {
        Task<Result<Guid>> CreateAsync(CreateTodoListRequest request);
        public Task<Result> UpdateAsync(UpdateTodoListRequest request);
        public Task<Result> DeleteAsync(Guid id);
        public Task<Result> ReorderAsync(IEnumerable<ReorderRequest> request);
    }
}