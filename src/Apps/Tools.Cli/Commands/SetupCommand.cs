using Spectre.Console;

namespace Tools.Cli.Commands;

public sealed class SetupCommand
{
    public int Execute(string featureName, bool full = false, string entityType = "Audited")
    {
        AnsiConsole.MarkupLine($"[green]Setting up feature for[/] [blue]{featureName}[/]");

        // Standardize feature name
        var normalizedFeatureName = NormalizeFeatureName(featureName);
        var singularFeatureName = normalizedFeatureName.EndsWith("s") ? normalizedFeatureName[..^1] : normalizedFeatureName;
        var pluralFeatureName = normalizedFeatureName.EndsWith("s") ? normalizedFeatureName : normalizedFeatureName + "s";

        // Base paths
        var basePath = @"c:\projects\CLIO";
        var controllersPath = Path.Combine(basePath, @"src\Apps\Api\Controllers");
        var applicationPath = Path.Combine(basePath, @"src\Core\Application");
        var domainPath = Path.Combine(basePath, @"src\Core\Domain");
        var permissionsPath = Path.Combine(domainPath, "PermissionsConstants");
        var persistencePath = Path.Combine(basePath, @"src\Infra\Persistence");
        var featurePath = Path.Combine(applicationPath, pluralFeatureName);
        var testsPath = Path.Combine(basePath, @"tests\Application\ApplicationTests", pluralFeatureName);
        var integrationTestsPath = Path.Combine(basePath, @"tests\Infra\InfraTests\Controllers");

        // Create Application folders
        Directory.CreateDirectory(featurePath);
        Directory.CreateDirectory(Path.Combine(featurePath, "Commands"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Commands", $"Create{singularFeatureName}"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Commands", $"Update{singularFeatureName}"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Commands", $"Delete{singularFeatureName}"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Queries"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Queries", $"Get{pluralFeatureName}"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Queries", $"Get{singularFeatureName}ById"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Events"));
        Directory.CreateDirectory(Path.Combine(featurePath, "Mapping"));

        // Create Domain folder
        var domainFeaturePath = Path.Combine(domainPath, pluralFeatureName);
        Directory.CreateDirectory(domainFeaturePath);

        // Create Persistence folder for entity configuration
        var entityConfigPath = Path.Combine(persistencePath, "Configurations", "Tenants", pluralFeatureName);
        Directory.CreateDirectory(entityConfigPath);

        // Create Tests folders
        Directory.CreateDirectory(testsPath);
        Directory.CreateDirectory(Path.Combine(testsPath, "Commands"));
        Directory.CreateDirectory(Path.Combine(testsPath, "Queries"));
        Directory.CreateDirectory(Path.Combine(integrationTestsPath));

        // Generate files
        GenerateDomainEntity(domainFeaturePath, singularFeatureName, pluralFeatureName, entityType);
        GenerateEntityConfiguration(entityConfigPath, singularFeatureName, pluralFeatureName);
        GenerateControllerFile(controllersPath, singularFeatureName, pluralFeatureName);
        GenerateQueries(featurePath, singularFeatureName, pluralFeatureName);
        GenerateCommands(featurePath, singularFeatureName, pluralFeatureName);
        GeneratePermissionsConstants(permissionsPath, singularFeatureName, pluralFeatureName);

        // Generate Unit Tests
        GenerateCommandTests(testsPath, singularFeatureName, pluralFeatureName);
        GenerateQueryTests(testsPath, singularFeatureName, pluralFeatureName);
        GenerateControllerTests(integrationTestsPath, singularFeatureName, pluralFeatureName);

        if (full)
        {
            GenerateEvents(featurePath, singularFeatureName, pluralFeatureName);
            GenerateMappingProfile(featurePath, singularFeatureName, pluralFeatureName);
        }

        // Display instructions for manually adding the DbSet to ITenantDbContext and TenantDbContext
        DisplayDbContextInstructions(singularFeatureName, pluralFeatureName);
        DisplayPermissionsInstructions(singularFeatureName, pluralFeatureName);

        AnsiConsole.MarkupLine($"[green]Feature setup complete for[/] [blue]{featureName}[/]");
        AnsiConsole.MarkupLine($"[yellow]Note: This is boilerplate code. You'll need to customize it with your business logic.[/]");

        return 0;
    }

    private string NormalizeFeatureName(string name)
    {
        // Convert first letter to uppercase and keep the rest unchanged
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        return char.ToUpper(name[0]) + name.Substring(1);
    }

    private void GenerateDomainEntity(string domainFeaturePath, string singularName, string pluralName, string entityType)
    {
        var entityPath = Path.Combine(domainFeaturePath, $"{singularName}.cs");

        var baseEntityType = entityType == "FullyAudited"
            ? "FullyAuditedEntity<Guid>"
            : "AuditedEntity<Guid>";

        var content = $@"using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.{pluralName};

/// <summary>
/// Represents a {singularName.ToLower()} in the system.
/// </summary>
public sealed class {singularName} : {baseEntityType}
{{
    /// <summary>
    /// Name of the {singularName.ToLower()}.
    /// </summary>
    public string Name {{ get; private set; }}
    
    // Add more properties as needed
    
    #region Constructors
    private {singularName}(Guid id) : base(id)
    {{
    }}

    /// <summary>
    /// Creates a new {singularName.ToLower()}.
    /// </summary>
    public static {singularName} Create(
        Guid id,
        string name)
    {{
        if (string.IsNullOrWhiteSpace(name))
        {{
            throw new ArgumentException(""{singularName} name cannot be empty"", nameof(name));
        }}

        var {singularName.ToLower()} = new {singularName}(id)
        {{
            Name = name
        }};

        return {singularName.ToLower()};
    }}
    #endregion

    #region Management
    /// <summary>
    /// Updates the {singularName.ToLower()} properties.
    /// </summary>
    public void Update(string name)
    {{
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(""{singularName} name cannot be empty"", nameof(name));

        Name = name;
    }}

    /// <summary>
    /// Marks the {singularName.ToLower()} as deleted.
    /// </summary>
    public override void Delete(Guid userId)
    {{
        // Add your delete validation logic here if needed
        // Example: if (SomeCollection.Count > 0) throw new InvalidOperationException(""Cannot delete with existing relations"");
        
        DeletedAt = DateTimeProvider.Instance.UtcNow();
        DeletedByUserId = userId;
    }}
    #endregion
}}
";
        File.WriteAllText(entityPath, content);
        AnsiConsole.MarkupLine($"[green]Created:[/] {entityPath}");
    }

    private void GenerateEntityConfiguration(string entityConfigPath, string singularName, string pluralName)
    {
        var configPath = Path.Combine(entityConfigPath, $"{singularName}EntityConfiguration.cs");

        var content = $@"using Domain.{pluralName};
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Tenants.{pluralName};

public sealed class {singularName}EntityConfiguration : IEntityTypeConfiguration<{singularName}>
{{
    public void Configure(EntityTypeBuilder<{singularName}> builder)
    {{
        // Basic configuration

        // Uncomment if you want soft delete behavior
        // builder.HasQueryFilter(x => x.DeletedAt == null && x.DeletedByUser == null);
        
        // Add any relationships or owned types here
        // Example for owned types:
        // builder.OwnsOne(x => x.SomeValueObject);
        
        // Example for relationships:
        // builder.HasMany(x => x.RelatedEntities)
        //     .WithOne(x => x.{singularName})
        //     .HasForeignKey(x => x.{singularName}Id)
        //     .OnDelete(DeleteBehavior.Restrict);
    }}
}}
";
        File.WriteAllText(configPath, content);
        AnsiConsole.MarkupLine($"[green]Created:[/] {configPath}");
    }

    private void DisplayDbContextInstructions(string singularName, string pluralName)
    {
        AnsiConsole.MarkupLine($"[yellow]Manual Step Required:[/] Add DbSet to ITenantDbContext and TenantDbContext");
        AnsiConsole.MarkupLine($"[blue]1. Open:[/] src/Core/Application/Common/ITenantDbContext.cs");
        AnsiConsole.MarkupLine($"[blue]   Add:[/] public DbSet<{singularName}> {pluralName} {{ get; init; }}");
        AnsiConsole.MarkupLine($"[blue]2. Open:[/] src/Infra/Persistence/TenantDbContext.cs");
        AnsiConsole.MarkupLine($"[blue]   Add:[/] public DbSet<{singularName}> {pluralName} {{ get; init; }} = null!;");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[blue]Remember to add the import statement to both files:[/] using Domain.{pluralName};");
        AnsiConsole.WriteLine();
    }

    private void GenerateControllerFile(string controllersPath, string singularName, string pluralName)
    {
        var controllerName = $"{pluralName}Controller";
        var filePath = Path.Combine(controllersPath, $"{controllerName}.cs");

        var content = $@"using Application.Common;
using Application.Pagination;
using Application.{pluralName}.Commands.Create{singularName};
using Application.{pluralName}.Commands.Delete{singularName};
using Application.{pluralName}.Commands.Update{singularName};
using Application.{pluralName}.Queries.Get{pluralName};
using Application.{pluralName}.Queries.Get{singularName}ById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// {pluralName} controller.
/// </summary>
/// <param name=""sender""></param>
[Route(""[controller]"")]
public sealed class {controllerName}(ISender sender) : BaseController
{{
    /// <summary>
    /// Get a paginated list of {pluralName.ToLower()}.
    /// </summary>
    /// <param name=""requestDto""></param>
    /// <param name=""cancellationToken""></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<{singularName}ForListDto>>> Get([FromQuery] {singularName}ForRequestDto requestDto, CancellationToken cancellationToken)
    {{
        return Ok(await sender.Send(new Get{pluralName}Query(requestDto), cancellationToken));
    }}

    /// <summary>
    /// Get a {singularName.ToLower()} by id.
    /// </summary>
    /// <param name=""id""></param>
    /// <param name=""cancellationToken""></param>
    /// <returns></returns>
    [HttpGet(""{{id:guid}}"")]
    public async Task<ActionResult<{singularName}ForReadDto>> GetById(Guid id, CancellationToken cancellationToken)
    {{
        return Ok(await sender.Send(new Get{singularName}ByIdQuery(id), cancellationToken));
    }}

    /// <summary>
    /// Create a new {singularName.ToLower()}.
    /// </summary>
    /// <param name=""requestDto""></param>
    /// <param name=""cancellationToken""></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> Post({singularName}ForCreateUpdateDto requestDto, CancellationToken cancellationToken)
    {{
        var {singularName.ToLower()}Id = await sender.Send(new Create{singularName}Command(requestDto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new {{ id = {singularName.ToLower()}Id }}, {singularName.ToLower()}Id);
    }}

    /// <summary>
    /// Update a {singularName.ToLower()}.
    /// </summary>
    /// <param name=""id""></param>
    /// <param name=""requestDto""></param>
    /// <param name=""cancellationToken""></param>
    /// <returns></returns>
    [HttpPut(""{{id:guid}}"")]
    public async Task<IActionResult> Put(Guid id, {singularName}ForCreateUpdateDto requestDto, CancellationToken cancellationToken)
    {{
        return Ok(await sender.Send(new Update{singularName}Command(id, requestDto), cancellationToken));
    }}

    /// <summary>
    /// Delete a {singularName.ToLower()}.
    /// </summary>
    /// <param name=""id""></param>
    /// <returns></returns>
    [HttpDelete(""{{id:guid}}"")]
    public async Task<IActionResult> Delete(Guid id)
    {{
        return Ok(await sender.Send(new Delete{singularName}Command(id)));
    }}
}}
";
        File.WriteAllText(filePath, content);
        AnsiConsole.MarkupLine($"[green]Created:[/] {filePath}");
    }

    private void GenerateQueries(string featurePath, string singularName, string pluralName)
    {
        // Get{plural}Query
        var getQueryPath = Path.Combine(featurePath, "Queries", $"Get{pluralName}", $"Get{pluralName}Query.cs");
        var getQueryContent = $@"using Application.Common;
using Application.Pagination;
using Application.Security;
using Domain.{pluralName};
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.{pluralName}.Queries.Get{pluralName};

//[Authorize(Permissions = [Permissions.{pluralName}.Actions.ViewAssigned, Permissions.{pluralName}.Actions.View])]
public sealed class Get{pluralName}Query({singularName}ForRequestDto requestDto) : ITenantQuery<PaginatedList<{singularName}ForListDto>>
{{
    public {singularName}ForRequestDto RequestDto {{ get; }} = requestDto;

    public sealed class Handler(ITenantDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<Get{pluralName}Query, PaginatedList<{singularName}ForListDto>>
    {{
        public async Task<PaginatedList<{singularName}ForListDto>> Handle(Get{pluralName}Query request, CancellationToken cancellationToken)
        {{
            var query = dbContext.{pluralName}
                .AsNoTracking();

            query = FilterByCurrentUser(query);

            // Add your mapping configuration here
            //var adapter = new TypeAdapterConfig();
            //adapter.Apply(new {singularName}MappingProfile());

            return await query.OrderBy(p => p.Name)  // Adjust ordering property as needed
                //.ProjectToType<{singularName}ForListDto>(adapter)
                .Select(p => new {singularName}ForListDto
                {{
                    Id = p.Id,
                    Name = p.Name,
                    // Map other properties from entity to DTO
                    CreatedAt = p.CreatedAt
                }})
                .PaginatedListAsync(request.RequestDto, cancellationToken);
        }}

        private IQueryable<{singularName}> FilterByCurrentUser(IQueryable<{singularName}> query)
        {{
            // Add your filtering logic here, for example:
            //if (currentUserService.HasAnyPermissionOf([Permissions.{pluralName}.Actions.View]))
            //    return query;

            //var userId = currentUserService.GetUserId();
            //return query.Where(p => p.CreatedByUserId == userId);
            
            return query; // Default: return all items
        }}
    }}
}}

public sealed record {singularName}ForRequestDto : PaginatedRequestDto
{{
    // Add custom filter properties here
}}

public record {singularName}ForListDto
{{
    public Guid Id {{ get; init; }}
    public string Name {{ get; init; }}
    public DateTime CreatedAt {{ get; init; }}
    // Add other properties needed for list view
}}
";
        File.WriteAllText(getQueryPath, getQueryContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {getQueryPath}");

        // Get{singular}ByIdQuery
        var getByIdQueryPath = Path.Combine(featurePath, "Queries", $"Get{singularName}ById", $"Get{singularName}ByIdQuery.cs");
        var getByIdQueryContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Application.Security;
using Domain.{pluralName};
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.{pluralName}.Queries.Get{singularName}ById;

//[Authorize(Permissions = [Permissions.{pluralName}.Actions.Profile])]
public sealed class Get{singularName}ByIdQuery(Guid id) : ITenantQuery<{singularName}ForReadDto>
{{
    public Guid Id {{ get; }} = id;

    public sealed class Handler(ITenantDbContext dbContext) : IRequestHandler<Get{singularName}ByIdQuery, {singularName}ForReadDto>
    {{
        public async Task<{singularName}ForReadDto> Handle(Get{singularName}ByIdQuery request, CancellationToken cancellationToken)
        {{
            var entity = await dbContext.{pluralName}
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) 
                ?? throw new NotFoundException(nameof({singularName}), request.Id);

            return new {singularName}ForReadDto
            {{
                Id = entity.Id,
                Name = entity.Name,
                // Map other properties from entity to DTO
                CreatedAt = entity.CreatedAt
            }};
        }}
    }}
}}

public record {singularName}ForReadDto
{{
    public Guid Id {{ get; init; }}
    public string Name {{ get; init; }}
    public DateTime CreatedAt {{ get; init; }}
    // Add other properties needed for detailed view
}}
";
        File.WriteAllText(getByIdQueryPath, getByIdQueryContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {getByIdQueryPath}");
    }

    private void GenerateCommands(string featurePath, string singularName, string pluralName)
    {
        // Create{singular}Command
        var createCommandPath = Path.Combine(featurePath, "Commands", $"Create{singularName}", $"Create{singularName}Command.cs");
        var createCommandContent = $@"using Application.Common;
using Application.{pluralName}.Events;
using Application.Security;
using Domain.{pluralName};
using MediatR;

namespace Application.{pluralName}.Commands.Create{singularName};

//[Authorize(Permissions = [Permissions.{pluralName}.Actions.Create])]
public sealed class Create{singularName}Command({singularName}ForCreateUpdateDto dto) : ITenantCommand<Guid>
{{
    public {singularName}ForCreateUpdateDto Dto {{ get; }} = dto;

    public sealed class Handler(ITenantDbContext dbContext, IPublisher publisher) : IRequestHandler<Create{singularName}Command, Guid>
    {{
        public async Task<Guid> Handle(Create{singularName}Command request, CancellationToken cancellationToken)
        {{
            var dto = request.Dto;
            var entity = {singularName}.Create(
                Guid.NewGuid(),
                dto.Name
                // Add other required parameters for your entity
            );
            
            // Add any related entities or additional setup here
            
            dbContext.{pluralName}.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Uncomment if implementing events
            //await publisher.Publish(new {singularName}CreatedEvent(entity.Id), cancellationToken);
            return entity.Id;
        }}
    }}
}}

public sealed record {singularName}ForCreateUpdateDto
{{
    public required string Name {{ get; init; }}
    // Add other properties needed for creation/update
}}
";
        File.WriteAllText(createCommandPath, createCommandContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {createCommandPath}");

        // Update{singular}Command
        var updateCommandPath = Path.Combine(featurePath, "Commands", $"Update{singularName}", $"Update{singularName}Command.cs");
        var updateCommandContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Application.{pluralName}.Events;
using Application.Security;
using Domain.{pluralName};
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.{pluralName}.Commands.Update{singularName};

//[Authorize(Permissions = [Permissions.{pluralName}.Actions.Update])]
public sealed class Update{singularName}Command(Guid id, {singularName}ForCreateUpdateDto dto) : ITenantCommand<Unit>
{{
    public Guid Id {{ get; }} = id;
    public {singularName}ForCreateUpdateDto Dto {{ get; }} = dto;

    public sealed class Handler(ITenantDbContext dbContext, IPublisher publisher) : IRequestHandler<Update{singularName}Command, Unit>
    {{
        public async Task<Unit> Handle(Update{singularName}Command request, CancellationToken cancellationToken)
        {{
            var entity = await dbContext.{pluralName}
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof({singularName}), request.Id);

            // Update entity properties
            entity.Update(
                request.Dto.Name
                // Add other update parameters as needed
            );

            dbContext.{pluralName}.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Uncomment if implementing events
            //await publisher.Publish(new {singularName}UpdatedEvent(entity.Id), cancellationToken);
            return Unit.Value;
        }}
    }}
}}
";
        File.WriteAllText(updateCommandPath, updateCommandContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {updateCommandPath}");

        // Delete{singular}Command
        var deleteCommandPath = Path.Combine(featurePath, "Commands", $"Delete{singularName}", $"Delete{singularName}Command.cs");
        var deleteCommandContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Application.{pluralName}.Events;
using Application.Security;
using Domain.{pluralName};
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.{pluralName}.Commands.Delete{singularName};

//[Authorize(Permissions = [Permissions.{pluralName}.Actions.Delete])]
public sealed class Delete{singularName}Command(Guid id) : ITenantCommand<Unit>
{{
    public Guid Id {{ get; }} = id;

    public sealed class Handler(ITenantDbContext dbContext, IPublisher publisher, ICurrentUserService currentUserService) : IRequestHandler<Delete{singularName}Command, Unit>
    {{
        public async Task<Unit> Handle(Delete{singularName}Command request, CancellationToken cancellationToken)
        {{
            var entity = await dbContext.{pluralName}
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof({singularName}), request.Id);

            entity.Delete(currentUserService.GetUserId());
            
            dbContext.{pluralName}.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            // Uncomment if implementing events
            //await publisher.Publish(new {singularName}DeletedEvent(entity.Id), cancellationToken);
            return Unit.Value;
        }}
    }}
}}
";
        File.WriteAllText(deleteCommandPath, deleteCommandContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {deleteCommandPath}");
    }

    private void GenerateEvents(string featurePath, string singularName, string pluralName)
    {
        // {singular}CreatedEvent
        var createdEventPath = Path.Combine(featurePath, "Events", $"{singularName}CreatedEvent.cs");
        var createdEventContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Domain.AuditLogs;
using Domain.{pluralName};
using MediatR;

namespace Application.{pluralName}.Events;

public sealed class {singularName}CreatedEvent(Guid {singularName.ToLower()}Id) : INotification
{{
    public Guid {singularName}Id {{ get; }} = {singularName.ToLower()}Id;

    public sealed class Handler(ITenantDbContext dbContext) : INotificationHandler<{singularName}CreatedEvent>
    {{
        public async Task Handle({singularName}CreatedEvent notification, CancellationToken cancellationToken)
        {{
            var entity = await dbContext.{pluralName}.FindAsync(notification.{singularName}Id) 
                ?? throw new NotFoundException(nameof({singularName}), notification.{singularName}Id);

            await AddAuditLog(entity, cancellationToken);
        }}

        private async Task AddAuditLog({singularName} entity, CancellationToken cancellationToken)
        {{
            var auditLog = new AuditLog(Guid.NewGuid(), ActorType.User, ""{singularName} Created"", AuditLogEventType.{singularName}Created);

            // Set the entity ID for the audit log
            auditLog.SetEntityId(entity.Id);
            
            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync(cancellationToken);
        }}
    }}
}}
";
        File.WriteAllText(createdEventPath, createdEventContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {createdEventPath}");

        // {singular}UpdatedEvent
        var updatedEventPath = Path.Combine(featurePath, "Events", $"{singularName}UpdatedEvent.cs");
        var updatedEventContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Domain.AuditLogs;
using Domain.{pluralName};
using MediatR;

namespace Application.{pluralName}.Events;

public sealed class {singularName}UpdatedEvent(Guid {singularName.ToLower()}Id) : INotification
{{
    public Guid {singularName}Id {{ get; }} = {singularName.ToLower()}Id;

    public sealed class Handler(ITenantDbContext dbContext) : INotificationHandler<{singularName}UpdatedEvent>
    {{
        public async Task Handle({singularName}UpdatedEvent notification, CancellationToken cancellationToken)
        {{
            var entity = await dbContext.{pluralName}.FindAsync(notification.{singularName}Id) 
                ?? throw new NotFoundException(nameof({singularName}), notification.{singularName}Id);

            await AddAuditLog(entity, cancellationToken);
        }}

        private async Task AddAuditLog({singularName} entity, CancellationToken cancellationToken)
        {{
            var auditLog = new AuditLog(Guid.NewGuid(), ActorType.User, ""{singularName} Updated"", AuditLogEventType.{singularName}Updated);

            // Set the entity ID for the audit log
            auditLog.SetEntityId(entity.Id);
            
            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync(cancellationToken);
        }}
    }}
}}
";
        File.WriteAllText(updatedEventPath, updatedEventContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {updatedEventPath}");

        // {singular}DeletedEvent
        var deletedEventPath = Path.Combine(featurePath, "Events", $"{singularName}DeletedEvent.cs");
        var deletedEventContent = $@"using Application.Common;
using Domain.AuditLogs;
using MediatR;

namespace Application.{pluralName}.Events;

public sealed class {singularName}DeletedEvent(Guid {singularName.ToLower()}Id) : INotification
{{
    public Guid {singularName}Id {{ get; }} = {singularName.ToLower()}Id;

    public sealed class Handler(ITenantDbContext dbContext) : INotificationHandler<{singularName}DeletedEvent>
    {{
        public async Task Handle({singularName}DeletedEvent notification, CancellationToken cancellationToken)
        {{
            await AddAuditLog(notification.{singularName}Id, cancellationToken);
        }}

        private async Task AddAuditLog(Guid {singularName.ToLower()}Id, CancellationToken cancellationToken)
        {{
            var auditLog = new AuditLog(Guid.NewGuid(), ActorType.User, ""{singularName} Deleted"", AuditLogEventType.{singularName}Deleted);

            // Set the entity ID for the audit log
            auditLog.SetEntityId({singularName.ToLower()}Id);
            
            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync(cancellationToken);
        }}
    }}
}}
";
        File.WriteAllText(deletedEventPath, deletedEventContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {deletedEventPath}");
    }

    private void GenerateMappingProfile(string featurePath, string singularName, string pluralName)
    {
        var mappingPath = Path.Combine(featurePath, "Mapping", $"{singularName}MappingProfile.cs");
        var mappingContent = $@"using Application.{pluralName}.Queries.Get{singularName}ById;
using Application.{pluralName}.Queries.Get{pluralName};
using Domain.{pluralName};
using Mapster;

namespace Application.{pluralName}.Mapping;

public class {singularName}MappingProfile : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{
        config.NewConfig<{singularName}, {singularName}ForListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);
            // Add other mapping rules as needed
            
        config.NewConfig<{singularName}, {singularName}ForReadDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);
            // Add other mapping rules as needed
    }}
}}
";
        File.WriteAllText(mappingPath, mappingContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {mappingPath}");
    }

    private void GenerateCommandTests(string testsPath, string singularName, string pluralName)
    {
        // Create Command Test
        var createCommandTestPath = Path.Combine(testsPath, "Commands", $"Create{singularName}CommandTests.cs");
        var createCommandTestContent = $@"using Application.Common;
using Application.{pluralName}.Commands.Create{singularName};
using Application.{pluralName}.Events;
using BaseTests;
using Domain.{pluralName};
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ApplicationTests.{pluralName}.Commands;

public class Create{singularName}CommandTests : BaseTest
{{
    private readonly ITenantDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly Create{singularName}Command.Handler _handler;

    public Create{singularName}CommandTests()
    {{
        _dbContext = GetDbContext();
        _publisher = Substitute.For<IPublisher>();
        _handler = new Create{singularName}Command.Handler(_dbContext, _publisher);
    }}

    [Fact]
    public async Task Handle_Should_Create{singularName}_AndReturnId()
    {{
        // Arrange
        var dto = new {singularName}ForCreateUpdateDto
        {{
            Name = ""Test {singularName}""
            // Set other required properties
        }};
        
        var command = new Create{singularName}Command(dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        var created{singularName} = await _dbContext.{pluralName}
            .FirstOrDefaultAsync(x => x.Id == result);
            
        created{singularName}.Should().NotBeNull();
        created{singularName}!.Name.Should().Be(dto.Name);
        
        // Uncomment when events are implemented
        // await _publisher.Received(1).Publish(
        //    Arg.Is<{singularName}CreatedEvent>(e => e.{singularName}Id == result),
        //    Arg.Any<CancellationToken>());
    }}
}}
";
        File.WriteAllText(createCommandTestPath, createCommandTestContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {createCommandTestPath}");

        // Update Command Test
        var updateCommandTestPath = Path.Combine(testsPath, "Commands", $"Update{singularName}CommandTests.cs");
        var updateCommandTestContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Application.{pluralName}.Commands.Update{singularName};
using Application.{pluralName}.Events;
using BaseTests;
using Domain.{pluralName};
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ApplicationTests.{pluralName}.Commands;

public class Update{singularName}CommandTests : BaseTest
{{
    private readonly ITenantDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly Update{singularName}Command.Handler _handler;
    private readonly Guid _existing{singularName}Id = Guid.NewGuid();

    public Update{singularName}CommandTests()
    {{
        _dbContext = GetDbContext();
        _publisher = Substitute.For<IPublisher>();
        _handler = new Update{singularName}Command.Handler(_dbContext, _publisher);
        
        // Setup test data
        Setup{singularName}();
    }}
    
    private void Setup{singularName}()
    {{
        var {singularName.ToLower()} = {singularName}.Create(_existing{singularName}Id, ""Original {singularName}"");
        _dbContext.{pluralName}.Add({singularName.ToLower()});
        _dbContext.SaveChanges();
    }}

    [Fact]
    public async Task Handle_Should_Update{singularName}_WhenFound()
    {{
        // Arrange
        var dto = new {singularName}ForCreateUpdateDto
        {{
            Name = ""Updated {singularName}""
            // Set other properties
        }};
        
        var command = new Update{singularName}Command(_existing{singularName}Id, dto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updated{singularName} = await _dbContext.{pluralName}
            .FirstOrDefaultAsync(x => x.Id == _existing{singularName}Id);
            
        updated{singularName}.Should().NotBeNull();
        updated{singularName}!.Name.Should().Be(dto.Name);
        
        // Uncomment when events are implemented
        // await _publisher.Received(1).Publish(
        //    Arg.Is<{singularName}UpdatedEvent>(e => e.{singularName}Id == _existing{singularName}Id),
        //    Arg.Any<CancellationToken>());
    }}
    
    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenNot{singularName}Found()
    {{
        // Arrange
        var dto = new {singularName}ForCreateUpdateDto
        {{
            Name = ""Updated {singularName}""
        }};
        
        var command = new Update{singularName}Command(Guid.NewGuid(), dto);

        // Act & Assert
        await FluentActions.Invoking(() => 
            _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }}
}}
";
        File.WriteAllText(updateCommandTestPath, updateCommandTestContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {updateCommandTestPath}");

        // Delete Command Test
        var deleteCommandTestPath = Path.Combine(testsPath, "Commands", $"Delete{singularName}CommandTests.cs");
        var deleteCommandTestContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Application.{pluralName}.Commands.Delete{singularName};
using Application.{pluralName}.Events;
using Application.Security;
using BaseTests;
using Domain.{pluralName};
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ApplicationTests.{pluralName}.Commands;

public class Delete{singularName}CommandTests : BaseTest
{{
    private readonly ITenantDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ICurrentUserService _currentUserService;
    private readonly Delete{singularName}Command.Handler _handler;
    private readonly Guid _existing{singularName}Id = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public Delete{singularName}CommandTests()
    {{
        _dbContext = GetDbContext();
        _publisher = Substitute.For<IPublisher>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _currentUserService.GetUserId().Returns(_userId);
        
        _handler = new Delete{singularName}Command.Handler(_dbContext, _publisher, _currentUserService);
        
        // Setup test data
        Setup{singularName}();
    }}
    
    private void Setup{singularName}()
    {{
        var {singularName.ToLower()} = {singularName}.Create(_existing{singularName}Id, ""Test {singularName}"");
        _dbContext.{pluralName}.Add({singularName.ToLower()});
        _dbContext.SaveChanges();
    }}

    [Fact]
    public async Task Handle_Should_SoftDelete{singularName}_WhenFound()
    {{
        // Arrange
        var command = new Delete{singularName}Command(_existing{singularName}Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var deleted{singularName} = await _dbContext.{pluralName}
            .IgnoreQueryFilters() // To find soft-deleted entities
            .FirstOrDefaultAsync(x => x.Id == _existing{singularName}Id);
            
        deleted{singularName}.Should().NotBeNull();
        deleted{singularName}!.DeletedByUserId.Should().Be(_userId);
        deleted{singularName}.DeletedAt.Should().NotBeNull();
        
        // Uncomment when events are implemented
        // await _publisher.Received(1).Publish(
        //    Arg.Is<{singularName}DeletedEvent>(e => e.{singularName}Id == _existing{singularName}Id),
        //    Arg.Any<CancellationToken>());
    }}
    
    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenNot{singularName}Found()
    {{
        // Arrange
        var command = new Delete{singularName}Command(Guid.NewGuid());

        // Act & Assert
        await FluentActions.Invoking(() => 
            _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }}
}}
";
        File.WriteAllText(deleteCommandTestPath, deleteCommandTestContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {deleteCommandTestPath}");
    }

    private void GenerateQueryTests(string testsPath, string singularName, string pluralName)
    {
        // Get{plural}Query Test
        var getQueryTestPath = Path.Combine(testsPath, "Queries", $"Get{pluralName}QueryTests.cs");
        var getQueryTestContent = $@"using Application.Common;
using Application.Pagination;
using Application.{pluralName}.Queries.Get{pluralName};
using Application.Security;
using BaseTests;
using Domain.{pluralName};
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ApplicationTests.{pluralName}.Queries;

public class Get{pluralName}QueryTests : BaseTest
{{
    private readonly ITenantDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly Get{pluralName}Query.Handler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    
    public Get{pluralName}QueryTests()
    {{
        _dbContext = GetDbContext();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _currentUserService.GetUserId().Returns(_userId);
        
        _handler = new Get{pluralName}Query.Handler(_dbContext, _currentUserService);
        
        // Setup test data
        Setup{pluralName}();
    }}
    
    private void Setup{pluralName}()
    {{
        var {pluralName.ToLower()} = new List<{singularName}>
        {{
            {singularName}.Create(Guid.NewGuid(), ""First {singularName}""),
            {singularName}.Create(Guid.NewGuid(), ""Second {singularName}""),
            {singularName}.Create(Guid.NewGuid(), ""Third {singularName}"")
        }};
        
        _dbContext.{pluralName}.AddRange({pluralName.ToLower()});
        _dbContext.SaveChanges();
    }}

    [Fact]
    public async Task Handle_Should_Return_Paginated{singularName}List()
    {{
        // Arrange
        var requestDto = new {singularName}ForRequestDto
        {{
            PageSize = 10,
            PageNumber = 1
        }};
        
        var query = new Get{pluralName}Query(requestDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.PageNumber.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }}
    
    [Fact]
    public async Task Handle_Should_Return_EmptyList_WhenNo{pluralName}Exist()
    {{
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var requestDto = new {singularName}ForRequestDto
        {{
            PageSize = 10,
            PageNumber = 1
        }};
        
        var query = new Get{pluralName}Query(requestDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }}
}}
";
        File.WriteAllText(getQueryTestPath, getQueryTestContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {getQueryTestPath}");

        // Get{singular}ByIdQuery Test
        var getByIdQueryTestPath = Path.Combine(testsPath, "Queries", $"Get{singularName}ByIdQueryTests.cs");
        var getByIdQueryTestContent = $@"using Application.Common;
using Application.Common.Exceptions;
using Application.{pluralName}.Queries.Get{singularName}ById;
using BaseTests;
using Domain.{pluralName};
using FluentAssertions;

namespace ApplicationTests.{pluralName}.Queries;

public class Get{singularName}ByIdQueryTests : BaseTest
{{
    private readonly ITenantDbContext _dbContext;
    private readonly Get{singularName}ByIdQuery.Handler _handler;
    private readonly Guid _existing{singularName}Id = Guid.NewGuid();
    
    public Get{singularName}ByIdQueryTests()
    {{
        _dbContext = GetDbContext();
        _handler = new Get{singularName}ByIdQuery.Handler(_dbContext);
        
        // Setup test data
        Setup{singularName}();
    }}
    
    private void Setup{singularName}()
    {{
        var {singularName.ToLower()} = {singularName}.Create(_existing{singularName}Id, ""Test {singularName}"");
        _dbContext.{pluralName}.Add({singularName.ToLower()});
        _dbContext.SaveChanges();
    }}

    [Fact]
    public async Task Handle_Should_Return{singularName}_WhenFound()
    {{
        // Arrange
        var query = new Get{singularName}ByIdQuery(_existing{singularName}Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(_existing{singularName}Id);
        result.Name.Should().Be(""Test {singularName}"");
    }}
    
    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_When{singularName}NotFound()
    {{
        // Arrange
        var query = new Get{singularName}ByIdQuery(Guid.NewGuid());

        // Act & Assert
        await FluentActions.Invoking(() => 
            _handler.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }}
}}
";
        File.WriteAllText(getByIdQueryTestPath, getByIdQueryTestContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {getByIdQueryTestPath}");
    }

    private void GeneratePermissionsConstants(string permissionsPath, string singularName, string pluralFeatureName)
    {
        // Check if the Permissions.cs file exists, if not create it
        var permissionsFile = Path.Combine(permissionsPath, $"{pluralFeatureName}Permissions.cs");

        var content = $@"using Domain.Tenants;

namespace Domain.PermissionsConstants;

public static partial class Permissions
{{
    /// <summary>
    /// Permissions for {pluralFeatureName}.
    /// </summary>
    public static class {pluralFeatureName}
    {{
        /// <summary>
        /// The name of the {pluralFeatureName.ToLower()} module.
        /// </summary>
        public const string ModuleName = ""{pluralFeatureName}"";

        /// <summary>
        /// Group-level permissions for the {pluralFeatureName.ToLower()} module.
        /// </summary>
        public const string Default = ModuleName;

        /// <summary>
        /// Actions that can be performed on {pluralFeatureName.ToLower()}.
        /// </summary>
        public static class Actions
        {{
            /// <summary>
            /// Permission to view all {pluralFeatureName.ToLower()}.
            /// </summary>
            public const string View = $""{{ModuleName}}.View"";

            /// <summary>
            /// Permission to view only assigned {pluralFeatureName.ToLower()}.
            /// </summary>
            public const string ViewAssigned = $""{{ModuleName}}.ViewAssigned"";

            /// <summary>
            /// Permission to view a specific {singularName.ToLower()}'s profile.
            /// </summary>
            public const string Profile = $""{{ModuleName}}.Profile"";

            /// <summary>
            /// Permission to create new {pluralFeatureName.ToLower()}.
            /// </summary>
            public const string Create = $""{{ModuleName}}.Create"";

            /// <summary>
            /// Permission to update {pluralFeatureName.ToLower()}.
            /// </summary>
            public const string Update = $""{{ModuleName}}.Update"";

            /// <summary>
            /// Permission to delete {pluralFeatureName.ToLower()}.
            /// </summary>
            public const string Delete = $""{{ModuleName}}.Delete"";
        }}
    }}
}}";

        File.WriteAllText(permissionsFile, content);
        AnsiConsole.MarkupLine($"[green]Created:[/] {permissionsFile}");
    }

    private void DisplayPermissionsInstructions(string singularName, string pluralFeatureName)
    {
        AnsiConsole.MarkupLine($"[yellow]Manual Step Required:[/] Update Authorization in Commands and Queries");
        AnsiConsole.MarkupLine($"[blue]1. Uncomment the [[Authorize]] attributes in your queries and commands[/]");
        AnsiConsole.MarkupLine($"[blue]2. To use permissions in your controllers or handlers, add:[/]");
        AnsiConsole.MarkupLine($"[green]   using Domain.PermissionsConstants;[/]");
        AnsiConsole.WriteLine();
    }

    private void GenerateControllerTests(string integrationTestsPath, string singularName, string pluralFeatureName)
    {
        var controllerTestPath = Path.Combine(integrationTestsPath, $"{pluralFeatureName}ControllerTests.cs");
        var controllerTestContent = $@"using System.Net;
using System.Net.Http.Json;
using Application.{pluralFeatureName}.Commands.Create{singularName};
using Application.{pluralFeatureName}.Commands.Update{singularName};
using Application.{pluralFeatureName}.Queries.Get{singularName}ById;
using Application.{pluralFeatureName}.Queries.Get{pluralFeatureName};
using Application.Pagination;
using BaseTests;
using Domain.{pluralFeatureName};
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfraTests.Controllers;

// This test requires your API project to be configured with WebApplicationFactory
// You'll need to create an ApiFactory in your test project
public class {pluralFeatureName}ControllerTests : BaseTest, IClassFixture<WebApplicationFactory<Program>>
{{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private const string BasePath = ""{pluralFeatureName}"";

    public {pluralFeatureName}ControllerTests(WebApplicationFactory<Program> factory)
    {{
        _factory = factory;
        _client = factory.CreateClient();
        
        // If you use authentication in your API, you may need to add auth headers here
        // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(""Bearer"", ""your-token"");
    }}

    [Fact]
    public async Task Get_Returns{pluralFeatureName}List()
    {{
        // Arrange - seed some data if needed
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITenantDbContext>();
        
        // Act
        var response = await _client.GetAsync(BasePath);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<{singularName}ForListDto>>();
        result.Should().NotBeNull();
    }}

    [Fact]
    public async Task GetById_Returns{singularName}_WhenFound()
    {{
        // Arrange
        var id = Guid.NewGuid();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITenantDbContext>();
        
        var {singularName.ToLower()} = {singularName}.Create(id, ""Test {singularName}"");
        dbContext.{pluralFeatureName}.Add({singularName.ToLower()});
        await dbContext.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($""{{BasePath}}/{{id}}"");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<{singularName}ForReadDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be(""Test {singularName}"");
    }}

    [Fact]
    public async Task Post_Creates{singularName}()
    {{
        // Arrange
        var dto = new {singularName}ForCreateUpdateDto
        {{
            Name = ""New {singularName}""
        }};
        
        // Act
        var response = await _client.PostAsJsonAsync(BasePath, dto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var resultId = await response.Content.ReadFromJsonAsync<Guid>();
        resultId.Should().NotBeEmpty();
        
        // Verify the entity was created
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITenantDbContext>();
        var created = await dbContext.{pluralFeatureName}.FirstOrDefaultAsync(x => x.Id == resultId);
        created.Should().NotBeNull();
        created!.Name.Should().Be(dto.Name);
    }}

    [Fact]
    public async Task Put_Updates{singularName}()
    {{
        // Arrange
        var id = Guid.NewGuid();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITenantDbContext>();
        
        var {singularName.ToLower()} = {singularName}.Create(id, ""Original {singularName}"");
        dbContext.{pluralFeatureName}.Add({singularName.ToLower()});
        await dbContext.SaveChangesAsync();
        
        var dto = new {singularName}ForCreateUpdateDto
        {{
            Name = ""Updated {singularName}""
        }};
        
        // Act
        var response = await _client.PutAsJsonAsync($""{{BasePath}}/{{id}}"", dto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify the entity was updated
        var updated = await dbContext.{pluralFeatureName}.FirstOrDefaultAsync(x => x.Id == id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(dto.Name);
    }}

    [Fact]
    public async Task Delete_Deletes{singularName}()
    {{
        // Arrange
        var id = Guid.NewGuid();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITenantDbContext>();
        
        var {singularName.ToLower()} = {singularName}.Create(id, ""Test {singularName}"");
        dbContext.{pluralFeatureName}.Add({singularName.ToLower()});
        await dbContext.SaveChangesAsync();
        
        // Act
        var response = await _client.DeleteAsync($""{{BasePath}}/{{id}}"");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify the entity was deleted (soft delete)
        var deleted = await dbContext.{pluralFeatureName}
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        deleted.Should().NotBeNull();
        deleted!.DeletedAt.Should().NotBeNull();
    }}
}}
";
        File.WriteAllText(controllerTestPath, controllerTestContent);
        AnsiConsole.MarkupLine($"[green]Created:[/] {controllerTestPath}");
    }
}