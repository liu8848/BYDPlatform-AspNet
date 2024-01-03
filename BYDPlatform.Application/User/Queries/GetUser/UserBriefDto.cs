using BYDPlatform.Application.Common.Mappings;

namespace BYDPlatform.Application.User.Queries.GetUser;

public class UserBriefDto : IMapFrom<Domain.Entities.User>
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }
}