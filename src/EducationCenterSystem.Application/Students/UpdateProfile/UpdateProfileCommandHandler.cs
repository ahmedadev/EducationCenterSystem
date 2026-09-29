using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.UpdateProfile;

public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ErrorOr<Success>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

        if (student is null)
        {
            return Error.NotFound("Student.NotFound", "The student was not found.");
        }

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumberResult.IsError)
        {
            return phoneNumberResult.Errors;
        }

        var parentPhoneNumberResult = PhoneNumber.Create(request.ParentPhoneNumber);
        if (parentPhoneNumberResult.IsError)
        {
            return parentPhoneNumberResult.Errors;
        }

        var updateResult = student.UpdateProfile(
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            emailResult.Value,
            phoneNumberResult.Value,
            request.DateOfBirth,
            request.NationalId,
            parentPhoneNumberResult.Value,
            request.GradeLevel,
            request.StudentCode,
            request.SchoolName,
            request.Gender,
            request.Address,
            request.Notes);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        _studentRepository.Update(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
