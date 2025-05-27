using Spectre.Console;
using Tools.Cli.Configuration;
using Tools.Cli.Templates;
using Tools.Cli.Validation;

namespace Tools.Cli.Commands;

public sealed class InitCommand(ITemplateEngine templateEngine)
{
    public int Execute(bool force, string? template, bool interactive)
    {
        AnsiConsole.MarkupLine("[green]🏗️ Initializing QCLI project...[/]");

        // Check if already initialized
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "qcli.json");
        if (File.Exists(configPath) && !force)
        {
            if (!AnsiConsole.Confirm("QCLI is already initialized. Do you want to reinitialize?"))
            {
                return 0;
            }
        }

        var config = interactive ? CreateInteractiveConfiguration() : CreateDefaultConfiguration(template);
        
        // Validate configuration
        var validator = new ConfigurationValidator();
        var validationResult = validator.Validate(config);
        
        if (!validationResult.IsValid)
        {
            validationResult.DisplayErrors();
            return 1;
        }

        // Save configuration
        var json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        File.WriteAllText(configPath, json);

        AnsiConsole.MarkupLine($"[green]✅ QCLI initialized successfully![/]");
        AnsiConsole.MarkupLine($"[dim]Configuration saved to: {configPath}[/]");
        
        return 0;
    }

    private EnhancedCliConfiguration CreateInteractiveConfiguration()
    {
        var config = new EnhancedCliConfiguration();

        AnsiConsole.MarkupLine("[yellow]📝 Let's set up your project configuration...[/]");
        
        config.Project.Name = AnsiConsole.Ask<string>("Project name:");
        config.Project.Namespace = AnsiConsole.Ask("Root namespace:", config.Project.Name);
        config.Project.Description = AnsiConsole.Ask("Project description (optional):", "");
        config.Project.Author = AnsiConsole.Ask("Author name (optional):", "");

        // Paths configuration
        AnsiConsole.MarkupLine("\n[yellow]📁 Configure project paths:[/]");
        config.Paths.DomainPath = AnsiConsole.Ask("Domain path:", "src/Domain");
        config.Paths.ApplicationPath = AnsiConsole.Ask("Application path:", "src/Application");
        config.Paths.PersistencePath = AnsiConsole.Ask("Persistence path:", "src/Infrastructure/Persistence");
        config.Paths.ApiPath = AnsiConsole.Ask("WebApi path:", "src/WebApi");

        // Code generation settings
        AnsiConsole.MarkupLine("\n[yellow]⚙️ Code generation settings:[/]");
        config.CodeGeneration.GenerateTests = AnsiConsole.Confirm("Generate tests?", true);
        config.CodeGeneration.GeneratePermissions = AnsiConsole.Confirm("Generate permissions?", true);
        config.CodeGeneration.GenerateEvents = AnsiConsole.Confirm("Generate domain events?", true);

        return config;
    }

    private EnhancedCliConfiguration CreateDefaultConfiguration(string? template)
    {
        return template?.ToLower() switch
        {
            "minimal" => CreateMinimalConfiguration(),
            "ddd" => CreateDddConfiguration(),
            _ => CreateClioConfiguration()
        };
    }

    private EnhancedCliConfiguration CreateClioConfiguration()
    {
        return new EnhancedCliConfiguration
        {
            Project = new ProjectInfo
            {
                Name = Path.GetFileName(Directory.GetCurrentDirectory()),
                Namespace = Path.GetFileName(Directory.GetCurrentDirectory())
            }
        };
    }

    private EnhancedCliConfiguration CreateMinimalConfiguration()
    {
        var config = CreateClioConfiguration();
        config.CodeGeneration.GenerateEvents = false;
        config.CodeGeneration.GenerateMappingProfiles = false;
        return config;
    }

    private EnhancedCliConfiguration CreateDddConfiguration()
    {
        var config = CreateClioConfiguration();
        config.CodeGeneration.GenerateEvents = true;
        config.CodeGeneration.GeneratePermissions = true;
        return config;
    }
}

public sealed class ConfigurationValidator : IValidator<EnhancedCliConfiguration>
{
    public Tools.Cli.Validation.ValidationResult Validate(EnhancedCliConfiguration config)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(config.Project.Name))
            errors.Add(new ValidationError("Project name is required", nameof(config.Project.Name)));

        if (string.IsNullOrWhiteSpace(config.Project.Namespace))
            errors.Add(new ValidationError("Project namespace is required", nameof(config.Project.Namespace)));

        // Validate paths exist or can be created
        var paths = new[]
        {
            config.Paths.DomainPath,
            config.Paths.ApplicationPath,
            config.Paths.PersistencePath,
            config.Paths.ApiPath
        };

        foreach (var path in paths.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    // Try to create the directory to validate permissions
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError($"Cannot access path '{path}': {ex.Message}", "Paths"));
            }
        }

        return errors.Any() ? Tools.Cli.Validation.ValidationResult.Failure(errors) : Tools.Cli.Validation.ValidationResult.Success();
    }
}
