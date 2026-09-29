using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Guid>>
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
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

        if (!await _teacherRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
            return Error.Conflict("Teacher.Email", "The email is already in use.");

        if (!string.IsNullOrWhiteSpace(request.NationalId))
        {
            var existingByNationalId = await _teacherRepository.GetByNationalIdAsync(request.NationalId, cancellationToken);
            if (existingByNationalId is not null)
                return Error.Conflict("Teacher.NationalId", "The national ID is already in use.");
        }

        var existingByCode = await _teacherRepository.GetByTeacherCodeAsync(request.TeacherCode, cancellationToken);
        if (existingByCode is not null)
            return Error.Conflict("Teacher.TeacherCode", "The teacher code is already in use.");

        var teacherResult = Teacher.Register(
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            emailResult.Value,
            phoneResult.Value,
            request.DateOfBirth,
            request.NationalId,
            request.TeacherCode,
            request.Subject,
            request.Qualification,
            request.Gender,
            request.Address,
            request.Notes);

        if (teacherResult.IsError)
            return teacherResult.Errors;

        _teacherRepository.Add(teacherResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return teacherResult.Value.Id;
    }
}
