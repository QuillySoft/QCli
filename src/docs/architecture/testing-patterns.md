# Testing Patterns Architecture

## Overview

The QCLI tool generates comprehensive testing patterns following Clean Architecture principles. The generated tests cover unit tests for individual components, integration tests for complete workflows, and end-to-end tests for API endpoints. The testing strategy emphasizes maintainability, reliability, and fast feedback cycles.

## Testing Structure

### Test Project Organization

```
Tests/
├── Unit/
│   ├── Application/
│   │   ├── Commands/
│   │   │   ├── CreateClientCommandTests.cs
│   │   │   └── UpdateClientCommandTests.cs
│   │   ├── Queries/
│   │   │   ├── GetClientQueryTests.cs
│   │   │   └── GetClientsQueryTests.cs
│   │   └── Validators/
│   │       ├── CreateClientCommandValidatorTests.cs
│   │       └── UpdateClientCommandValidatorTests.cs
│   ├── Domain/
│   │   ├── Entities/
│   │   │   └── ClientTests.cs
│   │   └── ValueObjects/
│   │       └── EmailTests.cs
│   └── Infrastructure/
│       ├── Persistence/
│       │   └── ApplicationDbContextTests.cs
│       └── Services/
│           └── EmailServiceTests.cs
├── Integration/
│   ├── Application/
│   │   ├── Commands/
│   │   │   └── ClientCommandsIntegrationTests.cs
│   │   └── Queries/
│   │       └── ClientQueriesIntegrationTests.cs
│   └── Infrastructure/
│       └── Persistence/
│           └── ClientRepositoryIntegrationTests.cs
└── EndToEnd/
    ├── Controllers/
    │   └── ClientControllerTests.cs
    └── Api/
        └── ClientApiTests.cs
```

## Unit Testing Patterns

### Command Handler Tests

```csharp
[TestFixture]
public class CreateClientCommandHandlerTests
{
    private Mock<IApplicationDbContext> _mockContext;
    private Mock<IPublisher> _mockPublisher;
    private Mock<ICurrentUserService> _mockCurrentUser;
    private CreateClientCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockPublisher = new Mock<IPublisher>();
        _mockCurrentUser = new Mock<ICurrentUserService>();
        
        _mockCurrentUser.Setup(x => x.UserId).Returns("test-user-id");
        
        var mockDbSet = new Mock<DbSet<Client>>();
        _mockContext.Setup(x => x.Clients).Returns(mockDbSet.Object);
        _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

        _handler = new CreateClientCommandHandler(
            _mockContext.Object, 
            _mockPublisher.Object, 
            _mockCurrentUser.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_ShouldCreateClient()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "Test Client",
            Email = "test@example.com",
            Phone = "123-456-7890"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.GreaterThan(0));
        _mockContext.Verify(x => x.Clients.Add(It.IsAny<Client>()), Times.Once);
        _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ValidCommand_ShouldPublishDomainEvent()
    {
        // Arrange
        var command = new CreateClientCommand { Name = "Test Client", Email = "test@example.com" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockPublisher.Verify(
            x => x.Publish(It.IsAny<ClientCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ValidCommand_ShouldSetAuditFields()
    {
        // Arrange
        var command = new CreateClientCommand { Name = "Test Client", Email = "test@example.com" };
        Client capturedClient = null;

        _mockContext.Setup(x => x.Clients.Add(It.IsAny<Client>()))
                   .Callback<Client>(client => capturedClient = client);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(capturedClient, Is.Not.Null);
        Assert.That(capturedClient.CreatedBy, Is.EqualTo("test-user-id"));
        Assert.That(capturedClient.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
}
```

### Query Handler Tests

```csharp
[TestFixture]
public class GetClientQueryHandlerTests
{
    private Mock<IApplicationDbContext> _mockContext;
    private GetClientQueryHandler _handler;
    private List<Client> _clients;

    [SetUp]
    public void Setup()
    {
        _clients = new List<Client>
        {
            new() { Id = 1, Name = "Client 1", Email = "client1@example.com", IsActive = true },
            new() { Id = 2, Name = "Client 2", Email = "client2@example.com", IsActive = false }
        };

        var mockDbSet = MockDbSet.Create(_clients);
        _mockContext = new Mock<IApplicationDbContext>();
        _mockContext.Setup(x => x.Clients).Returns(mockDbSet);

        _handler = new GetClientQueryHandler(_mockContext.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ShouldReturnClient()
    {
        // Arrange
        var query = new GetClientQuery { Id = 1 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Client 1"));
    }

    [Test]
    public async Task Handle_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var query = new GetClientQuery { Id = 999 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_IncludeInactive_ShouldReturnInactiveClients()
    {
        // Arrange
        var query = new GetClientQuery { Id = 2, IncludeInactive = true };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.IsActive, Is.False);
    }
}
```

