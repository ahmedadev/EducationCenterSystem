using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Primitives;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class AttendanceRecord : Entity
{
    public Guid GroupSessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public GroupSession? GroupSession { get; private set; }
    public Student? Student { get; private set; }

    private AttendanceRecord() { } // EF Core

    private AttendanceRecord(Guid id, Guid groupSessionId, Guid studentId, AttendanceStatus status, string? notes)
        : base(id)
    {
        GroupSessionId = groupSessionId;
        StudentId = studentId;
        Status = status;
        Notes = notes;
    }

    public static ErrorOr<AttendanceRecord> Create(Guid groupSessionId, Guid studentId, AttendanceStatus status, string? notes = null)
    {
        if (groupSessionId == Guid.Empty)
            return Error.Validation("AttendanceRecord.GroupSessionId", "Group session ID is required.");
        
        if (studentId == Guid.Empty)
            return Error.Validation("AttendanceRecord.StudentId", "Student ID is required.");

        return new AttendanceRecord(Guid.NewGuid(), groupSessionId, studentId, status, notes);
    }
    
    public void UpdateStatus(AttendanceStatus newStatus, string? newNotes = null)
    {
        Status = newStatus;
        if (newNotes is not null)
        {
            Notes = newNotes;
        }
    }
}
