using Music.Backend.Global.Impl;
using Music.CommandHandlers;
using Music.Global.Contracts;
using Music.QueryHandlers;
using Music.Repositories;
using Music.Repositories.Contracts;

namespace Music.Backend;

public static class DependencyInjectionConfiguration
{
    private static void LoadAssemblies()
    {
        CommandHandlers.Load.Initialize();
        QueryHandlers.Load.Initialize();
        Services.Load.Initialize();
    }

    private static IEnumerable<Type> ExtractConcreteHandlers(Type[] extractFrom, Type[] baseCommandTypes)
    {
        return extractFrom
            .Where(p => p.GetInterfaces()
                .Any(i => baseCommandTypes.Contains(i.IsGenericType ? i.GetGenericTypeDefinition() : i)));
    }

    private static void RegisterCommandHandlers(WebApplicationBuilder builder, Type[] extractFrom)
    {
        var commandHandlers = ExtractConcreteHandlers(extractFrom, [ typeof(IBaseCommandHandler), typeof(IBaseCommandHandler<>), typeof(IBaseCommandHandler<,>) ]);
        foreach (var handler in commandHandlers)
            builder.Services.AddScoped(handler);
    }

    private static void RegisterQueryHandlers(WebApplicationBuilder builder, Type[] extractFrom)
    {
        var queryHandlers = ExtractConcreteHandlers(extractFrom, [ typeof(IBaseQueryHandler<>), typeof(IBaseQueryHandler<,>) ]);
        foreach (var handler in queryHandlers)
            builder.Services.AddScoped(handler);
    }

    private static void RegisterServices(WebApplicationBuilder builder, Type[] extractFrom)
    {
        var services = extractFrom.Where(s => s.Namespace is not null &&
                                              s.Namespace.StartsWith("Music.Services") &&
                                              s is { IsAbstract: false, IsSealed: false });
        foreach (var service in services)
            builder.Services.AddScoped(service);
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        LoadAssemblies();

        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ISessionRepository, SessionRepository>();

        builder.Services.AddScoped<IAuthContext, WebAuthContext>();
        builder.Services.AddHttpContextAccessor();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.StartsWith("Music.") ?? false);

        var types = assemblies.SelectMany(s => s.GetTypes()).ToArray();

        RegisterCommandHandlers(builder, types);
        RegisterQueryHandlers(builder, types);
        RegisterServices(builder, types);

        return builder;
    }
}
