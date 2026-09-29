using EducationCenterSystem.Application.EducationalGroups.Create;
using EducationCenterSystem.Application.EducationalGroups.GetAll;
using EducationCenterSystem.Application.EducationalGroups.GetStudents;
using EducationCenterSystem.Application.EducationalGroups.AddStudent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[Route("api/[controller]")]
public sealed class EducationalGroupsController : ApiController
{
    private readonly IMediator _mediator;

    public EducationalGroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllQuery(), cancellationToken);
        return result.Match(
            groups => Ok(groups),
            Problem);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            groupId => CreatedAtAction(nameof(GetAll), new { id = groupId }, groupId),
            Problem);
    }

    [HttpGet("{id}/students")]
    [Authorize]
    public async Task<IActionResult> GetStudents(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGroupStudentsQuery(id), cancellationToken);
        return result.Match(
            students => Ok(students),
            Problem);
    }

    [HttpPost("{id}/students")]
    [Authorize]
    public async Task<IActionResult> AddStudent(Guid id, [FromBody] AddStudentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddStudentCommand(id, request.StudentId), cancellationToken);
        return result.Match(
            success => Ok(),
            Problem);
    }
}

public sealed record AddStudentRequest(Guid StudentId);
