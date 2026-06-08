using System.Globalization;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization
{
    public interface IResourceProviderGroup
    {
        string Name { get; }

        ResxResourceProvider? Provider<T>();
        Task<TextContent> Translated<T>(string resourceKey, CultureInfo? cultureInfo = null);
    }
}
