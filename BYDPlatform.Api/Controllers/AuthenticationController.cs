using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Model;
using Microsoft.AspNetCore.Mvc;

namespace BYDPlatform.Api.Controllers;

[ApiController]
public class AuthenticationController:ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IIdentityService identityService, ILogger<AuthenticationController> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthentication userForAuthentication)
    {
        if (!await _identityService.ValidateUserAsync(userForAuthentication))
        {
            return Unauthorized();
        }

        return Ok(new { Token = await _identityService.CreateTokenAsync() });
    }
}