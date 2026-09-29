using FluentValidation;

namespace EducationCenterSystem.Application.Students.LinkParent;

public sealed class LinkParentCommandValidator : AbstractValidator<LinkParentCommand>
{
    public LinkParentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("ParentId is required.");
    }
}
