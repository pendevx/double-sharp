using Music.Global.Contracts;

namespace Music.Backend.Global.Impl;

public static class AppEnvironment
{
    public static GetEnvironment GetEnvironment(IWebHostEnvironment environment) =>
        () => environment.EnvironmentName;
}
