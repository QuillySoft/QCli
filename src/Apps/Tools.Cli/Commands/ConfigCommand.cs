using Cli.Configuration;
using Spectre.Console;

namespace Cli.Commands;

public sealed class ConfigCommand
{
    public int Execute(string subCommand, string? path = null, string? key = null, string? value = null)
    {
        return subCommand.ToLower() switch
        {
            "init" => InitializeConfig(path),
            "set" => SetConfigValue(key, value),
            "get" => GetConfigValue(key),
            "show" => ShowConfig(),
            "sample" => GenerateSampleConfig(path),
            _ => ShowConfigHelp()
        };
    }

    private int InitializeConfig(string? configPath = null)
    {
        configPath ??= Path.Combine(Directory.GetCurrentDirectory(), "quillysoft-cli.json");

        if (File.Exists(configPath))
        {
            var overwrite = AnsiConsole.Confirm($"Configuration file already exists at {configPath}. Overwrite?");
            if (!overwrite)
            {
                AnsiConsole.MarkupLine("[yellow]Configuration initialization cancelled.[/]");
                return 0;
            }
        }

        try
        {
            // Auto-detect paths
            var config = new CliConfiguration
            {
                Paths = ProjectPaths.AutoDetect()
            };

            // Ask user to confirm/modify detected paths
            AnsiConsole.MarkupLine("[blue]Detected project structure:[/]");
            AnsiConsole.MarkupLine($"Root Path: [green]{config.Paths.RootPath}[/]");

            var customize = AnsiConsole.Confirm("Would you like to customize the paths?");
            if (customize)
            {
                config.Paths = GatherCustomPaths(config.Paths);
            }

            config.Save(configPath);

            AnsiConsole.MarkupLine($"[green]✓ Configuration initialized at[/] [blue]{configPath}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error initializing configuration: {ex.Message}[/]");
            return 1;
        }
    }

    private ProjectPaths GatherCustomPaths(ProjectPaths defaultPaths)
    {
        var paths = new ProjectPaths();

        paths.RootPath = AnsiConsole.Ask("Root Path:", defaultPaths.RootPath);
        paths.ApiPath = AnsiConsole.Ask("API Path (relative to root):", defaultPaths.ApiPath);
        paths.ApplicationPath = AnsiConsole.Ask("Application Path (relative to root):", defaultPaths.ApplicationPath);
        paths.DomainPath = AnsiConsole.Ask("Domain Path (relative to root):", defaultPaths.DomainPath);
        paths.PersistencePath = AnsiConsole.Ask("Persistence Path (relative to root):", defaultPaths.PersistencePath);
        paths.ApplicationTestsPath = AnsiConsole.Ask("Application Tests Path (relative to root):", defaultPaths.ApplicationTestsPath);
        paths.IntegrationTestsPath = AnsiConsole.Ask("Integration Tests Path (relative to root):", defaultPaths.IntegrationTestsPath);
        paths.ControllersPath = AnsiConsole.Ask("Controllers Path (relative to root):", defaultPaths.ControllersPath);

        return paths;
    }

    private int SetConfigValue(string? key, string? value)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            AnsiConsole.MarkupLine("[red]Both key and value must be specified for set command.[/]");
            return 1;
        }

        try
        {
            var config = CliConfiguration.Load();
            
            // Simple key-value setting (can be extended)
            switch (key.ToLower())
            {
                case "rootpath":
                    config.Paths.RootPath = value;
                    break;
                case "entitytype":
                    config.CodeGeneration.DefaultEntityType = value;
                    break;
                default:
                    AnsiConsole.MarkupLine($"[red]Unknown configuration key: {key}[/]");
                    return 1;
            }

            config.Save();
            AnsiConsole.MarkupLine($"[green]✓ Set {key} = {value}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error setting configuration: {ex.Message}[/]");
            return 1;
        }
    }

    private int GetConfigValue(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return ShowConfig();
        }

        try
        {
            var config = CliConfiguration.Load();
            
            var value = key.ToLower() switch
            {
                "rootpath" => config.Paths.RootPath,
                "entitytype" => config.CodeGeneration.DefaultEntityType,
                _ => null
            };

            if (value != null)
            {
                AnsiConsole.MarkupLine($"{key}: [green]{value}[/]");
                return 0;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Unknown configuration key: {key}[/]");
                return 1;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error reading configuration: {ex.Message}[/]");
            return 1;
        }
    }

    private int ShowConfig()
    {
        try
        {
            var config = CliConfiguration.Load();

            var table = new Table();
            table.AddColumn("Setting");
            table.AddColumn("Value");

            table.AddRow("Project Type", config.ProjectType);
            table.AddRow("Root Path", config.Paths.RootPath);
            table.AddRow("API Path", config.Paths.ApiPath);
            table.AddRow("Application Path", config.Paths.ApplicationPath);
            table.AddRow("Domain Path", config.Paths.DomainPath);
            table.AddRow("Persistence Path", config.Paths.PersistencePath);
            table.AddRow("Application Tests Path", config.Paths.ApplicationTestsPath);
            table.AddRow("Integration Tests Path", config.Paths.IntegrationTestsPath);
            table.AddRow("Controllers Path", config.Paths.ControllersPath);
            table.AddRow("Default Entity Type", config.CodeGeneration.DefaultEntityType);
            table.AddRow("Generate Events", config.CodeGeneration.GenerateEvents.ToString());
            table.AddRow("Generate Mapping Profiles", config.CodeGeneration.GenerateMappingProfiles.ToString());
            table.AddRow("Generate Permissions", config.CodeGeneration.GeneratePermissions.ToString());
            table.AddRow("Generate Tests", config.CodeGeneration.GenerateTests.ToString());

            AnsiConsole.Write(table);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error showing configuration: {ex.Message}[/]");
            return 1;
        }
    }

    private int GenerateSampleConfig(string? path = null)
    {
        path ??= Path.Combine(Directory.GetCurrentDirectory(), "quillysoft-cli.sample.json");

        try
        {
            var sampleConfig = CliConfiguration.CreateSampleConfiguration();
            
            var json = System.Text.Json.JsonSerializer.Serialize(sampleConfig, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            File.WriteAllText(path, json);

            AnsiConsole.MarkupLine($"[green]✓ Sample configuration generated at[/] [blue]{path}[/]");
            AnsiConsole.MarkupLine("[yellow]Copy this file to 'quillysoft-cli.json' and customize as needed.[/]");
            
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error generating sample configuration: {ex.Message}[/]");
            return 1;
        }
    }

    private int ShowConfigHelp()
    {
        AnsiConsole.MarkupLine("[blue]Available config subcommands:[/]");
        AnsiConsole.MarkupLine("  [green]init[/]    - Initialize configuration file");
        AnsiConsole.MarkupLine("  [green]show[/]    - Show current configuration");
        AnsiConsole.MarkupLine("  [green]set[/]     - Set a configuration value");
        AnsiConsole.MarkupLine("  [green]get[/]     - Get a configuration value");
        AnsiConsole.MarkupLine("  [green]sample[/]  - Generate sample configuration file");
        
        return 1;
    }
}
