using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tools.Cli.Configuration;

public sealed class EnhancedCliConfiguration
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "https://quillysoft.com/schemas/qcli-config.json";
    
    public string Version { get; set; } = "1.0";
    public ProjectInfo Project { get; set; } = new();
    public ProjectPaths Paths { get; set; } = new();
    public CodeGenerationSettings CodeGeneration { get; set; } = new();
    public TemplateSettings Templates { get; set; } = new();
    public string ProjectType { get; set; } = "CLIO";
    public Dictionary<string, object> Extensions { get; set; } = new();

    public static string GetSampleConfiguration()
    {
        var sample = new EnhancedCliConfiguration
        {
            Project = new ProjectInfo
            {
                Name = "MyProject",
                Namespace = "MyProject",
                Description = "A sample CLIO project",
                Author = "Developer Name"
            },
            Paths = new ProjectPaths
            {
                DomainPath = "src/Domain",
                ApplicationPath = "src/Application",
                PersistencePath = "src/Infrastructure/Persistence",
                ApiPath = "src/WebApi",
                ApplicationTestsPath = "tests/Application.Tests",
                IntegrationTestsPath = "tests/Integration.Tests"
            },
            CodeGeneration = new CodeGenerationSettings
            {
                GenerateTests = true,
                GeneratePermissions = true,
                GenerateEvents = true,
                GenerateMappingProfiles = true
            },
            Templates = new TemplateSettings
            {
                DefaultTemplate = "clio",
                CustomTemplatesPath = "templates",
                EnableCustomTemplates = false
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(sample, options);
    }
}

public sealed class ProjectInfo
{
    public string Name { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string Description { get; set; } = "";
    public string Author { get; set; } = "";
    public string Version { get; set; } = "1.0.0";
}

public sealed class TemplateSettings
{
    public string DefaultTemplate { get; set; } = "clio";
    public string CustomTemplatesPath { get; set; } = "templates";
    public bool EnableCustomTemplates { get; set; } = false;
    public Dictionary<string, string> TemplateOverrides { get; set; } = new();
}
