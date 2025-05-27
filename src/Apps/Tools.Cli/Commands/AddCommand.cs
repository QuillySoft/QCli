using Spectre.Console;
using Tools.Cli.Configuration;
using Tools.Cli.Utils;

namespace Tools.Cli.Commands;

public sealed class AddCommand(CliConfiguration config)
{
    public sealed class Options
    {
        public string EntityName { get; init; } = "";
        public bool All { get; init; }
        public bool Create { get; init; }
        public bool Update { get; init; }
        public bool Delete { get; init; }
        public bool Read { get; init; }
        public string? EntityType { get; init; }
        public bool SkipTests { get; init; }
        public bool SkipPermissions { get; init; }
        public string? Template { get; init; }
        public string? OutputPath { get; init; }
        public bool DryRun { get; init; }
        public bool Verbose { get; init; }
    }

    public int Execute(Options options)
    {
        AnsiConsole.MarkupLine($"[green]🎯 Adding CRUD operations for[/] [blue]{options.EntityName}[/]");

        // Determine what to generate
        var operations = DetermineOperations(options);
        
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
        var normalizedEntityName = NormalizeEntityName(options.EntityName);
        var singularName = GetSingularName(normalizedEntityName);
        var pluralName = GetPluralName(normalizedEntityName);

        if (options.Verbose)
        {
            DisplayGenerationPlan(singularName, pluralName, operations, options);
        }

        if (options.DryRun)
        {
            return ShowDryRun(singularName, pluralName, operations, options);
        }

        AnsiConsole.MarkupLine($"[yellow]⚡ Generating operations:[/] {string.Join(", ", operations)}");

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
        var generator = new CrudGenerator(config);
        
        try
        {
            ProgressHelper.ShowSpinner($"Generating {singularName} CRUD operations...", () =>
            {
                generator.GenerateOperations(singularName, pluralName, operations, options.EntityType ?? "Audited");
            });

            ShowSuccessMessage(singularName, pluralName, operations);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Error generating code: {ex.Message}[/]");
            if (options.Verbose)
            {
                AnsiConsole.WriteException(ex);
            }
            return 1;
        }
    }

    public int Execute(string entityName, bool all = false, bool create = false, bool update = false, bool delete = false, bool read = false, string? entityType = null)
    {
        // Legacy method for backward compatibility
        return Execute(new Options
        {
            EntityName = entityName,
            All = all,
            Create = create,
            Update = update,
            Delete = delete,
            Read = read,
            EntityType = entityType
        });
    }

    private List<string> DetermineOperations(Options options)
    {
        var operations = new List<string>();

        if (options.All)
        {
            operations.AddRange(new[] { "Create", "Read", "Update", "Delete" });
        }
        else
        {
            if (options.Create) operations.Add("Create");
            if (options.Read) operations.Add("Read");
            if (options.Update) operations.Add("Update");
            if (options.Delete) operations.Add("Delete");
        }

        return operations;
    }

    private void DisplayGenerationPlan(string singularName, string pluralName, List<string> operations, Options options)
    {
        var panel = new Panel(new Markup(
            $"[bold]Entity:[/] {singularName} (plural: {pluralName})\n" +
            $"[bold]Operations:[/] {string.Join(", ", operations)}\n" +
            $"[bold]Entity Type:[/] {options.EntityType ?? "Audited"}\n" +
            $"[bold]Generate Tests:[/] {(!options.SkipTests ? "Yes" : "No")}\n" +
            $"[bold]Generate Permissions:[/] {(!options.SkipPermissions ? "Yes" : "No")}\n" +
            $"[bold]Template:[/] {options.Template ?? "default"}"))
        {
            Header = new PanelHeader(" Generation Plan "),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Blue)
        };
        
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private int ShowDryRun(string singularName, string pluralName, List<string> operations, Options options)
    {
        AnsiConsole.MarkupLine("[yellow]🔍 Dry run - Files that would be generated:[/]\n");

        var filesToGenerate = GetFilesToGenerate(singularName, pluralName, operations, options);

        var tree = new Tree($"[green]{singularName} CRUD Generation[/]");
        
        foreach (var category in filesToGenerate.GroupBy(f => f.Category))
        {
            var categoryNode = tree.AddNode($"[yellow]{category.Key}[/]");
            foreach (var file in category)
            {
                categoryNode.AddNode($"[dim]{file.Path}[/]");
            }
        }

        AnsiConsole.Write(tree);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Run without --dry-run to generate these files.[/]");

        return 0;
    }

