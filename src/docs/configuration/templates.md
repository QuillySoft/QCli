# Template System Configuration

The template system in QCLI controls how code is generated and allows for customization of the generated output. This system supports both built-in templates and custom templates to fit your specific project needs.

## Configuration Section

```json
{
  "templates": {
    "defaultTemplate": "clean-architecture",
    "customTemplatesPath": "templates",
    "enableCustomTemplates": false,
    "templateOverrides": {
      "entity": "custom-entity",
      "command": "enhanced-command"
    }
  }
}
```

## Template Settings

### Default Template

**Property:** `defaultTemplate`  
**Type:** `string`  
**Default:** `"clean-architecture"`  
**Valid Values:** `"clean-architecture"`, `"minimal"`, `"ddd"`

Sets the default template used for project initialization and code generation.

```json
{
  "templates": {
    "defaultTemplate": "clean-architecture"
  }
}
```

**Available Templates:**

#### clean-architecture (Default)
Complete Clean Architecture implementation with all layers and patterns.

**Features:**
- Separation of concerns across layers
- CQRS with MediatR
- Domain events and event handlers
- Repository pattern with EF Core
- Authorization and permissions
- Comprehensive test structure

**Project Structure:**
```
src/
├── Core/
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Events/
│   │   └── Mapping/
│   └── Domain/
│       ├── Entities/
│       ├── Events/
│       └── Permissions/
├── Apps/
│   └── Api/
│       └── Controllers/
└── Infra/
    └── Persistence/
        └── Configurations/
```

**Best For:**
- Enterprise applications
- Complex business logic
- Team development
- Long-term maintainability

#### minimal
Minimal setup with basic structure and reduced boilerplate.

**Features:**
- Simplified layer structure
- Basic CRUD operations
- Essential patterns only
- Reduced dependencies
- Quick project setup

**Project Structure:**
```
src/
├── Application/
│   ├── Features/
│   └── Common/
├── Domain/
│   └── Entities/
├── Infrastructure/
│   └── Persistence/
└── WebApi/
    └── Controllers/
```

**Best For:**
- Prototypes and MVPs
- Simple applications
- Learning and experimentation
- Quick development cycles

#### ddd
Domain-Driven Design focused with rich domain models and aggregates.

**Features:**
- Rich domain models
- Aggregate boundaries
- Domain services
- Value objects
- Domain events
- Specification pattern

**Project Structure:**
```
src/
├── Domain/
│   ├── Aggregates/
│   ├── ValueObjects/
│   ├── DomainServices/
│   └── Specifications/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   └── DomainEventHandlers/
└── Infrastructure/
    └── Repositories/
```

**Best For:**
- Complex business domains
- Domain expert collaboration
- Legacy system modernization
- Microservices architecture

### Custom Templates Path

**Property:** `customTemplatesPath`  
**Type:** `string`  
**Default:** `"templates"`

Specifies the directory where custom templates are stored.

```json
{
  "templates": {
    "customTemplatesPath": "custom-templates"
  }
}
```

**Directory Structure:**
```
custom-templates/
├── entity.template
├── command.template
├── query.template
├── controller.template
└── test.template
```

**Template File Example:**
```handlebars
// entity.template
using Domain.Common;

namespace Domain.{{PluralName}};

public sealed class {{SingularName}} : {{#if EntityType}}{{EntityType}}{{else}}BaseEntity{{/if}}
{
    public string Name { get; private set; }
    
    private {{SingularName}}() { } // EF Core constructor
    
    public {{SingularName}}(Guid id, string name) : base(id)
    {
        Name = name;
    }
    
    {{#if GenerateEvents}}
    public void UpdateName(string name)
    {
        Name = name;
        AddDomainEvent(new {{SingularName}}UpdatedEvent(Id, name));
    }
    {{/if}}
}
```

### Enable Custom Templates

**Property:** `enableCustomTemplates`  
**Type:** `boolean`  
**Default:** `false`

Controls whether custom templates can be used in addition to built-in templates.

```json
{
  "templates": {
    "enableCustomTemplates": true
  }
}
```

**When Enabled:**
- QCLI searches for custom templates in the specified path
- Custom templates override built-in templates with same name
- Additional validation for custom template syntax
- Support for custom template variables

**Custom Template Discovery:**
```bash
# List available templates (including custom)
qcli list templates

# Use custom template
qcli add product --template custom-entity

# Validate custom templates
qcli doctor --check-templates
```

### Template Overrides

**Property:** `templateOverrides`  
**Type:** `Dictionary<string, string>`  
**Default:** `{}`

Allows mapping specific template types to custom template names.

```json
{
  "templates": {
    "templateOverrides": {
      "entity": "custom-entity",
      "command": "enhanced-command",
      "query": "paginated-query",
      "controller": "api-controller-v2"
    }
  }
}
```

