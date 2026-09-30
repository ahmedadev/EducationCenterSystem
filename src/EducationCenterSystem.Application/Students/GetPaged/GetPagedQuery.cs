using EducationCenterSystem.Application.Common.Models;
using EducationCenterSystem.Application.Students.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetPaged;

public sealed record GetPagedQuery(int PageNumber, int PageSize, string? SearchTerm = null, string? SortColumn = null, string? SortDirection = null)
    : IRequest<ErrorOr<PagedResult<StudentResponse>>>;
