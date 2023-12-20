using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BYDPlatform.Infrastructure.Identity;

public class IdentityService:IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser? _applicationUser;

    public IdentityService(ILogger<IdentityService> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName
        };
        await _userManager.CreateAsync(user, password);
        return user.Id;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthentication userForAuthentication)
    {
        _applicationUser = await _userManager.FindByNameAsync(userForAuthentication.UserName!);

        var result = _applicationUser != null && await _userManager.CheckPasswordAsync(_applicationUser, userForAuthentication.Password!);
        if (!result)
        {
            _logger.LogWarning($"{nameof(ValidateUserAsync)}:认证失败，账号或密码错误");
        }

        return result;
    }

    public async Task<string> CreateTokenAsync()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings")["SecretKey"] ??
                                         "BYDPlatformApiSecretKey");
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _applicationUser!.UserName)
        };
        var roles = await _userManager.GetRolesAsync(_applicationUser);
        claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken(
            jwtSettings["validIssuer"],
            jwtSettings["validAudience"],
            claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }
}