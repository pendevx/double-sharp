using System.IO;
using System.Reflection;

namespace Music.CDK;

public static class Utils
{
    public static string ReadCodeFromEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyName = assembly.GetName().Name;

        using var stream = assembly.GetManifestResourceStream($"{assemblyName}.Resources.{fileName}");

        if (stream is null)
            throw new FileNotFoundException($"{nameof(fileName)}: Embedded resource '{fileName}' not found.");

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}
