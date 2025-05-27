using Spectre.Console;
using Tools.Cli.Templates;

namespace Tools.Cli.Commands;

public sealed class ListCommand(ITemplateEngine templateEngine)
{
    public int Execute(string type)
    {
        return type.ToLower() switch
        {
            "templates" => ListTemplates(),
            "entities" => ListEntities(),
            "operations" => ListOperations(),
            _ => ShowHelp()
        };
    }

    private int ListTemplates()
    {
        AnsiConsole.MarkupLine("[green]📋 Available Templates:[/]\n");

        var templates = new[]
        {
            new { Name = "clio", Description = "Complete CLIO architecture with all layers", Status = "✅ Default" },
            new { Name = "minimal", Description = "Minimal setup with basic layers", Status = "✅ Available" },
            new { Name = "ddd", Description = "Domain-Driven Design with rich domain model", Status = "✅ Available" },
            new { Name = "microservice", Description = "Microservice template with API Gateway", Status = "🚧 Coming Soon" },
            new { Name = "blazor", Description = "Blazor Server/WASM with CLIO backend", Status = "🚧 Coming Soon" }
        };

        var table = new Table();
        table.AddColumn("[bold]Template[/]");
        table.AddColumn("[bold]Description[/]");
        table.AddColumn("[bold]Status[/]");

        foreach (var template in templates)
        {
            table.AddRow(
                $"[yellow]{template.Name}[/]",
                template.Description,
                template.Status
            );
        }

        AnsiConsole.Write(table);
        
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Usage: qcli init -t <template> or qcli scaffold <project> -t <template>[/]");
        
        return 0;
    }

    private int ListEntities()
    {
        AnsiConsole.MarkupLine("[green]📋 Discovered Entities:[/]\n");

        try
        {
            var entities = DiscoverEntities();
            
            if (!entities.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No entities found in the current project.[/]");
                AnsiConsole.MarkupLine("[dim]Use 'qcli add <EntityName> --all' to create your first entity.[/]");
                return 0;
            }

            var table = new Table();
            table.AddColumn("[bold]Entity[/]");
            table.AddColumn("[bold]Location[/]");
            table.AddColumn("[bold]CRUD Operations[/]");

            foreach (var entity in entities)
            {
                table.AddRow(
                    $"[yellow]{entity.Name}[/]",
                    entity.Path,
                    entity.Operations
                );
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Error discovering entities: {ex.Message}[/]");
            return 1;
        }

        return 0;
    }

    private int ListOperations()
    {
        AnsiConsole.MarkupLine("[green]📋 Available CRUD Operations:[/]\n");

        var operations = new[]
        {
            new { Flag = "--all", Description = "Generate all CRUD operations", Example = "qcli add Order --all" },
            new { Flag = "--create", Description = "Generate Create operation", Example = "qcli add Order --create" },
            new { Flag = "--read", Description = "Generate Read operations (queries)", Example = "qcli add Order --read" },
            new { Flag = "--update", Description = "Generate Update operation", Example = "qcli add Order --update" },
            new { Flag = "--delete", Description = "Generate Delete operation", Example = "qcli add Order --delete" }
        };

        var table = new Table();
        table.AddColumn("[bold]Flag[/]");
        table.AddColumn("[bold]Description[/]");
        table.AddColumn("[bold]Example[/]");

        foreach (var op in operations)
        {
            table.AddRow(
                $"[yellow]{op.Flag}[/]",
                op.Description,
                $"[dim]{op.Example}[/]"
            );
        }

        AnsiConsole.Write(table);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Additional Options:[/]");
        AnsiConsole.MarkupLine("  [yellow]--entity-type[/] <type>    Entity type (Audited, FullyAudited)");
        AnsiConsole.MarkupLine("  [yellow]--no-tests[/]             Skip generating tests");
        AnsiConsole.MarkupLine("  [yellow]--no-permissions[/]       Skip generating permissions");
        AnsiConsole.MarkupLine("  [yellow]--template[/] <template>  Use specific template");
        AnsiConsole.MarkupLine("  [yellow]--dry-run[/]             Preview changes without creating files");

        return 0;
    }

    private int ShowHelp()
    {
        AnsiConsole.MarkupLine("[red]❌ Invalid list type.[/]\n");
        AnsiConsole.MarkupLine("[yellow]Available list types:[/]");
        AnsiConsole.MarkupLine("  [green]templates[/]  - List available project templates");
        AnsiConsole.MarkupLine("  [green]entities[/]   - List discovered entities in the project");
        AnsiConsole.MarkupLine("  [green]operations[/] - List available CRUD operations");
        
        return 1;
    }

    private IEnumerable<EntityInfo> DiscoverEntities()
    {
        var entities = new List<EntityInfo>();
        var currentDir = Directory.GetCurrentDirectory();
        
        // Look for entity files in common locations
        var searchPaths = new[]
        {
            Path.Combine(currentDir, "src", "Domain"),
            Path.Combine(currentDir, "Domain"),
            Path.Combine(currentDir, "src", "Application"),
            Path.Combine(currentDir, "Application")
        };

        foreach (var searchPath in searchPaths.Where(Directory.Exists))
        {
            var files = Directory.GetFiles(searchPath, "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var relativePath = Path.GetRelativePath(currentDir, file);
                
                // Simple heuristic to identify entities
                if (IsLikelyEntity(file))
                {
                    var operations = AnalyzeOperations(fileName, currentDir);
                    entities.Add(new EntityInfo(fileName, relativePath, operations));
                }
            }
        }

        return entities.DistinctBy(e => e.Name);
    }

    private bool IsLikelyEntity(string filePath)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return content.Contains("class ") && 
                   (content.Contains(": Entity") || content.Contains(": AuditedEntity") || 
                    content.Contains(": BaseEntity") || content.Contains("public class"));
        }
        catch
        {
            return false;
        }
    }

    private string AnalyzeOperations(string entityName, string basePath)
    {
        var operations = new List<string>();
        
        // Check for Create command
        if (HasOperation(entityName, "Create", basePath))
            operations.Add("C");
        
        // Check for Read queries
        if (HasOperation(entityName, "Get", basePath))
            operations.Add("R");
        
        // Check for Update command
        if (HasOperation(entityName, "Update", basePath))
            operations.Add("U");
        
        // Check for Delete command
        if (HasOperation(entityName, "Delete", basePath))
            operations.Add("D");

        return operations.Any() ? string.Join("", operations) : "None";
    }

    private bool HasOperation(string entityName, string operation, string basePath)
    {
        var searchPaths = new[]
        {
            Path.Combine(basePath, "src", "Application"),
            Path.Combine(basePath, "Application")
        };

        foreach (var searchPath in searchPaths.Where(Directory.Exists))
        {
            var pattern = $"*{operation}*{entityName}*.cs";
            var files = Directory.GetFiles(searchPath, pattern, SearchOption.AllDirectories);
            if (files.Any())
                return true;
        }

        return false;
    }
}

public sealed record EntityInfo(string Name, string Path, string Operations);
