using BYDPlatform.Application.BusinessDivision.Commands.CreateBusinessDivision;
using BYDPlatform.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.BusinessDivision.Validators;

public class CreateBusinessDivisionCommandValidator : AbstractValidator<CreateBusinessDivisionCommand>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;

    public CreateBusinessDivisionCommandValidator(IRepository<Domain.Entities.BusinessDivision> repository)
    {
        _repository = repository;

        RuleFor(v => v.BuName)
            .MaximumLength(50).WithMessage("事业部名称最大长度不超过50个字符,").WithSeverity(Severity.Warning)
            .NotEmpty().WithMessage("事业部名称不能为空").WithSeverity(Severity.Warning)
            .MustAsync(BeUniqueBuName).WithMessage("事业部已存在.");
    }

    public async Task<bool> BeUniqueBuName(string title, CancellationToken cancellationToken)
    {
        return await _repository.GetAsQueryable().AllAsync(l => !l.BuName.Equals(title), cancellationToken);
    }
}