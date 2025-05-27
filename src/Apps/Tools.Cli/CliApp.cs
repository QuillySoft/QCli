using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Tools.Cli.Commands;
using Tools.Cli.Configuration;

namespace Tools.Cli;

public sealed class CliApp(IServiceProvider serviceProvider)
{
    private TCommand GetCmd<TCommand>() where TCommand : notnull => serviceProvider.GetRequiredService<TCommand>();

    public int Execute(string[] args)
    {        var app = new CommandLineApplication
        {
            Name = "qcli",
            Description = "🚀 QCLI - A powerful CLI tool for generating CRUD operations and managing CLIO architecture projects",
        };

        app.HelpOption(true);
        app.VersionOption("-v|--version", "1.0.0");

        // Add command to create CRUD operations
        app.Command("add", add =>
        {
            add.Description = "Generate CRUD operations for an entity";

            var entityName = add.Argument<string>("entity", "Entity name (e.g., 'Order' or 'Product')").IsRequired();
            var allOption = add.Option("-a|--all", "Generate all CRUD operations (Create, Read, Update, Delete)", CommandOptionType.NoValue);
            var createOption = add.Option("-c|--create", "Generate create operation", CommandOptionType.NoValue);
            var readOption = add.Option("-r|--read", "Generate read operations (queries)", CommandOptionType.NoValue);
            var updateOption = add.Option("-u|--update", "Generate update operation", CommandOptionType.NoValue);
            var deleteOption = add.Option("-d|--delete", "Generate delete operation", CommandOptionType.NoValue);
            var entityTypeOption = add.Option("-e|--entity-type <type>", "Entity type to use (Audited or FullyAudited)", CommandOptionType.SingleValue);
            entityTypeOption.DefaultValue = "Audited";

            add.OnExecute(() =>
            {
                var config = CliConfiguration.Load();
                var addCommand = new AddCommand(config);
                return addCommand.Execute(
                    entityName.ParsedValue,
                    allOption.HasValue(),
                    createOption.HasValue(),
                    updateOption.HasValue(),
                    deleteOption.HasValue(),
                    readOption.HasValue(),
                    entityTypeOption.Value());
            });
        });

        // Configuration commands
        app.Command("config", config =>
        {
            config.Description = "Manage quillysoft-cli configuration";

            var subCommand = config.Argument<string>("subcommand", "Config subcommand (init, show, set, get, sample)").IsRequired();
            var pathOption = config.Option("-p|--path <path>", "Configuration file path", CommandOptionType.SingleValue);
            var keyOption = config.Option("-k|--key <key>", "Configuration key", CommandOptionType.SingleValue);
            var valueOption = config.Option("--value <value>", "Configuration value", CommandOptionType.SingleValue);

            config.OnExecute(() => GetCmd<ConfigCommand>().Execute(
                subCommand.ParsedValue,
                pathOption.Value(),
                keyOption.Value(),
                valueOption.Value()));
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
}