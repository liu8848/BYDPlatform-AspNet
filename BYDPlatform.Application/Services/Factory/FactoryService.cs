using System.Dynamic;
using AutoMapper;
using BYDPlatform.Application.Common.Attributes;
using BYDPlatform.Application.Common.Exceptions;
using BYDPlatform.Application.Common.Extensions;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Mappings;
using BYDPlatform.Application.Common.Model;
using BYDPlatform.Domain.DTOs;
using BYDPlatform.Domain.DTOs.Base;
using BYDPlatform.Domain.DTOs.RegisterFactory;
using BYDPlatform.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Application.Services.Factory;

[Service(LifeTime = ServiceLifetime.Scoped)]
public class FactoryService : IFactoryService
{
    private readonly IDataShaper<RegisterFactory> _dataShaper;
    private readonly IMapper _mapper;
    private readonly IRepository<RegisterFactory> _repository;


    public FactoryService(
        IRepository<RegisterFactory> repository,
        IDataShaper<RegisterFactory> dataShaper,
        IMapper mapper
    )
    {
        _repository = repository;
        _dataShaper = dataShaper;
        _mapper = mapper;
    }

    public async Task<RegisterFactory> GetById(int id)
    {
        var registerFactory = await _repository.GetAsync(id);
        if (registerFactory is null)
            throw new NotFoundException(nameof(RegisterFactory), id);
        return registerFactory;
    }

    public async Task<List<RegisterFactory>> GetAll()
    {
        return await _repository.GetAsQueryable().ToListAsync();
    }

    public async Task<List<RegisterFactory>> GetListQuery(IBaseQueryDto query)
    {
        var queryDto = (RegisterFactoryQueryDto)query;
        var list = await _repository.GetAsQueryable()
            .AsNoTracking()
            .Where(
                f =>
                    (queryDto.BuId == 0 || f.BuId == queryDto.BuId) &&
                    (string.IsNullOrEmpty(queryDto.FactoryName) || f.FactoryName.Contains(queryDto.FactoryName)) &&
                    (queryDto.Level == null || f.Level == queryDto.Level)
            )
            .ToListAsync();
        return list;
    }

    public async Task<PaginatedList<RegisterFactory>> GetPageQueryList(IBaseQueryDto query)
    {
        var queryDto = (RegisterFactoryQueryDto)query;

        var pageList = await _repository.GetAsQueryable()
            .AsNoTracking()
            .Where(
                f =>
                    (queryDto.BuId == 0 || f.BuId == queryDto.BuId) &&
                    (string.IsNullOrEmpty(queryDto.FactoryName) || f.FactoryName.Contains(queryDto.FactoryName)) &&
                    (queryDto.Level == null || f.Level == queryDto.Level)
            ).PaginatedListAsync(queryDto.PageNumber, queryDto.PageSize);
        return pageList;
    }

    public async Task<PaginatedList<ExpandoObject>> GetPageQueryShapeList(IBaseQueryDto query)
    {
        var pageQueryList = await GetPageQueryList(query);
        return pageQueryList.ShapeData(_dataShaper, query.Fields);
    }

    public async Task<RegisterFactory> Create(IBaseCreateOrUpdateDto create)
    {
        var dto = (RegisterFactoryCreateOrUpdateDto)create;

        var entity = new RegisterFactory();

        entity.BuId = dto.BuId;
        entity.FactoryName = dto.FactoryName;
        entity.Level = dto.Level;

        await _repository.AddAsync(entity);
        return entity;
    }

    public async Task<RegisterFactory> Update(IBaseCreateOrUpdateDto update)
    {
        var updateDto = (RegisterFactoryCreateOrUpdateDto)update;

        var entity = await _repository.GetAsync(updateDto.Id);
        if (entity is null)
            throw new NotFoundException(nameof(RegisterFactory), updateDto.Id);

        entity.FactoryName = updateDto.FactoryName ?? entity.FactoryName;
        entity.Level = updateDto.Level;

        await _repository.UpdateAsync(entity);
        return entity;
    }

    public async Task<object> Delete(object id)
    {
        var entity = _repository
            .GetAsQueryable()
            .FirstOrDefault(f => f.Id == (int)id);
        if (entity is null) throw new NotFoundException(nameof(RegisterFactory), id);

        await _repository.DeleteAsync(entity);
        return Unit.Value;
    }

    public async Task<List<RegisterFactory>> BatchInsert(List<RegisterFactory> insertList)
    {
        await _repository.AddRange(insertList);
        return insertList;
    }
}