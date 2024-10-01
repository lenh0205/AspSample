
# Customize the behavior of 'AuthorizationMiddleware'
* -> Apps can register an **`IAuthorizationMiddlewareResultHandler`** to **`customize how 'AuthorizationMiddleware' handles authorization results`**
* -> Apps can use **IAuthorizationMiddlewareResultHandler** to return **`customized responses`** and enhance the **`default challenge`** or **`forbid responses`**

```cs - Ex: 
// implementation of "IAuthorizationMiddlewareResultHandler" 
// that returns a "custom response" for specific "authorization failures":
public class SampleAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If the authorization was forbidden and the resource had a specific requirement,
        // provide a custom 404 response.
        if (authorizeResult.Forbidden
            && authorizeResult.AuthorizationFailure!.FailedRequirements
                .OfType<Show404Requirement>().Any())
        {
            // Return a 404 to make it appear as if the resource doesn't exist.
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}

public class Show404Requirement : IAuthorizationRequirement { }


// Register this implementation of IAuthorizationMiddlewareResultHandler in Program.cs:
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler, SampleAuthorizationMiddlewareResultHandler>();

var app = builder.Build();
```

