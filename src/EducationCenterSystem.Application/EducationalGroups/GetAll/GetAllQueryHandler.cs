using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.GetAll;

public sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyList<EducationalGroupResponse>>>
{
    private readonly IEducationalGroupRepository _groupRepository;

    public GetAllQueryHandler(IEducationalGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<EducationalGroupResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetAllAsync(cancellationToken);

        return groups.Select(g => new EducationalGroupResponse(
            g.Id,
            g.CourseId,
            g.Course?.Name ?? "N/A",
            g.TeacherId,
            g.Teacher != null ? $"{g.Teacher.FirstName} {g.Teacher.LastName}" : "N/A",
            g.Name,
            g.MaxCapacity,
            g.MonthlyFee,
            g.ScheduleDescription,
            g.Status)).ToList().AsReadOnly();
    }
}
