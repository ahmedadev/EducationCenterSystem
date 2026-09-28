using EducationCenterSystem.Application.EducationalGroups.Create;
using EducationCenterSystem.Application.EducationalGroups.GetAll;
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
}
