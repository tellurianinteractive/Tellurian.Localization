# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Tellurian.Localization is a .NET library for internationalization (i18n) and localization (l10n). It provides a unified model for retrieving translations from multiple resource types: RESX files, Markdown files, and object properties.

## Build Commands

```bash
# Build the project
dotnet build

# Build release configuration
dotnet build -c Release

# Create NuGet package (automatically done on build)
dotnet pack
```

## Project Structure

- **Tellurian.Localization/** - Main library project
  - Core interfaces: `ILanguageService`, `IResourceProvider`, `IResourceProviderGroup`
  - `DependencyInjection/` - Service registration extensions
  - `Implementatioms/` - Resource provider implementations (ResxResourceProvider, MarkdownResourceProvider, ObjectResourceProvider)

## Architecture

### Resource Provider Pattern
The library uses an extensible provider pattern. All providers implement `IResourceProvider` and return `TextContent` records containing the translated text, source type, and optional timestamp.

In-memory providers (RESX, Object) additionally implement `ISynchronousResourceProvider`, exposing a synchronous `GetTranslation`. File/HTTP-backed providers (Markdown, HttpMarkdown) deliberately do not, so they cannot be called synchronously. On `IResourceProviderGroup` the synchronous lookup is `Translated<T>` and the async one is `TranslatedAsync<T>`.

Three built-in providers:
1. **ResxResourceProvider** - Uses .NET ResourceManager for compiled RESX files
2. **MarkdownResourceProvider** - Reads markdown files with naming convention `{key}.{lang}.md`
3. **ObjectResourceProvider** - Uses reflection to find properties matching language codes (useful for database entities with language columns)

### Dependency Injection
Services are registered as keyed singletons:
- `ILanguageService` - Standard singleton
- `IResourceProviderGroup` (key: "Resx") - For RESX providers
- `IResourceProvider` (key: "Markdown") - For Markdown provider
- `IResourceProvider` (key: "Object") - For Object provider

### Fallback Mechanism
There is no single library-wide fallback chain — each provider falls back independently:

- **ResxResourceProvider** delegates to the .NET `ResourceManager`, which probes the culture
  hierarchy (`sv-SE` → `sv` → neutral). The neutral/fallback resources are the ones compiled
  into the main assembly, governed by `<NeutralLanguage>` (`NeutralResourcesLanguageAttribute`),
  set to `en-GB` here. Returns the resource key if nothing is found.
- **MarkdownResourceProvider** tries `{key}.{lang}.md`, then the suffixless `{key}.md`,
  then returns the resource key.
- **ObjectResourceProvider** reads the property matching `CultureInfo.TwoLetterISOLanguageName`,
  returning an empty result if there is no such property.

`Language.IsFallback` marks the intended default language for callers' own use; the providers
above do not consult it.

### Markdown File Organization
- Naming: `{resourcekey}.{language}.md` (e.g., `welcome.sv.md`)
- Hyphens in keys map to directories: `help-faq-intro` → `help/faq/intro.{lang}.md`
- Files without language suffix are used as fallback

## Configuration

Uses Microsoft.Extensions.Options with `Settings` class:
- `Languages` - List of supported `Language` records
- `ResxTypeNames` - Fully qualified type names for RESX resources
- `MarkdownFilesBasePath` - Base path for markdown translation files

## Known Issues

- Directory name typo: `Implementatioms` (should be `Implementations`)

## CI/CD

GitHub Actions workflow (`.github/workflows/build-and-publish.yml`):
- Triggers on push to master
- Builds the project in Release mode
- Publishes to nuget.org only when version in `Directory.Build.props` is bumped
- Version detection uses git tags (creates `v{version}` tag after publish)

To release a new version:
1. Update `<Version>` in `Directory.Build.props`
2. Push to master
3. Workflow automatically publishes if tag doesn't exist

## Target Framework

- .NET 10.0
- Nullable reference types enabled
- Implicit usings enabled
