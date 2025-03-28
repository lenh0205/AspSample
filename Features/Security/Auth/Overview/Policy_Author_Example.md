
# ASP.NET Core Web API Application

## Define permissions (Scopes)
* -> **`permissions`** let us define **`how resources can be accessed`** on behalf of the user with **a given access token**
* -> define allowed permissions through Identity Platform

```r - Example
// we might choose to grant "read access" to the messages resource if users have the "manager access level" => "read:message" scope
// and a "write access" to that resource if they have the "administrator access level"
```

## Install Dependencies
* -> to **allow our application to validate access tokens**, add a reference to the **`Microsoft.AspNetCore.Authentication.JwtBearer`** Nuget package

## Configure the middleware

```c# - Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // configure default scheme
    .AddJwtBearer(options => // register the JWT Bearer authentication scheme
    {
        // config our Identity platform domain as the authority
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";

        // config Identity platform API Identifier as the audience
        options.Audience = builder.Configuration["Auth0:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

var app = builder.Build();

// add the authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();
```

```json - appsettings.json
{
  "Auth0": {
    "Domain": "dev-fstvn12lsvswf2xl.us.auth0.com",
    "Audience": "https://lenh-auth"
  }
}
```

## Validate scopes - Policy-Based Authorization in the ASP.NET Core

```c# - Program.cs
builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy("read:messages", policy =>
                // check the "scope" claim
                policy.Requirements.Add(new HasScopeRequirement("read:messages", domain))
        );
    });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
```

```c# - HasScopeHandler.cs
public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
  protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    HasScopeRequirement requirement
  ) {
    // If user does not have the scope claim, get out of here
    if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
      return Task.CompletedTask;

    // Split the scopes string into an array
    var scopes = context.User
      .FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer).Value.Split(' ');

    // Succeed if the scope array contains the required scope
    if (scopes.Any(s => s == requirement.Scope))
      context.Succeed(requirement);

    return Task.CompletedTask;
  }
}
```

```c# - HasScopeRequirement.cs
public class HasScopeRequirement : IAuthorizationRequirement
{
    public string Issuer { get; }
    public string Scope { get; }

    public HasScopeRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }
}
```

## Protect API endpoints

```c#
[Route("api")]
public class ApiController : Controller
{
    [HttpGet("private")]
    [Authorize]
    public IActionResult Private()
    {
        return Ok(new
        {
            Message = "Hello from a private endpoint!"
        });
    }

    [HttpGet("private-scoped")]
    [Authorize("read:messages")] // validate "scope"
    public IActionResult Scoped()
    {
        return Ok(new
        {
            Message = "Hello from a private-scoped endpoint!"
        });
    }
}
```

## Get an access token
* _regardless of the type of application we are developing or the framework we are using, we need an **`access token`** to call our API
* -> if we are calling our API from a **Single-Page Application (SPA) or a Native application**, **`after the authorization flow completes`**, we will get an access token
* -> if we are calling the API from **CLI or another service - where a user entering credentials does not exist**, use the **OAuth Client Credentials Flow**
* _to do so, register a **`Machine-to-Machine Application`** and pass **client_id** and **client_secret** parameter_ 
* _and the **API Identifier** (the same value we used to configure the middleware earlier) as the **`audience`** parameter when making the following request:_

```r 
curl --request POST \
  --url 'https://dev-fstvn12lsvswf2xl.us.auth0.com/oauth/token' \
  --header 'content-type: application/x-www-form-urlencoded' \
  --data grant_type=client_credentials \
  --data 'client_id=9YZPz8tmDEfycqe8igbP0DI4EXGPZGe7' \
  --data client_secret=YOUR_CLIENT_SECRET \
  --data 'audience=https://lenh-auth'
```

### Call a secure endpoint
* -> When calling a secure endpoint, we must **include the access token** as a **`Bearer token`** in the **`Authorization header`** of the request
* -> For example, we can make a request to the **`/api/private`** endpoint

```r 
curl --request GET \
  --url http://localhost:3010/api/private \
  --header 'authorization: Bearer YOUR_ACCESS_TOKEN'
```

* -> call the **`/api/private-scoped`** endpoint in a similar way, but ensure that the API permissions are configured correctly and that the access token includes the scope value (_VD: read:messages_)
