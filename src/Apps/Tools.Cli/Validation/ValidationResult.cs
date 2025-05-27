using Spectre.Console;

namespace Tools.Cli.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T item);
}

public sealed class ValidationResult
{
    public bool IsValid { get; init; }
    public List<ValidationError> Errors { get; init; } = new();
    
    public static ValidationResult Success() => new() { IsValid = true };
    
    public static ValidationResult Failure(string error, string? property = null) => 
        new() { IsValid = false, Errors = { new ValidationError(error, property) } };
    
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) => 
        new() { IsValid = false, Errors = errors.ToList() };

    public void DisplayErrors()
    {
        if (!IsValid)
        {
            AnsiConsole.MarkupLine("[red]❌ Validation failed:[/]");
            foreach (var error in Errors)
            {
                var property = !string.IsNullOrEmpty(error.Property) ? $"[{error.Property}] " : "";
                AnsiConsole.MarkupLine($"   [red]•[/] {property}{error.Message}");
            }
        }
    }
}

public sealed record ValidationError(string Message, string? Property = null);
