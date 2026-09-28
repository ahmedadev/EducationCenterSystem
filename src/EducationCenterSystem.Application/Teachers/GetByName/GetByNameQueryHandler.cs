using EducationCenterSystem.Application.Teachers.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetByName;

internal sealed class GetByNameQueryHandler : IRequestHandler<GetByNameQuery, ErrorOr<IReadOnlyList<TeacherResponse>>>
{
    private readonly ITeacherRepository _teacherRepository;

    public GetByNameQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

    public async Task<ErrorOr<IReadOnlyList<TeacherResponse>>> Handle(GetByNameQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _teacherRepository.GetByNameAsync(request.Name, cancellationToken);

        var response = teachers.Select(teacher => new TeacherResponse(
            teacher.Id,
            teacher.FirstName,
            teacher.LastName,
            teacher.Email.Value,
            teacher.PhoneNumber.Value,
            teacher.DateOfBirth,
            teacher.RegisteredOnUtc,
            teacher.NationalId,
            teacher.TeacherCode,
            teacher.Subject,
            teacher.Qualification,
            teacher.Gender,
            teacher.Address,
            teacher.Status,
            teacher.Notes)).ToList();

        return response;
    }
}
