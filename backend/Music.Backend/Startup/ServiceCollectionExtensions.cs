namespace Music.Backend.Startup;

public static class ServiceCollectionExtensions
{
    public static void AddDelegate<T>(this IServiceCollection serviceCollection, Delegate @delegate, ServiceLifetime lifetime)
        where T : Delegate =>
        serviceCollection.Add(new ServiceDescriptor(
            typeof(T),
            sp => @delegate.DynamicInvoke(GetDelegateParameterTypes(sp, @delegate)) as T ?? throw new InvalidOperationException(),
            lifetime));

    private static object[] GetDelegateParameterTypes(IServiceProvider serviceProvider, Delegate @delegate) =>
        @delegate.Method.GetParameters()
            .Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType))
            .ToArray();
}
