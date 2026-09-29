using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.AddStudent;

public sealed class AddStudentCommandHandler : IRequestHandler<AddStudentCommand, ErrorOr<Success>>
{
    private readonly IEducationalGroupRepository _educationalGroupRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddStudentCommandHandler(
        IEducationalGroupRepository educationalGroupRepository,
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _educationalGroupRepository = educationalGroupRepository;
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
    {
        var group = await _educationalGroupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            return Error.NotFound("EducationalGroup.NotFound", "The specified educational group was not found.");
        }

        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
        {
            return Error.NotFound("Student.NotFound", "The specified student was not found.");
        }

        bool isEnrolled = await _educationalGroupRepository.IsStudentEnrolledAsync(request.StudentId, request.GroupId, cancellationToken);
        if (isEnrolled)
        {
            return Error.Conflict("EducationalGroup.StudentAlreadyEnrolled", "The student is already enrolled in this group.");
        }

        var studentGroupResult = StudentGroup.Create(request.StudentId, request.GroupId);
        if (studentGroupResult.IsError)
        {
            return studentGroupResult.Errors;
        }

        _educationalGroupRepository.AddEnrollment(studentGroupResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
