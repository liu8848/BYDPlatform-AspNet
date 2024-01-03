using System.Linq.Expressions;
using BYDPlatform.Application.Common;
using BYDPlatform.Application.Common.Interfaces;
using BydPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    private readonly BydPlatformDbContext _dbContext;

    public RepositoryBase(BydPlatformDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task AddRange(List<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().AddRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual ValueTask<T?> GetAsync(object key)
    {
        return _dbContext.Set<T>().FindAsync(key);
    }

    public async Task DeleteAsync(object key)
    {
        var entity = await GetAsync(key);
        if (entity is not null) await DeleteAsync(entity);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    //基础查询接口实现
    public IQueryable<T> GetAsQueryable()
    {
        return _dbContext.Set<T>();
    }

    public IQueryable<T> GetAsQueryable(ISpecification<T> spec)
    {
        return ApplySpecification(spec);
    }

    //查询数量相关接口实现
    public int Count(ISpecification<T>? spec = null)
    {
        return null != spec ? ApplySpecification(spec).Count() : _dbContext.Set<T>().Count();
    }

    public int Count(Expression<Func<T, bool>> condition)
    {
        return _dbContext.Set<T>().Count(condition);
    }

    public Task<int> CountAsync(ISpecification<T>? spec)
    {
        return ApplySpecification(spec).CountAsync();
    }

    //查询存在性相关接口实现
    public bool Any(ISpecification<T>? spec)
    {
        return ApplySpecification(spec).Any();
    }

    public bool Any(Expression<Func<T, bool>>? condition = null)
    {
        return null != condition ? _dbContext.Set<T>().Any(condition) : _dbContext.Set<T>().Any();
    }

    //根据条件获取原始实体类型数据相关接口实现
    public async Task<T?> GetAsync(Expression<Func<T, bool>> condition)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(condition);
    }

    public async Task<IReadOnlyList<T>> GetAsync()
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T>? spec)
    {
        return await ApplySpecification(spec).AsNoTracking().ToListAsync();
    }

    //根据条件获取映射实体类型相关数据接口
    public TResult? SelectFirstOrDefault<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector)
    {
        return ApplySpecification(spec).AsNoTracking().Select(selector).FirstOrDefault();
    }

    public Task<TResult?> SelectFirstOrDefaultAsync<TResult>(ISpecification<T>? spec,
        Expression<Func<T, TResult>> selector)
    {
        return ApplySpecification(spec).AsNoTracking().Select(selector).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        return await _dbContext.Set<T>().AsNoTracking().Select(selector).ToListAsync();
    }


    public async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(ISpecification<T>? spec,
        Expression<Func<T, TResult>> selector)
    {
        return await ApplySpecification(spec).AsNoTracking().Select(selector).ToListAsync();
    }

    public async Task<IReadOnlyList<TResult>> SelectAsync<TGroup, TResult>(Expression<Func<T, TGroup>> groupExpression,
        Expression<Func<IGrouping<TGroup, T>, TResult>> selector, ISpecification<T>? spec = null)
    {
        return null != spec
            ? await ApplySpecification(spec).AsNoTracking().GroupBy(groupExpression).Select(selector).ToListAsync()
            : await _dbContext.Set<T>().AsNoTracking().GroupBy(groupExpression).Select(selector).ToListAsync();
    }


    private IQueryable<T> ApplySpecification(ISpecification<T>? spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
    }
}