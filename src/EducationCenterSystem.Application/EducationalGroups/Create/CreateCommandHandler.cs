using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.Create;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, ErrorOr<Guid>>
{
    private readonly IEducationalGroupRepository _groupRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandHandler(
        IEducationalGroupRepository groupRepository,
        ICourseRepository courseRepository,
        ITeacherRepository teacherRepository,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _courseRepository = courseRepository;
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "The specified course was not found.");
        }

        var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Error.NotFound("Teacher.NotFound", "The specified teacher was not found.");
        }

        if (await _groupRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return Error.Conflict("EducationalGroup.Exists", "A group with the same name already exists.");
        }

        var groupResult = EducationalGroup.Create(
            request.CourseId,
            request.TeacherId,
            request.Name,
            request.MaxCapacity,
            request.MonthlyFee,
            request.ScheduleDescription);

        if (groupResult.IsError)
        {
            return groupResult.Errors;
        }

        var group = groupResult.Value;
        _groupRepository.Add(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
