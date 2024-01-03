using BYDPlatform.Application.Common.Implements;

namespace BYDPlatform.Application.Specs;

public sealed class UserSpec : SpecificationBase<Domain.Entities.User>
{
    public UserSpec(int id, string userName) : base(u => u.Id == id && u.UserName.Equals(userName))
    {
    }

    public UserSpec(Domain.Entities.User user)
    {
        if (user.Id != 0) AddCriteria(u => u.Id == user.Id);

        if (!string.IsNullOrEmpty(user.UserName)) AddCriteria(u => u.UserName.Equals(user.UserName));
    }
}