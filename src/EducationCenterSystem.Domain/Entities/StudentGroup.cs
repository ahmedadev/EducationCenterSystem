using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Primitives;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class StudentGroup : Entity
{
    public Guid StudentId { get; private set; }
    public Guid EducationalGroupId { get; private set; }
    public DateTime EnrollmentDateUtc { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    public Student Student { get; private set; } = null!;
    public EducationalGroup EducationalGroup { get; private set; } = null!;

    private StudentGroup() : base()
    {
    }

    private StudentGroup(Guid studentId, Guid educationalGroupId) : base(Guid.NewGuid())
    {
        StudentId = studentId;
        EducationalGroupId = educationalGroupId;
        EnrollmentDateUtc = DateTime.UtcNow;
        Status = EnrollmentStatus.Active;
    }

    public static ErrorOr<StudentGroup> Create(Guid studentId, Guid educationalGroupId)
    {
        if (studentId == Guid.Empty)
            return Error.Validation("StudentGroup.StudentId", "Student ID is required.");
            
        if (educationalGroupId == Guid.Empty)
            return Error.Validation("StudentGroup.EducationalGroupId", "EducationalGroup ID is required.");

        return new StudentGroup(studentId, educationalGroupId);
    }

    public void ChangeStatus(EnrollmentStatus status)
    {
        Status = status;
    }
}
