using BYDPlatform.Domain.Attributes;
using BYDPlatform.Domain.DTOs.Base;

namespace BYDPlatform.Domain.DTOs.BusinessDivision;

public class BusinessDivisionCreateOrUpdateDto:IBaseCreateOrUpdateDto
{
    public int Id { get; set; }
    
    [Excel(EntityFieldName = "BuName",ExcelFieldName = "事业部名称")]
    public string? BuName { get; set; }
}