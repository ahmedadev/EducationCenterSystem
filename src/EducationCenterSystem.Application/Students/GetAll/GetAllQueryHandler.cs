using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetAll;

internal sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyList<StudentResponse>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetAllQueryHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

    public async Task<ErrorOr<IReadOnlyList<StudentResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetAllAsync(cancellationToken);

        var response = students.Select(student => new StudentResponse(
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
            student.Notes)).ToList();

        return response;
    }
}
