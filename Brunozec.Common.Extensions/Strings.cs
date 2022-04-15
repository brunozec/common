using System.Globalization;
using System.Text;

namespace Brunozec.Common.Extensions;

public static class Strings
{
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static string RemoveDiacritics(this string text)
    {
        try
        {
            if (string.IsNullOrEmpty(text)) return "";

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        catch (Exception)
        {
            return text;
        }
    }
}