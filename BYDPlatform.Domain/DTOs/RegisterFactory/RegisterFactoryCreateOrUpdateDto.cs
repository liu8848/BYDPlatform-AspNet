using BYDPlatform.Domain.DTOs.Base;
using BYDPlatform.Domain.Enums;

namespace BYDPlatform.Domain.DTOs.RegisterFactory;

public class RegisterFactoryCreateOrUpdateDto:IBaseCreateOrUpdateDto
{
    public int Id { get; set; }

    public int  BuId { get; set; }

    public string FactoryName { get; set; }

    public FactoryLevel Level { get; set; }
}