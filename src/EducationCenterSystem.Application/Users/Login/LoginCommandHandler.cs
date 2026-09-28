using EducationCenterSystem.Application.Common.Interfaces;
using EducationCenterSystem.Application.Users.Common;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsError)
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "بيانات تسجيل الدخول غير صحيحة.");
        }

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "بيانات تسجيل الدخول غير صحيحة أو الحساب غير مفعّل.");
        }

        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "بيانات تسجيل الدخول غير صحيحة.");
        }

        user.RecordLogin(DateTime.UtcNow);

        var roles = user.UserRoles
            .Where(ur => ur.Role is not null)
            .Select(ur => ur.Role.Name)
            .ToList();

        var permissions = await _userRepository.GetPermissionsByUserIdAsync(user.Id, cancellationToken);

        string token = _jwtTokenGenerator.GenerateToken(user, roles, permissions);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.PhoneNumber?.Value,
            roles,
            permissions,
            token);
    }
}
