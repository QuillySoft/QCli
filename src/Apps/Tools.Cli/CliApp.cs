using Cli.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Cli;

public sealed class CliApp(IServiceProvider serviceProvider)
{
    private TCommand GetCmd<TCommand>() where TCommand : notnull => serviceProvider.GetRequiredService<TCommand>();

    public int Execute(string[] args)
    {
        var app = new CommandLineApplication
        {
            Name = "Quillysoft",
            Description = "CLI for Quillysoft",
        };

        app.HelpOption(true);

        app.Command("setup", setup =>
        {
            setup.Description = "Generate boilerplate code for a new feature (controller, CQS, domain entity, events)";

            var featureName = setup.Argument<string>("name", "Feature name (e.g., 'Order' or 'Product')").IsRequired();
            var fullOption = setup.Option("-f|--full", "Generate full implementation including events and mapping profile", CommandOptionType.NoValue);
            var entityTypeOption = setup.Option("-e|--entity-type <type>", "Entity type to use (Audited or FullyAudited)", CommandOptionType.SingleValue);
            entityTypeOption.DefaultValue = "Audited";

            setup.OnExecute(() => GetCmd<SetupCommand>().Execute(
                featureName.ParsedValue,
                fullOption.HasValue(),
                entityTypeOption.Value() ?? "Audited"));
        });

        return app.Execute(args);
    }
}