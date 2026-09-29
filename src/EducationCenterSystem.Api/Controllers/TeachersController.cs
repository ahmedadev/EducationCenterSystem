using EducationCenterSystem.Api.Authentication;
using EducationCenterSystem.Api.Contracts;
using EducationCenterSystem.Application.Teachers.Delete;
using EducationCenterSystem.Application.Teachers.GetAll;
using EducationCenterSystem.Application.Teachers.GetByName;
using EducationCenterSystem.Application.Teachers.GetByNationalId;
using EducationCenterSystem.Application.Teachers.GetPaged;
using EducationCenterSystem.Application.Teachers.Register;
using EducationCenterSystem.Application.Teachers.UpdateProfile;
using EducationCenterSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[ApiController]
[Route("api/teachers")]
public sealed class TeachersController : ApiController
{
    private readonly ISender _sender;

    public TeachersController(ISender sender) => _sender = sender;

    [HttpPost]
    [HasPermission(Permission.TeachersCreate)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [HasPermission(Permission.TeachersRead)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPagedQuery(page, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("all")]
    [HasPermission(Permission.TeachersRead)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("by-name/{name}")]
    [HasPermission(Permission.TeachersRead)]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var query = new GetByNameQuery(name);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("by-national-id/{nationalId}")]
    [HasPermission(Permission.TeachersRead)]
    public async Task<IActionResult> GetByNationalId(string nationalId, CancellationToken cancellationToken)
    {
        var query = new GetByNationalIdQuery(nationalId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.TeachersUpdate)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTeacherRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand(
            id,
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.DateOfBirth,
            request.NationalId,
            request.TeacherCode,
            request.Subject,
            request.Qualification,
            request.Gender,
            request.Address,
            request.Notes);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.TeachersDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }
}
