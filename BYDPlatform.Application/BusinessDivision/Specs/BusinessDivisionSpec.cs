using BYDPlatform.Application.Common.Implements;

namespace BYDPlatform.Application.BusinessDivision.Specs;

public class BusinessDivisionSpec : SpecificationBase<Domain.Entities.BusinessDivision>
{
    public BusinessDivisionSpec(Domain.Entities.BusinessDivision bu)
    {
        if (bu.Id != 0) AddCriteria(x => x.Id == bu.Id);

        if (!string.IsNullOrEmpty(bu.BuName)) AddCriteria(x => x.BuName.Equals(bu.BuName));
    }
}