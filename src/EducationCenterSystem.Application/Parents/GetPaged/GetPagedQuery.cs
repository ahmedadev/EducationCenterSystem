using EducationCenterSystem.Application.Common.Models;
using EducationCenterSystem.Application.Parents.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.GetPaged;

public sealed record GetPagedQuery(
    string? SearchTerm,
    int Page,
    int PageSize) : IRequest<ErrorOr<PagedResult<ParentResponse>>>;
