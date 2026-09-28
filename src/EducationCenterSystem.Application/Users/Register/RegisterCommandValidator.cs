using FluentValidation;

namespace EducationCenterSystem.Application.Users.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("الاسم الأول مطلوب.")
            .MaximumLength(100).WithMessage("الاسم الأول يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("الاسم الأخير مطلوب.")
            .MaximumLength(100).WithMessage("الاسم الأخير يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.")
            .MaximumLength(255).WithMessage("البريد الإلكتروني يجب ألا يتجاوز 255 حرفاً.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
            .MinimumLength(8).WithMessage("كلمة المرور يجب ألا تقل عن 8 أحرف وأرقام.")
            .Matches(@"[A-Za-z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف واحد على الأقل.")
            .Matches(@"[0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.");
    }
}
