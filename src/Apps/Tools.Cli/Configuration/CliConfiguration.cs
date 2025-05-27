using System.Text.Json;

namespace Cli.Configuration;

public sealed class CliConfiguration
{
    public ProjectPaths Paths { get; set; } = new();
    public CodeGenerationSettings CodeGeneration { get; set; } = new();
    public string ProjectType { get; set; } = "CLIO";

    public static CliConfiguration Load(string? configPath = null)
    {
        // Try to load from specified config path first
        if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
        {
            return LoadFromFile(configPath);
        }

        // Try to find config in current directory and parent directories
        var currentDir = Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(currentDir);

        while (dir != null)
        {
            var configFile = Path.Combine(dir.FullName, "quillysoft-cli.json");
            if (File.Exists(configFile))
            {
                return LoadFromFile(configFile);
            }
            dir = dir.Parent;
        }

        // Return default configuration with auto-detected paths
        return CreateDefaultConfiguration();
    }

    private static CliConfiguration LoadFromFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<CliConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });
            return config ?? CreateDefaultConfiguration();
        }
        catch
        {
            return CreateDefaultConfiguration();
        }
    }

    private static CliConfiguration CreateDefaultConfiguration()
    {
        return new CliConfiguration
        {
            Paths = ProjectPaths.AutoDetect(),
            CodeGeneration = new CodeGenerationSettings(),
            ProjectType = "CLIO"
        };
    }

    public void Save(string? configPath = null)
    {
        configPath ??= Path.Combine(Directory.GetCurrentDirectory(), "quillysoft-cli.json");
        
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        File.WriteAllText(configPath, json);
    }

    public static CliConfiguration CreateSampleConfiguration()
    {
        return new CliConfiguration
        {
            ProjectType = "CLIO",
            Paths = new ProjectPaths
            {
                RootPath = "c:\\projects\\MyProject",
                ApiPath = "src\\Apps\\Api",
                ApplicationPath = "src\\Core\\Application",
                DomainPath = "src\\Core\\Domain",
                PersistencePath = "src\\Infra\\Persistence",
                ApplicationTestsPath = "tests\\Application\\ApplicationTests",
                IntegrationTestsPath = "tests\\Infra\\InfraTests\\Controllers",
                ControllersPath = "src\\Apps\\Api\\Controllers"
            },
            CodeGeneration = new CodeGenerationSettings
            {
                DefaultEntityType = "Audited",
                GenerateEvents = true,
                GenerateMappingProfiles = true,
                GeneratePermissions = true,
                GenerateTests = true
            }
        };
    }
}

public sealed class ProjectPaths
{
    public string RootPath { get; set; } = string.Empty;
    public string ApiPath { get; set; } = "src\\Apps\\Api";
    public string ApplicationPath { get; set; } = "src\\Core\\Application";
    public string DomainPath { get; set; } = "src\\Core\\Domain";
    public string PersistencePath { get; set; } = "src\\Infra\\Persistence";
    public string ApplicationTestsPath { get; set; } = "tests\\Application\\ApplicationTests";
    public string IntegrationTestsPath { get; set; } = "tests\\Infra\\InfraTests\\Controllers";
    public string ControllersPath { get; set; } = "src\\Apps\\Api\\Controllers";

    public string GetFullPath(string relativePath)
    {
        return Path.Combine(RootPath, relativePath);
    }

    public static ProjectPaths AutoDetect()
    {
        try
        {
            var rootPath = Paths.GetRepositoryRootPath();
            return new ProjectPaths
            {
                RootPath = rootPath
            };
        }
        catch
        {
            return new ProjectPaths
            {
                RootPath = Directory.GetCurrentDirectory()
            };
        }
    }
}

public sealed class CodeGenerationSettings
{
    public string DefaultEntityType { get; set; } = "Audited";
    public bool GenerateEvents { get; set; } = false;
    public bool GenerateMappingProfiles { get; set; } = false;
    public bool GeneratePermissions { get; set; } = true;
    public bool GenerateTests { get; set; } = true;
}
