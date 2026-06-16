using System.Globalization;
using System.Resources;

namespace Tellurian.Localization.Implementatioms;

public class ResxResourceProvider(Type type) : IResourceProvider, ISynchronousResourceProvider
{
    const string FileSuffix = ".resx";
    public Type ResourceType { get; } = type;
    private readonly ResourceManager _resourceManager = new(type);
    public string Name => ResourceType.Name;

    public TextContent GetTranslation(object resourceKey, CultureInfo? cultureInfo = null)
    {
        var stringKey = (string)resourceKey;
        if (stringKey is null) return stringKey.ToTextContent();
        var translated = _resourceManager.GetString(stringKey, cultureInfo ?? CultureInfo.CurrentUICulture);
        if (translated is null) return stringKey.ToTextContent();
        return translated.ToTextContent(FileSuffix);
    }

    public Task<TextContent> GetTranslationAsync(object resourceKey, CultureInfo? cultureInfo = null) =>
        Task.FromResult(GetTranslation(resourceKey, cultureInfo));
}
