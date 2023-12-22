using AutoMapper;
using BYDPlatform.Domain.DTOs.BusinessDivision;

namespace BYDPlatform.Application.Common.Mappings;

public class BusinessDivisionProfile:Profile
{
    public BusinessDivisionProfile()
    {
        CreateMap<BusinessDivisionCreateOrUpdateDto, Domain.Entities.BusinessDivision>()
            .ForMember(dest => dest.BuName, 
                opt 
                    => opt.MapFrom(src => src.BuName));
    }
}