# Domain Events Architecture

## Overview

The QCLI tool generates domain events following the Domain-Driven Design (DDD) pattern. Domain events represent important business occurrences that other parts of the application need to know about. The generated events integrate with MediatR's `INotification` interface for decoupled event handling.

## Domain Event Pattern

### Event Structure

All generated domain events inherit from `INotification` and follow a consistent pattern:

```csharp
public record ClientCreatedEvent(Client Client) : INotification;

public record ClientUpdatedEvent(Client Client, Client OldClient) : INotification;
```

### Key Characteristics

- **Immutable Records**: Events use C# records for immutability
- **MediatR Integration**: Implement `INotification` for event publishing
- **Rich Context**: Include both current and previous state for update events
- **Type Safety**: Strongly typed with the specific entity

## Generated Event Types

### Create Events

Generated when an entity is created:

```csharp
public record ClientCreatedEvent(Client Client) : INotification;
```

**Properties:**
- Contains the newly created entity
- Triggered after successful creation
- Used for audit logging, notifications, cache invalidation

### Update Events

Generated when an entity is updated:

```csharp
public record ClientUpdatedEvent(Client Client, Client OldClient) : INotification;
```

**Properties:**
- Contains both current and previous entity state
- Triggered after successful update
- Enables change tracking and audit trails
- Supports conditional logic based on what changed

### Delete Events (if configured)

```csharp
public record ClientDeletedEvent(Client Client) : INotification;
```

**Properties:**
- Contains the deleted entity
- Triggered before or after deletion
- Used for cleanup operations and audit logging

## Event Publishing

### In Command Handlers

Events are published within command handlers using MediatR:

```csharp
public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IPublisher _publisher;

    public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Client
        {
            Name = request.Name,
            Email = request.Email
        };

        _context.Clients.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish domain event
        await _publisher.Publish(new ClientCreatedEvent(entity), cancellationToken);

        return entity.Id;
    }
}
```

### In Update Operations

Update events include both old and new state:

```csharp
public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
{
    public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Clients.FindAsync(request.Id);
        var oldEntity = entity.Clone(); // Create a copy of the original state

        // Apply updates
        entity.Name = request.Name;
        entity.Email = request.Email;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish with both old and new state
        await _publisher.Publish(new ClientUpdatedEvent(entity, oldEntity), cancellationToken);
    }
}
```

## Event Handlers

### Creating Event Handlers

Event handlers implement `INotificationHandler<T>`:

```csharp
public class ClientCreatedEventHandler : INotificationHandler<ClientCreatedEvent>
{
    private readonly ILogger<ClientCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public ClientCreatedEventHandler(
        ILogger<ClientCreatedEventHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Client created: {ClientId} - {ClientName}", 
            notification.Client.Id, notification.Client.Name);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(notification.Client.Email, cancellationToken);

        // Update cache
        // Trigger integrations
        // etc.
    }
}
```

### Multiple Handlers

Multiple handlers can process the same event:

```csharp
// Audit logging handler
public class ClientAuditEventHandler : 
    INotificationHandler<ClientCreatedEvent>,
    INotificationHandler<ClientUpdatedEvent>
{
    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        await LogAuditEvent("Client Created", notification.Client);
    }

    public async Task Handle(ClientUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await LogAuditEvent("Client Updated", notification.Client, notification.OldClient);
    }
}

// Cache invalidation handler
public class ClientCacheEventHandler : 
    INotificationHandler<ClientCreatedEvent>,
    INotificationHandler<ClientUpdatedEvent>
{
    private readonly ICacheService _cache;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _cache.InvalidateAsync($"client:{notification.Client.Id}");
        await _cache.InvalidateAsync("clients:list");
    }

    public async Task Handle(ClientUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _cache.InvalidateAsync($"client:{notification.Client.Id}");
        await _cache.InvalidateAsync("clients:list");
    }
}
```

## Configuration and Customization

### Event Generation Configuration

Configure event generation in `qcli.json`:

```json
{
  "codeGeneration": {
    "events": {
      "generateCreateEvents": true,
      "generateUpdateEvents": true,
      "generateDeleteEvents": false,
      "eventSuffix": "Event",
      "includeOldStateInUpdates": true
    }
  }
}
```

### Custom Event Properties

Extend generated events with additional properties:

```csharp
public record ClientCreatedEvent(Client Client, string CreatedBy, DateTime CreatedAt) : INotification;
```

### Event Filtering

Filter events based on conditions:

