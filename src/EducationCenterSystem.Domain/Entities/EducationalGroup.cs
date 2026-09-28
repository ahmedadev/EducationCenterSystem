using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Primitives;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class EducationalGroup : AggregateRoot
{
    public Guid CourseId { get; private set; }
    public Guid TeacherId { get; private set; }
    public string Name { get; private set; }
    public int MaxCapacity { get; private set; }
    public decimal MonthlyFee { get; private set; }
    public string ScheduleDescription { get; private set; }
    public GroupStatus Status { get; private set; }
    
    public Course Course { get; private set; } = null!;
    public Teacher Teacher { get; private set; } = null!;
    
    private readonly List<StudentGroup> _enrollments = new();
    public IReadOnlyCollection<StudentGroup> Enrollments => _enrollments.AsReadOnly();

    private EducationalGroup() : base()
    {
        Name = default!;
        ScheduleDescription = default!;
    }

    private EducationalGroup(
        Guid id,
        Guid courseId,
        Guid teacherId,
        string name,
        int maxCapacity,
        decimal monthlyFee,
        string scheduleDescription) : base(id)
    {
        CourseId = courseId;
        TeacherId = teacherId;
        Name = name;
        MaxCapacity = maxCapacity;
        MonthlyFee = monthlyFee;
        ScheduleDescription = scheduleDescription;
        Status = GroupStatus.Active;
    }

    public static ErrorOr<EducationalGroup> Create(
        Guid courseId,
        Guid teacherId,
        string name,
        int maxCapacity,
        decimal monthlyFee,
        string scheduleDescription)
    {
        if (courseId == Guid.Empty)
            return Error.Validation("EducationalGroup.CourseId", "Course ID is required.");
            
        if (teacherId == Guid.Empty)
            return Error.Validation("EducationalGroup.TeacherId", "Teacher ID is required.");
            
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("EducationalGroup.Name", "Group name cannot be empty.");
            
        if (maxCapacity <= 0)
            return Error.Validation("EducationalGroup.MaxCapacity", "Max capacity must be greater than zero.");
            
        if (monthlyFee < 0)
            return Error.Validation("EducationalGroup.MonthlyFee", "Monthly fee cannot be negative.");
            
        if (string.IsNullOrWhiteSpace(scheduleDescription))
            return Error.Validation("EducationalGroup.ScheduleDescription", "Schedule description cannot be empty.");

        var group = new EducationalGroup(
            Guid.NewGuid(),
            courseId,
            teacherId,
            name,
            maxCapacity,
            monthlyFee,
            scheduleDescription);

        return group;
    }

    public ErrorOr<Success> Update(
        string name,
        int maxCapacity,
        decimal monthlyFee,
        string scheduleDescription)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("EducationalGroup.Name", "Group name cannot be empty.");
            
        if (maxCapacity <= 0)
            return Error.Validation("EducationalGroup.MaxCapacity", "Max capacity must be greater than zero.");
            
        if (monthlyFee < 0)
            return Error.Validation("EducationalGroup.MonthlyFee", "Monthly fee cannot be negative.");
            
        if (string.IsNullOrWhiteSpace(scheduleDescription))
            return Error.Validation("EducationalGroup.ScheduleDescription", "Schedule description cannot be empty.");

        Name = name;
        MaxCapacity = maxCapacity;
        MonthlyFee = monthlyFee;
        ScheduleDescription = scheduleDescription;

        return Result.Success;
    }

    public void ChangeStatus(GroupStatus newStatus)
    {
        Status = newStatus;
    }
}
