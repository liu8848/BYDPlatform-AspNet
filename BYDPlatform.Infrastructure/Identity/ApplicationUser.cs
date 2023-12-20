using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Infrastructure.Identity;

public class ApplicationUser:IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}