### Validator Tests

```csharp
[TestFixture]
public class CreateClientCommandValidatorTests
{
    private CreateClientCommandValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateClientCommandValidator();
    }

    [Test]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "Valid Client Name",
            Email = "valid@example.com",
            Phone = "123-456-7890"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task Validate_EmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "",
            Email = "valid@example.com"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required.");
    }

    [Test]
    public async Task Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "Valid Name",
            Email = "invalid-email"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Invalid email format.");
    }

    [Test]
    public async Task Validate_NameTooLong_ShouldHaveError()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = new string('A', 201), // Exceeds 200 character limit
            Email = "valid@example.com"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name must not exceed 200 characters.");
    }
}
```

### Domain Entity Tests

```csharp
[TestFixture]
public class ClientTests
{
    [Test]
    public void Create_ValidData_ShouldCreateClient()
    {
        // Arrange
        var name = "Test Client";
        var email = "test@example.com";

        // Act
        var client = new Client
        {
            Name = name,
            Email = email
        };

        // Assert
        Assert.That(client.Name, Is.EqualTo(name));
        Assert.That(client.Email, Is.EqualTo(email));
        Assert.That(client.IsActive, Is.True); // Default value
    }

    [Test]
    public void Activate_InactiveClient_ShouldActivateClient()
    {
        // Arrange
        var client = new Client { Name = "Test", Email = "test@example.com", IsActive = false };

        // Act
        client.Activate();

        // Assert
        Assert.That(client.IsActive, Is.True);
    }

    [Test]
    public void Deactivate_ActiveClient_ShouldDeactivateClient()
    {
        // Arrange
        var client = new Client { Name = "Test", Email = "test@example.com", IsActive = true };

        // Act
        client.Deactivate();

        // Assert
        Assert.That(client.IsActive, Is.False);
    }

    [Test]
    public void UpdateContactInfo_ValidData_ShouldUpdateInfo()
    {
        // Arrange
        var client = new Client { Name = "Test", Email = "old@example.com" };
        var newEmail = "new@example.com";
        var newPhone = "123-456-7890";

        // Act
        client.UpdateContactInfo(newEmail, newPhone);

        // Assert
        Assert.That(client.Email, Is.EqualTo(newEmail));
        Assert.That(client.Phone, Is.EqualTo(newPhone));
    }
}
```

## Integration Testing Patterns

### Database Integration Tests

```csharp
[TestFixture]
public class ClientCommandsIntegrationTests : BaseIntegrationTest
{
    [Test]
    public async Task CreateClient_ValidCommand_ShouldPersistToDatabase()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "Integration Test Client",
            Email = "integration@example.com",
            Phone = "123-456-7890"
        };

        // Act
        var clientId = await SendAsync(command);

        // Assert
        var client = await FindAsync<Client>(clientId);
        Assert.That(client, Is.Not.Null);
        Assert.That(client.Name, Is.EqualTo(command.Name));
        Assert.That(client.Email, Is.EqualTo(command.Email));
        Assert.That(client.Phone, Is.EqualTo(command.Phone));
    }

    [Test]
    public async Task UpdateClient_ValidCommand_ShouldUpdateDatabase()
    {
        // Arrange
        var clientId = await CreateClientAsync("Original Name", "original@example.com");
        var command = new UpdateClientCommand
        {
            Id = clientId,
            Name = "Updated Name",
            Email = "updated@example.com"
        };

        // Act
        await SendAsync(command);

        // Assert
        var client = await FindAsync<Client>(clientId);
        Assert.That(client.Name, Is.EqualTo("Updated Name"));
        Assert.That(client.Email, Is.EqualTo("updated@example.com"));
    }

    [Test]
    public async Task DeleteClient_ExistingClient_ShouldRemoveFromDatabase()
    {
        // Arrange
        var clientId = await CreateClientAsync("Test Client", "test@example.com");
        var command = new DeleteClientCommand { Id = clientId };

        // Act
        await SendAsync(command);

        // Assert
        var client = await FindAsync<Client>(clientId);
        Assert.That(client, Is.Null);
    }

    private async Task<int> CreateClientAsync(string name, string email)
    {
        var command = new CreateClientCommand { Name = name, Email = email };
        return await SendAsync(command);
    }
}
```

