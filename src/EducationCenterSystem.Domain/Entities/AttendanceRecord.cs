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
    public DateTime? CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }

    // Navigation properties
    public GroupSession? GroupSession { get; private set; }
    public Student? Student { get; private set; }

    private AttendanceRecord() { } // EF Core

    private AttendanceRecord(Guid id, Guid groupSessionId, Guid studentId, AttendanceStatus status, string? notes, DateTime? checkInTime = null, DateTime? checkOutTime = null)
        : base(id)
    {
        GroupSessionId = groupSessionId;
        StudentId = studentId;
        Status = status;
        Notes = notes;
        CheckInTime = checkInTime;
        CheckOutTime = checkOutTime;
    }

    public static ErrorOr<AttendanceRecord> Create(Guid groupSessionId, Guid studentId, AttendanceStatus status, string? notes = null, DateTime? checkInTime = null)
    {
        if (groupSessionId == Guid.Empty)
            return Error.Validation("AttendanceRecord.GroupSessionId", "Group session ID is required.");
        
        if (studentId == Guid.Empty)
            return Error.Validation("AttendanceRecord.StudentId", "Student ID is required.");

        return new AttendanceRecord(Guid.NewGuid(), groupSessionId, studentId, status, notes, checkInTime);
    }
    
    public void UpdateStatus(AttendanceStatus newStatus, string? newNotes = null)
    {
        Status = newStatus;
        if (newNotes is not null)
        {
            Notes = newNotes;
        }
    }

    public ErrorOr<Success> CheckOut(DateTime checkOutTime)
    {
        if (checkOutTime <= CheckInTime)
            return Error.Validation("AttendanceRecord.CheckOutTime", "Check-out time must be after check-in time.");

        CheckOutTime = checkOutTime;
        return Result.Success;
    }

    public void SetCheckInTime(DateTime checkInTime)
    {
        CheckInTime = checkInTime;
    }
}
