using Spectre.Console;
using Tools.Cli.Configuration;
using Tools.Cli.Templates;

namespace Tools.Cli.Commands;

public sealed class ScaffoldCommand(ITemplateEngine templateEngine)
{
    public int Execute(string projectName, string? template, string? outputPath)
    {
        AnsiConsole.MarkupLine($"[green]🏗️ Scaffolding project '{projectName}'...[/]");

        var targetPath = outputPath ?? Path.Combine(Directory.GetCurrentDirectory(), projectName);
        
        if (Directory.Exists(targetPath) && Directory.GetFiles(targetPath).Length > 0)
        {
            if (!AnsiConsole.Confirm($"Directory '{targetPath}' already exists and is not empty. Continue?"))
            {
                return 1;
            }
        }

        try
        {
            CreateProjectStructure(projectName, targetPath, template ?? "clean-architecture");
            AnsiConsole.MarkupLine($"[green]✅ Project '{projectName}' scaffolded successfully![/]");
            AnsiConsole.MarkupLine($"[dim]Project created at: {targetPath}[/]");
            
            ShowNextSteps(projectName, targetPath);
            
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Error scaffolding project: {ex.Message}[/]");
            return 1;
        }
    }

    private void CreateProjectStructure(string projectName, string targetPath, string template)
    {
        var structure = GetProjectStructure(template);
        
        foreach (var directory in structure.Directories)
        {
            var fullPath = Path.Combine(targetPath, directory);
            Directory.CreateDirectory(fullPath);
        }

        foreach (var file in structure.Files)
        {
            var fullPath = Path.Combine(targetPath, file.Path);
            var content = templateEngine.ProcessTemplate(file.TemplateName, new { ProjectName = projectName });
            File.WriteAllText(fullPath, content);
        }

        // Create qcli.json configuration
        var config = new CliConfiguration
        {
            Project = new ProjectInfo
            {
                Name = projectName,
                Namespace = projectName,
                Description = $"A {template} architecture project"
            }
        };

        var configJson = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        File.WriteAllText(Path.Combine(targetPath, "qcli.json"), configJson);
    }

    private ProjectStructure GetProjectStructure(string template)
    {
        return template.ToLower() switch
        {
            "minimal" => GetMinimalStructure(),
            "ddd" => GetDddStructure(),
            _ => GetCleanArchitectureStructure()
        };
    }

    private ProjectStructure GetCleanArchitectureStructure()
    {
        return new ProjectStructure
        {
            Directories = new[]
            {
                "src/Domain",
                "src/Application",
                "src/Infrastructure/Persistence",
                "src/WebApi",
                "tests/Application.Tests",
                "tests/Integration.Tests",
                "docs"
            },
            Files = new[]
            {
                new ProjectFile("README.md", "readme"),
                new ProjectFile(".gitignore", "gitignore"),
                new ProjectFile("src/Domain/.gitkeep", "gitkeep"),
                new ProjectFile("src/Application/.gitkeep", "gitkeep"),
                new ProjectFile("docs/architecture.md", "architecture-doc")
            }
        };
    }

    private ProjectStructure GetMinimalStructure()
    {
        return new ProjectStructure
        {
            Directories = new[]
            {
                "src/Domain",
                "src/Application",
                "src/WebApi"
            },
            Files = new[]
            {
                new ProjectFile("README.md", "readme-minimal"),
                new ProjectFile(".gitignore", "gitignore")
            }
        };
    }

    private ProjectStructure GetDddStructure()
    {
        return new ProjectStructure
        {
            Directories = new[]
            {
                "src/Domain/Entities",
                "src/Domain/ValueObjects",
                "src/Domain/Events",
                "src/Domain/Services",
                "src/Application/Commands",
                "src/Application/Queries",
                "src/Application/Events",
                "src/Infrastructure/Persistence",
                "src/Infrastructure/Services",
                "src/WebApi",
                "tests/Domain.Tests",
                "tests/Application.Tests",
                "tests/Integration.Tests"
            },
            Files = new[]
            {
                new ProjectFile("README.md", "readme-ddd"),
                new ProjectFile(".gitignore", "gitignore"),
                new ProjectFile("docs/domain-model.md", "domain-model-doc")
            }
        };
    }

    private void ShowNextSteps(string projectName, string targetPath)
    {
        var panel = new Panel(new Markup(
            $"[bold green]🎉 Project '{projectName}' created successfully![/]\n\n" +
            "[yellow]Next steps:[/]\n" +
            $"1. [green]cd {targetPath}[/]\n" +
            "2. [green]qcli doctor[/] - Check project health\n" +
            "3. [green]qcli add Order --all[/] - Generate your first entity\n" +
            "4. [green]qcli config show[/] - Review configuration\n\n" +
            "[dim]Happy coding! 🚀[/]"))
        {
            Header = new PanelHeader(" Next Steps "),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Green)
        };
        
        AnsiConsole.Write(panel);
    }
}

public sealed record ProjectStructure
{
    public string[] Directories { get; init; } = Array.Empty<string>();
    public ProjectFile[] Files { get; init; } = Array.Empty<ProjectFile>();
}

public sealed record ProjectFile(string Path, string TemplateName);
