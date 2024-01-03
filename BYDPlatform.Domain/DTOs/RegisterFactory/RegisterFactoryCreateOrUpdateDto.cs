using BYDPlatform.Domain.Attributes;
using BYDPlatform.Domain.DTOs.Base;
using BYDPlatform.Domain.Enums;

namespace BYDPlatform.Domain.DTOs.RegisterFactory;

public class RegisterFactoryCreateOrUpdateDto : IBaseCreateOrUpdateDto
{
    public int Id { get; set; }

    [Excel(ExcelFieldName = "所属事业部编号", EntityFieldName = "BuId", ColumnIndex = 1)]
    public int BuId { get; set; }

    [Excel(ExcelFieldName = "工厂名称", EntityFieldName = "FactoryName", ColumnIndex = 2)]
    public string FactoryName { get; set; }

    [Excel(ExcelFieldName = "工厂等级", EntityFieldName = "FactoryLevel", ColumnIndex = 3)]
    public FactoryLevel Level { get; set; }
}