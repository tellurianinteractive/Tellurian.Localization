using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tellurian.Localization;
using Tellurian.Localization.DependencyInjection;
using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        extension(IServiceCollection serviceCollection)
        {
            /// <summary>
            /// Adds all build in <see cref="IResourceProvider"/> services and the <see cref="ILanguageService"/>
            /// </summary>
            /// <param name="settings">for the dfferent providers.</param>
            public IServiceCollection AddTellurianLocalization(IOptions<Settings> options)
            {
                var settings = options.Value;
                serviceCollection.AddLanguageService(settings.Languages);
                var types = settings.ResxTypeNames
                    .Select(typeName => Type.GetType(typeName, false, true))
                    .OfType<Type>();
                if (types.Any()) serviceCollection.AddResxResourceProviders(types);
                if (settings.MarkdownFilesBasePath.HasValue) serviceCollection.AddMarkdownResourceProvider(settings.MarkdownFilesBasePath!);
                serviceCollection.AddObjectResourceProvider();
                return serviceCollection;
            }

            /// <summary>
            /// Adds the <see cref="LanguageService"/> to the <see cref="IServiceCollection"/>.
            /// </summary>
            public IServiceCollection AddLanguageService(IEnumerable<Language> languages) =>
                serviceCollection.AddSingleton<ILanguageService, LanguageService>((provider) => new LanguageService(languages));

            /// <summary>
            /// Add one <see cref="ResxResourceProvider"/> per resource type and adds the <see cref="ResxResourceProviders"/> to the <see cref="IServiceCollection"/>.
            /// </summary>
            /// <param name="type"></param>
            public IServiceCollection AddResxResourceProviders(IEnumerable<Type> types) =>
                serviceCollection.AddKeyedSingleton<IResourceProviderGroup, ResxResourceProviders>(
                    ResxResourceProviders.Key, (provider, key) => new ResxResourceProviders(types));

            /// <summary>
            /// Adds the <see cref="MarkdownResourceProvider"/> to the <see cref="IServiceCollection"/>.
            /// </summary>
            /// <param name="rootPath">Root path of markdown content realtive app root.</param>
            public IServiceCollection AddMarkdownResourceProvider(string rootPath) =>
                serviceCollection.AddKeyedSingleton<IResourceProvider, MarkdownResourceProvider>(
                    MarkdownResourceProvider.Key, (provider, key) => new MarkdownResourceProvider(rootPath));

            /// <summary>
            /// adds the <see cref="ObjectResourceProvider"/> to the <see cref="IServiceCollection"/>.
            /// </summary>
            public IServiceCollection AddObjectResourceProvider() =>
                serviceCollection.AddKeyedSingleton<IResourceProvider, ObjectResourceProvider>(
                    ObjectResourceProvider.Key, (provider, key) => new ObjectResourceProvider());
        }
    }

}
