using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.Delete;

public sealed record DeleteCommand(Guid StudentId) : IRequest<ErrorOr<Success>>;
