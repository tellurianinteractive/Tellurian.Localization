using System.Globalization;
using Tellurian.Localization;

namespace Tellurian.Localization.Implementatioms;

/// <summary>
/// Provides the application with information about supported languages.
/// </summary>
/// <param name="languages">Exactly one language is assumed to be the fallback (marked with <see cref="Language.IsFallback"/>).</param>
public sealed class LanguageService(IEnumerable<Language>? languages = null) : ILanguageService
{
    private readonly IList<Language> _languages = InitLanguages(languages);

    private static IList<Language> InitLanguages(IEnumerable<Language>? languages)
    {
        var list = new List<Language>(languages ?? []);
        if (list.Count > 0 && !list.Any(l => l.IsFallback))
        {
            list[0] = list[0] with { IsFallback = true };
        }
        return list;
    }
    public IList<Language> GetSupportedLanguages() => _languages;
    /// <summary>
    /// Adds a <see cref="Language"/> to the service's collection of languages. 
    /// </summary>
    /// <param name="language">Language to add.</param>
    /// <remarks>
    /// The language marked with <see cref="Language.IsFallback"/> (or the first in the collection) is the fallback.
    /// </remarks>
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
