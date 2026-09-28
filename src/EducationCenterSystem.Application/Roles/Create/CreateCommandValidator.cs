using FluentValidation;

namespace EducationCenterSystem.Application.Roles.Create;

public sealed class CreateCommandValidator : AbstractValidator<CreateCommand>
{
    public CreateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم الدور مطلوب.")
            .MaximumLength(50).WithMessage("اسم الدور يجب ألا يتجاوز 50 حرفاً.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("الوصف يجب ألا يتجاوز 250 حرفاً.");

        RuleFor(x => x.PermissionIds)
            .NotNull().WithMessage("قائمة الصلاحيات مطلوبة.");
    }
}
