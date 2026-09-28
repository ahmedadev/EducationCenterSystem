using EducationCenterSystem.Application.Students.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetByName;

internal sealed class GetByNameQueryHandler : IRequestHandler<GetByNameQuery, ErrorOr<IReadOnlyList<StudentResponse>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetByNameQueryHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

    public async Task<ErrorOr<IReadOnlyList<StudentResponse>>> Handle(GetByNameQuery request, CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetByNameAsync(request.Name, cancellationToken);

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
