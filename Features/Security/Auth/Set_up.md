
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
* set up the **authentication middleware** by configuring it in our application's **`Program.cs`** file:
* -> register the **authentication services** by making a call to the **`AddAuthentication`** method; configure **`JwtBearerDefaults.AuthenticationScheme`** as the default scheme.
* -> register the **JWT Bearer authentication scheme** by making a call to the **`AddJwtBearer`** method; configure our Identity platform **`domain as the authority`** and our Identity platform **`API Identifier as the audience`**, and be sure that our Identity platform domain and API Identifier are set in our application's **`appsettings.json`** file
* -> add the **authentication and authorization middleware** to the middleware pipeline by adding calls to the **`UseAuthentication`** and **`UseAuthorization`** methods under the var app = builder.Build(); method

* _in some cases, the **`access token`** will not have a **sub claim**_
* -> in this case, the **"User.Identity.Name" will be null**
* -> if we want to map a different claim to **`User.Identity.Name`**, add it to **options.TokenValidationParameters** within the **`AddJwtBearer()`** call

```c# - Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

var app = builder.Build();
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

## Validate scopes
* to ensure that an **`access token contains the correct scopes`**, use **Policy-Based Authorization in the ASP.NET Core**:
* -> create a new **authorization requirement** called **`HasScopeRequirement`**, which will check whether the **scope claim** issued by our Identity platform is present, and if so, will check that **`the claim contains the requested scope`**
-> under **`Program.cs`** file's _var builder = WebApplication.CreateBuilder(args);_ method, add a call to the **`app.AddAuthorization`** method
* -> **add policies for scopes** by calling **`AddPolicy`** for each scope
* -> register a **singleton** for the **`HasScopeHandler`** class

```c# - Program.cs
builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(
        "read:messages",
        policy => policy.Requirements.Add(
            new HasScopeRequirement("read:messages", domain)
        )
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
* -> the **JWT middleware** integrates with the **`standard ASP.NET Core Authentication and Authorization mechanisms`**
* -> to **secure an endpoint**, add the **`[Authorize] attribute`** to our **`controller action`** (_or the **`entire controller`** if you want to protect all of its actions_)
* -> when securing endpoints that require **`specific scopes`**, make sure that the **correct scope is present in the access_token**. To do so, add the **`Authorize attribute to the Scoped action`** and pass **`Scope value`** (_VD: read:messages_) as the **policy parameter**

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
    [Authorize("read:messages")]
    public IActionResult Scoped()
    {
        return Ok(new
        {
            Message = "Hello from a private-scoped endpoint!"
        });
    }
}
```

## Call API
* -> the way in which we call our API depends on the **`type of application`** we are developing and the **`framework`** we are using

### Get an access token
* -> regardless of the type of application we are developing or the framework we are using, **`to call our API`**, we need an **access token**

* -> if we are calling our API from a **Single-Page Application (SPA) or a Native application**, **`after the authorization flow completes`**, we will get an access token

* -> if we are calling the API from **`a command-line tool`** or **`another service`** where **a user entering credentials does not exist**, use the **OAuth Client Credentials Flow**
* -> to do so, register a **Machine-to-Machine Application**, and pass in the **Client ID** as the **`client_id`** parameter, the **Client Secret** as the **`client_secret`** parameter, 
* -> and the **API Identifier** (the same value we used to configure the middleware earlier) as the **`audience`** parameter when making the following request:

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
* -> now that we have an access token, we can use it to **`call secure API endpoints`**
* -> When calling a secure endpoint, we must **include the access token** as a **`Bearer token`** in the **`Authorization header`** of the request
* -> For example, we can make a request to the **`/api/private`** endpoint

```r 
curl --request GET \
  --url http://localhost:3010/api/private \
  --header 'authorization: Bearer YOUR_ACCESS_TOKEN'
```

* -> call the **`/api/private-scoped`** endpoint in a similar way, but ensure that the API permissions are configured correctly and that the access token includes the scope value (_VD: read:messages_)