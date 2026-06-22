using System.Globalization;
using System.Reflection;

namespace Tellurian.Localization.Implementatioms;

internal class ObjectResourceProvider : IResourceProvider, ISynchronousResourceProvider
{
    public const string Key = "Object";
    public string Name => Key;
    /// <summary>
    /// Get property values from in.memory object.
    /// </summary>
    /// <param name="objectWithLanguageProperties">An object with property names that matches <see cref="CultureInfo.TwoLetterISOLanguageName"/></param>
    /// <param name="cultureInfo">The property name as <see cref="CultureInfo.TwoLetterISOLanguageName"/></param>
    /// <returns>String value of the two letter property name correponding to the <see cref="CultureInfo.TwoLetterISOLanguageName"/></returns>
    public TextContent GetTranslation(object objectWithLanguageProperties, CultureInfo? cultureInfo = null)
    {
        var culture = cultureInfo ?? CultureInfo.CurrentUICulture;
        // Case-insensitive so callers can use conventional C# property names (EN, SV, ...).
        // CultureInfo.TwoLetterISOLanguageName is always lowercase, so an exact-case lookup
        // would miss PascalCase/uppercase properties.
        var property = objectWithLanguageProperties.GetType().GetProperty(
            culture.TwoLetterISOLanguageName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property is null) return objectWithLanguageProperties.ToEmptyTextContent();
        return property.GetValue(objectWithLanguageProperties) is not string value ?
            objectWithLanguageProperties.ToEmptyTextContent() :
            value.ToTextContent(".obj");
    }

    public Task<TextContent> GetTranslationAsync(object objectWithLanguageProperties, CultureInfo cultureInfo) =>
        Task.FromResult(GetTranslation(objectWithLanguageProperties, cultureInfo));
}
