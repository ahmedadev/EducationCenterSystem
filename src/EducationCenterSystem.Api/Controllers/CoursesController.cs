using EducationCenterSystem.Application.Courses.Create;
using EducationCenterSystem.Application.Courses.Delete;
using EducationCenterSystem.Application.Courses.GetAll;
using EducationCenterSystem.Application.Courses.GetById;
using EducationCenterSystem.Application.Courses.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[Route("api/[controller]")]
public sealed class CoursesController : ApiController
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllQuery(), cancellationToken);
        return result.Match(
            courses => Ok(courses),
            Problem);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetByIdQuery(id), cancellationToken);
        return result.Match(
            course => Ok(course),
            Problem);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            courseId => CreatedAtAction(nameof(GetById), new { id = courseId }, courseId),
            Problem);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, UpdateCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch.");

        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCommand(id), cancellationToken);
        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
