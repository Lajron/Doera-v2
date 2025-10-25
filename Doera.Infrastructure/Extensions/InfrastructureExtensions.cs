using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.DTOs.TodoList.Responses;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Doera.Infrastructure.Email;
using Doera.Infrastructure.Identity;
using Doera.Infrastructure.Persistance;
using Doera.Infrastructure.Queries;
using Doera.Infrastructure.Queries.TodoItemHandlers;
using Doera.Infrastructure.Queries.TodoListHandlers;
using Doera.Infrastructure.Repositories;
using Doera.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Doera.Infrastructure.Extensions {
    public static class InfrastructureExtensions {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services) {
            services.AddUtilities();
            services.AddRepositories();
            services.AddUnitOfWork();
            services.AddIdentityAdapters();
            services.AddQueryHandlers();

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services) {
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();
            services.AddScoped<ITodoItemTagRepository, TodoItemTagRepository>();
            services.AddScoped<ITodoListRepository, TodoListRepository>();

            //var assembly = typeof(UnitOfWork).Assembly;

            //var repoTypes = assembly.GetTypes()
            //    .Where(t => 
            //        t.IsClass && 
            //        !t.IsAbstract && 
            //        t.BaseType != null && 
            //        t.BaseType.IsGenericType &&
            //        t.BaseType.GetGenericTypeDefinition() == typeof(BaseRepository<>)
            //    ).ToList();

            //foreach (var repo in repoTypes) {
            //    var repoInterface = repo.GetInterfaces().First(ri => ri.Name.EndsWith("Repository"));
            //    services.AddScoped(repoInterface, repo);

            //}
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

            //Reflection to automatically register all query handlers
            var assembly = typeof(QueryDispatcher).Assembly;
            var handlerInterface = typeof(IQueryHandler<,>);

            var handlerTypes =
                assembly.GetTypes()
                .Where(
                    t => t.IsClass &&
                    !t.IsAbstract &&
                    !t.IsGenericType &&
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == handlerInterface
                    )
                )
                .ToList();

            foreach (var impl in handlerTypes) {
                var implInterface = impl.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface);
                // Why did i add another foreach wtf was I thinking
                // This should have just been:
                //services.AddScoped(implInterface.First(), impl)
                foreach (var iface in implInterface) {
                    services.AddScoped(iface, impl);
                }
            }

            //services.AddScoped<IQueryHandler<GetTodoItemByIdRequest, TodoItemDto>, GetTodoItemByIdHandler>();
            //services.AddScoped<IQueryHandler<GetTodoListByIdRequest, TodoListDto>, GetTodoListByIdHandler>();

            return services;
        }

        public static IServiceCollection AddUtilities(this IServiceCollection services) {
            services.AddScoped<ISlugGenerator, SlugGenerator>();
            return services;
        }
    }
}
