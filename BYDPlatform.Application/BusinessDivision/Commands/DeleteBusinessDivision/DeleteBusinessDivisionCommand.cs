using BYDPlatform.Application.Common.Exceptions;
using BYDPlatform.Application.Common.Interfaces;
using MediatR;

namespace BYDPlatform.Application.BusinessDivision.Commands.DeleteBusinessDivision;

public class DeleteBusinessDivisionCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteBusinessDivisionCommandHandler : IRequestHandler<DeleteBusinessDivisionCommand>
{
    private readonly IRepository<Domain.Entities.BusinessDivision> _repository;

    public DeleteBusinessDivisionCommandHandler(IRepository<Domain.Entities.BusinessDivision> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteBusinessDivisionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id);
        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.BusinessDivision), request.Id);

        await _repository.DeleteAsync(entity, cancellationToken);
        return Unit.Value;
    }
}