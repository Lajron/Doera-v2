using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.DTOs.TodoList.Responses;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Identity;
using Doera.Infrastructure.Persistance;
using Doera.Infrastructure.Queries;
using Doera.Infrastructure.Queries.TodoListHandlers;
using Doera.Infrastructure.Repositories;
using Doera.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Doera.Infrastructure.Extensions {
    public static class InfrastructureExtensions {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services) {
            services.AddUtilities();
            services.AddRepositories();
            services.AddUnitOfWork();
            services.AddIdentityAdapters();
            services.AddQueryHandlers();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services) {
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();
            services.AddScoped<ITodoItemTagRepository, TodoItemTagRepository>();
            services.AddScoped<ITodoListRepository, TodoListRepository>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services) {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddIdentityAdapters(this IServiceCollection services) {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IIdentityService, IdentityService>();
            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services) {
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            services.AddScoped<IQueryHandler<GetTodoListByIdRequest, GetTodoListByIdResponse?>, GetTodoListByIdHandler>();
            return services;
        }

        public static IServiceCollection AddUtilities(this IServiceCollection services) {
            services.AddScoped<ISlugGenerator, SlugGenerator>();
            return services;
        }
    }
}
