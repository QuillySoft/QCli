using System.Text.RegularExpressions;
using System.Reflection;

namespace Tools.Cli.Templates;

public sealed class TemplateEngine : ITemplateEngine
{
    private readonly Dictionary<string, string> _templates = new();
    
    public TemplateEngine()
    {
        LoadEmbeddedTemplates();
    }
    
    public string ProcessTemplate(string templateName, object model)
    {
        if (!_templates.TryGetValue(templateName, out var template))
        {
            throw new ArgumentException($"Template '{templateName}' not found");
        }
        
        return ProcessTemplate(template, model, templateName);
    }
    
    public string ProcessTemplate(string templateContent, object model, string templateName)
    {
        try
        {
            var result = templateContent;
            var properties = model.GetType().GetProperties();
            
            foreach (var property in properties)
            {
                var value = property.GetValue(model)?.ToString() ?? "";
                var placeholder = $"{{{{{property.Name}}}}}";
                result = result.Replace(placeholder, value);
            }
            
            // Handle conditional blocks
            result = ProcessConditionals(result, model);
            
            // Handle loops
            result = ProcessLoops(result, model);
            
            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing template '{templateName}': {ex.Message}", ex);
        }
    }
    
    public void RegisterTemplate(string name, string content)
    {
        _templates[name] = content;
    }
    
    public bool TemplateExists(string name)
    {
        return _templates.ContainsKey(name);
    }
    
    private void LoadEmbeddedTemplates()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".template"));
        
        foreach (var resourceName in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                var templateName = Path.GetFileNameWithoutExtension(resourceName.Split('.').Last());
                _templates[templateName] = content;
            }
        }
    }
    
    private string ProcessConditionals(string content, object model)
    {
        var conditionalRegex = new Regex(@"\{\{#if\s+(\w+)\}\}(.*?)\{\{/if\}\}", RegexOptions.Singleline);
        
        return conditionalRegex.Replace(content, match =>
        {
            var propertyName = match.Groups[1].Value;
            var conditionalContent = match.Groups[2].Value;
            
            var property = model.GetType().GetProperty(propertyName);
            if (property != null)
            {
                var value = property.GetValue(model);
                if (value is bool boolValue && boolValue)
                {
                    return conditionalContent;
                }
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    return conditionalContent;
                }
            }
            
            return string.Empty;
        });
    }
    
    private string ProcessLoops(string content, object model)
    {
        var loopRegex = new Regex(@"\{\{#each\s+(\w+)\}\}(.*?)\{\{/each\}\}", RegexOptions.Singleline);
        
        return loopRegex.Replace(content, match =>
        {
            var propertyName = match.Groups[1].Value;
            var loopContent = match.Groups[2].Value;
            
            var property = model.GetType().GetProperty(propertyName);
            if (property?.GetValue(model) is System.Collections.IEnumerable enumerable)
            {
                var result = "";
                foreach (var item in enumerable)
                {
                    var itemContent = loopContent;
                    var itemProperties = item.GetType().GetProperties();
                    
                    foreach (var itemProperty in itemProperties)
                    {
                        var value = itemProperty.GetValue(item)?.ToString() ?? "";
                        var placeholder = $"{{{{{itemProperty.Name}}}}}";
                        itemContent = itemContent.Replace(placeholder, value);
                    }
                    
                    result += itemContent;
                }
                return result;
            }
            
            return string.Empty;
        });
    }
}
