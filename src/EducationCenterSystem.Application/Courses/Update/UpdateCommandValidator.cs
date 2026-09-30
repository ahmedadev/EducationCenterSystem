using FluentValidation;

namespace EducationCenterSystem.Application.Courses.Update;

public sealed class UpdateCommandValidator : AbstractValidator<UpdateCommand>
{
    public UpdateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GradeLevel).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(100);
    }
}
