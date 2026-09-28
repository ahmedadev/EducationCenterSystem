using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Create;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, ErrorOr<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        if (await _courseRepository.ExistsByNameAsync(request.Name, request.GradeLevel, request.Subject, cancellationToken))
        {
            return Error.Conflict("Course.Exists", "A course with the same name, grade level, and subject already exists.");
        }

        var courseResult = Course.Create(
            request.Name,
            request.GradeLevel,
            request.Subject,
            request.Description);

        if (courseResult.IsError)
        {
            return courseResult.Errors;
        }

        var course = courseResult.Value;
        _courseRepository.Add(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return course.Id;
    }
}
