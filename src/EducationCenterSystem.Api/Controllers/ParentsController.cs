using EducationCenterSystem.Application.Parents.GetAll;
using EducationCenterSystem.Application.Parents.GetPaged;
using EducationCenterSystem.Application.Parents.Register;
using EducationCenterSystem.Application.Parents.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ParentsController : ControllerBase
{
    private readonly ISender _sender;

    public ParentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Id = result.Value });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id mismatch.");
        }

        var result = await _sender.Send(command);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetAllQuery());

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortColumn = null, [FromQuery] string? sortDirection = null)
    {
        var result = await _sender.Send(new GetPagedQuery(searchTerm, page, pageSize, sortColumn, sortDirection));

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}
