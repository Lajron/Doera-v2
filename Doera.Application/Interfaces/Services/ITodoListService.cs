using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoList.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doera.Application.Interfaces.Services {
    public interface ITodoListService {
        Task<Result<Guid>> CreateAsync(CreateTodoListRequest request);
    }
}