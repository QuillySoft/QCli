using Cli.Configuration;
using Spectre.Console;

namespace Cli.Commands;

public sealed class AddCommand
{
    private readonly CliConfiguration _config;

    public AddCommand(CliConfiguration config)
    {
        _config = config;
    }

    public int Execute(string entityName, bool all = false, bool create = false, bool update = false, bool delete = false, bool read = false, string? entityType = null)
    {
        AnsiConsole.MarkupLine($"[green]Adding CRUD operations for[/] [blue]{entityName}[/]");

        // Determine what to generate
        var operations = DetermineOperations(all, create, update, delete, read);
        
        if (!operations.Any())
        {
            AnsiConsole.MarkupLine("[red]No operations specified. Use --all or specify individual operations (--create, --read, --update, --delete)[/]");
            return 1;
        }

        // Validate configuration
        if (!ValidateConfiguration())
        {
            return 1;
        }

        // Normalize entity name
        var normalizedEntityName = NormalizeEntityName(entityName);
        var singularName = GetSingularName(normalizedEntityName);
        var pluralName = GetPluralName(normalizedEntityName);

        AnsiConsole.MarkupLine($"[yellow]Generating operations:[/] {string.Join(", ", operations)}");

        // Check if entity exists
        var entityExists = CheckEntityExists(singularName);
        if (!entityExists && operations.Any(op => op != "Create"))
        {
            var createEntity = AnsiConsole.Confirm($"Entity '{singularName}' doesn't exist. Create it?");
            if (createEntity)
            {
                operations.Add("Create Entity");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Cannot proceed without entity. Aborting.[/]");
                return 1;
            }
        }

        // Generate code
        var generator = new CrudGenerator(_config);
        
        try
        {
            generator.GenerateOperations(singularName, pluralName, operations, entityType ?? _config.CodeGeneration.DefaultEntityType);
            
            AnsiConsole.MarkupLine($"[green]✓ Successfully generated CRUD operations for[/] [blue]{entityName}[/]");
            AnsiConsole.MarkupLine($"[yellow]Note: Review and customize the generated code according to your business requirements.[/]");
            
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error generating code: {ex.Message}[/]");
            return 1;
        }
    }

    private List<string> DetermineOperations(bool all, bool create, bool update, bool delete, bool read)
    {
        var operations = new List<string>();

        if (all)
        {
            operations.AddRange(new[] { "Create", "Read", "Update", "Delete" });
        }
        else
        {
            if (create) operations.Add("Create");
            if (read) operations.Add("Read");
            if (update) operations.Add("Update");
            if (delete) operations.Add("Delete");
        }

        return operations;
    }

    private bool ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_config.Paths.RootPath))
        {
            AnsiConsole.MarkupLine("[red]Root path not configured. Run 'quillysoft-cli config init' to set up configuration.[/]");
            return false;
        }

        if (!Directory.Exists(_config.Paths.RootPath))
        {
            AnsiConsole.MarkupLine($"[red]Root path does not exist: {_config.Paths.RootPath}[/]");
            return false;
        }

        return true;
    }

    private bool CheckEntityExists(string entityName)
    {
        var domainPath = _config.Paths.GetFullPath(_config.Paths.DomainPath);
        var entityFile = Path.Combine(domainPath, $"{entityName}s", $"{entityName}.cs");
        return File.Exists(entityFile);
    }

    private string NormalizeEntityName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        return char.ToUpper(name[0]) + name.Substring(1);
    }

    private string GetSingularName(string name)
    {
        return name.EndsWith("s") && name.Length > 1 ? name[..^1] : name;
    }

    private string GetPluralName(string name)
    {
        return name.EndsWith("s") ? name : name + "s";
    }
}
