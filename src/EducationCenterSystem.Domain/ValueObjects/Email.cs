using ErrorOr;
using System.Text.RegularExpressions;

namespace EducationCenterSystem.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static ErrorOr<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Error.Validation("Email.Empty", "Email cannot be empty.");
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return Error.Validation("Email.Format", "Invalid email format.");
        }

        return new Email(email);
    }
}
