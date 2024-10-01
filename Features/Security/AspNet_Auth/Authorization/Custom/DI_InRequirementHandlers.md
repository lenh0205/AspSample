# Dependency injection in requirement handlers in ASP.NET Core
* -> **`Authorization handlers must be registered in the service collection`** during configuration using dependency injection
* -> suppose we had **a repository of rules** we wanted to **`evaluate inside an authorization handler`** and that **repository was registered in the service collection**
* -> **Authorization** will **`resolves and injects that into the constructor`**

```cs - For example: to use the .NET logging infrastructure, inject "ILoggerFactory" into the handler
public class SampleAuthorizationHandler : AuthorizationHandler<SampleRequirement>
{
    private readonly ILogger _logger;

    public SampleAuthorizationHandler(ILoggerFactory loggerFactory)
        => _logger = loggerFactory.CreateLogger(GetType().FullName);

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, SampleRequirement requirement)
    {
        _logger.LogInformation("Inside my handler");

        // ...

        return Task.CompletedTask;
    }
}
```

* -> the preceding handler can be registered with any **service lifetime**
* -> however, **`don't register authorization handlers that use Entity Framework (EF) as singletons`**

```cs - uses "AddSingleton" to register the preceding handler:
builder.Services.AddSingleton<IAuthorizationHandler, SampleAuthorizationHandler>();
```