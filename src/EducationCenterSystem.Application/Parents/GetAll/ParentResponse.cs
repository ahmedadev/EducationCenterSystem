namespace EducationCenterSystem.Application.Parents.GetAll;

public sealed record ParentResponse(
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
