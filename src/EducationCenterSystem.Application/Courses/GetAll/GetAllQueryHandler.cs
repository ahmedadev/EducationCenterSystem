using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.GetAll;

public sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyList<CourseResponse>>>
{
    private readonly ICourseRepository _courseRepository;

    public GetAllQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<CourseResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllAsync(cancellationToken);

        return courses.Select(c => new CourseResponse(
            c.Id,
            c.Name,
            c.GradeLevel,
            c.Subject,
            c.Description,
            c.IsActive)).ToList().AsReadOnly();
    }
}
