using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Interfaces {
    public interface ISpecification<TEntity> {
        Expression<Func<TEntity, bool>>? Expression { get; }
        List<Expression<Func<TEntity, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
    }
}
