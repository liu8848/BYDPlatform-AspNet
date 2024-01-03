using AutoMapper;
using AutoMapper.QueryableExtensions;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Application.User.Queries.GetUser;

public class GetUserQuery : IRequest<UserBriefDto>
{
    public int Id { get; set; } = 0;
    public string? UserName { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserBriefDto>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Domain.Entities.User> _repository;

    public GetUserQueryHandler(IRepository<Domain.Entities.User> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserBriefDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = new Domain.Entities.User
        {
            Id = request.Id,
            UserName = request.UserName
        };
        var spec = new UserSpec(user);

        return await _repository
            .GetAsQueryable(spec)
            .AsNoTracking()
            .ProjectTo<UserBriefDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}