using EducationCenterSystem.Domain.Primitives;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class GroupSession : AggregateRoot
{
    public Guid EducationalGroupId { get; private set; }
    public DateTime SessionDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public EducationalGroup? EducationalGroup { get; private set; }
    private readonly List<AttendanceRecord> _attendanceRecords = new();
    public IReadOnlyCollection<AttendanceRecord> AttendanceRecords => _attendanceRecords.AsReadOnly();

    private GroupSession() { } // EF Core

    private GroupSession(Guid id, Guid educationalGroupId, DateTime sessionDate, string? notes)
        : base(id)
    {
        EducationalGroupId = educationalGroupId;
        SessionDate = sessionDate;
        Notes = notes;
    }

    public static ErrorOr<GroupSession> Create(Guid educationalGroupId, DateTime sessionDate, string? notes = null)
    {
        if (educationalGroupId == Guid.Empty)
        {
            return Error.Validation("GroupSession.EducationalGroupId", "Educational group ID is required.");
        }

        return new GroupSession(Guid.NewGuid(), educationalGroupId, sessionDate, notes);
    }
}
