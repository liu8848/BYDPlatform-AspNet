using System.Dynamic;
using AutoMapper;
using BYDPlatform.Api.Filters;
using BYDPlatform.Api.Models;
using BYDPlatform.Application.Common.Extensions;
using BYDPlatform.Application.Common.Model;
using BYDPlatform.Application.Common.Tools;
using BYDPlatform.Application.Services.Factory;
using BYDPlatform.Domain.DTOs.RegisterFactory;
using BYDPlatform.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BYDPlatform.Api.Controllers;

[ApiController]
[Route("Factory")]
[Authorize(Policy = "OnlyAdmin")]
public class FactoryController : ControllerBase
{
    private readonly IFactoryService _factoryService;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterFactoryCreateOrUpdateDto> _validator;

    public FactoryController(IFactoryService factoryService,
        IValidator<RegisterFactoryCreateOrUpdateDto> validator,
        IMapper mapper)
    {
        _factoryService = factoryService;
        _validator = validator;
        _mapper = mapper;
    }

    [HttpPost]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<RegisterFactory>> Create([FromBody] RegisterFactoryCreateOrUpdateDto create)
    {
        await _validator.ValidateAndThrowAsync(create);

        var registerFactory = await _factoryService.Create(create);
        return ApiResponse<RegisterFactory>.Success(registerFactory);
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<object>> Delete(int id)
    {
        var result = await _factoryService.Delete(id);

        return ApiResponse<object>.Success(result);
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<RegisterFactory>> Update(int id, [FromBody] RegisterFactoryCreateOrUpdateDto update)
    {
        if (id != update.Id) return ApiResponse<RegisterFactory>.Fail("Query id not match with body");

        var result = await _factoryService.Update(update);
        return ApiResponse<RegisterFactory>.Success(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ApiResponse<RegisterFactory>> GetById(int id)
    {
        var result = await _factoryService.GetById(id);
        return ApiResponse<RegisterFactory>.Success(result);
    }

    [HttpGet]
    public async Task<ApiResponse<List<RegisterFactory>>> GetAll()
    {
        var registerFactories = await _factoryService.GetAll();
        return ApiResponse<List<RegisterFactory>>.Success(registerFactories);
    }

    [HttpGet("list/query")]
    public async Task<ApiResponse<List<RegisterFactory>>> GetListQuery([FromQuery] RegisterFactoryQueryDto query)
    {
        var list = await _factoryService.GetListQuery(query);
        return ApiResponse<List<RegisterFactory>>.Success(list);
    }

    [HttpGet("list/queryPage")]
    public async Task<ApiResponse<PaginatedList<RegisterFactory>>> GetListQueryPage(
        [FromQuery] RegisterFactoryQueryDto query)
    {
        var pageList = await _factoryService.GetPageQueryList(query);
        return ApiResponse<PaginatedList<RegisterFactory>>.Success(pageList);
    }

    [HttpGet("list/queryShapePage")]
    public async Task<ApiResponse<PaginatedList<ExpandoObject>>> GetQueryShapePage(
        [FromQuery] RegisterFactoryQueryDto query)
    {
        var pageQueryShapeList = await _factoryService.GetPageQueryShapeList(query);
        return ApiResponse<PaginatedList<ExpandoObject>>.Success(pageQueryShapeList);
    }

    [HttpPost("BatchInsert")]
    public async Task<IActionResult> BatchInsert([FromForm(Name = "file")] IFormFile file)
    {
        var createList = ExcelHelper.ImportExcelToEntityList<RegisterFactoryCreateOrUpdateDto>(file.OpenReadStream());
        
        //返回三个key-value
        //<"success",List<RegisterFactoryCreateOrUpdateDto>>
        //<"error",List<RegisterFactoryCreateOrUpdateDto>>
        //<"failure",List<List<ValidationFailure>>>
        var validateResultDic = await _validator.ValidateList(createList);
        var insertList = (List<RegisterFactoryCreateOrUpdateDto>)validateResultDic["success"];

        var factories = _mapper.Map<List<RegisterFactory>>(insertList);
        
        if (factories.Count > 0)
            await _factoryService.BatchInsert(factories);

        var errorList = (List<RegisterFactoryCreateOrUpdateDto>)validateResultDic["error"];
        var failureList = (List<List<ValidationFailure>>)validateResultDic["failure"];
        
        var dataTable = ExcelHelper.ListToDataTable(errorList);

        if (errorList.Count == 0)
            return Ok($"成功导入{factories.Count}条数据！");
        
        // using var fs = ExcelHelper.ExportDataTable(dataTable);

        using (var fs = ExcelHelper.ExportDataTableWithErrorMsg<RegisterFactoryCreateOrUpdateDto>(dataTable,
                   failureList))
        {
            return new FileStreamResult(fs, "application/vnd.ms-excel;charset=utf-8")
            {
                FileDownloadName = "错误列表.xlsx"
            };
        }
    }
}