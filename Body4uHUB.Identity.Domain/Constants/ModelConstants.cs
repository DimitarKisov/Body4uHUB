namespace Body4uHUB.Identity.Domain.Constants
{
    public class ModelConstants
    {
        public class UserConstants
        {
            public const int MinNameLength = 2;
            public const int MaxNameLength = 20;

            public const string EmailRequired = "Email is required";
            public const string EmailInvalid = "Invalid email format";
            public const string EmailLength = "Email cannot exceed 200 characters";
            public const string FirstNameRequired = "First name is required";
            public const string FirstNameLength = "First name must be between {0} and {1} characters long";
            public const string LastNameRequired = "Last name is required";
            public const string LastNameLength = "Last name must be between 2 and 20 characters long";
            public const string UserIdRequired = "User ID cannot be empty";
            public const string PasswordRequired = "Password is required";
            public const string PasswordRegex = @"^(?=.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_])(?!.*\s).*$";
            public const string PasswordInvalid = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character";
            public const string PhoneNumberRequired = "Phone number is required";
            public const string PhoneNumberRegex = @"^\+?[0-9]{10,15}$";
            public const string PhoneNumberInvalid = "Invalid phone number format";

            public const string EmailNotConfirmed = "Email is not confirmed.";
            public const string InvalidCredentials = "Invalid email or password.";
            public const string UserEmailExists = "User with the given email already exists.";
            public const string UserNotFound = "User not found.";
        }
    }
}
