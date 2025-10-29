using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.ValueObjects;
using Body4uHUB.Shared;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Domain.Models
{
    public class User : Entity<Guid>, IAggregateRoot
    {
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsEmailConfirmed { get; private set; }

        //За EF Core
        private User() : base(Guid.NewGuid()) { }

        internal User(Guid id, string passwordHash, string firstName, string lastName, ContactInfo contactInfo)
            : base(id)
        {
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
        }

        public static User Create(string passwordHash, string firstName, string lastName, string email, string phoneNumber)
        {
            Validate(passwordHash, firstName, lastName);

            var contactInfo = ContactInfo.Create(email, phoneNumber);

            return new User(Guid.NewGuid(), passwordHash, firstName, lastName, contactInfo);
        }

        public void UpdateFirstName(string firstName)
        {
            ValidateFirstName(firstName);
            FirstName = firstName;
        }

        public void UpdateLastName(string lastName)
        {
            ValidateLastName(lastName);
            LastName = lastName;
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            ValidatePasswordHash(passwordHash);
            PasswordHash = passwordHash;
        }

        public void UpdateContactInfo(string email, string phoneNumber)
        {
            ContactInfo = ContactInfo.Create(email, phoneNumber);
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void ConfirmEmail()
        {
            IsEmailConfirmed = true;
        }

        private static void Validate(string passwordHash, string firstName, string lastName)
        {
            ValidatePasswordHash(passwordHash);
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
        }

        private static void ValidateFirstName(string firstName)
        {
            Guard.AgainstEmptyString<InvalidUserException>(firstName, nameof(firstName));
            Guard.ForStringLength<InvalidUserException>(firstName, MinNameLength, MaxNameLength, nameof(firstName));
        }

        private static void ValidateLastName(string lastName)
        {
            Guard.AgainstEmptyString<InvalidUserException>(lastName, nameof(lastName));
            Guard.ForStringLength<InvalidUserException>(lastName, MinNameLength, MaxNameLength, nameof(lastName));
        }

        private static void ValidatePasswordHash(string passwordHash)
        {
            Guard.AgainstEmptyString<InvalidUserException>(passwordHash, nameof(passwordHash));
        }
    }
}
