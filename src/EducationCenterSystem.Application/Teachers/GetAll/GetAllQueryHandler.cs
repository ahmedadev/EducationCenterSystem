using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetAll;

internal sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyList<TeacherResponse>>>
{
    private readonly ITeacherRepository _teacherRepository;

    public GetAllQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

    public async Task<ErrorOr<IReadOnlyList<TeacherResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _teacherRepository.GetAllAsync(cancellationToken);

        var response = teachers.Select(teacher => new TeacherResponse(
            teacher.Id,
            teacher.FirstName,
            teacher.SecondName,
            teacher.ThirdName,
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
