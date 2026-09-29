using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Primitives;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class Parent : AggregateRoot
{
    public string FirstName { get; private set; }
    public string SecondName { get; private set; }
    public string ThirdName { get; private set; }
    public string LastName { get; private set; }
    public Email? Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? NationalId { get; private set; }
    public string? Job { get; private set; }
    public string? Address { get; private set; }
    public string? Notes { get; private set; }
    public DateTime RegisteredOnUtc { get; private set; }

    private readonly List<Student> _children = new();
    public IReadOnlyCollection<Student> Children => _children.AsReadOnly();

    private Parent() : base()
    {
        FirstName = default!;
        SecondName = default!;
        ThirdName = default!;
        LastName = default!;
        PhoneNumber = default!;
    }

    private Parent(
        Guid id,
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        Email? email,
        PhoneNumber phoneNumber,
        string? nationalId,
        string? job,
        string? address,
        string? notes,
        DateTime registeredOnUtc) : base(id)
    {
        FirstName = firstName;
        SecondName = secondName;
        ThirdName = thirdName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        NationalId = nationalId;
        Job = job;
        Address = address;
        Notes = notes;
        RegisteredOnUtc = registeredOnUtc;
    }

    public static ErrorOr<Parent> Register(
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        Email? email,
        PhoneNumber phoneNumber,
        string? nationalId,
        string? job,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(firstName) || 
            string.IsNullOrWhiteSpace(secondName) || 
            string.IsNullOrWhiteSpace(thirdName) || 
            string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("Parent.Name.Required", "All 4 parts of the name are required.");

        var parent = new Parent(
            Guid.NewGuid(),
            firstName,
            secondName,
            thirdName,
            lastName,
            email,
            phoneNumber,
            nationalId,
            job,
            address,
            notes,
            DateTime.UtcNow);

        return parent;
    }

    public ErrorOr<Success> UpdateProfile(
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        Email? email,
        PhoneNumber phoneNumber,
        string? nationalId,
        string? job,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(firstName) || 
            string.IsNullOrWhiteSpace(secondName) || 
            string.IsNullOrWhiteSpace(thirdName) || 
            string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("Parent.Name.Required", "All 4 parts of the name are required.");

        FirstName = firstName;
        SecondName = secondName;
        ThirdName = thirdName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        NationalId = nationalId;
        Job = job;
        Address = address;
        Notes = notes;

        return Result.Success;
    }
}
