using System.Globalization;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization
{
    public interface IResourceProvider
    {
        string Name { get; }
        Task<TextContent> GetTranslationAsync(object resourceKey, CultureInfo cultureInfo);
    }
}
