using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Domain.ValueObjects;

public class ContactInfo : ValueObject
{
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }

    private ContactInfo(string email, string phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static ContactInfo Create(string email, string phoneNumber)
    {
        email = NormalizeEmail(email);
        Validate(email, phoneNumber);
        return new ContactInfo(email, phoneNumber);
    }

    public static string NormalizeEmail(string email)
        => email?.Trim().ToLowerInvariant();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return PhoneNumber;
    }

    private static void Validate(string email, string phoneNumber)
    {
        Guard.AgainstEmptyString<InvalidContactInfoException>(email, nameof(email));
        Guard.AgainstWrongEmailFormat<InvalidContactInfoException>(email);
        Guard.ForStringLength<InvalidContactInfoException>(email, MinEmailLength, MaxEmailLength, nameof(email));

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            Guard.AgainstNotMatchingRegex<InvalidContactInfoException>(phoneNumber, nameof(phoneNumber), PhoneNumberRegex);
        }
    }
}
