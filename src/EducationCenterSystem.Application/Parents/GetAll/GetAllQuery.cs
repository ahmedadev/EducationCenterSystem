using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.GetAll;

public sealed record GetAllQuery : IRequest<ErrorOr<IReadOnlyCollection<ParentResponse>>>;
