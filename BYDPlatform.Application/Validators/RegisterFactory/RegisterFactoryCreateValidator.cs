using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.DTOs.RegisterFactory;
using BYDPlatform.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.Validators.RegisterFactory;

public class RegisterFactoryCreateValidator:AbstractValidator<RegisterFactoryCreateOrUpdateDto>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _businessDivisionRepository;
    private readonly IRepository<Domain.Entities.RegisterFactory> _factoryRepository;

    public RegisterFactoryCreateValidator(IRepository<Domain.Entities.BusinessDivision> businessDivisionRepository, IRepository<Domain.Entities.RegisterFactory> factoryRepository)
    {
        _businessDivisionRepository = businessDivisionRepository;
        _factoryRepository = factoryRepository;

        RuleFor(f => f.BuId)
            .MustAsync(BuIdExist).WithMessage($"事业部不存在").WithSeverity(Severity.Warning);

        RuleFor(f => f.FactoryName)
            .MaximumLength(50).WithMessage("工厂名最大长度为50个字符").WithSeverity(Severity.Warning)
            .NotEmpty().WithMessage("工厂名称不能为空").WithSeverity(Severity.Warning);

        RuleFor(f => f).MustAsync(BeUniqueFactoryName).WithMessage("工厂名称已存在").WithSeverity(Severity.Warning);
            
        RuleFor(f => f.Level)
            .Must(t=>Enum.IsDefined(typeof(FactoryLevel),t)).WithMessage("工厂等级输入错误").WithSeverity(Severity.Warning);

    }

    public async Task<bool> BeUniqueFactoryName(RegisterFactoryCreateOrUpdateDto factory,CancellationToken cancellationToken)
    {
        return !await _factoryRepository.GetAsQueryable()
            .AnyAsync(
                f =>
                    f.BuId == factory.BuId && f.FactoryName.Equals(factory.FactoryName), cancellationToken);
    }

    public async Task<bool> BuIdExist(int buId,CancellationToken cancellationToken)
    {
        return await _businessDivisionRepository.GetAsQueryable().AnyAsync(bu => bu.Id == buId,cancellationToken);
    }
    
    
    
}