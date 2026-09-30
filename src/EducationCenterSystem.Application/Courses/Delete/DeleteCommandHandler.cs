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
