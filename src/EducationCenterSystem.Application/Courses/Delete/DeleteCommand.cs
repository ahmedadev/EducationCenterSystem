using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Delete;

public sealed record DeleteCommand(Guid Id) : IRequest<ErrorOr<Success>>;
