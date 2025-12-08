namespace Tellurian.Localization.Implementatioms;

public static class TextContentExtensions
{
    extension(object resourceKey)
    {
        internal TextContent ToEmptyTextContent() =>
            new(resourceKey as string ?? string.Empty, string.Empty);

        internal Task<TextContent> ToEmptyResult()
            => Task.FromResult(resourceKey.ToEmptyTextContent());

    }
    extension(string text)
    {
        internal TextContent ToTextContent(string? fileSuffix = null) =>
            new(text, fileSuffix.ToValue);
        internal Task<TextContent> ToResult(string? fileSuffix = null) =>
            Task.FromResult(text.ToTextContent(fileSuffix.ToValue));
    }

    extension(string? fileSuffix)
    {
        private string ToValue => string.IsNullOrWhiteSpace(fileSuffix) ? string.Empty : fileSuffix;
    }

    extension(TextContent textContent)
    {
        internal Task<TextContent> ToResult() => Task.FromResult(textContent);
        public bool IsEmpty => string.IsNullOrWhiteSpace(textContent.FileSuffix);
    }
}