### Query Integration Tests

```csharp
[TestFixture]
public class ClientQueriesIntegrationTests : BaseIntegrationTest
{
    [Test]
    public async Task GetClients_WithData_ShouldReturnPaginatedResults()
    {
        // Arrange
        await CreateClientsAsync(15); // Create test data
        var query = new GetClientsQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result.Items, Has.Count.EqualTo(10));
        Assert.That(result.TotalCount, Is.EqualTo(15));
        Assert.That(result.TotalPages, Is.EqualTo(2));
        Assert.That(result.HasNextPage, Is.True);
    }

    [Test]
    public async Task GetClients_WithSearchTerm_ShouldReturnFilteredResults()
    {
        // Arrange
        await CreateClientAsync("Apple Corp", "apple@example.com");
        await CreateClientAsync("Banana Inc", "banana@example.com");
        await CreateClientAsync("Cherry LLC", "cherry@example.com");

        var query = new GetClientsQuery { SearchTerm = "Apple" };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items.First().Name, Is.EqualTo("Apple Corp"));
    }

    [Test]
    public async Task GetClient_ExistingId_ShouldReturnClientWithDetails()
    {
        // Arrange
        var clientId = await CreateClientAsync("Detailed Client", "detailed@example.com");
        var query = new GetClientQuery { Id = clientId };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(clientId));
        Assert.That(result.Name, Is.EqualTo("Detailed Client"));
        Assert.That(result.Email, Is.EqualTo("detailed@example.com"));
    }

    private async Task CreateClientsAsync(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            await CreateClientAsync($"Client {i}", $"client{i}@example.com");
        }
    }

    private async Task<int> CreateClientAsync(string name, string email)
    {
        var command = new CreateClientCommand { Name = name, Email = email };
        return await SendAsync(command);
    }
}
```

### Base Integration Test Class

```csharp
public abstract class BaseIntegrationTest : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;
    protected readonly IApplicationDbContext Context;
    protected readonly IMediator Mediator;

    protected BaseIntegrationTest()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace database with in-memory database
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

                    // Mock external services
                    services.Replace(ServiceDescriptor.Singleton<IEmailService, MockEmailService>());
                });
            });

        _scope = _factory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        Mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();

        // Ensure database is created
        ((ApplicationDbContext)Context).Database.EnsureCreated();
    }

    protected async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return await Mediator.Send(request);
    }

    protected async Task SendAsync(IRequest request)
    {
        await Mediator.Send(request);
    }

    protected async Task<T?> FindAsync<T>(params object[] keyValues) where T : class
    {
        return await ((ApplicationDbContext)Context).FindAsync<T>(keyValues);
    }

    protected async Task AddAsync<T>(T entity) where T : class
    {
        ((ApplicationDbContext)Context).Add(entity);
        await ((ApplicationDbContext)Context).SaveChangesAsync();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        _factory?.Dispose();
    }
}
```

## End-to-End Testing Patterns

### API Controller Tests

```csharp
[TestFixture]
public class ClientControllerTests : BaseApiTest
{
    [Test]
    public async Task CreateClient_ValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = new CreateClientCommand
        {
            Name = "API Test Client",
            Email = "api@example.com",
            Phone = "123-456-7890"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateClientResponse>(content);
        Assert.That(result.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetClient_ExistingId_ShouldReturnClient()
    {
        // Arrange
        var clientId = await CreateTestClientAsync();

        // Act
        var response = await Client.GetAsync($"/api/clients/{clientId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ClientDto>(content);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(clientId));
    }

    [Test]
    public async Task GetClients_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        await CreateMultipleTestClientsAsync(15);

        // Act
        var response = await Client.GetAsync("/api/clients?pageNumber=1&pageSize=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginatedList<ClientDto>>(content);
        
        Assert.That(result.Items, Has.Count.EqualTo(10));
        Assert.That(result.TotalCount, Is.EqualTo(15));
    }

    [Test]
    public async Task UpdateClient_ValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var clientId = await CreateTestClientAsync();
        var request = new UpdateClientCommand
        {
            Id = clientId,
            Name = "Updated Client Name",
            Email = "updated@example.com"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/clients/{clientId}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteClient_ExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var clientId = await CreateTestClientAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/clients/{clientId}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task CreateClient_InvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateClientCommand
        {
            Name = "", // Invalid: empty name
            Email = "invalid-email" // Invalid: bad email format
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/clients", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        var content = await response.Content.ReadAsStringAsync();
        var errors = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
        Assert.That(errors.Errors, Is.Not.Empty);
    }

    private async Task<int> CreateTestClientAsync()
    {
        var request = new CreateClientCommand
        {
            Name = "Test Client",
            Email = "test@example.com"
        };

        var response = await Client.PostAsJsonAsync("/api/clients", request);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateClientResponse>(content);
        return result.Id;
    }

    private async Task CreateMultipleTestClientsAsync(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            var request = new CreateClientCommand
            {
                Name = $"Test Client {i}",
                Email = $"test{i}@example.com"
            };
            await Client.PostAsJsonAsync("/api/clients", request);
        }
    }
}
```

