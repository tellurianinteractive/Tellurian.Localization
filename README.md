# Tellurian.Localization
This library is helpful for .NET developers that creates
applications intended for an international market.

## Objectives
This library is developed to support the follwing scenarios:
- A unified model for retriving translations from a varity of
  language resource types.
- A centralised language translation service. It is posible to have all resources 
  in one project, also accessible from GUI and other usage.
- Provide consistent ways in code to retrieve translations.
- Easy to add new language translations, eventually with help from AI.
- Full control over translations, which is specially important
  in applications targeting special domain areas where terminology
  is important.

## Cross-platform Considerations
When devloping cross-platform applications, it is important
that localisation behaves consistent on each platform.

.NET 10 uses the [International Components for Unicode (ICU)](https://icu.unicode.org/)
which is supportet on both Windows and Linux including macOS.
This ensure consistent behaviour. Minor differences may occur depending 
on what version of ICU that is installed on the machine the app runs.

## Invariant Globalisation
In order for applications to use localized resources, *invariant globalisation*
must be turned off. This can be declared in the project file. 
If omitted,the default behaviour is that *invariant globaisation* is *off*,
so you don't need to declare it explicit as in the example below.
```xml
<PropertyGroup>
    <InvariantGlobalization>false</InvariantGlobalization>
</PropertyGroup>
```

## Resource Providers
This library has an extensible model for adding new sources of language resources,
*Resource Providers*. This library implements three *resource providers*;
- **ResxResourceProvider** for getting resources from .NET RESX-files.
  This provider uses standard .NET mechanism - the *Resource Manager* class.
- **MarkdownResourceProvider** gets resources in form of markdown files with 
  a naming convenstion *resourcename*.*language/culture*.md. Markdown files
  should be structure in a base-folder, but can contain sub-folders. 
- **ObjectResourceProvider** uses reflection to find string properties that
  matches the language, and returns the text of the property. This is 
  useful for example when you load an object from a database with columns
  for each supported language.

You can add new providers, for example getting trainslations online
or in other file formats.

### Fallback Behaviour
You must define a default language. This will be the fallback if a 
translation for the specific language is not found.

When seaching for a translation of a specific *resource key*, 
the search uses a fallback mechanism:
1. Use specific language with culture, example **sv-SE**.
2. Else use language, example **sv**.
3. Else use default language with culture, example **en-GB**.
4. Else use language, example **en**.
5. Else if no translation is found, return *resource key* to
   indicate that a translation is missing.

### Supported Languages
.NET Localization needs to know what languages the application
support. This is configured using the `Language` record:

```csharp
public record Language(string TwoLetterCode, bool IsFullySupported)
{
    public bool IsFallback { get; init; }
    public string? CultureCode { get; init; }
    public bool CapitalizesNouns { get; init; } = false;
}
```

**Properties:**
- **TwoLetterCode** - The [ISO 639-1](https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes) two letter language code, e.g. `en`, `sv`, `de`.
- **IsFullySupported** - Indicates if the language has complete translations.
- **IsFallback** - Marks the default language. If no language is marked, the first one in the list is used as the fallback.
- **CultureCode** - Optional culture specifier, e.g. `GB` for British English (`en-GB`).
- **CapitalizesNouns** - Indicates if the language capitalizes nouns (e.g. German).

**Example:**
```csharp
var languages = new List<Language>
{
    new("en", true) { IsFallback = true, CultureCode = "GB" },  // British English (default)
    new("sv", true) { CultureCode = "SE" },                      // Swedish
    new("de", false) { CapitalizesNouns = true },                // German (partial support)
};
```

## Code Structure

### Core Interfaces

| Interface | Description |
|-----------|-------------|
| `ILanguageService` | Provides information about supported languages and the fallback language. |
| `IResourceProvider` | Retrieves translations from a specific source (RESX, Markdown, Object). |
| `ISynchronousResourceProvider` | Implemented by providers whose lookups are genuinely in-memory (RESX, Object), exposing a synchronous `GetTranslation`. File/HTTP-backed providers do *not* implement it, so they can't be called synchronously by mistake. |
| `IResourceProviderGroup` | Manages multiple resource providers of the same type. |

### Key Classes

| Class | Description |
|-------|-------------|
| `Language` | Record representing a supported language with its properties. |
| `LanguageService` | Implementation of `ILanguageService`. |
| `TextContent` | Record containing the translated text, file suffix, and last modified timestamp. |
| `Settings` | Configuration class for dependency injection setup. |
| `ResxResourceProvider` | Provides translations from .NET RESX files via `ResourceManager`. |
| `ResxResourceProviders` | Groups multiple RESX providers for different resource types. |
| `MarkdownResourceProvider` | Provides translations from markdown files. |
| `ObjectResourceProvider` | Provides translations from object properties using reflection. |

### TextContent
All resource providers return a `TextContent` record:
```csharp
public record TextContent(string Text, string FileSuffix, DateTimeOffset? LastModified = null);
```
- **Text** - The translated string.
- **FileSuffix** - Source format (`.resx`, `.md`, `.obj`).
- **LastModified** - Timestamp for cache invalidation (mainly used by Markdown provider).

## Dependency Injection Setup

### Configuration
The library uses `Settings` for configuration:
```csharp
public class Settings
{
    public IEnumerable<Language> Languages { get; set; } = [];
    public IEnumerable<string> ResxTypeNames { get; set; } = [];
    public string? MarkdownFilesBasePath { get; set; }
}
```

### Registration in Program.cs
```csharp
using Microsoft.Extensions.Options;
using Tellurian.Localization;
using Tellurian.Localization.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure settings
builder.Services.Configure<Settings>(options =>
{
    options.Languages = new List<Language>
    {
        new("en", true) { IsFallback = true, CultureCode = "GB" },
        new("sv", true) { CultureCode = "SE" },
        new("de", false)
    };
    // Type names for RESX resources (fully qualified)
    options.ResxTypeNames = new[]
    {
        "MyApp.Resources.Labels, MyApp",
        "MyApp.Resources.Messages, MyApp"
    };
    // Base path for markdown files
    options.MarkdownFilesBasePath = "Content/Translations";
});

// Register localization services
var options = builder.Services.BuildServiceProvider()
    .GetRequiredService<IOptions<Settings>>();
builder.Services.AddTellurianLocalization(options);
```

> **Blazor WebAssembly:** `AddTellurianLocalization` registers the file-based `MarkdownResourceProvider`,
> which has no file-system access in the browser. Register the providers individually instead and use
> `AddHttpMarkdownResourceProvider` for markdown (it fetches over HTTP relative to the app base address):
>
> ```csharp
> builder.Services.AddLanguageService(languages);
> builder.Services.AddResxResourceProviders([typeof(Labels)]);
> builder.Services.AddHttpMarkdownResourceProvider("Content");
> builder.Services.AddObjectResourceProvider();
> ```

### Retrieving Services
The providers are registered as **keyed singletons**:
```csharp
// Get language service
var languageService = serviceProvider.GetRequiredService<ILanguageService>();

// Get RESX providers group
var resxProviders = serviceProvider.GetRequiredKeyedService<IResourceProviderGroup>("Resx");

// Get Markdown provider
var markdownProvider = serviceProvider.GetRequiredKeyedService<IResourceProvider>("Markdown");

// Get Object provider
var objectProvider = serviceProvider.GetRequiredKeyedService<IResourceProvider>("Object");
```

## Usage Examples

### Using the Language Service
```csharp
public class MyService(ILanguageService languageService)
{
    public void ShowSupportedLanguages()
    {
        var languages = languageService.GetSupportedLanguages();

        foreach (var lang in languages)
        {
            Console.WriteLine($"{lang.TwoLetterCode}: Fully supported = {lang.IsFullySupported}");
        }
    }

    public bool IsLanguageSupported(CultureInfo culture)
    {
        return languageService.SupportsLanguage(culture);
    }
}
```

### Using RESX Resources
RESX files are compiled into satellite assemblies by .NET.
The `ResxResourceProvider` uses the standard `ResourceManager` to retrieve translations.

Because RESX (and Object) lookups are genuinely in-memory, `IResourceProviderGroup`
exposes both a **synchronous** `Translated<T>` and an **asynchronous** `TranslatedAsync<T>`.
Use the synchronous overload from synchronous contexts (e.g. Razor component markup)
to avoid sync-over-async; use the async overload where you are already in an async flow.

```csharp
public class TranslationService(
    [FromKeyedServices("Resx")] IResourceProviderGroup resxProviders)
{
    public string GetLabel(string key)
    {
        // Synchronous, uses CultureInfo.CurrentUICulture automatically
        var content = resxProviders.Translated<Labels>(key);
        return content.Text;
    }

    public async Task<string> GetLabelAsync(string key, CultureInfo culture)
    {
        // Asynchronous overload
        var content = await resxProviders.TranslatedAsync<Labels>(key, culture);
        return content.Text;
    }
}
```

### Using Markdown Resources
```csharp
public class ContentService(
    [FromKeyedServices("Markdown")] IResourceProvider markdownProvider)
{
    public async Task<string> GetPageContent(string resourceKey)
    {
        var content = await markdownProvider.GetTranslationAsync(
            resourceKey,
            CultureInfo.CurrentUICulture);
        return content.Text;
    }
}
```

### Using Object Resources
Useful for objects loaded from a database with language-specific columns:
```csharp
// Example: Entity with language properties
public class ProductDescription
{
    public int Id { get; set; }
    public string en { get; set; } = "";  // English description
    public string sv { get; set; } = "";  // Swedish description
    public string de { get; set; } = "";  // German description
}

public class ProductService(
    [FromKeyedServices("Object")] IResourceProvider objectProvider)
{
    public async Task<string> GetDescription(ProductDescription product)
    {
        // Returns the property value matching CurrentUICulture.TwoLetterISOLanguageName
        var content = await objectProvider.GetTranslationAsync(
            product,
            CultureInfo.CurrentUICulture);
        return content.Text;
    }
}
```

### Using Extension Methods
The library provides convenient extension methods:
```csharp
// Uses CultureInfo.CurrentUICulture automatically
var content = await resourceProvider.Translated("MyResourceKey");

// For objects with language properties
var content = await objectProvider.Translate(myDatabaseEntity);
```

## Markdown File Organization

### Naming Convention
Markdown files follow the pattern: `{resourcekey}.{language}.md`

**Examples:**
English is assumed to be the *neutral* language that is used 
as fallback if specific translation is missing.
- `welcome.md` - English welcome content
- `welcome.sv.md` - Swedish welcome content
- `about-us.md` - English "about us" content
- `about-us.sv.md` - Swedish "about us" content

### Path Mapping
Resource keys with hyphens are converted to directory paths:
- Key `help-faq-intro` maps to file path `help/faq/intro.{lang}.md`

**Example folder structure:**
```
Content/Translations/
├── welcome.md 
├── welcome.sv.md
├── welcome.de.md
├── about-us.md
├── about-us.sv.md
├── about-us.de.md
└── help/
    └── faq/
        ├── intro.md
        └── intro.sv.md
        └── intro.de.md
```

### Fallback Behavior
The Markdown provider searches in this order:
1. `{path}.{TwoLetterISOLanguageName}.md` (e.g., `welcome.sv.md`)
2. `{path}.md` (default file without language suffix)
3. Returns the resource key if no file is found