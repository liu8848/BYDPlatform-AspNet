using System.Linq.Expressions;

namespace BYDPlatform.Application.Common.Interfaces;

public interface IRepository<T> where T:class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    ValueTask<T?> GetAsync(object key);
    Task DeleteAsync(object key);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    //基础查询接口
    IQueryable<T> GetAsQueryable();
    IQueryable<T> GetAsQueryable(ISpecification<T> spec);
    
    //查询数量相关接口
    int Count(ISpecification<T>? spec = null);
    int Count(Expression<Func<T, bool>> condition);
    Task<int> CountAsync(ISpecification<T>? spec);
    
    //查询存在性相关接口
    bool Any(ISpecification<T>? spec);
    bool Any(Expression<Func<T, bool>>? condition = null);
    
    //根据条件获取原始实体类型相关接口
    Task<T?> GetAsync(Expression<Func<T, bool>> condition);
    Task<IReadOnlyList<T>> GetAsync();
    Task<IReadOnlyList<T>> GetAsync(ISpecification<T>? spec);

    //根据条件获取映射实体类型相关数据接口，使用Selector传入映射
    TResult? SelectFirstOrDefault<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector);
    Task<TResult?> SelectFirstOrDefaultAsync<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector);

    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector);
    Task<IReadOnlyList<TResult>> SelectAsync<TGroup, TResult>(Expression<Func<T, TGroup>> groupExpression, 
        Expression<Func<IGrouping<TGroup, T>, TResult>> selector, ISpecification<T>? spec = null);

}