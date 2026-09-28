using FluentValidation;

namespace EducationCenterSystem.Application.EducationalGroups.Create;

public sealed class CreateCommandValidator : AbstractValidator<CreateCommand>
{
    public CreateCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.TeacherId)
            .NotEmpty().WithMessage("Teacher ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Group name is required.")
            .MaximumLength(200).WithMessage("Group name must not exceed 200 characters.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Max capacity must be greater than zero.");

        RuleFor(x => x.MonthlyFee)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly fee cannot be negative.");

        RuleFor(x => x.ScheduleDescription)
            .NotEmpty().WithMessage("Schedule description is required.")
            .MaximumLength(500).WithMessage("Schedule description must not exceed 500 characters.");
    }
}
