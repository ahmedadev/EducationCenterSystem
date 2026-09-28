using EducationCenterSystem.Application.Students.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.GetByNationalId;

public sealed record GetByNationalIdQuery(string NationalId) : IRequest<ErrorOr<StudentResponse>>;
