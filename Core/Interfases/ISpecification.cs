using System.Linq.Expressions;

namespace Core.Interfases;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    Expression<Func<T, object>>? orderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }

    bool IsDistinct { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }

    IQueryable<T> ApplayCriteria(IQueryable<T> query);
}


public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Select { get; }
}