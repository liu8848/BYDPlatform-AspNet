namespace BYDPlatform.Domain.DTOs.BusinessDivision;

public class BusinessDivisionQueryDto:IBaseQueryDto
{
    public int Id { get; set; }
    public string? BuName { get; set; }
    
    
    public string? Fields { get; set; }
    public string? SortOrder { get; set; } = "title desc";
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}