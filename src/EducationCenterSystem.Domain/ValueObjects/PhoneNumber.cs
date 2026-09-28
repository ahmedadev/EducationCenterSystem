using ErrorOr;

namespace EducationCenterSystem.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static ErrorOr<PhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Error.Validation("PhoneNumber.Empty", "Phone number cannot be empty.");
        }

        if (phoneNumber.Length < 10 || phoneNumber.Length > 15)
        {
            return Error.Validation("PhoneNumber.Length", "Invalid phone number length.");
        }

        return new PhoneNumber(phoneNumber);
    }
}
