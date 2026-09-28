using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.Delete;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand, ErrorOr<Success>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

        if (student is null)
        {
            return Error.NotFound("Student.NotFound", "The student was not found.");
        }

        _studentRepository.Remove(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
