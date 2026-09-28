using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.GetByGroupId;

public sealed class GetByGroupIdQueryHandler : IRequestHandler<GetByGroupIdQuery, ErrorOr<IReadOnlyList<GroupSessionResponse>>>
{
    private readonly IGroupSessionRepository _sessionRepository;

    public GetByGroupIdQueryHandler(IGroupSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<GroupSessionResponse>>> Handle(GetByGroupIdQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetByGroupIdAsync(request.GroupId, cancellationToken);

        return sessions.Select(s => new GroupSessionResponse(
            s.Id,
            s.EducationalGroupId,
            s.SessionDate,
            s.Notes,
            s.AttendanceRecords.Count)).ToList().AsReadOnly();
    }
}