```csharp
public class ConditionalClientEventHandler : INotificationHandler<ClientUpdatedEvent>
{
    public async Task Handle(ClientUpdatedEvent notification, CancellationToken cancellationToken)
    {
        // Only process if email changed
        if (notification.Client.Email != notification.OldClient.Email)
        {
            await ProcessEmailChange(notification.Client, notification.OldClient);
        }

        // Only process if status changed to active
        if (notification.Client.IsActive && !notification.OldClient.IsActive)
        {
            await ProcessActivation(notification.Client);
        }
    }
}
```

## Best Practices

### 1. Event Naming

- Use past tense: `ClientCreated`, `OrderShipped`
- Include entity name: `ClientCreatedEvent`
- Be specific: `EmailChangedEvent` vs `ClientUpdatedEvent`

### 2. Event Content

- Include only necessary data
- Consider serialization requirements
- Keep events immutable
- Include correlation IDs for tracking

### 3. Handler Design

- Keep handlers focused and single-purpose
- Handle exceptions gracefully
- Use async/await properly
- Consider idempotency

### 4. Performance Considerations

- Avoid heavy operations in event handlers
- Use background processing for time-consuming tasks
- Consider event ordering and dependencies
- Monitor handler performance

## Integration Patterns

### With Audit Logging

```csharp
public class AuditEventHandler : 
    INotificationHandler<ClientCreatedEvent>,
    INotificationHandler<ClientUpdatedEvent>
{
    private readonly IAuditService _auditService;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _auditService.LogAsync(new AuditEntry
        {
            EntityType = nameof(Client),
            EntityId = notification.Client.Id.ToString(),
            Action = "Created",
            Timestamp = DateTime.UtcNow,
            Data = JsonSerializer.Serialize(notification.Client)
        });
    }

    public async Task Handle(ClientUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var changes = DetectChanges(notification.OldClient, notification.Client);
        
        await _auditService.LogAsync(new AuditEntry
        {
            EntityType = nameof(Client),
            EntityId = notification.Client.Id.ToString(),
            Action = "Updated",
            Timestamp = DateTime.UtcNow,
            Changes = changes
        });
    }
}
```

### With External Systems

```csharp
public class IntegrationEventHandler : INotificationHandler<ClientCreatedEvent>
{
    private readonly IServiceBus _serviceBus;
    private readonly IExternalApiClient _apiClient;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Publish to service bus
        await _serviceBus.PublishAsync(new ExternalClientCreatedMessage
        {
            ClientId = notification.Client.Id,
            Name = notification.Client.Name,
            Email = notification.Client.Email
        });

        // Sync with external system
        await _apiClient.CreateClientAsync(new ExternalClientDto
        {
            Id = notification.Client.Id,
            Name = notification.Client.Name,
            Email = notification.Client.Email
        });
    }
}
```

### With Background Jobs

```csharp
public class BackgroundJobEventHandler : INotificationHandler<ClientCreatedEvent>
{
    private readonly IBackgroundJobClient _jobClient;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Schedule background job
        _jobClient.Enqueue<IWelcomeEmailJob>(job => 
            job.SendWelcomeEmailAsync(notification.Client.Id));

        // Schedule delayed job
        _jobClient.Schedule<IFollowUpJob>(
            job => job.SendFollowUpEmailAsync(notification.Client.Id),
            TimeSpan.FromDays(7));
    }
}
```

## Testing Domain Events

### Unit Testing Event Publishing

```csharp
[Test]
public async Task CreateClient_ShouldPublishClientCreatedEvent()
{
    // Arrange
    var mockPublisher = new Mock<IPublisher>();
    var handler = new CreateClientCommandHandler(_context, mockPublisher.Object);
    var command = new CreateClientCommand { Name = "Test Client", Email = "test@example.com" };

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    mockPublisher.Verify(p => p.Publish(
        It.Is<ClientCreatedEvent>(e => 
            e.Client.Name == "Test Client" && 
            e.Client.Email == "test@example.com"),
        It.IsAny<CancellationToken>()), 
        Times.Once);
}
```

### Integration Testing Event Handlers

```csharp
[Test]
public async Task ClientCreatedEvent_ShouldSendWelcomeEmail()
{
    // Arrange
    var mockEmailService = new Mock<IEmailService>();
    var handler = new ClientCreatedEventHandler(Mock.Of<ILogger<ClientCreatedEventHandler>>(), mockEmailService.Object);
    var client = new Client { Id = 1, Name = "Test Client", Email = "test@example.com" };
    var eventNotification = new ClientCreatedEvent(client);

    // Act
    await handler.Handle(eventNotification, CancellationToken.None);

    // Assert
    mockEmailService.Verify(s => s.SendWelcomeEmailAsync("test@example.com", It.IsAny<CancellationToken>()), Times.Once);
}
```