### Base API Test Class

```csharp
public abstract class BaseApiTest : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient Client;

    protected BaseApiTest()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    // Replace database with in-memory database
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

                    // Mock external services
                    services.Replace(ServiceDescriptor.Singleton<IEmailService, MockEmailService>());
                    services.Replace(ServiceDescriptor.Singleton<ICurrentUserService, MockCurrentUserService>());

                    // Configure JSON options for testing
                    services.Configure<JsonOptions>(options =>
                    {
                        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });
                });
            });

        Client = _factory.CreateClient();
        
        // Set up authentication if needed
        SetupAuthentication();
    }

    private void SetupAuthentication()
    {
        // Add authorization header for testing
        Client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test", "ValidToken");
    }

    public void Dispose()
    {
        Client?.Dispose();
        _factory?.Dispose();
    }
}
```

## Test Data Builders

### Entity Builders

```csharp
public class ClientBuilder
{
    private Client _client;

    public ClientBuilder()
    {
        _client = new Client
        {
            Name = "Default Client",
            Email = "default@example.com",
            Phone = "123-456-7890",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test-user"
        };
    }

    public ClientBuilder WithId(int id)
    {
        _client.Id = id;
        return this;
    }

    public ClientBuilder WithName(string name)
    {
        _client.Name = name;
        return this;
    }

    public ClientBuilder WithEmail(string email)
    {
        _client.Email = email;
        return this;
    }

    public ClientBuilder WithPhone(string phone)
    {
        _client.Phone = phone;
        return this;
    }

    public ClientBuilder Inactive()
    {
        _client.IsActive = false;
        return this;
    }

    public ClientBuilder Active()
    {
        _client.IsActive = true;
        return this;
    }

    public ClientBuilder CreatedBy(string userId)
    {
        _client.CreatedBy = userId;
        return this;
    }

    public ClientBuilder CreatedAt(DateTime createdAt)
    {
        _client.CreatedAt = createdAt;
        return this;
    }

    public Client Build() => _client;

    public static implicit operator Client(ClientBuilder builder) => builder.Build();
}

// Usage in tests
[Test]
public void TestMethod()
{
    var client = new ClientBuilder()
        .WithName("Test Client")
        .WithEmail("test@example.com")
        .Inactive()
        .Build();
    
    // Or using implicit conversion
    Client client2 = new ClientBuilder()
        .WithName("Another Client")
        .WithEmail("another@example.com");
}
```

### Command Builders

```csharp
public class CreateClientCommandBuilder
{
    private CreateClientCommand _command;

    public CreateClientCommandBuilder()
    {
        _command = new CreateClientCommand
        {
            Name = "Default Client",
            Email = "default@example.com",
            Phone = "123-456-7890"
        };
    }

    public CreateClientCommandBuilder WithName(string name)
    {
        _command.Name = name;
        return this;
    }

    public CreateClientCommandBuilder WithEmail(string email)
    {
        _command.Email = email;
        return this;
    }

    public CreateClientCommandBuilder WithPhone(string phone)
    {
        _command.Phone = phone;
        return this;
    }

    public CreateClientCommandBuilder Invalid()
    {
        _command.Name = "";
        _command.Email = "invalid-email";
        return this;
    }

    public CreateClientCommand Build() => _command;

    public static implicit operator CreateClientCommand(CreateClientCommandBuilder builder) => builder.Build();
}
```

## Mock Services

### Mock External Services

