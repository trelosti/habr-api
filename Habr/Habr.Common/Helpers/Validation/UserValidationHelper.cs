using System.Text.RegularExpressions;

namespace Habr.Common.Helpers.Validation
{
    public static class UserValidationHelper
    {
        public static bool IsEmailValid(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            var isEmailPatternValid = Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            var isEmailLengthValid = email.Length <= 200;

            return isEmailPatternValid && isEmailLengthValid;
        }

        // A utility method for processing the console input, will be removed in future
        public static string? ValidateUserName(string? name)
        {
            return string.IsNullOrWhiteSpace(name) ? null : name;
        }
    }
}
