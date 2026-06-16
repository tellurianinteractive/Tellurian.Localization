using System.Globalization;
using Tellurian.Localization;

namespace Tellurian.Localization.Implementatioms;

/// <summary>
/// Provides the application with information about supported languages.
/// </summary>
public sealed class LanguageService(IEnumerable<Language>? languages = null) : ILanguageService
{
    private readonly IList<Language> _languages = new List<Language>(languages ?? []);

    public IList<Language> GetSupportedLanguages() => _languages;
    /// <summary>
    /// Adds a <see cref="Language"/> to the service's collection of languages.
    /// </summary>
    /// <param name="language">Language to add.</param>
    public void Add(Language language)
    {
        if (_languages.Contains(language)) return;
        _languages.Add(language);
    }

    public Language? GetLanguage(CultureInfo cultureInfo) =>
        _languages.SingleOrDefault(l => l.CultureInfo.Equals(cultureInfo));
    public bool SupportsLanguage(CultureInfo cultureInfo) =>
        _languages.Any(l => l.CultureInfo.Equals(cultureInfo));
}
