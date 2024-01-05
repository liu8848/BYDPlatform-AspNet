using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.DTOs.RegisterFactory;
using BYDPlatform.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.Validators.RegisterFactory;

public class RegisterFactoryCreateValidator : AbstractValidator<RegisterFactoryCreateOrUpdateDto>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _businessDivisionRepository;
    private readonly IRepository<Domain.Entities.RegisterFactory> _factoryRepository;

    public RegisterFactoryCreateValidator(IRepository<Domain.Entities.BusinessDivision> businessDivisionRepository,
        IRepository<Domain.Entities.RegisterFactory> factoryRepository)
    {
        _businessDivisionRepository = businessDivisionRepository;
        _factoryRepository = factoryRepository;

        RuleFor(f => f.BuId)
            .MustAsync(BuIdExist).WithMessage("事业部不存在").WithSeverity(Severity.Warning);

        RuleFor(f => f.FactoryName)
            .MustAsync(BeUniqueFactoryName).WithMessage("工厂名称有重复，若与其他事业部工厂重名，请在工厂名称后添加\"(所属事业部名称)\"").WithSeverity(Severity.Warning)
            .MaximumLength(50).WithMessage("工厂名最大长度为50个字符").WithSeverity(Severity.Warning)
            .NotEmpty().WithMessage("工厂名称不能为空").WithSeverity(Severity.Warning);
        
        RuleFor(f => f.Level)
            .Must(t => Enum.IsDefined(typeof(FactoryLevel), t)).WithMessage("工厂等级输入错误").WithSeverity(Severity.Warning);
    }

    public async Task<bool> BeUniqueFactoryName(string factoryName,
        CancellationToken cancellationToken)
    {
        return !await _factoryRepository.GetAsQueryable()
            .AnyAsync(
                f => f.FactoryName.Equals(factoryName), cancellationToken);
    }

    public async Task<bool> BuIdExist(int buId, CancellationToken cancellationToken)
    {
        return await _businessDivisionRepository.GetAsQueryable().AnyAsync(bu => bu.Id == buId, cancellationToken);
    }
}