using System.IO;
using System.Reflection;

namespace Music.CDK;

public static class Utils
{
    private static Assembly Assembly => Assembly.GetExecutingAssembly();
    private static string AssemblyName => Assembly.GetName().Name;
    public static string GetResourceName(string fileName) => $"{AssemblyName}.Resources.{fileName}";


    public static string ReadCodeFromEmbeddedResource(string fileName)
    {
        using var stream = Assembly.GetManifestResourceStream(GetResourceName(fileName));

        if (stream is null)
            throw new FileNotFoundException($"{nameof(fileName)}: Embedded resource '{fileName}' not found.");

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}
