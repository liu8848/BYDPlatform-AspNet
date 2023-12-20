using System.Dynamic;
using AutoMapper;
using BYDPlatform.Application.Common.Extensions;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Mappings;
using BYDPlatform.Application.Common.Model;
using MediatR;

namespace BYDPlatform.Application.BusinessDivision.Queries;
/**
 * 条件分页查询Command
 */
public class GetBusinessDivisionWithPaginationQuery:IRequest<PaginatedList<ExpandoObject>>
{
 public string? BuName { get; set; }
 
 //前端指明需要返回字段
 public string? Fields { get; set; }
 public string? SortOrder { get; set; } = "title_asc";
 public int PageSize { get; set; } = 10;
 public int PageNumber { get; set; } = 1;
}


public class GetBusinessDivisionWithPaginationQueryHandler
 : IRequestHandler<GetBusinessDivisionWithPaginationQuery, PaginatedList<ExpandoObject>>
{
 private readonly IRepository<Domain.Entities.BusinessDivision> _repository;
 private readonly IMapper _mapper;
 private readonly IDataShaper<Domain.Entities.BusinessDivision> _dataShaper;

 public GetBusinessDivisionWithPaginationQueryHandler(
  IRepository<Domain.Entities.BusinessDivision> repository, 
  IMapper mapper,
  IDataShaper<Domain.Entities.BusinessDivision> dataShaper)
 {
  _repository = repository;
  _mapper = mapper;
  _dataShaper = dataShaper;
 }


 public async Task<PaginatedList<ExpandoObject>> Handle(GetBusinessDivisionWithPaginationQuery request, CancellationToken cancellationToken)
 {
  var result = await _repository
   .GetAsQueryable()
   .Where(x=>(string.IsNullOrEmpty(request.BuName)||x.BuName.Contains(request.BuName!)))
   .OrderBy(x => x.Id)
   .PaginatedListAsync(request.PageNumber, request.PageSize);
  return result.ShapeData(_dataShaper, request.Fields);
 }
}