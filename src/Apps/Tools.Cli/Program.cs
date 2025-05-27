using Cli;
using Cli.Commands;
using Cli.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Local.json", optional: true)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);

services.AddTransient<SetupCommand>();
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