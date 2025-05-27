using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Tools.Cli.Commands;
using Tools.Cli.Configuration;
using Spectre.Console;

namespace Tools.Cli;

public sealed class CliApp(IServiceProvider serviceProvider)
{
    private TCommand GetCmd<TCommand>() where TCommand : notnull => serviceProvider.GetRequiredService<TCommand>();

    public int Execute(string[] args)
    {
        var app = new CommandLineApplication
        {
            Name = "qcli",
            Description = "🚀 QCLI - A powerful CLI tool for generating CRUD operations and managing CLIO architecture projects",
        };

        app.HelpOption(true);
        app.VersionOption("-v|--version", "1.0.0");

        // Global options
        var verboseOption = app.Option("-V|--verbose", "Enable verbose output", CommandOptionType.NoValue);
        var configOption = app.Option("--config <path>", "Path to configuration file", CommandOptionType.SingleValue);

        // Welcome message when no command is provided
        app.OnExecute(() =>
        {
            ShowWelcomeMessage();
            app.ShowHelp();
            return 0;
        });

        // Generate/Add command - Main CRUD generation
        app.Command("add", add =>
        {
            add.Description = "🎯 Generate CRUD operations for an entity";
            add.AddName("generate"); // Alias

            var entityName = add.Argument<string>("entity", "Entity name (e.g., 'Order' or 'Product')").IsRequired();
            var allOption = add.Option("-a|--all", "Generate all CRUD operations (Create, Read, Update, Delete)", CommandOptionType.NoValue);
            var createOption = add.Option("-c|--create", "Generate create operation", CommandOptionType.NoValue);
            var readOption = add.Option("-r|--read", "Generate read operations (queries)", CommandOptionType.NoValue);
            var updateOption = add.Option("-u|--update", "Generate update operation", CommandOptionType.NoValue);
            var deleteOption = add.Option("-d|--delete", "Generate delete operation", CommandOptionType.NoValue);
            var entityTypeOption = add.Option("-e|--entity-type <type>", "Entity type to use (Audited or FullyAudited)", CommandOptionType.SingleValue);
            var noTestsOption = add.Option("--no-tests", "Skip generating tests", CommandOptionType.NoValue);
            var noPermissionsOption = add.Option("--no-permissions", "Skip generating permissions", CommandOptionType.NoValue);
            var templateOption = add.Option("-t|--template <template>", "Use specific template (default, minimal, advanced)", CommandOptionType.SingleValue);
            var outputOption = add.Option("-o|--output <path>", "Output directory", CommandOptionType.SingleValue);
            var dryRunOption = add.Option("--dry-run", "Show what would be generated without creating files", CommandOptionType.NoValue);
            
            entityTypeOption.DefaultValue = "Audited";
            templateOption.DefaultValue = "default";

            add.OnExecute(() =>
            {
                var config = CliConfiguration.Load(configOption.Value());
                var addCommand = new AddCommand(config);
                return addCommand.Execute(new AddCommand.Options
                {
                    EntityName = entityName.ParsedValue,
                    All = allOption.HasValue(),
                    Create = createOption.HasValue(),
                    Update = updateOption.HasValue(),
                    Delete = deleteOption.HasValue(),
                    Read = readOption.HasValue(),
                    EntityType = entityTypeOption.Value(),
                    SkipTests = noTestsOption.HasValue(),
                    SkipPermissions = noPermissionsOption.HasValue(),
                    Template = templateOption.Value(),
                    OutputPath = outputOption.Value(),
                    DryRun = dryRunOption.HasValue(),
                    Verbose = verboseOption.HasValue()
                });
            });
        });

        // New commands for better functionality
        app.Command("init", init =>
        {
            init.Description = "🏗️ Initialize QCLI in a new or existing project";
            
            var forceOption = init.Option("-f|--force", "Overwrite existing configuration", CommandOptionType.NoValue);
            var templateOption = init.Option("-t|--template <template>", "Project template (clio, minimal, ddd)", CommandOptionType.SingleValue);
            var interactiveOption = init.Option("-i|--interactive", "Use interactive mode", CommandOptionType.NoValue);
            
            templateOption.DefaultValue = "clio";

            init.OnExecute(() =>
            {
                var initCommand = GetCmd<InitCommand>();
                return initCommand.Execute(
                    forceOption.HasValue(),
                    templateOption.Value(),
                    interactiveOption.HasValue());
            });
        });

        app.Command("scaffold", scaffold =>
        {
            scaffold.Description = "🏗️ Scaffold entire project structure";
            
            var nameArg = scaffold.Argument<string>("name", "Project name").IsRequired();
            var templateOption = scaffold.Option("-t|--template <template>", "Project template", CommandOptionType.SingleValue);
            var pathOption = scaffold.Option("-p|--path <path>", "Output path", CommandOptionType.SingleValue);
            
            templateOption.DefaultValue = "clio";

            scaffold.OnExecute(() =>
            {
                var scaffoldCommand = GetCmd<ScaffoldCommand>();
                return scaffoldCommand.Execute(nameArg.ParsedValue, templateOption.Value(), pathOption.Value());
            });
        });

        app.Command("list", list =>
        {
            list.Description = "📋 List available templates, entities, or operations";
            
            var typeArg = list.Argument<string>("type", "What to list (templates, entities, operations)").IsRequired();

            list.OnExecute(() =>
            {
                var listCommand = GetCmd<ListCommand>();
                return listCommand.Execute(typeArg.ParsedValue);
            });
        });

        // Enhanced configuration commands
        app.Command("config", config =>
        {
            config.Description = "⚙️ Manage QCLI configuration";

            var subCommand = config.Argument<string>("subcommand", "Config subcommand (init, show, set, get, sample, validate)").IsRequired();
            var pathOption = config.Option("-p|--path <path>", "Configuration file path", CommandOptionType.SingleValue);
            var keyOption = config.Option("-k|--key <key>", "Configuration key", CommandOptionType.SingleValue);
            var valueOption = config.Option("--value <value>", "Configuration value", CommandOptionType.SingleValue);
            var globalOption = config.Option("-g|--global", "Use global configuration", CommandOptionType.NoValue);            config.OnExecute(() => GetCmd<ConfigCommand>().Execute(
                subCommand.ParsedValue,
                pathOption.Value(),
                keyOption.Value(),
                valueOption.Value()));
        });

        app.Command("doctor", doctor =>
        {
            doctor.Description = "🩺 Diagnose and fix common issues";
            
            var fixOption = doctor.Option("--fix", "Automatically fix issues when possible", CommandOptionType.NoValue);

            doctor.OnExecute(() =>
            {
                var doctorCommand = GetCmd<DoctorCommand>();
                return doctorCommand.Execute(fixOption.HasValue());
            });
        });

        app.Command("update", update =>
        {
            update.Description = "🔄 Update templates and check for new versions";
            
            var checkOption = update.Option("--check", "Check for updates without installing", CommandOptionType.NoValue);
            var templatesOption = update.Option("--templates", "Update templates only", CommandOptionType.NoValue);

            update.OnExecute(() =>
            {
                var updateCommand = GetCmd<UpdateCommand>();
                return updateCommand.Execute(checkOption.HasValue(), templatesOption.HasValue());
            });
        });

        // Legacy setup command for backward compatibility
        app.Command("setup", setup =>
        {
            setup.Description = "[DEPRECATED] Use 'add --all' instead. Generate boilerplate code for a new feature";

            var featureName = setup.Argument<string>("name", "Feature name (e.g., 'Order' or 'Product')").IsRequired();
            var fullOption = setup.Option("-f|--full", "Generate full implementation including events and mapping profile", CommandOptionType.NoValue);
            var entityTypeOption = setup.Option("-e|--entity-type <type>", "Entity type to use (Audited or FullyAudited)", CommandOptionType.SingleValue);
            entityTypeOption.DefaultValue = "Audited";

            setup.OnExecute(() =>
            {
                var config = CliConfiguration.Load();
                var addCommand = new AddCommand(config);
                return addCommand.Execute(
                    featureName.ParsedValue,
                    true, // equivalent to --all
                    false, false, false, false,
                    entityTypeOption.Value() ?? "Audited");
            });
        });

        return app.Execute(args);
    }

    private static void ShowWelcomeMessage()
    {
        var panel = new Panel(new Markup(
            "[bold blue]🚀 Welcome to QCLI![/]\n\n" +
            "[yellow]A powerful CLI tool for generating CRUD operations and managing CLIO architecture projects.[/]\n\n" +
            "[dim]Quick Start:[/]\n" +
            "• [green]qcli init[/] - Initialize QCLI in your project\n" +
            "• [green]qcli add Order --all[/] - Generate complete CRUD for Order entity\n" +
            "• [green]qcli config show[/] - View current configuration\n" +
            "• [green]qcli doctor[/] - Check for issues\n\n" +
            "[dim]Use [yellow]qcli <command> --help[/] for detailed command information.[/]"))
        {
            Header = new PanelHeader(" QCLI "),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Blue)
        };
        
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}