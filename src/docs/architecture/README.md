# QCLI Architecture Documentation

## Overview

This documentation provides comprehensive guidance on the architectural patterns and principles implemented by the QCLI (Quick Clean Architecture Layer Interface) tool. QCLI generates complete CRUD operations following Clean Architecture principles, incorporating modern patterns like CQRS, Domain Events, and comprehensive validation.

## Architecture Documentation

### Core Architecture Patterns

#### [Clean Architecture](./clean-architecture.md)
- **Overview**: Comprehensive guide to the Clean Architecture implementation in QCLI
- **Topics Covered**:
  - Layer responsibilities and boundaries
  - Dependency flow and inversion
  - Project structure and organization
  - Generated code examples
  - Best practices and common patterns
  - Integration guidelines

#### [CQRS (Command Query Responsibility Segregation)](./cqrs.md)
- **Overview**: Detailed explanation of CQRS implementation using MediatR
- **Topics Covered**:
  - Commands vs. Queries separation
  - MediatR integration patterns
  - Request/Response pipelines
  - Performance considerations
  - Advanced CQRS patterns
  - Best practices and troubleshooting

### Entity and Data Patterns

#### [Entity Types](./entity-types.md)
- **Overview**: Guide to entity inheritance hierarchy and choosing appropriate entity types
- **Topics Covered**:
  - Entity base classes (`Entity`, `AuditedEntity`, `FullyAuditedEntity`)
  - Decision matrix for entity type selection
  - Generated code examples
  - Customization options
  - Migration strategies
  - Best practices

#### [Mapping Profiles](./mapping-profiles.md)
- **Overview**: Object-to-object mapping using Mapster for efficient transformations
- **Topics Covered**:
  - Mapster configuration and setup
  - Entity to DTO mappings
  - Command to Entity mappings
  - Complex mapping scenarios
  - Performance optimizations
  - Best practices and troubleshooting

### Security and Validation

#### [Permissions](./permissions.md)
- **Overview**: Comprehensive authorization system with hierarchical permissions
- **Topics Covered**:
  - Permission generation and structure
  - Authorization attributes and integration
  - ASP.NET Core authorization pipeline
  - Permission constants and management
  - Testing authorization
  - Troubleshooting and best practices

#### [Validation](./validation.md)
- **Overview**: FluentValidation integration with comprehensive error handling
- **Topics Covered**:
  - `AppBaseAbstractValidator<T>` base class
  - Validation pipeline with MediatR behaviors
  - Error handling patterns
  - Custom validation rules
  - Testing validation
  - Performance considerations

### Event-Driven Architecture

#### [Domain Events](./domain-events.md)
- **Overview**: Domain-Driven Design events for decoupled architecture
- **Topics Covered**:
  - Event structure and patterns
  - MediatR notification integration
  - Event handlers and publishing
  - Configuration and customization
  - Integration patterns
  - Testing domain events

### Testing Strategy

#### [Testing Patterns](./testing-patterns.md)
- **Overview**: Comprehensive testing strategy for Clean Architecture applications
- **Topics Covered**:
  - Unit testing patterns
  - Integration testing strategies
  - End-to-end API testing
  - Test data builders and mock services
  - Performance testing
  - Testing configuration

## Getting Started

### Prerequisites

Before diving into the architecture documentation, ensure you have:

1. **Basic understanding of Clean Architecture principles**
2. **Familiarity with .NET and C#**
3. **Knowledge of Entity Framework Core**
4. **Understanding of dependency injection concepts**

### Recommended Reading Order

For new users, we recommend reading the documentation in this order:

1. **[Clean Architecture](./clean-architecture.md)** - Start here to understand the overall structure
2. **[Entity Types](./entity-types.md)** - Learn about entity design decisions
3. **[CQRS](./cqrs.md)** - Understand command and query separation
4. **[Validation](./validation.md)** - Learn input validation patterns
5. **[Permissions](./permissions.md)** - Understand authorization implementation
6. **[Mapping Profiles](./mapping-profiles.md)** - Learn object transformation patterns
7. **[Domain Events](./domain-events.md)** - Understand event-driven patterns
8. **[Testing Patterns](./testing-patterns.md)** - Learn comprehensive testing strategies

### Quick Reference Guide

#### Common Commands

```bash
# Initialize a new project
qcli init

# Add CRUD operations for an entity
qcli add Client

# List available entities
qcli list

# View configuration
qcli config

# Generate with custom options
qcli add Product --skip-tests --include-soft-delete
```

#### Key Configuration Options

```json
{
  "solution": {
    "name": "MyApplication",
    "namespace": "MyApplication"
  },
  "codeGeneration": {
    "generateTests": true,
    "generateValidators": true,
    "generatePermissions": true,
    "generateEvents": true,
    "entityType": "FullyAudited"
  }
}
```

#### Generated File Structure

