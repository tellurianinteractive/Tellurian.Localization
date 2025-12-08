using System.Globalization;

namespace Tellurian.Localization.Implementatioms;

internal class ObjectResourceProvider : IResourceProvider
{
    public const string Key = "Object";
    public string Name => Key;
    /// <summary>
    /// Get property values from in.memory object.
    /// </summary>
    /// <param name="objectWithLanguageProperties">An object with property names that matches <see cref="CultureInfo.TwoLetterISOLanguageName"/></param>
    /// <param name="cultureInfo">The property name as <see cref="CultureInfo.TwoLetterISOLanguageName"/></param>
    /// <returns>String value of the two letter property name correponding to the <see cref="CultureInfo.TwoLetterISOLanguageName"/></returns>
    public Task<TextContent> GetTranslationAsync(object objectWithLanguageProperties, CultureInfo cultureInfo)
    {
        var property = objectWithLanguageProperties.GetType().GetProperty(cultureInfo.TwoLetterISOLanguageName);
        if (property is null) return objectWithLanguageProperties.ToEmptyResult();
        return property.GetValue(objectWithLanguageProperties) is not string value ?
            objectWithLanguageProperties.ToEmptyResult() :
            value.ToResult(".obj");
    }
}