### Testing Event Ordering

```csharp
[Test]
public async Task Events_ShouldBeProcessedInCorrectOrder()
{
    // Arrange
    var events = new List<string>();
    var handler1 = new TestEventHandler1(events);
    var handler2 = new TestEventHandler2(events);
    
    var mediator = new Mock<IMediator>();
    mediator.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
           .Returns((INotification notification, CancellationToken ct) =>
           {
               if (notification is ClientCreatedEvent)
               {
                   handler1.Handle((ClientCreatedEvent)notification, ct);
                   handler2.Handle((ClientCreatedEvent)notification, ct);
               }
               return Task.CompletedTask;
           });

    // Act
    await mediator.Object.Publish(new ClientCreatedEvent(new Client()), CancellationToken.None);

    // Assert
    Assert.That(events, Is.EqualTo(new[] { "Handler1", "Handler2" }));
}
```

## Error Handling

### Handler Exception Handling

```csharp
public class RobustEventHandler : INotificationHandler<ClientCreatedEvent>
{
    private readonly ILogger<RobustEventHandler> _logger;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await ProcessEvent(notification);
        }
        catch (ExternalServiceException ex)
        {
            _logger.LogWarning(ex, "External service unavailable for client {ClientId}", notification.Client.Id);
            // Maybe retry later or use circuit breaker
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ClientCreatedEvent for client {ClientId}", notification.Client.Id);
            // Don't rethrow to avoid affecting other handlers
        }
    }
}
```

### Dead Letter Handling

```csharp
public class DeadLetterEventHandler : INotificationHandler<ClientCreatedEvent>
{
    private readonly IDeadLetterService _deadLetterService;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await ProcessEvent(notification);
        }
        catch (Exception ex)
        {
            await _deadLetterService.SendToDeadLetterAsync(notification, ex);
            throw; // Re-throw to ensure other error handling mechanisms are triggered
        }
    }
}
```

## Common Patterns

### Event Sourcing Integration

```csharp
public class EventSourcingHandler : INotificationHandler<ClientUpdatedEvent>
{
    private readonly IEventStore _eventStore;

    public async Task Handle(ClientUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var changes = DetectChanges(notification.OldClient, notification.Client);
        
        foreach (var change in changes)
        {
            await _eventStore.AppendEventAsync(new ClientPropertyChangedEvent
            {
                ClientId = notification.Client.Id,
                PropertyName = change.PropertyName,
                OldValue = change.OldValue,
                NewValue = change.NewValue,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
```

### Saga Pattern

```csharp
public class ClientOnboardingSaga : 
    INotificationHandler<ClientCreatedEvent>,
    INotificationHandler<EmailVerifiedEvent>,
    INotificationHandler<ProfileCompletedEvent>
{
    private readonly ISagaRepository _sagaRepository;

    public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
    {
        var saga = new ClientOnboardingSagaData
        {
            ClientId = notification.Client.Id,
            IsClientCreated = true,
            CreatedAt = DateTime.UtcNow
        };

        await _sagaRepository.SaveAsync(saga);
        await CheckCompletionAsync(saga);
    }

    // Handle other events and update saga state...
}
```

## Troubleshooting

### Common Issues

1. **Events Not Being Published**
   - Check MediatR registration
   - Verify publisher injection
   - Ensure events implement `INotification`

2. **Handlers Not Executing**
   - Verify handler registration in DI container
   - Check handler implementation
   - Review exception logs

3. **Performance Issues**
   - Monitor handler execution time
   - Consider async processing
   - Implement circuit breakers

4. **Event Ordering Issues**
   - Use sequential processing if order matters
   - Implement custom notification publisher
   - Consider event versioning

### Debugging Tips

- Add logging to track event flow
- Use correlation IDs across handlers
- Monitor handler performance metrics
- Test event scenarios in isolation

## Related Documentation

- [Clean Architecture](./clean-architecture.md) - Overall architecture patterns
- [CQRS](./cqrs.md) - Command and query handling
- [Validation](./validation.md) - Input validation patterns
- [Permissions](./permissions.md) - Authorization patterns

## Conclusion

Domain events provide a powerful way to implement decoupled, event-driven architectures. The QCLI tool generates events that follow best practices and integrate seamlessly with MediatR, enabling robust and maintainable applications. Use the patterns and practices outlined in this guide to effectively implement domain events in your Clean Architecture applications.
