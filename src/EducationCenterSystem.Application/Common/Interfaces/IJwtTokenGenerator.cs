using EducationCenterSystem.Domain.Entities;

namespace EducationCenterSystem.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
}
