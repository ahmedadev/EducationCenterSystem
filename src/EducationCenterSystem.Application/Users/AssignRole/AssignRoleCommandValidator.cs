using FluentValidation;

namespace EducationCenterSystem.Application.Users.AssignRole;

public sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("معرف المستخدم مطلوب.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("معرف الدور مطلوب.");
    }
}
