using EducationCenterSystem.Application.Users.Common;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.GetAll;

internal sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<IReadOnlyList<UserResponse>>>
{
    private readonly IUserRepository _userRepository;

    public GetAllQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<UserResponse>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var response = users.Select(u => new UserResponse(
            u.Id,
            u.FirstName,
            u.LastName,
            u.Email.Value,
            u.PhoneNumber?.Value,
            u.IsActive,
            u.CreatedOnUtc,
            u.LastLoginOnUtc,
            u.UserRoles
                .Where(ur => ur.Role is not null)
                .Select(ur => ur.Role.Name)
                .ToList()
        )).ToList();

        return response;
    }
}
