using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.Delete;

public sealed record DeleteCommand(Guid TeacherId) : IRequest<ErrorOr<Success>>;
