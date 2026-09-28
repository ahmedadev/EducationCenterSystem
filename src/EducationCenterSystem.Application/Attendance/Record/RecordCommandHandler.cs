using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.Record;

public sealed class RecordCommandHandler : IRequestHandler<RecordCommand, ErrorOr<Guid>>
{
    private readonly IGroupSessionRepository _sessionRepository;
    private readonly IAttendanceRecordRepository _attendanceRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IEducationalGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordCommandHandler(
        IGroupSessionRepository sessionRepository,
        IAttendanceRecordRepository attendanceRepository,
        IStudentRepository studentRepository,
        IEducationalGroupRepository groupRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
        _studentRepository = studentRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(RecordCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.GroupSessionId, cancellationToken);
        if (session is null)
        {
            return Error.NotFound("GroupSession.NotFound", "The specified session was not found.");
        }

        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
        {
            return Error.NotFound("Student.NotFound", "The specified student was not found.");
        }

        bool isEnrolled = await _groupRepository.IsStudentEnrolledAsync(request.StudentId, session.EducationalGroupId, cancellationToken);
        if (!isEnrolled)
        {
            return Error.Validation("Attendance.NotEnrolled", "The student is not enrolled in this group.");
        }

        var existingRecords = await _attendanceRepository.GetBySessionIdAsync(session.Id, cancellationToken);
        var existingRecord = existingRecords.FirstOrDefault(r => r.StudentId == request.StudentId);

        if (existingRecord is not null)
        {
            // Update existing record
            existingRecord.UpdateStatus(request.Status, request.Notes);
            _attendanceRepository.Update(existingRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return existingRecord.Id;
        }

        // Create new record
        var recordResult = AttendanceRecord.Create(request.GroupSessionId, request.StudentId, request.Status, request.Notes);
        if (recordResult.IsError)
        {
            return recordResult.Errors;
        }

        var record = recordResult.Value;
        _attendanceRepository.Add(record);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return record.Id;
    }
}