    private void ShowSuccessMessage(string singularName, string pluralName, List<string> operations)
    {
        var panel = new Panel(new Markup(
            $"[bold green]🎉 {singularName} CRUD operations generated successfully![/]\n\n" +
            $"[yellow]Generated operations:[/] {string.Join(", ", operations)}\n\n" +
            "[dim]Next steps:[/]\n" +
            "• Review generated files\n" +
            "• Add necessary business logic\n" +
            "• Run tests to ensure everything works\n" +
            "• Update database with migrations if needed"))
        {
            Header = new PanelHeader(" Success! "),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Green)
        };
        
        AnsiConsole.Write(panel);
    }

    private List<GeneratedFile> GetFilesToGenerate(string singularName, string pluralName, List<string> operations, Options options)
    {
        var files = new List<GeneratedFile>();

        // Entity files
        files.Add(new GeneratedFile("Domain", $"src/Domain/{pluralName}/{singularName}.cs"));
        files.Add(new GeneratedFile("Persistence", $"src/Infrastructure/Persistence/Configurations/{singularName}Configuration.cs"));

        // Operation-specific files
        foreach (var operation in operations)
        {
            switch (operation)
            {
                case "Create":
                    files.Add(new GeneratedFile("Application", $"src/Application/{pluralName}/Commands/Create{singularName}/Create{singularName}Command.cs"));
                    files.Add(new GeneratedFile("Application", $"src/Application/{pluralName}/Commands/Create{singularName}/Create{singularName}CommandHandler.cs"));
                    if (!options.SkipTests)
                        files.Add(new GeneratedFile("Tests", $"tests/Application.Tests/{pluralName}/Commands/Create{singularName}CommandTests.cs"));
                    break;

                case "Read":
                    files.Add(new GeneratedFile("Application", $"src/Application/{pluralName}/Queries/Get{pluralName}/Get{pluralName}Query.cs"));
                    files.Add(new GeneratedFile("Application", $"src/Application/{pluralName}/Queries/Get{singularName}ById/Get{singularName}ByIdQuery.cs"));
                    break;

                case "Update":
                    files.Add(new GeneratedFile("Application", $"src/Application/{pluralName}/Commands/Update{singularName}/Update{singularName}Command.cs"));
                    break;

                case "Delete":
                    files.Add(new GeneratedFile("Application", $"src/Application/{pluralName}/Commands/Delete{singularName}/Delete{singularName}Command.cs"));
                    break;
            }
        }

        // Controller
        files.Add(new GeneratedFile("WebApi", $"src/WebApi/Controllers/{pluralName}Controller.cs"));

        // Permissions
        if (!options.SkipPermissions)
        {
            files.Add(new GeneratedFile("Application", $"src/Application/Authorization/Permissions/{pluralName}Permissions.cs"));
        }

        return files;
    }

    private bool ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(config.Paths.RootPath))
        {
            AnsiConsole.MarkupLine("[red]Root path not configured. Run 'qcli init' to set up configuration.[/]");
            return false;
        }

        if (!Directory.Exists(config.Paths.RootPath))
        {
            AnsiConsole.MarkupLine($"[red]Root path does not exist: {config.Paths.RootPath}[/]");
            return false;
        }

        return true;
    }

    private bool CheckEntityExists(string entityName)
    {
        var domainPath = config.Paths.GetFullPath(config.Paths.DomainPath);
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
