using EducationCenterSystem.Api.Authentication;
using EducationCenterSystem.Application.Students.Register;
using EducationCenterSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[ApiController]
[Route("api/students")]
public sealed class StudentsController : ApiController
{
    private readonly ISender _sender;

    public StudentsController(ISender sender) => _sender = sender;

    [HttpPost]
    [HasPermission(Permission.StudentsCreate)]
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
    [HasPermission(Permission.StudentsRead)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var query = new Application.Students.GetPaged.GetPagedQuery(page, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("all")]
    [HasPermission(Permission.StudentsRead)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new Application.Students.GetAll.GetAllQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("by-name/{name}")]
    [HasPermission(Permission.StudentsRead)]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var query = new Application.Students.GetByName.GetByNameQuery(name);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("by-national-id/{nationalId}")]
    [HasPermission(Permission.StudentsRead)]
    public async Task<IActionResult> GetByNationalId(string nationalId, CancellationToken cancellationToken)
    {
        var query = new Application.Students.GetByNationalId.GetByNationalIdQuery(nationalId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.StudentsUpdate)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] Contracts.UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new Application.Students.UpdateProfile.UpdateProfileCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.DateOfBirth,
            request.NationalId,
            request.ParentPhoneNumber,
            request.GradeLevel,
            request.StudentCode,
            request.SchoolName,
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
    [HasPermission(Permission.StudentsDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new Application.Students.Delete.DeleteCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return NoContent();
    }
}
