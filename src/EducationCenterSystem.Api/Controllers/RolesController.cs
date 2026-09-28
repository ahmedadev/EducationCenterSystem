using EducationCenterSystem.Api.Authentication;
using EducationCenterSystem.Application.Roles.Create;
using EducationCenterSystem.Application.Roles.GetAll;
using EducationCenterSystem.Application.Roles.GetPermissions;
using EducationCenterSystem.Application.Users.AssignRole;
using EducationCenterSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[ApiController]
[Route("api/roles")]
public sealed class RolesController : ApiController
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permission.RolesManage)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var query = new GetAllQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [HasPermission(Permission.RolesManage)]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(new { RoleId = result.Value });
    }

    [HttpGet("permissions")]
    [HasPermission(Permission.RolesManage)]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var query = new GetAllPermissionsQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPost("assign")]
    [HasPermission(Permission.RolesManage)]
    public async Task<IActionResult> AssignRoleToUser(
        [FromBody] AssignRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok();
    }
}
