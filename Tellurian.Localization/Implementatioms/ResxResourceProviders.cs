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

    public ResxResourceProvider? Provider<T>() where T : ResxResourceProvider
    {
        var provider = GetProvider(typeof(T));
        if (provider is null) return null;
        return provider;
    }

    public async Task<TextContent> Translated<T>(string resourceKey, CultureInfo? cultureInfo = null) where T : ResxResourceProvider
    {
        const string FileSuffix = ".resx";
        var provider = Provider<T>();
        if (provider is null) return resourceKey.ToTextContent(FileSuffix);
        return await provider.GetTranslationAsync(resourceKey, cultureInfo);
    }
}
