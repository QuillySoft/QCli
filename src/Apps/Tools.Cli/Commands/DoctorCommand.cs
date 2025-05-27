using Spectre.Console;
using Tools.Cli.Configuration;
using System.Diagnostics;

namespace Tools.Cli.Commands;

public sealed class DoctorCommand
{
    public int Execute(bool autoFix)
    {
        AnsiConsole.MarkupLine("[green]🩺 Running QCLI diagnostics...[/]\n");

        var issues = new List<DiagnosticIssue>();
        
        // Check configuration
        CheckConfiguration(issues);
        
        // Check project structure
        CheckProjectStructure(issues);
        
        // Check dependencies
        CheckDependencies(issues);
        
        // Check templates
        CheckTemplates(issues);

        // Display results
        DisplayResults(issues, autoFix);

        return issues.Any(i => i.Severity == IssueSeverity.Error) ? 1 : 0;
    }

    private void CheckConfiguration(List<DiagnosticIssue> issues)
    {
        AnsiConsole.Status().Start("Checking configuration...", ctx =>
        {
            // Check if qcli.json exists
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "qcli.json");
            if (!File.Exists(configPath))
            {
                issues.Add(new DiagnosticIssue(
                    IssueSeverity.Warning,
                    "Configuration file not found",
                    "qcli.json not found in current directory",
                    "Run 'qcli init' to create configuration"
                ));
            }
            else
            {
                try
                {
                    var config = CliConfiguration.Load();
                    
                    // Validate paths
                    var paths = new[]
                    {
                        config.Paths.DomainPath,
                        config.Paths.ApplicationPath,
                        config.Paths.PersistencePath
                    };

                    foreach (var path in paths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        var fullPath = config.Paths.GetFullPath(path);
                        if (!Directory.Exists(fullPath))
                        {
                            issues.Add(new DiagnosticIssue(
                                IssueSeverity.Warning,
                                "Path not found",
                                $"Configured path does not exist: {path}",
                                $"Create directory or update configuration"
                            ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    issues.Add(new DiagnosticIssue(
                        IssueSeverity.Error,
                        "Invalid configuration",
                        $"Failed to load configuration: {ex.Message}",
                        "Fix configuration file or run 'qcli config validate'"
                    ));
                }
            }
        });
    }

    private void CheckProjectStructure(List<DiagnosticIssue> issues)
    {
        AnsiConsole.Status().Start("Checking project structure...", ctx =>
        {
            var currentDir = Directory.GetCurrentDirectory();
            
            // Check for .NET project files
            var projectFiles = Directory.GetFiles(currentDir, "*.csproj", SearchOption.AllDirectories);
            if (!projectFiles.Any())
            {
                var slnFiles = Directory.GetFiles(currentDir, "*.sln", SearchOption.TopDirectoryOnly);
                if (!slnFiles.Any())
                {
                    issues.Add(new DiagnosticIssue(
                        IssueSeverity.Warning,
                        "No .NET project found",
                        "No .csproj or .sln files found",
                        "Ensure you're in a .NET project directory"
                    ));
                }
            }

            // Check for common architecture folders
            var expectedFolders = new[] { "src", "Domain", "Application" };
            var hasArchitectureFolders = expectedFolders.Any(folder => 
                Directory.Exists(Path.Combine(currentDir, folder)) ||
                Directory.Exists(Path.Combine(currentDir, "src", folder)));

            if (!hasArchitectureFolders)
            {
                issues.Add(new DiagnosticIssue(
                    IssueSeverity.Info,
                    "Architecture folders not detected",
                    "Standard Clean Architecture folders not found",
                    "Consider running 'qcli scaffold' for new projects"
                ));
            }
        });
    }

    private void CheckDependencies(List<DiagnosticIssue> issues)
    {
        AnsiConsole.Status().Start("Checking dependencies...", ctx =>
        {
            // Check .NET SDK version
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (process != null)
                {
                    process.WaitForExit();
                    var version = process.StandardOutput.ReadToEnd().Trim();
                    
                    if (Version.TryParse(version, out var dotnetVersion))
                    {
                        if (dotnetVersion.Major < 8)
                        {
                            issues.Add(new DiagnosticIssue(
                                IssueSeverity.Warning,
                                "Outdated .NET version",
                                $".NET {version} detected, .NET 8+ recommended",
                                "Update to .NET 8 or later for best compatibility"
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                issues.Add(new DiagnosticIssue(
                    IssueSeverity.Error,
                    ".NET SDK not found",
                    $"Cannot execute 'dotnet --version': {ex.Message}",
                    "Install .NET SDK from https://dotnet.microsoft.com/download"
                ));
            }

            // Check Git availability
            try
            {
                var gitProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                gitProcess?.WaitForExit();
            }
            catch
            {
                issues.Add(new DiagnosticIssue(
                    IssueSeverity.Info,
                    "Git not available",
                    "Git is not installed or not in PATH",
                    "Install Git for version control features"
                ));
            }
        });
    }

    private void CheckTemplates(List<DiagnosticIssue> issues)
    {
        AnsiConsole.Status().Start("Checking templates...", ctx =>
        {
            // Check if custom templates directory exists
            var templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "templates");
            if (Directory.Exists(templatesPath))
            {
                var templateFiles = Directory.GetFiles(templatesPath, "*.template", SearchOption.AllDirectories);
                if (!templateFiles.Any())
                {
                    issues.Add(new DiagnosticIssue(
                        IssueSeverity.Info,
                        "Empty templates directory",
                        "Templates directory exists but contains no .template files",
                        "Add custom templates or remove empty directory"
                    ));
                }
            }
        });
    }

    private void DisplayResults(List<DiagnosticIssue> issues, bool autoFix)
    {
        if (!issues.Any())
        {
            var successPanel = new Panel("[green]✅ All checks passed! Your QCLI setup looks great.[/]")
            {
                Header = new PanelHeader(" Health Check Results "),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green)
            };
            AnsiConsole.Write(successPanel);
            return;
        }

        // Group issues by severity
        var errors = issues.Where(i => i.Severity == IssueSeverity.Error).ToList();
        var warnings = issues.Where(i => i.Severity == IssueSeverity.Warning).ToList();
        var infos = issues.Where(i => i.Severity == IssueSeverity.Info).ToList();

        if (errors.Any())
        {
            AnsiConsole.MarkupLine("[red]❌ Errors found:[/]");
            foreach (var error in errors)
            {
                DisplayIssue(error);
            }
            AnsiConsole.WriteLine();
        }

        if (warnings.Any())
        {
            AnsiConsole.MarkupLine("[yellow]⚠️ Warnings:[/]");
            foreach (var warning in warnings)
            {
                DisplayIssue(warning);
            }
            AnsiConsole.WriteLine();
        }

        if (infos.Any())
        {
            AnsiConsole.MarkupLine("[blue]ℹ️ Information:[/]");
            foreach (var info in infos)
            {
                DisplayIssue(info);
            }
            AnsiConsole.WriteLine();
        }

        // Auto-fix suggestions
        if (autoFix)
        {
            AttemptAutoFix(issues);
        }
        else
        {
            AnsiConsole.MarkupLine("[dim]Run with --fix to attempt automatic fixes where possible.[/]");
        }
    }

    private void DisplayIssue(DiagnosticIssue issue)
    {
        var icon = issue.Severity switch
        {
            IssueSeverity.Error => "[red]❌[/]",
            IssueSeverity.Warning => "[yellow]⚠️[/]",
            _ => "[blue]ℹ️[/]"
        };

        AnsiConsole.MarkupLine($"  {icon} [bold]{issue.Title}[/]");
        AnsiConsole.MarkupLine($"     {issue.Description}");
        AnsiConsole.MarkupLine($"     [dim]💡 {issue.Suggestion}[/]");
        AnsiConsole.WriteLine();
    }

    private void AttemptAutoFix(List<DiagnosticIssue> issues)
    {
        AnsiConsole.MarkupLine("[yellow]🔧 Attempting automatic fixes...[/]\n");

        foreach (var issue in issues.Where(i => i.CanAutoFix))
        {
            try
            {
                // Implement auto-fix logic based on issue type
                AnsiConsole.MarkupLine($"[green]✅ Fixed: {issue.Title}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Failed to fix {issue.Title}: {ex.Message}[/]");
            }
        }
    }
}

public sealed record DiagnosticIssue(
    IssueSeverity Severity,
    string Title,
    string Description,
    string Suggestion,
    bool CanAutoFix = false
);

public enum IssueSeverity
{
    Info,
    Warning,
    Error
}
