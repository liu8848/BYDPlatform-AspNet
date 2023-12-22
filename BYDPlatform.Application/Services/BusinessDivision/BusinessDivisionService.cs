using AutoMapper;
using BYDPlatform.Application.Common.Attributes;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.DTOs;
using BYDPlatform.Domain.DTOs.BusinessDivision;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Application.Services.BusinessDivision;


[Service(LifeTime = ServiceLifetime.Scoped)]
public class BusinessDivisionService:IBusinessDivisionService
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;
    private readonly IMapper _mapper;

    public BusinessDivisionService(
        IRepository<Domain.Entities.BusinessDivision> repository,
        IMapper mapper
        )
    {
        _repository = repository;
        _mapper = mapper;
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

    public async Task<Domain.Entities.BusinessDivision> Create(IBaseCreateDto create)
    {
        var createDto = (BusinessDivisionCreateDivision)create;
        var bu = _mapper.Map<Domain.Entities.BusinessDivision>(createDto);
        await _repository.AddAsync(bu);
        return bu;
    }
}