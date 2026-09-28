using EducationCenterSystem.Application.Common.Models;
using EducationCenterSystem.Application.Teachers.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetPaged;

public sealed record GetPagedQuery(int PageNumber, int PageSize)
    : IRequest<ErrorOr<PagedResult<TeacherResponse>>>;
