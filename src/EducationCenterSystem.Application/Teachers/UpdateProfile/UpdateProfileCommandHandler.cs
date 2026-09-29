using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.UpdateProfile;

public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ErrorOr<Success>>
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Error.NotFound("Teacher.NotFound", "The teacher was not found.");
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

        if (teacher.Email.Value != emailResult.Value.Value &&
            !await _teacherRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
        {
            return Error.Conflict("Teacher.Email", "The email is already in use.");
        }

        if (!string.IsNullOrWhiteSpace(request.NationalId) && request.NationalId != teacher.NationalId)
        {
            var existingByNationalId = await _teacherRepository.GetByNationalIdAsync(request.NationalId, cancellationToken);
            if (existingByNationalId is not null && existingByNationalId.Id != teacher.Id)
            {
                return Error.Conflict("Teacher.NationalId", "The national ID is already in use.");
            }
        }

        if (request.TeacherCode != teacher.TeacherCode)
        {
            var existingByCode = await _teacherRepository.GetByTeacherCodeAsync(request.TeacherCode, cancellationToken);
            if (existingByCode is not null && existingByCode.Id != teacher.Id)
            {
                return Error.Conflict("Teacher.TeacherCode", "The teacher code is already in use.");
            }
        }

        var updateResult = teacher.UpdateProfile(
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            emailResult.Value,
            phoneNumberResult.Value,
            request.DateOfBirth,
            request.NationalId,
            request.TeacherCode,
            request.Subject,
            request.Qualification,
            request.Gender,
            request.Address,
            request.Notes);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        _teacherRepository.Update(teacher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
