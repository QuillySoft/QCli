# Mapping Profiles Architecture

## Overview

The QCLI tool generates mapping profiles using Mapster for efficient object-to-object mapping between different layers of the Clean Architecture. These mappings handle the transformation between entities, DTOs, commands, queries, and view models while maintaining performance and type safety.

## Mapster Integration

### TypeAdapterConfig Setup

The generated mapping profiles use Mapster's `TypeAdapterConfig` for centralized configuration:

```csharp
public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Entity to DTO mappings
        config.NewConfig<Client, ClientDto>()
              .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
              .Map(dest => dest.IsActive, src => src.Status == ClientStatus.Active);

        // Command to Entity mappings
        config.NewConfig<CreateClientCommand, Client>()
              .Ignore(dest => dest.Id)
              .Ignore(dest => dest.CreatedAt)
              .Ignore(dest => dest.CreatedBy);

        // Query result mappings
        config.NewConfig<Client, ClientListDto>()
              .Map(dest => dest.DisplayName, src => src.Name)
              .Map(dest => dest.ContactInfo, src => $"{src.Email} | {src.Phone}");
    }
}
```

### IRegister Pattern

All mapping profiles implement `IRegister` for automatic registration:

```csharp
public class ClientMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        RegisterClientMappings(config);
        RegisterClientRelatedMappings(config);
    }

    private static void RegisterClientMappings(TypeAdapterConfig config)
    {
        // Client entity mappings
        config.NewConfig<Client, ClientDto>();
        config.NewConfig<Client, ClientDetailsDto>();
        config.NewConfig<Client, ClientListDto>();
    }

    private static void RegisterClientRelatedMappings(TypeAdapterConfig config)
    {
        // Related entity mappings
        config.NewConfig<Address, AddressDto>();
        config.NewConfig<Contact, ContactDto>();
    }
}
```

## Generated Mapping Types

### Entity to DTO Mappings

**Basic DTO Mapping:**
```csharp
config.NewConfig<Client, ClientDto>()
      .Map(dest => dest.Id, src => src.Id)
      .Map(dest => dest.Name, src => src.Name)
      .Map(dest => dest.Email, src => src.Email)
      .Map(dest => dest.CreatedAt, src => src.CreatedAt);
```

**List DTO Mapping:**
```csharp
config.NewConfig<Client, ClientListDto>()
      .Map(dest => dest.Id, src => src.Id)
      .Map(dest => dest.Name, src => src.Name)
      .Map(dest => dest.Status, src => src.IsActive ? "Active" : "Inactive")
      .Map(dest => dest.LastModified, src => src.ModifiedAt ?? src.CreatedAt);
```

**Details DTO Mapping:**
```csharp
config.NewConfig<Client, ClientDetailsDto>()
      .Map(dest => dest.Addresses, src => src.Addresses.Adapt<List<AddressDto>>())
      .Map(dest => dest.ContactPerson, src => src.Contacts.FirstOrDefault())
      .Map(dest => dest.TotalOrders, src => src.Orders.Count);
```

### Command to Entity Mappings

**Create Command Mapping:**
```csharp
config.NewConfig<CreateClientCommand, Client>()
      .Ignore(dest => dest.Id)
      .Ignore(dest => dest.CreatedAt)
      .Ignore(dest => dest.CreatedBy)
      .Ignore(dest => dest.ModifiedAt)
      .Ignore(dest => dest.ModifiedBy)
      .Map(dest => dest.Name, src => src.Name.Trim())
      .Map(dest => dest.Email, src => src.Email.ToLowerInvariant());
```

**Update Command Mapping:**
```csharp
config.NewConfig<UpdateClientCommand, Client>()
      .Ignore(dest => dest.Id)
      .Ignore(dest => dest.CreatedAt)
      .Ignore(dest => dest.CreatedBy)
      .Map(dest => dest.ModifiedAt, src => DateTime.UtcNow)
      .Map(dest => dest.Name, src => src.Name.Trim())
      .Map(dest => dest.Email, src => src.Email.ToLowerInvariant());
```

### Query Result Mappings

**Paginated Results:**
```csharp
config.NewConfig<PaginatedList<Client>, PaginatedList<ClientDto>>()
      .Map(dest => dest.Items, src => src.Items.Adapt<List<ClientDto>>())
      .Map(dest => dest.TotalCount, src => src.TotalCount)
      .Map(dest => dest.PageNumber, src => src.PageNumber)
      .Map(dest => dest.TotalPages, src => src.TotalPages);
```

