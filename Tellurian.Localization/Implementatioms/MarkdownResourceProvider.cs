using System.Globalization;

namespace Tellurian.Localization.Implementatioms;

public class MarkdownResourceProvider(string basePath) : IResourceProvider
{
    private readonly string _basePath = basePath;

    public const string Key = "Markdown";
    public string Name => Key;
    public async Task<TextContent> GetTranslationAsync(object resourceKey, CultureInfo cultureInfo)
    {
        const string FileSuffix = ".md";
        var stringKey = resourceKey as string;
        if (stringKey is null) return stringKey.ToTextContent();
        var contentPath = stringKey.ContentPath;
        var path = Path.Combine(_basePath, contentPath);

        var specificCultureFile = new FileInfo($"{path}.{cultureInfo.TwoLetterISOLanguageName}{FileSuffix}");
        if (specificCultureFile.Exists)
            return new TextContent(await File.ReadAllTextAsync(specificCultureFile.FullName), FileSuffix, specificCultureFile.LastWriteTimeUtc);
        var defaultCultureFile = new FileInfo($"{path}{FileSuffix}");
        if (defaultCultureFile.Exists)
            return new TextContent(await File.ReadAllTextAsync(defaultCultureFile.FullName), FileSuffix, defaultCultureFile.LastWriteTimeUtc);
        return stringKey.ToTextContent();
    }
}
