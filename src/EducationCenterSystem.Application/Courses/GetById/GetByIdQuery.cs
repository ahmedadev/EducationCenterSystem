using EducationCenterSystem.Application.Courses.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.GetById;

public sealed record GetByIdQuery(Guid Id) : IRequest<ErrorOr<CourseResponse>>;
