using FluentValidation;

namespace EducationCenterSystem.Application.Teachers.GetByName;

public sealed class GetByNameQueryValidator : AbstractValidator<GetByNameQuery>
{
    public GetByNameQueryValidator() => RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
}
