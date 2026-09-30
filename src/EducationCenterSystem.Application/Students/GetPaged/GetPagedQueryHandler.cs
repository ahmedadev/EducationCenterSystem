using EducationCenterSystem.Application.Common.Models;
using EducationCenterSystem.Application.Students.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetPaged;

public sealed class GetPagedQueryHandler
    : IRequestHandler<GetPagedQuery, ErrorOr<PagedResult<StudentResponse>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetPagedQueryHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

    public async Task<ErrorOr<PagedResult<StudentResponse>>> Handle(
        GetPagedQuery request,
        CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = request.PageSize <= 0 ? 15 : request.PageSize;

        var (students, totalCount) = await _studentRepository.GetPagedAsync(
            pageNumber, 
            pageSize, 
            request.SearchTerm, 
            request.SortColumn, 
            request.SortDirection, 
            cancellationToken);

        var responseList = students.Select(student => new StudentResponse(
            student.Id,
            student.FirstName,
            student.SecondName,
            student.ThirdName,
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

        int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
        if (totalPages < 1) totalPages = 1;

        return new PagedResult<StudentResponse>(
            responseList,
            pageNumber,
            pageSize,
            totalCount,
            totalPages);
    }
}
