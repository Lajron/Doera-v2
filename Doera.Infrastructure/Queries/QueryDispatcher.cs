using Doera.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Queries {
    public class QueryDispatcher(IServiceProvider _serviceProvider) : IQueryDispatcher {
        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery {
            var service = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await service.HandleAsync(query);
        }
    }
}
