using Habr.Common.Resources;

namespace Habr.Common.Helpers.Validation
{
    public static class PostValidationHelper
    {
        public static Tuple<bool, string> IsPostDataValid(
            string title,
            string text,
            int titleMaxLength,
            int textMaxLength)
        {
            var errorMessage = string.Empty;
            var isDataValid = true;

            if (string.IsNullOrWhiteSpace(title))
            {
                errorMessage += PostMessageResource.TitleRequired;
                isDataValid = false;
            }

            if (title.Length > titleMaxLength)
            {
                errorMessage += PostMessageResource.TitleMaxLength;
                isDataValid = false;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessage += PostMessageResource.TextRequired;
                isDataValid = false;
            }

            if (text.Length > textMaxLength)
            {
                errorMessage += PostMessageResource.TextMaxLength;
                isDataValid = false;
            }

            return new Tuple<bool, string>(isDataValid, errorMessage);
        }
    }
}
