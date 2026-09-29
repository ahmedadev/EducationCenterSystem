using EducationCenterSystem.Application.Attendance.Create;
using EducationCenterSystem.Application.Attendance.Record;
using EducationCenterSystem.Application.Attendance.GetBySessionId;
using EducationCenterSystem.Application.Attendance.GetByGroupId;
using EducationCenterSystem.Application.Attendance.BatchRecord;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[Route("api/[controller]")]
public sealed class AttendanceController : ApiController
{
    private readonly IMediator _mediator;

    public AttendanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("sessions/{groupId}")]
    [Authorize]
    public async Task<IActionResult> GetSessionsByGroup(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetByGroupIdQuery(groupId), cancellationToken);
        return result.Match(
            sessions => Ok(sessions),
            Problem);
    }

    [HttpPost("sessions")]
    [Authorize]
    public async Task<IActionResult> CreateSession(CreateCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            sessionId => Ok(new { Id = sessionId }),
            Problem);
    }

    [HttpGet("records/{sessionId}")]
    [Authorize]
    public async Task<IActionResult> GetAttendanceRecords(Guid sessionId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBySessionIdQuery(sessionId), cancellationToken);
        return result.Match(
            records => Ok(records),
            Problem);
    }

    [HttpPost("records")]
    [Authorize]
    public async Task<IActionResult> RecordAttendance(RecordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            recordId => Ok(new { Id = recordId }),
            Problem);
    }

    [HttpPost("batch")]
    [Authorize]
    public async Task<IActionResult> BatchRecordAttendance(BatchRecordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            success => Ok(),
            Problem);
    }
}
