using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Application.Common.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BYDPlatform.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser? _applicationUser;

    public IdentityService(ILogger<IdentityService> logger, IConfiguration configuration,
        UserManager<ApplicationUser> userManager)
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

        var result = _applicationUser != null &&
                     await _userManager.CheckPasswordAsync(_applicationUser, userForAuthentication.Password!);
        if (!result) _logger.LogWarning($"{nameof(ValidateUserAsync)}:认证失败，账号或密码错误");

        return result;
    }

    public async Task<ApplicationToken> CreateTokenAsync(bool populateExpiry)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        var refreshToken = GeneratetRefreshToken();
        _applicationUser!.RefreshToken = refreshToken;
        if (populateExpiry)
            _applicationUser!.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(_applicationUser);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new ApplicationToken(accessToken, refreshToken);
    }

    public async Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token)
    {
        var principal = GetPrincipalFromExpiredToken(token.AccessToken);
        var user = await _userManager.FindByNameAsync(principal.Identity?.Name);
        if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new BadHttpRequestException("provided token has some invalid value");

        _applicationUser = user;
        return await CreateTokenAsync(true);
    }

    private string GeneratetRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
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
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["JwtSettings"] ?? "BYDPlatformApiSecretKey")),
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}