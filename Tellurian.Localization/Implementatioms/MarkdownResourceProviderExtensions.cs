namespace Tellurian.Localization.Implementatioms;

internal static class MarkdownResourceProviderExtensions
{
    extension(string resourceKey)
    {
        internal string ContentPath => resourceKey?.Replace('-', '/') ?? string.Empty;
    }
}
