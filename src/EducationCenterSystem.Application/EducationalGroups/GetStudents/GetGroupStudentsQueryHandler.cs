using EducationCenterSystem.Application.Students.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.GetStudents;

public sealed class GetGroupStudentsQueryHandler : IRequestHandler<GetGroupStudentsQuery, ErrorOr<IEnumerable<StudentResponse>>>
{
    private readonly IEducationalGroupRepository _educationalGroupRepository;

    public GetGroupStudentsQueryHandler(IEducationalGroupRepository educationalGroupRepository)
    {
        _educationalGroupRepository = educationalGroupRepository;
    }

    public async Task<ErrorOr<IEnumerable<StudentResponse>>> Handle(GetGroupStudentsQuery request, CancellationToken cancellationToken)
    {
        var group = await _educationalGroupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        
        if (group is null)
        {
            return Error.NotFound("EducationalGroup.NotFound", "The specified educational group was not found.");
        }

        var students = await _educationalGroupRepository.GetStudentsByGroupIdAsync(request.GroupId, cancellationToken);

        var responses = students.Select(s => new StudentResponse(
            s.Id,
            s.FirstName,
            s.LastName,
            s.Email.Value,
            s.PhoneNumber.Value,
            s.DateOfBirth,
            s.RegisteredOnUtc,
            s.NationalId,
            s.ParentPhoneNumber.Value,
            s.GradeLevel,
            s.StudentCode,
            s.SchoolName,
            s.Gender,
            s.Address,
            s.Status,
            s.Notes
        ));

        return ErrorOrFactory.From(responses);
    }
}
