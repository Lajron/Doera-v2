using Doera.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.Interfaces {
    public interface IQueryHandler<TQuery, TResult> where TQuery : class {
        Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
