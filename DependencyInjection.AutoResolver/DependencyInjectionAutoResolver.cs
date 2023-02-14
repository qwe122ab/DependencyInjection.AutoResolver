using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DependencyInjection.AutoResolver;

public static class DependencyInjectionAutoResolver
{
    private static readonly List<Type> Interfaces = new() {
        typeof(IResolveAsScoped),
        typeof(IResolveAsTransient),
        typeof(IResolveAsSingleton)
    };

    public static void AutoResolve(this IServiceCollection serviceCollection)
    {
        if (serviceCollection is null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        foreach (var implementationType in GetServiceTypes())
        {
            var interfaceTypes = implementationType.GetInterfaces();
            foreach (var item in Interfaces)
            {
                if (interfaceTypes.Any(c => c == item))
                {
                    var interfaceType = interfaceTypes.FirstOrDefault(c => c != typeof(IResolveAsSelf) && c != item);
                    if (interfaceTypes.Any(c => c == typeof(IResolveAsSelf)) || interfaceType is null)
                    {
                        interfaceType = implementationType;
                    }

                    var serviceLifetime = GetServiceLifetime(item);
                    var serviceDescriptor = new ServiceDescriptor(interfaceType, implementationType, serviceLifetime);

                    serviceCollection.Add(serviceDescriptor);
                }
            }
        }
    }

    private static ServiceLifetime GetServiceLifetime(Type type)
    {
        if (type == typeof(IResolveAsScoped))
        {
            return ServiceLifetime.Scoped;
        }

        if (type == typeof(IResolveAsSingleton))
        {
            return ServiceLifetime.Singleton;
        }

        if (type == typeof(IResolveAsTransient))
        {
            return ServiceLifetime.Transient;
        }

        throw new ArgumentOutOfRangeException(nameof(type));
    }

    private static IEnumerable<Type> GetServiceTypes()
    {
        var types = GetTypes().ToList();

        return types.Where(c => c.IsClass &&
            c.GetInterfaces().Length >= 1 &&
            c.GetInterfaces().Any(f => Interfaces.Contains(f)));
    }

    private static IEnumerable<Type> GetTypes()
    {
        var types = new List<Type>();
        
        var entryAssembly = Assembly.GetEntryAssembly();
        var entryAssemblyTypes = entryAssembly.GetTypes().ToList();

        var referencedAssembliesTypes = entryAssembly.GetReferencedAssemblies()
            .Select(c => Assembly.Load(c))
            .SelectMany(c => c.GetTypes());

        types.AddRange(entryAssemblyTypes);
        types.AddRange(referencedAssembliesTypes);

        return types;
    }
}