using EducationCenterSystem.Application.Common.Models;
using EducationCenterSystem.Application.Parents.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.GetPaged;

internal sealed class GetPagedQueryHandler : IRequestHandler<GetPagedQuery, ErrorOr<PagedResult<ParentResponse>>>
{
    private readonly IParentRepository _parentRepository;

    public GetPagedQueryHandler(IParentRepository parentRepository)
    {
        _parentRepository = parentRepository;
    }

    public async Task<ErrorOr<PagedResult<ParentResponse>>> Handle(GetPagedQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.Page <= 0 ? 1 : request.Page;
        int pageSize = request.PageSize <= 0 ? 15 : request.PageSize;

        var result = await _parentRepository.GetPagedAsync(
            request.SearchTerm, 
            pageNumber, 
            pageSize, 
            request.SortColumn, 
            request.SortDirection, 
            cancellationToken);

        var parents = result.Parents.Select(p => new ParentResponse(
            p.Id,
            p.FirstName,
            p.SecondName,
            p.ThirdName,
            p.LastName,
            p.Email?.Value,
            p.PhoneNumber.Value,
            p.NationalId,
            p.Job,
            p.Address,
            p.Notes,
            p.RegisteredOnUtc,
            p.Children.Count
        )).ToList();

        int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)result.TotalCount / pageSize) : 0;
        if (totalPages < 1) totalPages = 1;

        return new PagedResult<ParentResponse>(
            parents,
            pageNumber,
            pageSize,
            result.TotalCount,
            totalPages);
    }
}
