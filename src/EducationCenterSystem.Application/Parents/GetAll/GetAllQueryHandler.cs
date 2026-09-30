using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.GetAll;

internal sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyCollection<ParentResponse>>>
{
    private readonly IParentRepository _parentRepository;

    public GetAllQueryHandler(IParentRepository parentRepository)
    {
        _parentRepository = parentRepository;
    }

    public async Task<ErrorOr<IReadOnlyCollection<ParentResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var result = await _parentRepository.GetPagedAsync(null, 1, 1000, null, null, cancellationToken);

        var response = result.Parents.Select(p => new ParentResponse(
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

        return response.AsReadOnly();
    }
}
