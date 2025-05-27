using Spectre.Console;

namespace Tools.Cli.Utils;

public static class ProgressHelper
{
    public static async Task ExecuteWithProgress<T>(
        string title,
        IEnumerable<T> items,
        Func<T, Task<string>> action,
        bool showDetails = true)
    {
        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(title);
                var itemsList = items.ToList();
                task.MaxValue = itemsList.Count;

                foreach (var item in itemsList)
                {
                    var result = await action(item);
                    if (showDetails)
                    {
                        AnsiConsole.MarkupLine($"[green]✓[/] {result}");
                    }
                    task.Increment(1);
                }
            });
    }

    public static void ExecuteWithProgress<T>(
        string title,
        IEnumerable<T> items,
        Func<T, string> action,
        bool showDetails = true)
    {
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask(title);
                var itemsList = items.ToList();
                task.MaxValue = itemsList.Count;

                foreach (var item in itemsList)
                {
                    var result = action(item);
                    if (showDetails)
                    {
                        AnsiConsole.MarkupLine($"[green]✓[/] {result}");
                    }
                    task.Increment(1);
                }
            });
    }

    public static void ShowSpinner(string message, Action action)
    {
        AnsiConsole.Status()
            .Start(message, ctx =>
            {
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));
                action();
            });
    }

    public static async Task ShowSpinnerAsync(string message, Func<Task> action)
    {
        await AnsiConsole.Status()
            .StartAsync(message, async ctx =>
            {
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));
                await action();
            });
    }
}
