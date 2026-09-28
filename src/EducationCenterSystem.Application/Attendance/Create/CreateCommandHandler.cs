using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.Create;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, ErrorOr<Guid>>
{
    private readonly IGroupSessionRepository _sessionRepository;
    private readonly IEducationalGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandHandler(
        IGroupSessionRepository sessionRepository,
        IEducationalGroupRepository groupRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.EducationalGroupId, cancellationToken);
        if (group is null)
        {
            return Error.NotFound("EducationalGroup.NotFound", "The specified group was not found.");
        }

        if (await _sessionRepository.ExistsForDateAsync(request.EducationalGroupId, request.SessionDate, cancellationToken))
        {
            return Error.Conflict("GroupSession.Exists", "A session for this group already exists on this date.");
        }

        var sessionResult = GroupSession.Create(request.EducationalGroupId, request.SessionDate, request.Notes);

        if (sessionResult.IsError)
        {
            return sessionResult.Errors;
        }

        var session = sessionResult.Value;
        _sessionRepository.Add(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
