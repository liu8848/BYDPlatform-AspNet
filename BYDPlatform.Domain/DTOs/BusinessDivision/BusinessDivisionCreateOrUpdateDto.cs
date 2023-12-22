using BYDPlatform.Domain.DTOs.Base;

namespace BYDPlatform.Domain.DTOs.BusinessDivision;

public class BusinessDivisionCreateOrUpdateDto:IBaseCreateOrUpdateDto
{
    public int Id { get; set; }
    public string? BuName { get; set; }
}