```csharp
public class MockEmailService : IEmailService
{
    public List<EmailMessage> SentEmails { get; } = new();

    public Task SendEmailAsync(string to, string subject, string body)
    {
        SentEmails.Add(new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            SentAt = DateTime.UtcNow
        });
        
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return SendEmailAsync(email, "Welcome!", "Welcome to our service!");
    }

    public void Verify(string to, string subject)
    {
        var email = SentEmails.FirstOrDefault(e => e.To == to && e.Subject == subject);
        if (email == null)
        {
            throw new AssertionException($"No email sent to {to} with subject '{subject}'");
        }
    }

    public void VerifyCount(int expectedCount)
    {
        if (SentEmails.Count != expectedCount)
        {
            throw new AssertionException($"Expected {expectedCount} emails, but {SentEmails.Count} were sent");
        }
    }
}

public class EmailMessage
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime SentAt { get; set; }
}
```

### Mock Current User Service

```csharp
public class MockCurrentUserService : ICurrentUserService
{
    public string UserId { get; set; } = "test-user-id";
    public string Email { get; set; } = "test@example.com";
    public bool IsAuthenticated { get; set; } = true;
    public IEnumerable<string> Roles { get; set; } = new[] { "User" };

    public MockCurrentUserService WithUserId(string userId)
    {
        UserId = userId;
        return this;
    }

    public MockCurrentUserService WithEmail(string email)
    {
        Email = email;
        return this;
    }

    public MockCurrentUserService WithRoles(params string[] roles)
    {
        Roles = roles;
        return this;
    }

    public MockCurrentUserService AsAnonymous()
    {
        IsAuthenticated = false;
        UserId = null;
        Email = null;
        Roles = Array.Empty<string>();
        return this;
    }
}
```

## Performance Testing

### Load Testing

```csharp
[TestFixture]
public class ClientPerformanceTests : BaseIntegrationTest
{
    [Test]
    public async Task CreateClients_BulkOperations_ShouldMeetPerformanceRequirements()
    {
        // Arrange
        const int clientCount = 1000;
        var commands = Enumerable.Range(1, clientCount)
            .Select(i => new CreateClientCommand
            {
                Name = $"Performance Test Client {i}",
                Email = $"perf{i}@example.com"
            })
            .ToList();

        var stopwatch = Stopwatch.StartNew();

        // Act
        var tasks = commands.Select(command => SendAsync(command));
        await Task.WhenAll(tasks);

        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), 
            $"Creating {clientCount} clients took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 5s limit");

        var totalClients = await Context.Clients.CountAsync();
        Assert.That(totalClients, Is.EqualTo(clientCount));
    }

    [Test]
    public async Task GetClients_LargeDataset_ShouldReturnResultsQuickly()
    {
        // Arrange
        await CreateLargeDatasetAsync(10000);
        var query = new GetClientsQuery { PageSize = 50 };

        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await SendAsync(query);

        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000),
            $"Query took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 1s limit");
        Assert.That(result.Items, Has.Count.EqualTo(50));
    }

    private async Task CreateLargeDatasetAsync(int count)
    {
        var clients = Enumerable.Range(1, count)
            .Select(i => new Client
            {
                Name = $"Bulk Client {i}",
                Email = $"bulk{i}@example.com",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "bulk-import"
            })
            .ToList();

        ((ApplicationDbContext)Context).Clients.AddRange(clients);
        await ((ApplicationDbContext)Context).SaveChangesAsync();
    }
}
```

## Test Configuration

### Test Settings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=:memory:"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "EmailService": {
    "Provider": "Mock"
  },
  "Authentication": {
    "DisableAuthentication": true
  }
}
```

### Test Helpers

```csharp
public static class MockDbSet
{
    public static DbSet<T> Create<T>(IEnumerable<T> data) where T : class
    {
        var queryableData = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

        return mockSet.Object;
    }
}

public static class TestExtensions
{
    public static async Task<T> ShouldEventuallyReturn<T>(this Task<T> task, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        
        using var cts = new CancellationTokenSource(timeout.Value);
        try
        {
            return await task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Operation did not complete within {timeout.Value.TotalSeconds} seconds");
        }
    }
}
```

## Related Documentation

- [Clean Architecture](./clean-architecture.md) - Overall architecture patterns
- [CQRS](./cqrs.md) - Testing commands and queries
- [Validation](./validation.md) - Testing validation rules
- [Domain Events](./domain-events.md) - Testing event publishing and handling

## Conclusion

The QCLI testing patterns provide comprehensive coverage for Clean Architecture applications. The generated tests follow best practices for maintainability, performance, and reliability. Use these patterns to ensure your applications are thoroughly tested and maintain high quality standards throughout development and deployment cycles.
