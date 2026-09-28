using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetAll;

public sealed record GetAllQuery() : IRequest<ErrorOr<IReadOnlyList<StudentResponse>>>;
