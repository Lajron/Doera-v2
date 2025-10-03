using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Validation.TodoItem;
using Doera.Application.Validation.TodoList;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Doera.Application.Extensions;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateTodoItemRequest>,CreateTodoItemRequestValidator>();
        services.AddScoped<IValidator<CreateTodoListRequest>,CreateTodoListRequestValidator>();

        return services;
    }
}