using BydPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace BYDPlatform.Api.Controllers;
[ApiController]
[Route("/hello")]
public class HelloController : Controller
{
    public required BydPlatformDbContext _context { protected get; init; }

    public HelloController(BydPlatformDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public string Index()
    {
        return "Hello";
    }
}
