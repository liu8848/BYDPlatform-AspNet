using BYDPlatform.Application.Common.Model;

namespace BYDPlatform.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> CreateUserAsync(string userName, string password);
    Task<bool> ValidateUserAsync(UserForAuthentication userForAuthentication);
    Task<string> CreateTokenAsync();
}