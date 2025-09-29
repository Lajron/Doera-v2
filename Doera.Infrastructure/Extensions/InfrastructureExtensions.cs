using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Identity;
using Doera.Infrastructure.Persistance;
using Doera.Infrastructure.Queries;
using Doera.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Doera.Infrastructure.Extensions {
    public static class InfrastructureExtensions {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) {
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
            return services;
        }
    }
}
