========================================================================
# Policy-based authorization in ASP.NET Core
* -> underneath the covers, **role-based authorization** and **claims-based authorization** use **`a requirement`**, **`a requirement handler`**, and **`a preconfigured policy`**
* => these building blocks support the **expression of authorization evaluations in code**; the result is a richer, reusable, testable authorization structure

* -> **an authorization policy** consists of one or more **`requirements`**
* -> register it as part of the **authorization service configuration**, in the app's Program.cs file:
```cs
builder.Services.AddAuthorization(options =>
{
    // an "AtLeast21" policy is created
    options.AddPolicy("AtLeast21", policy =>
        // has a single requirement—that of a minimum age, which is supplied as a parameter to the requirement
        policy.Requirements.Add(new MinimumAgeRequirement(21)));
});
```

# For Authorization
* -> use **`IAuthorizationService`**, **`[Authorize(Policy = "Something")]`**, or **`RequireAuthorization("Something")`**

```cs - Ex:  a typical authorization service configuration:
// Add all of your handlers to DI.
builder.Services.AddSingleton<IAuthorizationHandler, MyHandler1>();
// MyHandler2, ...

builder.Services.AddSingleton<IAuthorizationHandler, MyHandlerN>();

// Configure your policies
builder.Services.AddAuthorization(options =>
    options.AddPolicy(
        "Something", 
        policy => policy.RequireClaim("Permission", "CanViewPage", "CanViewAnything")
    ));
```

========================================================================
# IAuthorizationService
* -> the primary service that **`determines if authorization is successful`** is **IAuthorizationService**:
```cs
public interface IAuthorizationService
{
    Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, 
                                     IEnumerable<IAuthorizationRequirement> requirements);

    Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName);
}
```

* _the **simplified default implementation** of **`the authorization service`**:_
```cs
public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, 
             object resource, IEnumerable<IAuthorizationRequirement> requirements)
{
    // Create a tracking context from the authorization inputs.
    var authContext = _contextFactory.CreateContext(requirements, user, resource);

    // By default this returns an IEnumerable<IAuthorizationHandler> from DI.
    var handlers = await _handlers.GetHandlersAsync(authContext);

    // Invoke all handlers.
    foreach (var handler in handlers)
    {
        await handler.HandleAsync(authContext);
    }

    // Check the context, by default success is when all requirements have been met.
    return _evaluator.Evaluate(authContext);
}
```

## IAuthorizationRequirement
* -> "IAuthorizationRequirement" is **a marker service with no methods**, and **`the mechanism for tracking whether authorization is successful`**

## IAuthorizationHandler
* -> each "IAuthorizationHandler" is responsible for **`checking if requirements are met`**:
```cs
/// <summary>
/// Classes implementing this interface are able to make a decision if authorization
/// is allowed.
/// </summary>
public interface IAuthorizationHandler
{
    /// <summary>
    /// Makes a decision if authorization is allowed.
    /// </summary>
    /// <param name="context">The authorization information.</param>
    Task HandleAsync(AuthorizationHandlerContext context);
}
```

## AuthorizationHandlerContext
* -> the **`AuthorizationHandlerContext`** class is what **the handler uses to mark whether requirements have been met**:
```cs
context.Succeed(requirement)
```

========================================================================
# Apply policies to MVC controllers
* -> apply policies to controllers by using the **[Authorize] attribute with the policy name**:
```cs
[Authorize(Policy = "AtLeast21")]
public class AtLeast21Controller : Controller
{
    public IActionResult Index() => View();
}
```

* -> if **`multiple policies`** are applied at the **controller** and **action** levels, all policies must pass before access is granted
```cs
[Authorize(Policy = "AtLeast21")]
public class AtLeast21Controller2 : Controller
{
    [Authorize(Policy = "IdentificationValidated")]
    public IActionResult Index() => View();
}
```

# Apply policies to Razor Pages
* -> apply policies to Razor Pages by using the **`[Authorize] attribute with the policy name`**
* -> policies **`can not be applied at the Razor Page handler level`**, they must be applied to the Page
* -> **policies can also be applied to Razor Pages** by using **`an authorization convention`**

```cs
[Authorize(Policy = "AtLeast21")]
public class AtLeast21Model : PageModel { }
```

# Apply policies to endpoints
* -> apply policies to **endpoints** by using **`RequireAuthorization`** with the policy name
```cs
app.MapGet("/helloworld", () => "Hello World!").RequireAuthorization("AtLeast21");
```

========================================================================
# Requirements
* -> **an authorization requirement** is **`a collection of data parameters`** that **`a policy can use to evaluate the current user principal`**
* -> if **an authorization policy** contains **`multiple authorization requirements`**, **`all requirements must pass in order for the policy evaluation to succeed`**

