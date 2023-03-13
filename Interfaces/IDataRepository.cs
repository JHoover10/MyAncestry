using MyAncestry.Models;
using System.Linq.Expressions;

namespace MyAncestry.Interfaces;

public interface IDataRepository
{
    IEnumerable<TResult> Query<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate = null, Func<TEntity, TResult> selector = null);
}
