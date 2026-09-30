using EducationCenterSystem.Application.Courses.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.GetById;

internal sealed class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ErrorOr<CourseResponse>>
{
    private readonly ICourseRepository _courseRepository;

    public GetByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<ErrorOr<CourseResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
            return Error.NotFound("Course.NotFound", "The course was not found.");

        return new CourseResponse(
            course.Id,
            course.Name,
            course.GradeLevel,
            course.Subject,
            course.Description,
            course.IsActive);
    }
}
