using System.Globalization;
using System.Resources;

namespace Tellurian.Localization.Implementatioms;

public class ResxResourceProvider(Type type) : IResourceProvider
{
    const string FileSuffix = ".resx";
    public Type ResourceType { get; } = type;
    private readonly ResourceManager _resourceManager = new(type);
    public string Name => ResourceType.Name;
    public Task<TextContent> GetTranslationAsync(object resourceKey, CultureInfo? cultureInfo = null)
    {
        var stringKey = (string)resourceKey;
        if (stringKey is null) return stringKey.ToResult();
        var translated = _resourceManager.GetString(stringKey, cultureInfo ?? CultureInfo.CurrentUICulture);
        if (translated is null) return stringKey.ToResult();
        return translated.ToResult(FileSuffix);
    }
}
