using System.Text;
using System.Text.RegularExpressions;

public static class SlugHelper
{
    public static string Slugify(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        var normalized = input
            .Normalize(NormalizationForm.FormD)
            .ToLowerInvariant();

        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var cleaned = Regex.Replace(sb.ToString(), @"[^a-z0-9]+", "-");
        return cleaned.Trim('-');
    }
}