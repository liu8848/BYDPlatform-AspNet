using BYDPlatform.Domain.Enums;

namespace BYDPlatform.Domain.DTOs.RegisterFactory;

public class RegisterFactoryQueryDto:IBaseQueryDto
{

    public int BuId { get; set; }
    public string? FactoryName { get; set; }
    public FactoryLevel? Level { get; set; }
    
    public string? Fields { get; set; }
    public string? SortOrder { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}