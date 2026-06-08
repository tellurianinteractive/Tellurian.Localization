using System.Globalization;

namespace Tellurian.Localization;

/// <summary>
/// Represents a language that are fully or partially supported in an application.
/// </summary>
/// <param name="TwoLetterCode">The <see href="https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes">ISO 639-1</see> two letter language code.</param>
/// <param name="IsFullySupported">The language is expected to be fully supported in the application.</param>
public record Language(string TwoLetterCode, bool IsFullySupported)
{
    public bool IsFallback { get; init; }
    public string? CultureCode { get; init; }
    public bool CapitalizesNouns { get; init; } = false;
    public CultureInfo CultureInfo => CultureInfo.CreateSpecificCulture(ToString()) ?? CultureInfo.CurrentUICulture;
    public override string ToString() => CultureCode is null ? TwoLetterCode : $"{TwoLetterCode}-{CultureCode}";
}

public static class LanguageExtensions
{
    extension(IEnumerable<Language>? languages)
    {
        public bool IsOk =>
            languages is not null &&
            languages.Any() &&
            (languages.GroupBy(l => l.TwoLetterCode).Count() == languages.Count() &&
            languages.Count(l => l.IsFallback) == 1);
    }
}
