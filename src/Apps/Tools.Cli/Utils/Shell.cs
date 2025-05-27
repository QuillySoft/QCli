using System.Diagnostics;

namespace Cli.Utils;

public static class Shell
{
    public static int Execute(string command, string args)
    {
        var startInfo = new ProcessStartInfo(command)
        {
            Arguments = args,
            UseShellExecute = false,
        };

        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception($"Could not start process: {command} with args: {args}");
        }

        process.WaitForExit();

        return process.ExitCode;
    }

    public static (int exitCode, string output) RunAndGetOutput(string command, string args)
    {
        var startInfo = new ProcessStartInfo(command)
        {
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        };

        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception($"Could not start process: {command} with args: {args}");
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, output);
    }
}