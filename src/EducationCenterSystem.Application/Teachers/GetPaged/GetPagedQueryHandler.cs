using EducationCenterSystem.Application.Common.Models;
using EducationCenterSystem.Application.Teachers.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetPaged;

public sealed class GetPagedQueryHandler
    : IRequestHandler<GetPagedQuery, ErrorOr<PagedResult<TeacherResponse>>>
{
    private readonly ITeacherRepository _teacherRepository;

    public GetPagedQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

    public async Task<ErrorOr<PagedResult<TeacherResponse>>> Handle(
        GetPagedQuery request,
        CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = request.PageSize <= 0 ? 15 : request.PageSize;

        var (teachers, totalCount) = await _teacherRepository.GetPagedAsync(
            pageNumber, 
            pageSize, 
            request.SortColumn, 
            request.SortDirection, 
            cancellationToken);

        var responseList = teachers.Select(teacher => new TeacherResponse(
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

        int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
        if (totalPages < 1) totalPages = 1;

        return new PagedResult<TeacherResponse>(
            responseList,
            pageNumber,
            pageSize,
            totalCount,
            totalPages);
    }
}
