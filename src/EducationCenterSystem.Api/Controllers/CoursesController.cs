using EducationCenterSystem.Application.Courses.Create;
using EducationCenterSystem.Application.Courses.GetAll;
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            courseId => CreatedAtAction(nameof(GetAll), new { id = courseId }, courseId),
            Problem);
    }
}
