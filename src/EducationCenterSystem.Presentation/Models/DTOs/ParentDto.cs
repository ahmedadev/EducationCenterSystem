namespace EducationCenterSystem.Presentation.WinForms.Models.DTOs;

public sealed record ParentDto(
    Guid Id,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    string? Email,
    string PhoneNumber,
    string? NationalId,
    string? Job,
    string? Address,
    string? Notes,
    DateTime RegisteredOnUtc,
    int ChildrenCount);
