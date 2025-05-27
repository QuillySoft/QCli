using System.Reflection;
using Tools.Cli.Utils;

namespace Tools.Cli;

public sealed class Paths
{
    public static string RepositoryRootPath { get; }

    public static string ApiAppDirPath => Path.Combine(RepositoryRootPath, "src", "Apps", "Api");

    public static string WebAppDirPath => Path.Combine(RepositoryRootPath, "src", "Apps", "ClientApp");

    public static string ClioDbContextLibDirPath => Path.Combine(RepositoryRootPath, "src", "Infra", "Persistence");

    static Paths()
    {
        RepositoryRootPath = GetRepositoryRootPath();
    }

    public static string GetRepositoryRootPath()
    {
        try
        {
            var result = Shell.RunAndGetOutput("git", "rev-parse --show-toplevel");
            if (result.exitCode == 0)
            {
                var path = result.output.Trim();
                if (path != string.Empty && Directory.Exists(path))
                {
                    return path;
                }
            }
        }
        catch (Exception)
        {
            // Silently continue with alternative repository detection
        }

        // Walk up the dir tree starting with current directory, looking for the repo root along the way.
        var repoRoot = new DirectoryInfo(Environment.CurrentDirectory);
        while (repoRoot.Parent != null && !Directory.Exists(Path.Combine(repoRoot.FullName, ".git")))
        {
            repoRoot = repoRoot.Parent;
        }

        // Walk up the dir tree starting with directory where this executable is located, looking for the repo root along the way.
        if (Directory.Exists(Path.Combine(repoRoot.FullName, ".git")))
        {
            return repoRoot.FullName;
        }

        repoRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                     ?? throw new Exception("Could not find executing assembly location"));
        while (repoRoot.Parent != null && !Directory.Exists(Path.Combine(repoRoot.FullName, ".git")))
        {
            repoRoot = repoRoot.Parent;
        }

        if (Directory.Exists(Path.Combine(repoRoot.FullName, ".git")))
        {
            return repoRoot.FullName;
        }

        throw new Exception("Cannot find repository root folder in current directory or in any parent up the directory tree.");
    }
}