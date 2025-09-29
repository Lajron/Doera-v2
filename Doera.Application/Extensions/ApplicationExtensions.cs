using Doera.Application.Interfaces.Services;
using Doera.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.Extensions {
    public static class ApplicationExtensions {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITodoItemService, TodoItemService>();
            services.AddScoped<ITodoListService, TodoListService>();
            services.AddScoped<IAdminService, AdminService>();

            return services;
        }
    }
}