**Filtered Results:**
```csharp
config.NewConfig<Client, ClientDto>()
      .Map(dest => dest.Id, src => src.Id)
      .Map(dest => dest.Name, src => src.Name)
      .Map(dest => dest.Email, src => src.Email)
      .Map(dest => dest.IsActive, src => src.Status == ClientStatus.Active)
      .AfterMapping((src, dest) => dest.CanEdit = CheckEditPermission(src.Id));
```

## Complex Mapping Scenarios

### Nested Object Mapping

```csharp
config.NewConfig<Client, ClientDetailsDto>()
      .Map(dest => dest.PrimaryAddress, src => src.Addresses.FirstOrDefault(a => a.IsPrimary))
      .Map(dest => dest.ContactDetails, src => new ContactDetailsDto
      {
          Email = src.Email,
          Phone = src.Phone,
          PreferredContactMethod = src.PreferredContactMethod
      })
      .Map(dest => dest.OrderSummary, src => new OrderSummaryDto
      {
          TotalOrders = src.Orders.Count,
          TotalValue = src.Orders.Sum(o => o.TotalAmount),
          LastOrderDate = src.Orders.Max(o => o.OrderDate)
      });
```

### Conditional Mapping

```csharp
config.NewConfig<Client, ClientDto>()
      .Map(dest => dest.Status, src => src.IsActive ? "Active" : "Inactive")
      .Map(dest => dest.StatusColor, src => src.IsActive ? "green" : "red")
      .Map(dest => dest.ContactInfo, src => !string.IsNullOrEmpty(src.Phone) 
          ? $"{src.Email} | {src.Phone}" 
          : src.Email)
      .Map(dest => dest.CanDelete, src => !src.Orders.Any());
```

### Custom Value Resolvers

```csharp
public class ClientMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Client, ClientDto>()
              .Map(dest => dest.FullAddress, src => ResolveFullAddress(src))
              .Map(dest => dest.StatusBadge, src => ResolveStatusBadge(src.Status));
    }

    private static string ResolveFullAddress(Client client)
    {
        var primaryAddress = client.Addresses?.FirstOrDefault(a => a.IsPrimary);
        if (primaryAddress == null) return "No address";

        return $"{primaryAddress.Street}, {primaryAddress.City}, {primaryAddress.State} {primaryAddress.ZipCode}";
    }

    private static StatusBadgeDto ResolveStatusBadge(ClientStatus status)
    {
        return status switch
        {
            ClientStatus.Active => new StatusBadgeDto { Text = "Active", Color = "success" },
            ClientStatus.Inactive => new StatusBadgeDto { Text = "Inactive", Color = "warning" },
            ClientStatus.Suspended => new StatusBadgeDto { Text = "Suspended", Color = "danger" },
            _ => new StatusBadgeDto { Text = "Unknown", Color = "secondary" }
        };
    }
}
```

## Performance Optimizations

### Compiled Mappings

```csharp
public static class MappingExtensions
{
    private static readonly Func<Client, ClientDto> ClientToDtoMapper = 
        TypeAdapter.For<Client, ClientDto>().CreateMapper();

    private static readonly Func<Client, ClientListDto> ClientToListDtoMapper = 
        TypeAdapter.For<Client, ClientListDto>().CreateMapper();

    public static ClientDto ToDto(this Client client) => ClientToDtoMapper(client);
    
    public static ClientListDto ToListDto(this Client client) => ClientToListDtoMapper(client);

    public static List<ClientDto> ToDto(this IEnumerable<Client> clients) =>
        clients.Select(ClientToDtoMapper).ToList();
}
```

### Bulk Mapping Configuration

```csharp
config.NewConfig<Client, ClientDto>()
      .EnableNonPublicMembers()
      .PreserveReference()
      .MaxDepth(3)
      .IgnoreNullValues(true);
```

### Memory-Efficient Mappings

```csharp
config.NewConfig<Client, ClientListDto>()
      .Map(dest => dest.Id, src => src.Id)
      .Map(dest => dest.Name, src => src.Name)
      .Map(dest => dest.Email, src => src.Email)
      .Ignore(dest => dest.Orders)        // Don't map heavy collections
      .Ignore(dest => dest.Addresses)     // for list views
      .Ignore(dest => dest.Documents);
```

## Configuration and Customization

### Global Configuration

