using Tellurian.Localization.Implementatioms;

namespace Tellurian.Localization.DependencyInjection
{
    public static class ServiceProviderExtensions
    {

        extension(IServiceProvider serviceProvider)
        {
            private IServiceProvider Add(IResourceProvider resourceProvider)
            {
                serviceProvider.Add(resourceProvider);
                return serviceProvider;
            }
            /// <summary>
            /// Adds a <see cref="ResxResourceProvider"/> to the <see cref="IServiceProvider"/>
            /// </summary>
            /// <param name="type">The type of resource to retrieve translations from.</param>
            /// <remarks>
            /// You must add a <see cref="ResxResourceProvider"/> for each resource type.
            /// </remarks>
            public IServiceProvider AddResxResourceProviderFor(Type type) =>
                serviceProvider.Add(new ResxResourceProvider(type));
        }
    }

}
