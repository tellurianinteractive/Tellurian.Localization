using System.Globalization;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization
{
    public static class IResourceProviderExtensions
    {
        extension(IResourceProvider resourceProvider)
        {
            public Task<TextContent> Translated(string resourceKey) =>
                resourceProvider.GetTranslationAsync(resourceKey, CultureInfo.CurrentUICulture);

            public Task<TextContent> Translate<T>(T objectWithTwoLetterLanguageProperties)
            {
                if (objectWithTwoLetterLanguageProperties is null) return "null".ToEmptyResult();
                return resourceProvider.GetTranslationAsync(objectWithTwoLetterLanguageProperties, CultureInfo.CurrentUICulture);
            }
        }
    }
}
