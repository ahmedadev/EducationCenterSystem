$AppPath = "d:\projects\EducationCenterSystem\src\EducationCenterSystem.Application\Courses"

New-Item -Path "$AppPath\GetById" -ItemType Directory -Force
New-Item -Path "$AppPath\Update" -ItemType Directory -Force
New-Item -Path "$AppPath\Delete" -ItemType Directory -Force

# GetByIdQuery.cs
@"
using EducationCenterSystem.Application.Courses.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.GetById;

public sealed record GetByIdQuery(Guid Id) : IRequest<ErrorOr<CourseResponse>>;
"@ | Out-File -FilePath "$AppPath\GetById\GetByIdQuery.cs" -Encoding utf8

# GetByIdQueryHandler.cs
@"
using EducationCenterSystem.Application.Courses.GetAll;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.GetById;

internal sealed class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ErrorOr<CourseResponse>>
{
    private readonly ICourseRepository _courseRepository;

    public GetByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<ErrorOr<CourseResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
            return Error.NotFound("Course.NotFound", "The course was not found.");

        return new CourseResponse(
            course.Id,
            course.Name,
            course.GradeLevel,
            course.Subject,
            course.Description,
            course.IsActive);
    }
}
"@ | Out-File -FilePath "$AppPath\GetById\GetByIdQueryHandler.cs" -Encoding utf8

# UpdateCommand.cs
@"
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Update;

public sealed record UpdateCommand(
    Guid Id,
    string Name,
    string GradeLevel,
    string Subject,
    string? Description) : IRequest<ErrorOr<Success>>;
"@ | Out-File -FilePath "$AppPath\Update\UpdateCommand.cs" -Encoding utf8

# UpdateCommandHandler.cs
@"
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Update;

internal sealed class UpdateCommandHandler : IRequestHandler<UpdateCommand, ErrorOr<Success>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
            return Error.NotFound("Course.NotFound", "The course was not found.");

        var updateResult = course.Update(request.Name, request.GradeLevel, request.Subject, request.Description);
        if (updateResult.IsError)
            return updateResult.Errors;

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
"@ | Out-File -FilePath "$AppPath\Update\UpdateCommandHandler.cs" -Encoding utf8

# UpdateCommandValidator.cs
@"
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
"@ | Out-File -FilePath "$AppPath\Update\UpdateCommandValidator.cs" -Encoding utf8

# DeleteCommand.cs
@"
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Delete;

public sealed record DeleteCommand(Guid Id) : IRequest<ErrorOr<Success>>;
"@ | Out-File -FilePath "$AppPath\Delete\DeleteCommand.cs" -Encoding utf8

# DeleteCommandHandler.cs
@"
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Delete;

internal sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand, ErrorOr<Success>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
            return Error.NotFound("Course.NotFound", "The course was not found.");

        _courseRepository.Remove(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
"@ | Out-File -FilePath "$AppPath\Delete\DeleteCommandHandler.cs" -Encoding utf8
