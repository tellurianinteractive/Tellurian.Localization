using System.Globalization;

namespace Tellurian.Localization;
/// <summary>
/// Provides the application with information about supported languages.
/// </summary>
/// <remarks>
/// The language marked with <see cref="Language.IsFallback"/> (or the first in the collection) is the fallback.
/// For RESX resources the effective fallback is the assembly's neutral language
/// (<see cref="System.Resources.NeutralResourcesLanguageAttribute"/>), resolved by the .NET <c>ResourceManager</c>.
/// </remarks>
public interface ILanguageService
{
    IList<Language> GetSupportedLanguages();
    bool SupportsLanguage(CultureInfo cultureInfo);
    Language? GetLanguage(CultureInfo cultureInfo);
}
