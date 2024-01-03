using BYDPlatform.Application.Common.Interfaces;
using MediatR;

namespace BYDPlatform.Application.BusinessDivision.Commands.CreateBusinessDivision;

public class CreateBusinessDivisionCommand : IRequest<Domain.Entities.BusinessDivision>
{
    public string BuName { get; set; }
}

public class
    CreateBusinessDivisionHandler : IRequestHandler<CreateBusinessDivisionCommand, Domain.Entities.BusinessDivision>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;

    public CreateBusinessDivisionHandler(IRepository<Domain.Entities.BusinessDivision> repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.BusinessDivision> Handle(CreateBusinessDivisionCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.BusinessDivision
        {
            BuName = request.BuName
        };

        await _repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}