**Common Template Types:**
- `entity` - Domain entity templates
- `command` - Command handler templates
- `query` - Query handler templates
- `controller` - API controller templates
- `test` - Unit test templates
- `configuration` - Entity configuration templates
- `mapping` - Mapping profile templates
- `permissions` - Permission constant templates

## Template Engine Features

### Variable Substitution

Templates support variable replacement using handlebars syntax:

```handlebars
public class {{SingularName}}Controller : ControllerBase
{
    private readonly IMediator _mediator;
    
    public {{SingularName}}Controller(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<{{SingularName}}ForListDto>>> Get{{PluralName}}()
    {
        return await _mediator.Send(new Get{{PluralName}}Query());
    }
}
```

### Conditional Blocks

Include content based on configuration settings:

```handlebars
{{#if GenerateEvents}}
using Application.{{PluralName}}.Events;
{{/if}}

public sealed class Create{{SingularName}}Command
{
    {{#if GenerateEvents}}
    // Event publishing code
    await _publisher.Publish(new {{SingularName}}CreatedEvent(result.Id));
    {{/if}}
}
```

### Loops and Collections

Process collections of data:

```handlebars
{{#each Operations}}
[HttpPost("{{Name}}")]
public async Task<ActionResult> {{Name}}{{../SingularName}}()
{
    // {{Description}}
    return Ok();
}
{{/each}}
```

### Available Variables

Common variables available in templates:

| Variable | Description | Example |
|----------|-------------|---------|
| `SingularName` | Entity name (singular) | `Product` |
| `PluralName` | Entity name (plural) | `Products` |
| `EntityType` | Base entity type | `AuditedEntity` |
| `Namespace` | Project namespace | `MyApp.Domain` |
| `ProjectName` | Project name | `MyApp` |
| `Operations` | Selected operations | `["Create", "Read", "Update"]` |
| `GenerateEvents` | Generate events flag | `true/false` |
| `GenerateTests` | Generate tests flag | `true/false` |
| `GeneratePermissions` | Generate permissions flag | `true/false` |

## Command Line Usage

### Using Templates

```bash
# Use default template
qcli add product

# Use specific template
qcli add product --template minimal

# Use custom template (if enabled)
qcli add product --template my-custom-template

# Initialize project with template
qcli init --template ddd
```

### Template Management

```bash
# List available templates
qcli list templates

# Validate templates
qcli doctor --check-templates

# Update templates
qcli update templates

# Show template information
qcli list templates --detailed
```

## Creating Custom Templates

### Template Structure

Create a custom template directory:

```bash
mkdir custom-templates
cd custom-templates
```

### Entity Template Example

**File:** `entity.template`
```handlebars
using Domain.Common;
{{#if GenerateEvents}}
using Domain.Events;
{{/if}}

namespace Domain.{{PluralName}};

public sealed class {{SingularName}} : {{EntityType}}
{
    public string Name { get; private set; }
    
    // Additional properties
    {{#each Properties}}
    public {{Type}} {{Name}} { get; private set; }
    {{/each}}
    
    private {{SingularName}}() { } // EF Core constructor
    
    public {{SingularName}}(Guid id, string name) : base(id)
    {
        Name = name;
        {{#if GenerateEvents}}
        AddDomainEvent(new {{SingularName}}CreatedEvent(id, name));
        {{/if}}
    }
    
    public void UpdateName(string name)
    {
        Name = name;
        {{#if GenerateEvents}}
        AddDomainEvent(new {{SingularName}}UpdatedEvent(Id, name));
        {{/if}}
    }
}
```

### Command Template Example

**File:** `command.template`
```handlebars
using Application.Common;
using Application.Security;
using Domain.{{PluralName}};
{{#if GeneratePermissions}}
using Domain.PermissionsConstants;
{{/if}}
using MediatR;

namespace Application.{{PluralName}}.Commands.Create{{SingularName}};

{{#if GeneratePermissions}}
[Authorize(Permissions = [Permissions.{{PluralName}}.Actions.Create])]
{{/if}}
public sealed class Create{{SingularName}}Command({{SingularName}}ForCreateDto {{SingularName.ToLower}}Dto) : IRequest<Guid>
{
    public {{SingularName}}ForCreateDto {{SingularName}}Dto { get; } = {{SingularName.ToLower}}Dto;

    public sealed class Handler(IApplicationDbContext dbContext{{#if GenerateEvents}}, IPublisher publisher{{/if}}) 
        : IRequestHandler<Create{{SingularName}}Command, Guid>
    {
        public async Task<Guid> Handle(Create{{SingularName}}Command request, CancellationToken cancellationToken)
        {
            var {{SingularName.ToLower}} = new {{SingularName}}(Guid.NewGuid(), request.{{SingularName}}Dto.Name);
            
            dbContext.{{PluralName}}.Add({{SingularName.ToLower}});
            await dbContext.SaveChangesAsync(cancellationToken);
            
            {{#if GenerateEvents}}
            await publisher.Publish(new {{SingularName}}CreatedEvent({{SingularName.ToLower}}.Id), cancellationToken);
            {{/if}}
            
            return {{SingularName.ToLower}}.Id;
        }
    }
}
```

