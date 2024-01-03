using AutoMapper;
using BYDPlatform.Domain.DTOs.RegisterFactory;
using BYDPlatform.Domain.Entities;

namespace BYDPlatform.Application.Common.Mappings;

public class RegisterFactoryProfile : Profile
{
    public RegisterFactoryProfile()
    {
        CreateMap<RegisterFactoryCreateOrUpdateDto, RegisterFactory>()
            .ForMember(dest => dest.FactoryName,
                opt => opt.MapFrom(src => src.FactoryName))
            .ForMember(dest => dest.BuId,
                opt => opt.MapFrom(src => src.BuId))
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom(src => src.Level));
    }
}