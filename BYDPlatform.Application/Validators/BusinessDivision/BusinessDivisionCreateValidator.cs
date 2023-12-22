using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.DTOs.BusinessDivision;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.Validators.BusinessDivision;

public class BusinessDivisionCreateValidator:AbstractValidator<BusinessDivisionCreateOrUpdateDto>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;

    public BusinessDivisionCreateValidator(IRepository<Domain.Entities.BusinessDivision> repository)
    {
        _repository = repository;
        
        RuleFor(bu=>bu.BuName)
            .MaximumLength(50).WithMessage("事业部名称最大长度不超过50个字符,").WithSeverity(Severity.Warning)
            .NotEmpty().WithMessage("事业部名称不能为空").WithSeverity(Severity.Warning)
            .MustAsync(BeUniqueBuName).WithMessage("事业部已存在.");
    }
    
    public async Task<bool> BeUniqueBuName(string title, CancellationToken cancellationToken)
    {
        return await _repository.GetAsQueryable().AllAsync(l => !l.BuName.Equals(title), cancellationToken);
    }
}