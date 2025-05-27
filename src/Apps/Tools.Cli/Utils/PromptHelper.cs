using System.Text.RegularExpressions;
using Spectre.Console;

namespace Cli.Utils;

public static partial class PromptHelper
{
    [GeneratedRegex(@"\{\d+\}", RegexOptions.Compiled)]
    private static partial Regex StringFormatPlaceholders();

    private static void WriteOutput(string tag, string color, string text, params object?[] values)
    {
        text = text.EscapeMarkup();
        var placeholderRegex = StringFormatPlaceholders();
        text = placeholderRegex.IsMatch(text)
            ? string.Format(placeholderRegex.Replace(text, match => $"[{color}]{match.Value}[/]"), values)
            : text;

        AnsiConsole.Markup($"[bold][{color}]{tag}:[/][/] ");
        AnsiConsole.MarkupLine(text);
    }

    public static void Info(string text, params object?[] values) => WriteOutput("inf", "turquoise2", text, values);
    public static void Done(string text, params object?[] values) => WriteOutput("done", "green", text, values);
    public static void Error(string text, params object?[] values) => WriteOutput("err", "red", text, values);
    public static void Warn(string text, params object?[] values) => WriteOutput("wrn", "gold1", text, values);
    public static void Cancelled(string text, params object?[] values) => WriteOutput("cancel", "olive", text, values);
    public static void Skip(string text, params object?[] values) => WriteOutput("skip", "turquoise2", text, values);

    public static bool AskYesNo(string question, bool defaultAnswerIsNo = true)
    {
        var opts = defaultAnswerIsNo ? "y/[underline]N[/]" : "[underline]Y[/]/n";
        AnsiConsole.Markup($"\n[bold][hotpink_1]ask:[/][/] {question.TrimEnd('?')}? [[[hotpink_1]{opts}[/]]]: ");
        var yes = Console.ReadLine()?.ToLowerInvariant() is "y" or "yes";

        if (Console.IsInputRedirected)
        {
            // For example when: `yes | qln-cli-app.exe` on ci/cd, we want to see the confirmation in the logs preferably so it's clear 
            Console.Write(yes ? "y" : "n");
            Console.WriteLine();
        }

        return yes;
    }
}