```csharp
public class GlobalMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Global settings
        config.Default
              .Settings.IgnoreNullValues = true;
              .Settings.PreserveReference = true;
              .Settings.MaxDepth = 5;

        // Global naming conventions
        config.Default
              .NameMatchingStrategy(NameMatchingStrategy.Flexible);

        // Global type conversions
        config.ForType<DateTime, string>()
              .Map(dest => dest, src => src.ToString("yyyy-MM-dd HH:mm:ss"));

        config.ForType<string, DateTime>()
              .Map(dest => dest, src => DateTime.Parse(src));
    }
}
```

### Environment-Specific Mappings

```csharp
public class EnvironmentMappingProfile : IRegister
{
    private readonly IWebHostEnvironment _environment;

    public EnvironmentMappingProfile(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void Register(TypeAdapterConfig config)
    {
        if (_environment.IsDevelopment())
        {
            config.NewConfig<Client, ClientDto>()
                  .Map(dest => dest.Debug, src => $"ID: {src.Id}, Created: {src.CreatedAt}");
        }

        if (_environment.IsProduction())
        {
            config.NewConfig<Client, ClientDto>()
                  .Ignore(dest => dest.InternalNotes)
                  .Ignore(dest => dest.DebugInfo);
        }
    }
}
```

## Best Practices

### 1. Mapping Organization

**Separate by Domain:**
```
Mappings/
├── Client/
│   ├── ClientMappingProfile.cs
│   ├── ClientExtensions.cs
│   └── ClientDtoProfiles.cs
├── Order/
│   ├── OrderMappingProfile.cs
│   └── OrderExtensions.cs
└── Common/
    ├── GlobalMappingProfile.cs
    └── CommonExtensions.cs
```

### 2. Extension Methods

```csharp
public static class ClientMappingExtensions
{
    public static ClientDto ToDto(this Client client) => client.Adapt<ClientDto>();
    
    public static Client ToEntity(this CreateClientCommand command) => command.Adapt<Client>();
    
    public static void UpdateEntity(this UpdateClientCommand command, Client entity)
    {
        command.Adapt(entity);
    }

    public static PaginatedList<ClientDto> ToDto(this PaginatedList<Client> clients) =>
        clients.Adapt<PaginatedList<ClientDto>>();
}
```

### 3. Validation Integration

```csharp
config.NewConfig<CreateClientCommand, Client>()
      .BeforeMapping((src, dest) => ValidateCommand(src))
      .Map(dest => dest.Name, src => src.Name.Trim())
      .AfterMapping((src, dest) => ValidateEntity(dest));

private static void ValidateCommand(CreateClientCommand command)
{
    if (string.IsNullOrWhiteSpace(command.Name))
        throw new ValidationException("Name is required");
}

private static void ValidateEntity(Client entity)
{
    if (entity.CreatedAt == default)
        entity.CreatedAt = DateTime.UtcNow;
}
```

### 4. Error Handling

```csharp
public static class SafeMappingExtensions
{
    public static ClientDto? ToDto(this Client? client)
    {
        try
        {
            return client?.Adapt<ClientDto>();
        }
        catch (Exception ex)
        {
            // Log the error
            Logger.LogError(ex, "Error mapping Client to ClientDto");
            return null;
        }
    }

    public static List<ClientDto> ToDto(this IEnumerable<Client> clients)
    {
        return clients
            .Select(ToDto)
            .Where(dto => dto != null)
            .ToList()!;
    }
}
```

## Integration Patterns

### With AutoQuery

```csharp
public class ClientQueryMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetClientsQuery, Client>()
              .Map(dest => dest.Name, src => src.SearchTerm)
              .Map(dest => dest.Email, src => src.SearchTerm)
              .Map(dest => dest.IsActive, src => src.IsActive ?? true);
    }
}
```

### With EF Core Projections

```csharp
public static class QueryableExtensions
{
    public static IQueryable<ClientDto> ProjectToDto(this IQueryable<Client> clients)
    {
        return clients.ProjectToType<ClientDto>();
    }

    public static IQueryable<ClientListDto> ProjectToListDto(this IQueryable<Client> clients)
    {
        return clients.ProjectToType<ClientListDto>();
    }
}

// Usage in queries
public async Task<PaginatedList<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
{
    return await _context.Clients
        .Where(c => c.IsActive)
        .ProjectToDto()
        .PaginatedListAsync(request.PageNumber, request.PageSize);
}
```

