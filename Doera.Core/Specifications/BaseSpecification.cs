using Doera.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Specifications {
    public class BaseSpecification<TEntity>(
            Expression<Func<TEntity, bool>>? expression = null
        ) : ISpecification<TEntity> {

        public Expression<Func<TEntity, bool>>? Expression { get; } = expression;
        public List<Expression<Func<TEntity, object>>> Includes { get; } = [];
        public List<string> IncludeStrings { get; } = [];

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) {
            Includes.Add(includeExpression);
        }

        // God forsake me if i ever have to use this
        // "PostTags.Tag" or "ApplicationUser.Followers" on _dbSet.Posts.Include(...)
        protected void AddInclude(string includeString) {
            IncludeStrings.Add(includeString);
        }


    }
}
