using EducationCenterSystem.Application.Teachers.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetByNationalId;

public sealed record GetByNationalIdQuery(string NationalId) : IRequest<ErrorOr<TeacherResponse>>;
