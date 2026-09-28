using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.GetAll;

public sealed record GetAllQuery() : IRequest<ErrorOr<IReadOnlyList<CourseResponse>>>;
