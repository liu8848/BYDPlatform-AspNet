using System.Dynamic;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Model;

namespace BYDPlatform.Application.Common.Extensions;

public static class DataShaperExtensions
{
    public static IEnumerable<ExpandoObject> ShapeData<T>(this IEnumerable<T> entities,
        IDataShaper<T> shaper, string? fieldString)
    {
        return shaper.ShapeDate(entities, fieldString);
    }

    public static PaginatedList<ExpandoObject> ShapeData<T>(this PaginatedList<T> paginatedList,
        IDataShaper<T> shaper, string? fieldString)
    {
        var items = shaper.ShapeDate(paginatedList.Items, fieldString).ToList();
        return new PaginatedList<ExpandoObject>(items, paginatedList.PageNumber, paginatedList.TotalCount,
            paginatedList.PageSize);
    }
}