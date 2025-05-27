namespace Tools.Cli.Templates;

public interface ITemplateEngine
{
    string ProcessTemplate(string templateName, object model);
    string ProcessTemplate(string templateContent, object model, string templateName);
    void RegisterTemplate(string name, string content);
    bool TemplateExists(string name);
}
