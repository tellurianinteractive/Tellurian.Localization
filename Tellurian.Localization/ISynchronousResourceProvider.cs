using System.Globalization;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization;

/// <summary>
/// Implemented by resource providers whose lookups are genuinely synchronous and in-memory
/// (for example the resx and object providers). This lets callers translate without paying for,
/// or blocking on, the async API. Providers that perform real I/O — file or HTTP backed — do
/// <em>not</em> implement this, so they cannot be invoked synchronously by mistake.
/// </summary>
public interface ISynchronousResourceProvider
{
    /// <summary>
    /// Synchronously resolves the translation for <paramref name="resourceKey"/> in
    /// <paramref name="cultureInfo"/> (defaulting to <see cref="CultureInfo.CurrentUICulture"/>),
    /// falling back to the key itself when no translation exists.
    /// </summary>
    TextContent GetTranslation(object resourceKey, CultureInfo? cultureInfo = null);
}
