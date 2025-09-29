using Doera.Application.Interfaces;
using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Persistance;
using Doera.Infrastructure.Queries;
using Doera.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Extensions {
    public static class InfrastructureExtensions {

        /// <summary>
        /// Adds infrastructure services to the IServiceCollection.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) {
            services.AddRepositories();
            services.AddUnitOfWork();
            services.AddQueryHandlers();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services) {
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoItemTagRepository, TodoItemTagRepository>();
            services.AddScoped<IUserTagRepository, UserTagRepository>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services) {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services) {
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            // services.AddScoped<IQueryHandler<SomeQuery, SomeResult>, SomeQueryHandler>();
            return services;
        }


    }
}
