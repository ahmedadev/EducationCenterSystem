using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.BatchRecord;

public sealed class BatchRecordCommandHandler : IRequestHandler<BatchRecordCommand, ErrorOr<Success>>
{
    private readonly IGroupSessionRepository _groupSessionRepository;
    private readonly IAttendanceRecordRepository _attendanceRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BatchRecordCommandHandler(
        IGroupSessionRepository groupSessionRepository,
        IAttendanceRecordRepository attendanceRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _groupSessionRepository = groupSessionRepository;
        _attendanceRecordRepository = attendanceRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(BatchRecordCommand request, CancellationToken cancellationToken)
    {
        // 1. Get or create session for the group and date
        var groupSessions = await _groupSessionRepository.GetByGroupIdAsync(request.GroupId, cancellationToken);
        var session = groupSessions.FirstOrDefault(s => s.SessionDate.Date == request.SessionDate.Date);

        if (session is null)
        {
            var sessionResult = GroupSession.Create(request.GroupId, request.SessionDate);
            if (sessionResult.IsError)
            {
                return sessionResult.Errors;
            }
            session = sessionResult.Value;
            _groupSessionRepository.Add(session);
        }

        // 2. Fetch existing records for this session
        var existingRecords = (await _attendanceRecordRepository.GetBySessionIdAsync(session.Id, cancellationToken)).ToList();

        // 3. Process incoming records
        foreach (var reqRecord in request.Records)
        {
            var existing = existingRecords.FirstOrDefault(r => r.StudentId == reqRecord.StudentId);
            if (existing is not null)
            {
                // Update
                existing.UpdateStatus(reqRecord.Status, reqRecord.Notes);
                if (reqRecord.CheckInTime.HasValue)
                    existing.SetCheckInTime(reqRecord.CheckInTime.Value);
                if (reqRecord.CheckOutTime.HasValue)
                    existing.CheckOut(reqRecord.CheckOutTime.Value);
                
                _attendanceRecordRepository.Update(existing);
            }
            else
            {
                // Create
                var newRecordResult = AttendanceRecord.Create(session.Id, reqRecord.StudentId, reqRecord.Status, reqRecord.Notes, reqRecord.CheckInTime);
                if (newRecordResult.IsError)
                {
                    return newRecordResult.Errors;
                }
                
                if (reqRecord.CheckOutTime.HasValue)
                {
                    var checkoutResult = newRecordResult.Value.CheckOut(reqRecord.CheckOutTime.Value);
                    if (checkoutResult.IsError)
                        return checkoutResult.Errors;
                }

                _attendanceRecordRepository.Add(newRecordResult.Value);
            }
        }

        // 4. Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
