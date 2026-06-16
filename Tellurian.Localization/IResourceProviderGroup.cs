using System.Globalization;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization
{
    public interface IResourceProviderGroup
    {
        string Name { get; }

        ResxResourceProvider? Provider<T>();

        /// <summary>
        /// Synchronously resolves the translation for <paramref name="resourceKey"/> from the resx
        /// resources of type <typeparamref name="T"/>, falling back to the key when not found.
        /// </summary>
        TextContent Translated<T>(string resourceKey, CultureInfo? cultureInfo = null);

        /// <summary>
        /// Asynchronously resolves the translation for <paramref name="resourceKey"/> from the resx
        /// resources of type <typeparamref name="T"/>, falling back to the key when not found.
        /// </summary>
        Task<TextContent> TranslatedAsync<T>(string resourceKey, CultureInfo? cultureInfo = null);
    }
}
