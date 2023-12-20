using AutoMapper;
using AutoMapper.QueryableExtensions;
using BYDPlatform.Application.Common.Model;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.Common.Mappings;

public static class MappingExtension
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber, int pageSize)
    {
        return PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);
    }

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
        this IQueryable queryable,
        IConfigurationProvider configurationProvider)
    {
        return queryable.ProjectTo<TDestination>(configurationProvider).ToListAsync();
    }
}