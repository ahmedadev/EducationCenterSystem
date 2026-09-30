using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Update;

internal sealed class UpdateCommandHandler : IRequestHandler<UpdateCommand, ErrorOr<Success>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
            return Error.NotFound("Course.NotFound", "The course was not found.");

        var updateResult = course.Update(request.Name, request.GradeLevel, request.Subject, request.Description);
        if (updateResult.IsError)
            return updateResult.Errors;

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
