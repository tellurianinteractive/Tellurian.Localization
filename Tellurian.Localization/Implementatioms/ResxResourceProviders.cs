using System.Globalization;

namespace Tellurian.Localization.Implementatioms;

public class ResxResourceProviders : IResourceProviderGroup
{
    public const string Key = "Resx";
    public string Name => Key;

    private readonly Dictionary<Type, ResxResourceProvider> _providers = [];


    public ResxResourceProviders(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            _providers.Add(type, new ResxResourceProvider(type));
        }
    }

    internal ResxResourceProvider? GetProvider(Type type) =>
        _providers.TryGetValue(type, out ResxResourceProvider? value) ? value : null;

    public ResxResourceProvider? Provider<T>() => GetProvider(typeof(T));

    public TextContent Translated<T>(string resourceKey, CultureInfo? cultureInfo = null)
    {
        const string FileSuffix = ".resx";
        var provider = Provider<T>();
        if (provider is null) return resourceKey.ToTextContent(FileSuffix);
        return provider.GetTranslation(resourceKey, cultureInfo);
    }

    public async Task<TextContent> TranslatedAsync<T>(string resourceKey, CultureInfo? cultureInfo = null)
    {
        const string FileSuffix = ".resx";
        var provider = Provider<T>();
        if (provider is null) return resourceKey.ToTextContent(FileSuffix);
        return await provider.GetTranslationAsync(resourceKey, cultureInfo);
    }
}
