using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.GetByGroupId;

public sealed record GetByGroupIdQuery(Guid GroupId) : IRequest<ErrorOr<IReadOnlyList<GroupSessionResponse>>>;
