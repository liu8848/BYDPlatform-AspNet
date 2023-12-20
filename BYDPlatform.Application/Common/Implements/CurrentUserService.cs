using System.Security.Claims;
using BYDPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BYDPlatform.Application.Common.Implements;

public class CurrentUserService:ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name).Value;
}