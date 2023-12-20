using AutoMapper;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Mappings;
using BYDPlatform.Application.Common.Model;
using MediatR;

namespace BYDPlatform.Application.BusinessDivision.Queries;
/**
 * 条件分页查询Command
 */
public class GetBusinessDivisionWithPaginationQuery:IRequest<PaginatedList<Domain.Entities.BusinessDivision>>
{
 public string? BuName { get; set; }
 public int PageSize { get; set; } = 1;
 public int PageNumber { get; set; } = 10;
}


public class GetBusinessDivisionWithPaginationQueryHandler
 : IRequestHandler<GetBusinessDivisionWithPaginationQuery, PaginatedList<Domain.Entities.BusinessDivision>>
{
 private readonly IRepository<Domain.Entities.BusinessDivision> _repository;
 private readonly IMapper _mapper;

 public GetBusinessDivisionWithPaginationQueryHandler(IRepository<Domain.Entities.BusinessDivision> repository, IMapper mapper)
 {
  _repository = repository;
  _mapper = mapper;
 }


 public async Task<PaginatedList<Domain.Entities.BusinessDivision>> Handle(GetBusinessDivisionWithPaginationQuery request, CancellationToken cancellationToken)
 {
  return await _repository
   .GetAsQueryable()
   .Where(x=>(string.IsNullOrEmpty(request.BuName)||x.BuName.Contains(request.BuName!)))
   .OrderBy(x => x.Id)
   .PaginatedListAsync(request.PageNumber, request.PageSize);
 }
}