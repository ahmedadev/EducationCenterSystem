using EducationCenterSystem.Application.Roles.Common;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Roles.GetAll;

internal sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyList<RoleResponse>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<RoleResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        var response = roles.Select(r => new RoleResponse(
            r.Id,
            r.Name,
            r.Description,
            r.IsSystemRole,
            r.Permissions
                .Where(rp => rp.Permission is not null)
                .Select(rp => rp.Permission.Name)
                .ToList()
        )).ToList();

        return response;
    }
}
