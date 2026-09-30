using EducationCenterSystem.Application.Users.GetAll;
using EducationCenterSystem.Application.Users.AssignRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[Route("api/[controller]")]
public sealed class UsersController : ApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllQuery(), cancellationToken);
        return result.Match(
            users => Ok(users),
            Problem);
    }

    [HttpPost("assign-role")]
    [Authorize] // Ideally [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            success => Ok(),
            Problem);
    }
}
