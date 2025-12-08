using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization;

public class Settings
{
    /// <summary>
    /// At lease one <see cref="Language"/> must be provided, and only one language may be a fallback language.
    /// </summary>
    public IEnumerable<Language> Languages { get; set; } = [];
    /// <summary>
    /// The type names of the RESX files. May be empty if RESX-files are not used.
    /// </summary>
    public IEnumerable<string> ResxTypeNames { get; set; } = [];
    /// <summary>
    /// Path to were markdown files are located. If null or empty, the <see cref="MarkdownResourceProvider"/>
    /// </summary>
    public string? MarkdownFilesBasePath { get; set; }
}
