using BYDPlatform.Application.Common.Exceptions;
using BYDPlatform.Application.Common.Interfaces;
using MediatR;

namespace BYDPlatform.Application.BusinessDivision.Commands.UpdateBusinessDivision;

public class UpdateBusinessDivisionCommand:IRequest<Domain.Entities.BusinessDivision>
{
    public int Id { get; set; }
    public string? BuName { get; set; }
}

public class UpdateBusinessDivisionCommandHandler : IRequestHandler<UpdateBusinessDivisionCommand, Domain.Entities.BusinessDivision>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;

    public UpdateBusinessDivisionCommandHandler(IRepository<Domain.Entities.BusinessDivision> repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.BusinessDivision> Handle(UpdateBusinessDivisionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id);
        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.BusinessDivision), request.Id);
        }

        entity.BuName = request.BuName ?? entity.BuName;

        await _repository.UpdateAsync(entity, cancellationToken);
        return entity;
    }
}