```cs - Ex:
// in our "AtLeast21" policy, the requirement is a single parameter—the minimum age
// a requirement implements "IAuthorizationRequirement", which is an empty marker interface
// a parameterized minimum age requirement could be implemented as follows:
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;
    public int MinimumAge { get; }
}
```

========================================================================
# Authorization handlers
* -> "an authorization handler" is **`responsible for the evaluation of a requirement's properties`**
* -> "the authorization handler" **`evaluates the requirements against a provided 'AuthorizationHandlerContext'`** to determine **if access is allowed**

* -> **a requirement** can have **`multiple handlers`**
* -> **a handler** may inherit **`AuthorizationHandler<TRequirement>`**, where TRequirement is the requirement to be handled
* -> alternatively, **a handler may implement 'IAuthorizationHandler' directly** to **`handle more than one type of requirement`**

## Handler registration
* -> **register handlers** in the **`services collection during configuration`**

```cs
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
```

## Use a handler for one requirement
* -> a **`one-to-one relationship`** in which **a handler** handles **a single requirement**

```cs - 
// determines if the current "user principal" has a "date of birth claim" that has been issued by a known and trusted Issuer
// Authorization can't occur when the claim is missing, in which case a completed task is returned
// when a claim is present, the user's age is calculated
// If the user meets the minimum age defined by the requirement, authorization is considered successful
// when authorization is successful, "context.Succeed" is invoked with the satisfied requirement as its sole parameter

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(
            c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "http://contoso.com");

        if (dateOfBirthClaim is null)
        {
            return Task.CompletedTask;
        }

        var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
        int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
        {
            calculatedAge--;
        }

        if (calculatedAge >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

## Use a handler for multiple requirements
* -> **`a one-to-many relationship`** in which **a handler** can handle **different types of requirements**
* -> we traverses **`PendingRequirements`** - a property containing **requirements not marked as successful**

```cs
//  for a "ReadPermission" requirement, the user must be either an owner or a sponsor to access the requested resource
// for an "EditPermission" or "DeletePermission" requirement, they must be an owner to access the requested resource.
public class PermissionHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is ReadPermission)
            {
                if (IsOwner(context.User, context.Resource)
                    || IsSponsor(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement is EditPermission || requirement is DeletePermission)
            {
                if (IsOwner(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsOwner(ClaimsPrincipal user, object? resource)
    {
        // Code omitted for brevity
        return true;
    }

    private static bool IsSponsor(ClaimsPrincipal user, object? resource)
    {
        // Code omitted for brevity
        return true;
    }
}
```

========================================================================
# What should a handler return?

## Final result when calling "context.Fail" or "context.Succeed"
* -> **`a handler indicates success`** by calling **context.Succeed(IAuthorizationRequirement requirement)**, passing the requirement that has been successfully validated
* -> **`a handler doesn't need to handle failures`** generally, as **other handlers for the same requirement may succeed**
* -> to **`guarantee failure, even if other requirement handlers succeed`**, call **context.Fail**

## All handler still called
* -> if a handler calls **context.Succeed** or **context.Fail**, **`all other handlers are still called`**
* => this allows requirements to **`produce side effects`** (_such as logging_) which takes place even if another handler has **successfully validated** or **failed a requirement**

* -> **InvokeHandlersAfterFailure** defaults to **`true`**, in which case **`all handlers are called`**
* -> when set to **`false`**, the **InvokeHandlersAfterFailure** property **`short-circuits the execution of handlers`** when **context.Fail is called**

## Note
* -> **`Authorization handlers are called`** even if **authentication fails**
* -> also **`handlers can execute in any order`**, so **do not depend on them being called in any particular order**

========================================================================
# why would I want multiple handlers for a requirement?
* -> in cases where we want **evaluation to be on an "OR" basis**, **`implement multiple handlers for a single requirement`**
* -> ensure that **both handlers are registered**; if **`either handler succeeds when a policy evaluates the requirement`**, the **`policy evaluation succeeds`**

```r - For example:
// Microsoft has doors that only open with key cards
// if we leave our key card at home, the receptionist prints a temporary sticker and opens the door for us
// -> in this scenario, you would have a "single requirement" BuildingEntry, but "multiple handlers" each one examining a single requirement
```
```cs - BuildingEntryRequirement.cs
public class BuildingEntryRequirement : IAuthorizationRequirement { }
```

```cs - BadgeEntryHandler.cs
public class BadgeEntryHandler : AuthorizationHandler<BuildingEntryRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, BuildingEntryRequirement requirement)
    {
        if (context.User.HasClaim(
            c => c.Type == "BadgeId" && c.Issuer == "https://microsoftsecurity"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

```cs - TemporaryStickerHandler.cs
public class TemporaryStickerHandler : AuthorizationHandler<BuildingEntryRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, BuildingEntryRequirement requirement)
    {
        if (context.User.HasClaim(
            c => c.Type == "TemporaryBadgeId" && c.Issuer == "https://microsoftsecurity"))
        {
            // Code to check expiration date omitted for brevity.
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

========================================================================
# Use a func to fulfill a policy - policy require simple logic
* -> there may be situations in which **fulfilling a policy** is **`simple to express in code`**
* -> it's possible to supply a **`Func<AuthorizationHandlerContext, bool>`** when **configuring a policy** with the **`RequireAssertion`** policy builder

```cs - the previous "BadgeEntryHandler" could be rewritten as
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BadgeEntry", policy =>
        policy.RequireAssertion(context => context.User.HasClaim(c =>
            (c.Type == "BadgeId" || c.Type == "TemporaryBadgeId")
            && c.Issuer == "https://microsoftsecurity")));
});
```

========================================================================
# Access MVC request context in handlers
* -> the **`HandleRequirementAsync`** method has two parameters: an **AuthorizationHandlerContext** and the **TRequirement** being handled
* -> **frameworks such as MVC or SignalR** are **`free to add any object`** to the **`Resource`** property on the **AuthorizationHandlerContext** to **`pass extra information`**

## HttpContext
* -> when using **`endpoint routing`**, **authorization** is typically handled by the **Authorization Middleware**
* -> in this case, the **Resource** property is **`an instance of HttpContext`**
* -> **`the context can be used to access the current endpoint`**, which can be used to **probe the underlying resource to which we're routing**

```cs - For example:
if (context.Resource is HttpContext httpContext)
{
    var endpoint = httpContext.GetEndpoint();
    var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
    // ...
}
```

## AuthorizationFilterContext
* -> with **traditional routing (routes.MapRoute())**, or when authorization happens as **part of MVC's authorization filter**, the value of **`Resource`** is **`an AuthorizationFilterContext instance`**
* -> this property **`provides access`** to **HttpContext**, **RouteData**, and **everything else provided by MVC and Razor Pages**

```cs
using Microsoft.AspNetCore.Mvc.Filters;

if (context.Resource is AuthorizationFilterContext mvcContext)
{
    // Examine MVC-specific things like routing data.
}
```

## the use of the 'Resource' property is "framework-specific"
* -> using information in the Resource property **`limits our authorization policies to particular frameworks`** (_MVC / Razor /...._)
* -> **cast the 'Resource' property** using the **`is`** keyword, and then confirm the cast has succeeded to ensure our code doesn't crash with an **`InvalidCastException`** when run on other frameworks

========================================================================
# Globally require all users to be authenticated
 * -> using **`FallbackPolicy`** orr **`authorization filter in MVC or Razor pages`**

```cs - FallbackPolicy
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

========================================================================
# Authorization with external service sample
* -> sample code: https://github.com/dotnet/AspNetCore.Docs.Samples/tree/main/samples/aspnetcore-authz-with-ext-authz-service
* -> the sample **Contoso.API project** is secured with **Azure AD**
* -> an "additional authorization check" from the **Contoso.Security.API project** returns **`a payload describing`** whether the **Contoso.API client app** can **`invoke the "GetWeather" API`**

* => shows how to **`implement additional authorization requirements`** with **`an external authorization service`**

## Configure the sample
* -> create an **`application registration`** in our **Microsoft Entra ID tenant**
* -> assign it an **`AppRole`**
* -> under **API permissions**, add the **AppRole** as **`a permission`** and **`grant Admin consent`**
* _note that in this setup, this **app registration** represents both **`the API`** and **`the client`** invoking the API; however, we can create two app registrations if we like_
* _if we are using this setup, be sure to only **perform the API permissions, add AppRole as a permission step** for **`only the client`**; only the client app registration **`requires a client secret to be generated`**_

* _configure the **Contoso.API** project with the following settings:_
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "<Tenant name from AAD properties>.onmicrosoft.com">,
    "TenantId": "<Tenant Id from AAD properties>",
    "ClientId": "<Client Id from App Registration representing the API>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

* _configure **Contoso.Security.API** with the following settings:_
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AllowedClients": [
    "<Use the appropriate Client Id representing the Client calling the API>"
  ]
}
```

Open the ContosoAPI.collection.json file and configure an environment with the following:

ClientId: Client Id from app registration representing the client calling the API.
clientSecret: Client Secret from app registration representing the client calling the API.
TenantId: Tenant Id from AAD properties
Extract the commands from the ContosoAPI.collection.json file and use them to construct cURL commands to test the app.

Run the solution and use cURL to invoke the API. You can add breakpoints in the Contoso.Security.API.SecurityPolicyController and observe the client Id is being passed in that is used to assert whether it is allowed to Get Weather.