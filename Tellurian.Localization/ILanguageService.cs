using System.Globalization;

namespace Tellurian.Localization;
/// <summary>
/// Provides the application with information about supported languages.
/// </summary>
/// <remarks>
/// Fallback is handled by .NET itself: providers resolve translations from <see cref="CultureInfo.CurrentUICulture"/>,
/// and the framework walks the culture's parent chain down to the assembly's neutral language
/// (<see cref="System.Resources.NeutralResourcesLanguageAttribute"/>). There is no explicit fallback language to mark.
/// </remarks>
public interface ILanguageService
{
    IList<Language> GetSupportedLanguages();
    bool SupportsLanguage(CultureInfo cultureInfo);
    Language? GetLanguage(CultureInfo cultureInfo);
}
