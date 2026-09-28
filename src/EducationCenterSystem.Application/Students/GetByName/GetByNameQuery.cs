using EducationCenterSystem.Application.Students.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetByName;

public sealed record GetByNameQuery(string Name) : IRequest<ErrorOr<IReadOnlyList<StudentResponse>>>;
