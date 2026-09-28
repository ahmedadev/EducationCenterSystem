using EducationCenterSystem.Application.Roles.Common;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Roles.GetPermissions;

internal sealed class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, ErrorOr<IReadOnlyList<PermissionResponse>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllPermissionsQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<PermissionResponse>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _roleRepository.GetAllPermissionsAsync(cancellationToken);

        var response = permissions.Select(p => new PermissionResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Module
        )).ToList();

        return response;
    }
}
