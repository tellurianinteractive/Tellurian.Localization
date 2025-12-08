using System.Globalization;

namespace Tellurian.Localization;
/// <summary>
/// Provides the application with information about supported languages.
/// </summary>
/// <remarks>
/// The first language in the collection is assumed to be the <see cref="FallbackLangauge"/>.
/// </remarks>
public interface ILanguageService
{
    IList<Language> GetSupportedLanguages();
    Language FallbackLangauge { get; }
    bool SupportsLanguage(CultureInfo cultureInfo);
    Language? GetLanguage(CultureInfo cultureInfo);
}
