using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools.Cli;
using Tools.Cli.Commands;
using Tools.Cli.Templates;
using PromptHelper = Tools.Cli.Utils.PromptHelper;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Local.json", optional: true)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);

// Register template engine
services.AddSingleton<ITemplateEngine, TemplateEngine>();

// Register commands
services.AddTransient<SetupCommand>(); // Keep for backward compatibility
services.AddTransient<ConfigCommand>();
services.AddTransient<InitCommand>();
services.AddTransient<ScaffoldCommand>();
services.AddTransient<ListCommand>();
services.AddTransient<DoctorCommand>();
services.AddTransient<UpdateCommand>();
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