using EducationCenterSystem.Application.Users.Common;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;
