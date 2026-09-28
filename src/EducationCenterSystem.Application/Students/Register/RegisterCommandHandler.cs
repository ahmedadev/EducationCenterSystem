using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Guid>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsError)
            return emailResult.Errors;

        var phoneResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneResult.IsError)
            return phoneResult.Errors;

        var parentPhoneResult = PhoneNumber.Create(request.ParentPhoneNumber);
        if (parentPhoneResult.IsError)
            return parentPhoneResult.Errors;

        if (!await _studentRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
            return Error.Conflict("Student.Email", "The email is already in use.");

        var studentResult = Student.Register(
            request.FirstName,
            request.LastName,
            emailResult.Value,
            phoneResult.Value,
            request.DateOfBirth,
            request.NationalId,
            parentPhoneResult.Value,
            request.GradeLevel,
            request.StudentCode,
            request.SchoolName,
            request.Gender,
            request.Address,
            request.Notes);

        if (studentResult.IsError)
            return studentResult.Errors;

        _studentRepository.Add(studentResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return studentResult.Value.Id;
    }
}
