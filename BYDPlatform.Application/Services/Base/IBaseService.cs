using System.Dynamic;
using BYDPlatform.Application.Common.Model;
using BYDPlatform.Domain.DTOs;
using BYDPlatform.Domain.DTOs.Base;

namespace BYDPlatform.Application.Services.Base;

public interface IBaseService<T> where T : class
{
    Task<T> GetById(int id);
    Task<List<T>> GetAll();
    Task<List<T>> GetListQuery(IBaseQueryDto query);
    Task<PaginatedList<T>> GetPageQueryList(IBaseQueryDto query);
    Task<PaginatedList<ExpandoObject>> GetPageQueryShapeList(IBaseQueryDto query);

    Task<T> Create(IBaseCreateOrUpdateDto create);

    Task<T> Update(IBaseCreateOrUpdateDto update);

    Task<object> Delete(object id);

    Task<List<T>> BatchInsert(List<T> insertList);
}