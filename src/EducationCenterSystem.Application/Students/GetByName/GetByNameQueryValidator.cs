using FluentValidation;

namespace EducationCenterSystem.Application.Students.GetByName;

public sealed class GetByNameQueryValidator : AbstractValidator<GetByNameQuery>
{
    public GetByNameQueryValidator() => RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Student name cannot be empty.");
}
