using System.Globalization;
using elastic_app_v3.Constants;

namespace elastic_app_v3.Errors
{
    public static class ErrorMessages
    {
        public const string FirstNameEmpty = "FirstName cannot be empty";
        public const string FirstNameNonAlphabetical = "You can only include alphabetical characters in your FirstName";

        public const string LastNameEmpty = "LastName cannot be empty";
        public const string LastNameNonAlphabetical = "You can only include alphabetical characters in your LastName";

        public const string UserNameEmpty = "UserName cannot be empty";
        public const string UserNameTooShort = "UserName must be at least {0} characters";
        public static string UserNameTooShortMessage() =>
        string.Format(
            UserNameTooShort,
            ValidationConstants.UserNameMinLength
        );

        public const string UserNameTooLong = "UserName must be no longer than {0} characters";
        public static string UserNameTooLongMessage() =>
        string.Format(
            UserNameTooShort,
            ValidationConstants.UserNameMaxLength
        );

        public const string PasswordEmpty = "Password cannot be empty";
        public const string PasswordTooShort = "Password must be at least {0} characters";
        public static string PasswordTooShortMessage() =>
            string.Format(
                PasswordTooShort,
                ValidationConstants.PasswordMinLength
            );

        public const string PasswordTooLong = "Password must be no longer than {0} characters";
        public static string PasswordTooLongMessage() =>
            string.Format(
                PasswordTooLong,
                ValidationConstants.PasswordMaxLength
            );

        public const string ReEnteredPasswordEmpty = "ReEnteredPassword cannot be empty";
        public const string ReEnteredPasswordNotMatching = "Password must match ReEnteredPassword";

    }
}
