using EducationCenterSystem.Application.Students.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetByNationalId;

internal sealed class GetByNationalIdQueryHandler : IRequestHandler<GetByNationalIdQuery, ErrorOr<StudentResponse>>
{
    private readonly IStudentRepository _studentRepository;

    public GetByNationalIdQueryHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

    public async Task<ErrorOr<StudentResponse>> Handle(GetByNationalIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByNationalIdAsync(request.NationalId, cancellationToken);

        if (student is null)
            return Error.NotFound("Student.NotFound", "The student with the specified national ID was not found.");

        return new StudentResponse(
            student.Id,
            student.FirstName,
            student.LastName,
            student.Email.Value,
            student.PhoneNumber.Value,
            student.DateOfBirth,
            student.RegisteredOnUtc,
            student.NationalId,
            student.ParentPhoneNumber.Value,
            student.GradeLevel,
            student.StudentCode,
            student.SchoolName,
            student.Gender,
            student.Address,
            student.Status,
            student.Notes);
    }
}
