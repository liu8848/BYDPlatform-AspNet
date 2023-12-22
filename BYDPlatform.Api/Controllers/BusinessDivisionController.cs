using System.Dynamic;
using BYDPlatform.Api.Filters;
using BYDPlatform.Api.Models;
using BYDPlatform.Application.Common.Model;
using BYDPlatform.Application.Services.BusinessDivision;
using BYDPlatform.Domain.DTOs.BusinessDivision;
using BYDPlatform.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BYDPlatform.Api.Controllers;

[ApiController]
[Route("BusinessDivision")]
public class BusinessDivisionController:ControllerBase
{
    private readonly IBusinessDivisionService _businessDivisionService;

    private readonly IValidator<BusinessDivisionCreateOrUpdateDto> _validator;

    public BusinessDivisionController(IBusinessDivisionService businessDivisionService,
        IValidator<BusinessDivisionCreateOrUpdateDto> validator)
    {
        _businessDivisionService = businessDivisionService;
        _validator = validator;
    }

    [HttpGet("{id:int}")]
    public async Task<ApiResponse<BusinessDivision>> getOne(int id)
    {
        var businessDivision = await _businessDivisionService.GetById(id);
        return ApiResponse<BusinessDivision>.Success(businessDivision);
    }

    [HttpGet("queryList")]
    public async Task<ApiResponse<List<BusinessDivision>>> GetQueryList([FromQuery] BusinessDivisionQueryDto queryDto)
    {
        var results = await _businessDivisionService.GetListQuery(queryDto);
        return ApiResponse<List<BusinessDivision>>.Success(results);
    }

    [HttpGet("pageQueryList")]
    public async Task<ApiResponse<PaginatedList<BusinessDivision>>> GetPageQueryList(
        [FromQuery] BusinessDivisionQueryDto query)
    {
        var result = await _businessDivisionService.GetPageQueryList(query);
        return ApiResponse<PaginatedList<BusinessDivision>>.Success(result);
    }
    
    [HttpPost]
    [Route("create")]
    [Authorize(Policy = "OnlyAdmin")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<BusinessDivision>> CreateBusinessDivision([FromBody] BusinessDivisionCreateOrUpdateDto createOrUpdateDto)
    {
        if (!(await _validator.ValidateAsync(createOrUpdateDto)).IsValid)
        {
            await _validator.ValidateAndThrowAsync(createOrUpdateDto);
        }
        var bu = await _businessDivisionService.Create(createOrUpdateDto);
        return ApiResponse<BusinessDivision>.Success(bu);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "OnlyAdmin")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<BusinessDivision>> Update(int id, [FromBody] BusinessDivisionCreateOrUpdateDto update)
    {
        if (id != update.Id)
        {
            return ApiResponse<BusinessDivision>.Fail("Query id not match with body");
        }

        return ApiResponse<BusinessDivision>.Success(await _businessDivisionService.Update(update));
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<object>> Delete(int id)
    {
        return ApiResponse<object>.Success( await _businessDivisionService.Delete(id));
    }

    [HttpGet("BuList")]
    public async Task<ApiResponse<PaginatedList<ExpandoObject>>> GetBusinessDivisionWithPagination(
        [FromQuery] BusinessDivisionQueryDto query)
    {
        return ApiResponse<PaginatedList<ExpandoObject>>.Success(await _businessDivisionService.GetPageQueryShapeList(query));
    }
}