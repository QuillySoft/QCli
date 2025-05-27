using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools.Cli;
using Tools.Cli.Commands;
using PromptHelper = Tools.Cli.Utils.PromptHelper;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Local.json", optional: true)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);

// Register commands
services.AddTransient<SetupCommand>(); // Keep for backward compatibility
services.AddTransient<ConfigCommand>();
services.AddLogging();

var cliApp = new CliApp(services.BuildServiceProvider(true));
try
{
    var result = cliApp.Execute(args);
    return result;
}
catch (Exception e)
{
    PromptHelper.Error(e.ToString());
    return 1;
}