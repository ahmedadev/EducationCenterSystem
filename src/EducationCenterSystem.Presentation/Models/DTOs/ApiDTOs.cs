using System;

namespace EducationCenterSystem.Presentation.Models.DTOs;

public record CourseDto(Guid Id, string Name, string GradeLevel, string Subject, string Description, bool IsActive);
public record EducationalGroupDto(Guid Id, Guid CourseId, string CourseName, Guid TeacherId, string TeacherName, string Name, int MaxCapacity, decimal MonthlyFee, string ScheduleDescription, int Status);
public record GroupSessionDto(Guid Id, Guid GroupId, string GroupName, DateTime Date, string Notes);
public record AttendanceRecordDto(Guid Id, Guid GroupSessionId, Guid StudentId, string StudentName, int Status, string Notes);
