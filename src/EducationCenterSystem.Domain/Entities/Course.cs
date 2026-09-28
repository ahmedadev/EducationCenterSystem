using EducationCenterSystem.Domain.Primitives;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class Course : AggregateRoot
{
    public string Name { get; private set; }
    public string GradeLevel { get; private set; }
    public string Subject { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<EducationalGroup> _groups = new();
    public IReadOnlyCollection<EducationalGroup> Groups => _groups.AsReadOnly();

    private Course() : base()
    {
        Name = default!;
        GradeLevel = default!;
        Subject = default!;
    }

    private Course(
        Guid id, 
        string name, 
        string gradeLevel, 
        string subject, 
        string? description) : base(id)
    {
        Name = name;
        GradeLevel = gradeLevel;
        Subject = subject;
        Description = description;
        IsActive = true;
    }

    public static ErrorOr<Course> Create(string name, string gradeLevel, string subject, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Course.Name", "Course name cannot be empty.");
            
        if (string.IsNullOrWhiteSpace(gradeLevel))
            return Error.Validation("Course.GradeLevel", "Grade level cannot be empty.");
            
        if (string.IsNullOrWhiteSpace(subject))
            return Error.Validation("Course.Subject", "Subject cannot be empty.");

        var course = new Course(Guid.NewGuid(), name, gradeLevel, subject, description);

        // course.RaiseDomainEvent(new CourseCreatedEvent(course.Id));

        return course;
    }

    public ErrorOr<Success> Update(string name, string gradeLevel, string subject, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Course.Name", "Course name cannot be empty.");
            
        if (string.IsNullOrWhiteSpace(gradeLevel))
            return Error.Validation("Course.GradeLevel", "Grade level cannot be empty.");
            
        if (string.IsNullOrWhiteSpace(subject))
            return Error.Validation("Course.Subject", "Subject cannot be empty.");

        Name = name;
        GradeLevel = gradeLevel;
        Subject = subject;
        Description = description;

        return Result.Success;
    }

    public void ChangeStatus(bool isActive)
    {
        IsActive = isActive;
    }
}
