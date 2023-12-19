using BYDPlatform.Application.BusinessDivision.Specs;
using BYDPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.BusinessDivision.Queries;

public class GetBusinessDivisionQuery:IRequest<Domain.Entities.BusinessDivision>
{
    public int Id { get; set; } = 0;
    public string? BuName { get; set; }
}

public class GetBusinessDivision : IRequestHandler<GetBusinessDivisionQuery, Domain.Entities.BusinessDivision>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;

    public GetBusinessDivision(IRepository<Domain.Entities.BusinessDivision> repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.BusinessDivision?> Handle(GetBusinessDivisionQuery request, CancellationToken cancellationToken)
    {
        var bu = new Domain.Entities.BusinessDivision
        {
            Id = request.Id,
            BuName = request.BuName
        };

        var spec = new BusinessDivisionSpec(bu);
        return await _repository
            .GetAsQueryable(spec)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        
    }
}