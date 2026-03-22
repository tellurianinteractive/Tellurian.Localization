using System.Globalization;

namespace Tellurian.Localization.Implementatioms;

/// <summary>
/// Fetches localised markdown content via <see cref="HttpClient"/>,
/// suitable for environments without file system access such as Blazor WebAssembly.
/// Registered with the same key as <see cref="MarkdownResourceProvider"/>
/// so consumers can inject either variant transparently.
/// </summary>
public class HttpMarkdownResourceProvider(HttpClient http, string basePath) : IResourceProvider
{
    public string Name => MarkdownResourceProvider.Key;

    public async Task<TextContent> GetTranslationAsync(object resourceKey, CultureInfo cultureInfo)
    {
        const string fileSuffix = ".md";
        if (resourceKey is not string key) return resourceKey.ToEmptyTextContent();

        var contentPath = key.ContentPath;

        // Try culture-specific file first, then fall back to default.
        var text = await TryFetchAsync($"{basePath}/{contentPath}.{cultureInfo.TwoLetterISOLanguageName}{fileSuffix}");
        if (text is not null) return new TextContent(text, fileSuffix);

        text = await TryFetchAsync($"{basePath}/{contentPath}{fileSuffix}");
        if (text is not null) return new TextContent(text, fileSuffix);

        return key.ToTextContent(fileSuffix);
    }

    private async Task<string?> TryFetchAsync(string path)
    {
        try
        {
            var response = await http.GetAsync(path);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }
        catch
        {
            return null;
        }
    }
}