### With CQRS Commands

```csharp
public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
{
    public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // Use mapping to create entity
        var entity = request.Adapt<Client>();
        
        // Set audit fields
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = _currentUser.UserId;

        _context.Clients.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
```

## Testing Mapping Profiles

### Unit Testing Mappings

```csharp
[TestFixture]
public class ClientMappingProfileTests
{
    private TypeAdapterConfig _config;

    [SetUp]
    public void Setup()
    {
        _config = new TypeAdapterConfig();
        new ClientMappingProfile().Register(_config);
    }

    [Test]
    public void Should_Map_Client_To_ClientDto()
    {
        // Arrange
        var client = new Client
        {
            Id = 1,
            Name = "Test Client",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = client.Adapt<ClientDto>(_config);

        // Assert
        Assert.That(result.Id, Is.EqualTo(client.Id));
        Assert.That(result.Name, Is.EqualTo(client.Name));
        Assert.That(result.Email, Is.EqualTo(client.Email));
    }

    [Test]
    public void Should_Map_CreateClientCommand_To_Client()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "New Client",
            Email = "new@example.com"
        };

        // Act
        var result = command.Adapt<Client>(_config);

        // Assert
        Assert.That(result.Name, Is.EqualTo(command.Name));
        Assert.That(result.Email, Is.EqualTo(command.Email));
        Assert.That(result.Id, Is.EqualTo(0)); // Should be ignored
    }
}
```

### Integration Testing

```csharp
[Test]
public async Task Should_Map_Entities_In_Query_Pipeline()
{
    // Arrange
    var clients = new List<Client>
    {
        new() { Id = 1, Name = "Client 1", Email = "client1@example.com" },
        new() { Id = 2, Name = "Client 2", Email = "client2@example.com" }
    };

    _context.Clients.AddRange(clients);
    await _context.SaveChangesAsync();

    var query = new GetClientsQuery();
    var handler = new GetClientsQueryHandler(_context);

    // Act
    var result = await handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.Items, Has.Count.EqualTo(2));
    Assert.That(result.Items.First().Name, Is.EqualTo("Client 1"));
}
```

### Performance Testing

```csharp
[Test]
public void Should_Map_Large_Collections_Efficiently()
{
    // Arrange
    var clients = Enumerable.Range(1, 10000)
        .Select(i => new Client 
        { 
            Id = i, 
            Name = $"Client {i}", 
            Email = $"client{i}@example.com" 
        })
        .ToList();

    var stopwatch = Stopwatch.StartNew();

    // Act
    var result = clients.Adapt<List<ClientDto>>();

    // Assert
    stopwatch.Stop();
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000));
    Assert.That(result, Has.Count.EqualTo(10000));
}
```

## Troubleshooting

### Common Issues

1. **Mapping Configuration Not Applied**
   - Ensure `IRegister` implementation is registered in DI
   - Check TypeAdapterConfig initialization
   - Verify mapping profile registration order

2. **Null Reference Exceptions**
   - Use `IgnoreNullValues(true)` setting
   - Add null checks in custom mappings
   - Configure default values for required properties

3. **Performance Issues**
   - Use compiled mappings for hot paths
   - Avoid deep object graphs in list mappings
   - Consider projection instead of mapping

4. **Circular Reference Issues**
   - Set `MaxDepth` configuration
   - Use `PreserveReference(true)`
   - Break circular references in DTOs

### Debugging Tips

```csharp
// Enable mapping debugging
config.Default.Settings.EnableDebugging = true;

// Add logging to mappings
config.NewConfig<Client, ClientDto>()
      .BeforeMapping((src, dest) => 
          Logger.LogDebug("Mapping Client {Id} to ClientDto", src.Id))
      .AfterMapping((src, dest) => 
          Logger.LogDebug("Mapped Client {Id} to ClientDto", dest.Id));
```

## Related Documentation

- [Clean Architecture](./clean-architecture.md) - Layer separation and dependencies
- [CQRS](./cqrs.md) - Command and query object mappings
- [Validation](./validation.md) - Input validation before mapping
- [Domain Events](./domain-events.md) - Event object mappings

## Conclusion

Mapping profiles provide efficient and maintainable object-to-object transformations in Clean Architecture applications. The QCLI tool generates Mapster-based mappings that follow best practices for performance, type safety, and maintainability. Use the patterns and configurations outlined in this guide to implement robust mapping solutions in your applications.
