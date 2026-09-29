using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.LinkParent;

public sealed class LinkParentCommandHandler : IRequestHandler<LinkParentCommand, ErrorOr<Success>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IParentRepository _parentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LinkParentCommandHandler(
        IStudentRepository studentRepository,
        IParentRepository parentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _parentRepository = parentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(LinkParentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
            return Error.NotFound("Student.NotFound", "Student not found.");

        var parent = await _parentRepository.GetByIdAsync(request.ParentId, cancellationToken);
        if (parent is null)
            return Error.NotFound("Parent.NotFound", "Parent not found.");

        var result = student.LinkParent(request.ParentId);
        if (result.IsError)
            return result;

        _studentRepository.Update(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
