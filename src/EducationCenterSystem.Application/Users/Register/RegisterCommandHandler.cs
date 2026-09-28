using EducationCenterSystem.Application.Common.Interfaces;
using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (phoneResult.IsError)
            {
                return phoneResult.Errors;
            }
            phoneNumber = phoneResult.Value;
        }

        if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
        {
            return Error.Conflict("User.EmailInUse", "البريد الإلكتروني مسجل بالفعل.");
        }

        string passwordHash = _passwordHasher.Hash(request.Password);

        var userResult = User.Create(
            request.FirstName,
            request.LastName,
            emailResult.Value,
            passwordHash,
            phoneNumber);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;

        if (request.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId.Value, cancellationToken);
            if (role is null)
            {
                return Error.NotFound("Role.NotFound", "الدور المحدد غير موجود.");
            }

            var assignResult = user.AssignRole(role.Id);
            if (assignResult.IsError)
            {
                return assignResult.Errors;
            }
        }

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
