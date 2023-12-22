using System.Dynamic;
using BYDPlatform.Api.Filters;
using BYDPlatform.Api.Models;
using BYDPlatform.Application.BusinessDivision.Commands.DeleteBusinessDivision;
using BYDPlatform.Application.BusinessDivision.Commands.UpdateBusinessDivision;
using BYDPlatform.Application.BusinessDivision.Queries;
using BYDPlatform.Application.Common.Model;
using BYDPlatform.Application.Services.BusinessDivision;
using BYDPlatform.Domain.DTOs.BusinessDivision;
using BYDPlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BYDPlatform.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BusinessDivisionController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IBusinessDivisionService _businessDivisionService;

    public BusinessDivisionController(IMediator mediator,IBusinessDivisionService businessDivisionService)
    {
        _mediator = mediator;
        _businessDivisionService = businessDivisionService;
    }

    [HttpGet("{id:int}")]
    public async Task<ApiResponse<BusinessDivision>> getOne(int id)
    {
        var businessDivision = await _businessDivisionService.GetById(id);
        return ApiResponse<BusinessDivision>.Success(businessDivision);
    }

    [HttpGet("/queryList")]
    public async Task<ApiResponse<List<BusinessDivision>>> GetQueryList([FromQuery] BusinessDivisionQueryDto queryDto)
    {
        var results = await _businessDivisionService.GetListQuery(queryDto);
        return ApiResponse<List<BusinessDivision>>.Success(results);
    }
    


    [HttpPost]
    [Route("/create")]
    [Authorize(Policy = "OnlyAdmin")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<BusinessDivision>> CreateBusinessDivision([FromBody] BusinessDivisionCreateDivision createDto)
    {
        var bu = await _businessDivisionService.Create(createDto);
        return ApiResponse<BusinessDivision>.Success(bu);
    }

    [HttpGet]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Authorize(Policy = "OnlyAdmin")]
    public async Task<ApiResponse<BusinessDivision>> GetBu([FromQuery] GetBusinessDivisionQuery query)
    {
        BusinessDivision bu = await _mediator.Send(query);
        return ApiResponse<BusinessDivision>.Success(bu);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "OnlyAdmin")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<BusinessDivision>> Update(int id, [FromBody] UpdateBusinessDivisionCommand command)
    {
        if (id != command.Id)
        {
            return ApiResponse<BusinessDivision>.Fail("Query id not match with body");
        }

        return ApiResponse<BusinessDivision>.Success(await _mediator.Send(command));
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<object>> Delete(int id)
    {
        return ApiResponse<object>.Success(await _mediator.Send(new DeleteBusinessDivisionCommand { Id = id }));
    }

    [HttpGet("/BuList")]
    public async Task<ApiResponse<PaginatedList<ExpandoObject>>> GetBusinessDivisionWithPagination(
        [FromQuery] GetBusinessDivisionWithPaginationQuery query)
    {
        return ApiResponse<PaginatedList<ExpandoObject>>.Success(await _mediator.Send(query));
    }
}