using BYDPlatform.Application.Common.Interfaces;
using MediatR;

namespace BYDPlatform.Application.User.Commands.CreateUser;

public class CreateUserCommand : IRequest<int>
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly IRepository<Domain.Entities.User> _repository;

    public CreateUserCommandHandler(IRepository<Domain.Entities.User> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        await _repository.AddAsync(entity, cancellationToken);
        return entity.Id;
    }
}