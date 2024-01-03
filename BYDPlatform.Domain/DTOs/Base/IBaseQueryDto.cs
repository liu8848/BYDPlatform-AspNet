namespace BYDPlatform.Domain.DTOs;

public interface IBaseQueryDto
{
    public string? Fields { get; set; }
    public string? SortOrder { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}