### Template Validation

Custom templates are validated for:

- **Syntax**: Proper handlebars syntax
- **Variables**: Required variables are present
- **Structure**: File structure matches expected patterns
- **Compilation**: Generated code compiles successfully

```bash
# Validate all templates
qcli doctor --check-templates

# Validate specific template
qcli doctor --check-template entity

# Test template generation
qcli add product --template custom-entity --dry-run
```

## Environment-Specific Templates

Configure different templates for different environments:

### Development Configuration
```json
{
  "templates": {
    "defaultTemplate": "clean-architecture",
    "enableCustomTemplates": true,
    "customTemplatesPath": "dev-templates",
    "templateOverrides": {
      "entity": "debug-entity",
      "test": "comprehensive-test"
    }
  }
}
```

### Production Configuration
```json
{
  "templates": {
    "defaultTemplate": "clean-architecture",
    "enableCustomTemplates": false,
    "templateOverrides": {
      "entity": "optimized-entity"
    }
  }
}
```

## Best Practices

### Template Organization

1. **Consistent Naming**: Use descriptive template names
2. **Version Control**: Include templates in source control
3. **Documentation**: Document custom template usage
4. **Testing**: Validate templates regularly

### Template Development

1. **Start Simple**: Begin with built-in templates
2. **Incremental Changes**: Make small modifications first
3. **Variable Usage**: Use all relevant template variables
4. **Error Handling**: Include proper error handling in templates

### Team Collaboration

1. **Shared Templates**: Use team-wide custom templates
2. **Template Reviews**: Review template changes like code
3. **Standards**: Establish template coding standards
4. **Training**: Train team on template customization

## Integration with Other Features

### Configuration Integration

Templates integrate with all configuration sections:

```json
{
  "project": {
    "name": "ECommerce",
    "namespace": "ECommerce.Core"
  },
  "codeGeneration": {
    "generateEvents": true,
    "generateTests": true
  },
  "templates": {
    "defaultTemplate": "ddd",
    "enableCustomTemplates": true
  }
}
```

### Path Integration

Templates respect path configuration:

```json
{
  "paths": {
    "domainPath": "src/Core/Domain",
    "applicationPath": "src/Core/Application"
  },
  "templates": {
    "customTemplatesPath": "templates/custom"
  }
}
```

### Command Integration

Templates work with all QCLI commands:

```bash
# Project initialization
qcli init --template ddd

# Entity generation
qcli add product --template custom-entity

# Project scaffolding
qcli scaffold MyApp --template minimal
```

## Troubleshooting

### Common Issues

**Template Not Found:**
```bash
❌ Template 'custom-entity' not found
✅ Enable custom templates in configuration
✅ Check customTemplatesPath setting
✅ Verify template file exists
```

**Template Syntax Errors:**
```bash
❌ Template syntax error: Unclosed conditional block
✅ Validate handlebars syntax
✅ Check {{#if}} and {{/if}} pairs
✅ Use qcli doctor --check-templates
```

**Variable Substitution Issues:**
```bash
❌ Variable 'SingularName' not found
✅ Check available template variables
✅ Verify variable spelling and case
✅ Use --dry-run to preview generation
```

### Debug Commands

```bash
# List all templates
qcli list templates

# Check template health
qcli doctor --check-templates

# Preview template output
qcli add product --template custom-entity --dry-run

# Validate configuration
qcli config show
```

## Advanced Features

### Template Inheritance

Create base templates that other templates can extend:

```handlebars
<!-- base-entity.template -->
using Domain.Common;

namespace Domain.{{PluralName}};

public sealed class {{SingularName}} : {{EntityType}}
{
    {{> property-definitions}}
    
    {{> constructor-definition}}
    
    {{> method-definitions}}
}
```

### Template Plugins

Extend template functionality with custom plugins:

```csharp
public class CustomTemplatePlugin : ITemplatePlugin
{
    public void RegisterHelpers(ITemplateEngine engine)
    {
        engine.RegisterHelper("pluralize", (context, arguments) => 
        {
            // Custom pluralization logic
        });
    }
}
```

### Dynamic Template Selection

Choose templates based on context:

```json
{
  "templates": {
    "conditionalTemplates": {
      "entityTypeAudited": "audited-entity",
      "entityTypeSimple": "simple-entity",
      "projectTypeMicroservice": "microservice-template"
    }
  }
}
```

## Related Documentation

- [Configuration Guide](./configuration.md) - Main configuration overview
- [Code Generation Settings](./code-generation.md) - Generation behavior control
- [Path Configuration](./paths.md) - File location settings
- [Advanced Customization](../advanced/customization.md) - Advanced template features
