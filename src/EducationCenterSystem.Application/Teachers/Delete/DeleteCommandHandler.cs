using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.Delete;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand, ErrorOr<Success>>
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Error.NotFound("Teacher.NotFound", "The teacher was not found.");
        }

        _teacherRepository.Remove(teacher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
