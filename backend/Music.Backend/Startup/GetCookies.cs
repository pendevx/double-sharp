using System.Diagnostics;

namespace Music.Backend.Startup;

public static class GetCookies
{
    public static void Run()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "node", // Ensure 'node' is in your PATH
            Arguments = Path.Combine(Environment.CurrentDirectory, "_scripts/get-cookies"), // Path to the script
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.Environment["DOUBLESHARP_EMAIL"] = Environment.GetEnvironmentVariable("DOUBLESHARP_EMAIL");
        startInfo.Environment["DOUBLESHARP_PASSWORD"] = Environment.GetEnvironmentVariable("DOUBLESHARP_PASSWORD");

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();
    }
}
