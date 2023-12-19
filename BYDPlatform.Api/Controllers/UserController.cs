using BYDPlatform.Application.User.Commands.CreateUser;
using BYDPlatform.Application.User.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BYDPlatform.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController:ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<int> Create([FromBody] CreateUserCommand command)
    {
        var createdUser = await _mediator.Send(command);
        return createdUser;
    }

    [HttpGet]
    public async Task<ActionResult<UserBriefDto>> Get([FromQuery] GetUserQuery query)
    {
        return await _mediator.Send(query);
    }
}