```
src/
├── Application/
│   ├── Commands/
│   │   ├── CreateClientCommand.cs
│   │   ├── UpdateClientCommand.cs
│   │   └── DeleteClientCommand.cs
│   ├── Queries/
│   │   ├── GetClientQuery.cs
│   │   └── GetClientsQuery.cs
│   ├── DTOs/
│   │   ├── ClientDto.cs
│   │   └── ClientListDto.cs
│   ├── Validators/
│   │   ├── CreateClientCommandValidator.cs
│   │   └── UpdateClientCommandValidator.cs
│   ├── MappingProfiles/
│   │   └── ClientMappingProfile.cs
│   └── Events/
│       ├── ClientCreatedEvent.cs
│       └── ClientUpdatedEvent.cs
├── Domain/
│   └── Entities/
│       └── Client.cs
├── Infrastructure/
│   └── Authorization/
│       └── Permissions.cs
└── Tests/
    ├── Unit/
    ├── Integration/
    └── EndToEnd/
```

## Architecture Principles

### Dependency Rule

The fundamental rule of Clean Architecture: **dependencies point inward**. Source code dependencies must point only inward, toward higher-level policies.

```
┌─────────────────────────────────────┐
│             Presentation            │
│         (Controllers, Views)        │
├─────────────────────────────────────┤
│            Application              │
│      (Commands, Queries, DTOs)      │
├─────────────────────────────────────┤
│             Domain                  │
│        (Entities, Rules)            │
├─────────────────────────────────────┤
│           Infrastructure            │
│      (Database, External APIs)      │
└─────────────────────────────────────┘
```

### Key Design Patterns

1. **CQRS**: Separate read and write operations
2. **Mediator**: Decouple request handling
3. **Repository**: Abstract data access
4. **Unit of Work**: Manage transactions
5. **Domain Events**: Decouple domain logic
6. **Specification**: Encapsulate query logic

### SOLID Principles Application

- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes must be substitutable
- **Interface Segregation**: Many specific interfaces vs. one general
- **Dependency Inversion**: Depend on abstractions, not concretions

## Integration Patterns

### Database Integration

```csharp
// Entity Framework Core with repositories
public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
}

// Implementation in Infrastructure layer
public class ClientRepository : Repository<Client>, IClientRepository
{
    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == email);
    }
}
```

### External Services Integration

```csharp
// Application layer interface
public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email, CancellationToken cancellationToken = default);
}

// Infrastructure implementation
public class EmailService : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email, CancellationToken cancellationToken)
    {
        // External email service integration
    }
}
```

### API Integration

```csharp
// Clean API controllers
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    [Authorize(Permissions = Permissions.Clients.Actions.Create)]
    public async Task<ActionResult<int>> Create(CreateClientCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = result }, result);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = Permissions.Clients.Actions.Read)]
    public async Task<ActionResult<ClientDto>> Get(int id)
    {
        var result = await _mediator.Send(new GetClientQuery { Id = id });
        return result != null ? Ok(result) : NotFound();
    }
}
```

## Performance Considerations

### Query Optimization

- Use projection for list queries
- Implement proper indexing strategies
- Consider read models for complex queries
- Use async/await throughout the pipeline

### Caching Strategies

- Implement distributed caching for frequently accessed data
- Use cache-aside pattern with proper invalidation
- Consider query result caching for expensive operations

### Memory Management

- Use appropriate entity tracking for different scenarios
- Implement proper disposal patterns
- Consider memory usage in large data operations

## Security Best Practices

### Authentication & Authorization

- Implement JWT or similar token-based authentication
- Use role-based and permission-based authorization
- Implement proper token validation and refresh

### Data Protection

- Encrypt sensitive data at rest
- Use HTTPS for all communications
- Implement proper input validation and sanitization
- Follow OWASP security guidelines

### Audit Trail

- Log all data modifications
- Track user actions for compliance
- Implement proper monitoring and alerting

## Troubleshooting Guide

### Common Issues

1. **Circular Dependencies**: Review dependency flow and interfaces
2. **Performance Issues**: Check query patterns and database indexes
3. **Validation Errors**: Verify validator registration and rules
4. **Authorization Issues**: Check permission configuration and middleware
5. **Event Handling**: Verify event handler registration and exception handling

### Debugging Tips

- Enable detailed logging for development
- Use profiling tools for performance analysis
- Implement health checks for monitoring
- Use integration tests for complex scenarios

## Contributing

### Code Standards

- Follow C# coding conventions
- Use meaningful names and clear intent
- Write comprehensive tests for all features
- Document complex business logic
- Follow SOLID principles

### Testing Requirements

- Minimum 80% code coverage
- Include unit, integration, and end-to-end tests
- Test both happy path and error scenarios
- Include performance tests for critical paths

## Resources

### External Documentation

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Mapster Documentation](https://github.com/MapsterMapper/Mapster)

### Related Tools and Libraries

- **Entity Framework Core**: ORM for data access
- **MediatR**: Mediator pattern implementation
- **FluentValidation**: Validation library
- **Mapster**: Object mapping library
- **NUnit/xUnit**: Testing frameworks
- **Moq**: Mocking framework

## Support

For questions, issues, or contributions:

1. **Check the documentation** - Most questions are answered here
2. **Review the examples** - Sample implementations are provided
3. **Check existing issues** - Your question might already be answered
4. **Create an issue** - For bugs or feature requests
5. **Submit a pull request** - For contributions

---

*This documentation is maintained alongside the QCLI tool and is updated with each release. For the latest version, please check the repository.*
