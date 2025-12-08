using System.Globalization;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization
{
    public interface IResourceProviderGroup
    {
        string Name { get; }

        ResxResourceProvider? Provider<T>() where T : ResxResourceProvider;
        Task<TextContent> Translated<T>(string resourceKey, CultureInfo? cultureInfo = null) where T : ResxResourceProvider;
    }
}
