using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.LinkParent;

public sealed record LinkParentCommand(Guid StudentId, Guid ParentId) : IRequest<ErrorOr<Success>>;
