using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Roles.Create;

internal sealed class CreateCommandHandler : IRequestHandler<CreateCommand, ErrorOr<Guid>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        if (!await _roleRepository.IsRoleNameUniqueAsync(request.Name, cancellationToken))
        {
            return Error.Conflict("Role.NameInUse", "اسم هذا الدور مستخدم بالفعل.");
        }

        var roleResult = Role.Create(request.Name, request.Description, isSystemRole: false);
        if (roleResult.IsError)
        {
            return roleResult.Errors;
        }

        var role = roleResult.Value;

        foreach (int permId in request.PermissionIds)
        {
            var assignResult = role.AssignPermission(permId);
            if (assignResult.IsError)
            {
                return assignResult.Errors;
            }
        }

        _roleRepository.Add(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return role.Id;
    }
}
