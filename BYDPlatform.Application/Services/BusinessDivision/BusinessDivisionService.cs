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
using BYDPlatform.Domain.DTOs.BusinessDivision;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Application.Services.BusinessDivision;


[Service(LifeTime = ServiceLifetime.Scoped)]
public class BusinessDivisionService:IBusinessDivisionService
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;
    private readonly IMapper _mapper;
    private readonly IDataShaper<Domain.Entities.BusinessDivision> _dataShaper;

    public BusinessDivisionService(
        IRepository<Domain.Entities.BusinessDivision> repository,
        IMapper mapper,
        IDataShaper<Domain.Entities.BusinessDivision> dataShaper)
    {
        _repository = repository;
        _mapper = mapper;
        _dataShaper = dataShaper;
    }

    public async Task<Domain.Entities.BusinessDivision> GetById(int id)
    {
        var bu = await _repository.GetAsQueryable().FirstOrDefaultAsync(b => b.Id==id);
        return bu!;
    }

    public async Task<List<Domain.Entities.BusinessDivision>> GetAll()
    {
        return await _repository.GetAsQueryable().ToListAsync();
    }

    public async Task<List<Domain.Entities.BusinessDivision>> GetListQuery(IBaseQueryDto query)
    {
        var queryDto = (BusinessDivisionQueryDto)query;

        var buQueryList = await _repository.GetAsQueryable()
            .Where(
                bu =>
                    (queryDto.Id == 0 || bu.Id == queryDto.Id)
                    && (string.IsNullOrEmpty(queryDto.BuName) || bu.BuName.Contains(queryDto.BuName))
            )
            .AsNoTracking()
            .ToListAsync();
        return buQueryList;
    }

    public async Task<Domain.Entities.BusinessDivision> Create(IBaseCreateOrUpdateDto createOrUpdate)
    {
        var createDto = (BusinessDivisionCreateOrUpdateDto)createOrUpdate;
        var bu = _mapper.Map<Domain.Entities.BusinessDivision>(createDto);
        await _repository.AddAsync(bu);
        return bu;
    }

    public async Task<Domain.Entities.BusinessDivision> Update(IBaseCreateOrUpdateDto update)
    {
        var updateDto = (BusinessDivisionCreateOrUpdateDto)update;
        var bu = await _repository.GetAsync(updateDto.Id);
        if (bu is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.BusinessDivision), updateDto.Id);
        }

        bu.BuName = updateDto.BuName ?? bu.BuName;
        await _repository.UpdateAsync(bu);
        return bu;
    }

    public async Task<object> Delete(object id)
    {
        var bu = await _repository.GetAsync((int)id);
        if (bu is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.BusinessDivision), id);
        }

        await _repository.DeleteAsync(bu);
        return Unit.Value;
    }
    

    public async Task<PaginatedList<Domain.Entities.BusinessDivision>> GetPageQueryList(IBaseQueryDto query)
    {
        var queryDto = (BusinessDivisionQueryDto)query;
        var pageResult = await _repository
            .GetAsQueryable()
            .AsNoTracking()
            .Where(
                bu=>
                    (string.IsNullOrEmpty(queryDto.BuName)||bu.BuName.Contains(queryDto.BuName))
            )
            .OrderBy(bu=>bu.Id)
            .PaginatedListAsync(queryDto.PageNumber,queryDto.PageSize);
        return pageResult;
    }

    public async Task<PaginatedList<ExpandoObject>> GetPageQueryShapeList(IBaseQueryDto query)
    {
        var queryDto = (BusinessDivisionQueryDto)query;
        var pageResult = await _repository
            .GetAsQueryable()
            .AsNoTracking()
            .Where(
                bu=>
                    (string.IsNullOrEmpty(queryDto.BuName)||bu.BuName.Contains(queryDto.BuName))
            )
            .OrderBy(bu=>bu.Id)
            .PaginatedListAsync(queryDto.PageNumber,queryDto.PageSize);
        return pageResult.ShapeData(_dataShaper, queryDto.Fields);
    }
    
    public async Task<List<Domain.Entities.BusinessDivision>> BatchInsert(List<Domain.Entities.BusinessDivision> insertList)
    {
        throw new NotImplementedException();
    }
}