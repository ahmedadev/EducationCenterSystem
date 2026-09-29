using EducationCenterSystem.Application.Teachers.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetByNationalId;

internal sealed class GetByNationalIdQueryHandler : IRequestHandler<GetByNationalIdQuery, ErrorOr<TeacherResponse>>
{
    private readonly ITeacherRepository _teacherRepository;

    public GetByNationalIdQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

    public async Task<ErrorOr<TeacherResponse>> Handle(GetByNationalIdQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByNationalIdAsync(request.NationalId, cancellationToken);
        if (teacher is null)
        {
            return Error.NotFound("Teacher.NotFound", "The teacher with the specified national ID was not found.");
        }

        return new TeacherResponse(
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
            teacher.Notes);
    }
}
