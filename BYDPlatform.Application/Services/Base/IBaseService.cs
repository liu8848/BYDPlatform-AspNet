using BYDPlatform.Domain.DTOs;

namespace BYDPlatform.Application.Services.Base;

public interface IBaseService<T> where T:class
{
    Task<T> GetById(int id);

    Task<List<T>> GetAll();
    
    Task<List<T>> GetListQuery(IBaseQueryDto query);

    Task<T> Create(IBaseCreateDto create);
}