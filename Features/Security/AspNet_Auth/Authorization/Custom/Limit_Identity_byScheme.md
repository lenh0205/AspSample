> Specifying the default scheme results in the HttpContext.User property being set to that identity. If that behavior isn't desired, disable it by invoking the parameterless form of AddAuthentication.

=================================================================
# Authorize with a specific scheme in ASP.NET Core
* -> in some scenarios, such as **`Single Page Applications (SPAs)`**, it's common to **`use multiple authentication methods`**
* _For example, the app may **use `cookie-based` authentication to `log in`** and **`JWT bearer` authentication for `JavaScript requests`**

* -> in some cases, the app may have **`multiple instances of an authentication handler`**
* _For example, `two cookie handlers` where one **contains a basic identity** and one is **created when a multi-factor authentication (MFA) has been triggered**_
* _i **`MFA`** may be triggered because the **user requested an operation** that **`requires extra security`**_

```cs - an authentication scheme is named when the authentication service is configured during authentication
var builder = WebApplication.CreateBuilder(args);

// two authentication handlers have been added: one for cookies and one for bearer
builder.Services.AddAuthentication() // this one
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Unauthorized/";
            options.AccessDeniedPath = "/Account/Forbidden/";
        })
        .AddJwtBearer(options =>
        {
            options.Audience = "http://localhost:5001/";
            options.Authority = "http://localhost:5000/";
        });

builder.Services.AddAuthentication()
        .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
```

=================================================================
# Selecting the scheme with the 'Authorize' attribute
* -> **at the point of `authorization`**, the app **`indicates the handler to be used`**
* -> **select the handler** with which the app will **authorize** by **`passing a comma-delimited list of authentication schemes to [Authorize]`**
* -> the **[Authorize] attribute** specifies **`the authentication scheme or schemes`** to use **regardless of whether a default is configured**

```cs - For example:
// both the cookie and bearer handlers run 
// and have a chance to create and append an identity for the current user
[Authorize(AuthenticationSchemes = AuthSchemes)]
public class MixedController : Controller
{
    private const string AuthSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme;
    public ContentResult Index() => Content(MyWidgets.GetMyContent());
}

// specifying a single scheme only, only the handler with the "Bearer" scheme runs
// Any cookie-based identities are ignored
[Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
public class Mixed2Controller : Controller
{
    public ContentResult Index() => Content(MyWidgets.GetMyContent());
}
```

=================================================================
# Selecting the scheme with policies
* -> if we prefer to **`specify the desired schemes in policy`**, we can **set the `AuthenticationSchemes` collection when adding a policy**:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options => // this one
{
    // the "Over18" policy only runs against the identity created by the "Bearer" handler
    options.AddPolicy("Over18", policy => 
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new MinimumAgeRequirement(18));
    });
});

builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


//  Use the policy by setting the [Authorize] attribute's Policy property:
[Authorize(Policy = "Over18")]
public class RegistrationController : Controller
{
}
```

=================================================================
# Use multiple authentication schemes
* -> some apps may **need to support `multiple types of authentication`**
* -> in this case, the app should **`accept a JWT bearer token from several issuers`**

```r - For example: 
// our app might "authenticate" users from "Azure Active Directory" and from "a users database"
// another example is an app that "authenticates" users from both "Active Directory Federation Services" and "Azure Active Directory B2C"
```

* -> **add all authentication schemes** we'd like to accept
* -> however, **`only one`** **JWT bearer authentication** can by registered with the default authentication scheme **`JwtBearerDefaults.AuthenticationScheme`**
* -> additional authentication has to be **registered with a unique authentication scheme**
* -> as **the default authorization policy is `overridden`**, it's possible to use the **[Authorize] attribute** in controllers; the controller then accepts requests with **`JWT issued by the first or second issuer`**

* -> there's an **`issue`** when using multiple authentication schemes: https://github.com/dotnet/aspnetcore/issues/26002

```cs
var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // this one
        .AddJwtBearer(options =>
        {
            options.Audience = "https://localhost:5000/";
            options.Authority = "https://localhost:5000/identity/";
        })
        .AddJwtBearer("AzureAD", options =>
        {
            options.Audience = "https://localhost:5000/";
            options.Authority = "https://login.microsoftonline.com/eb971100-7f436/";
        });

// Authorization
// update the default authorization policy to accept both authentication schemes
builder.Services.AddAuthorization(options => // this one
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme,
        "AzureAD");
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddAuthentication()
        .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
```

* _uses **Azure Active Directory B2C** and another **Azure Active Directory tenant**:_
* -> **ForwardDefaultSelector** is used to **select a `default scheme` for the current request** that **`authentication handlers should forward all authentication operations to by default`**
* -> **the default forwarding logic** checks the most specific **`ForwardAuthenticate`**, **`ForwardChallenge`**, **`ForwardForbid`**, **`ForwardSignIn`**, and **`ForwardSignOut`** setting first, 
* -> followed by checking the **`ForwardDefaultSelector`**, followed by **`ForwardDefault`**
* -> the **`first non null result`** is used as **the target scheme to forward to**

```cs
var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services.AddAuthentication(options => // this one
{
    options.DefaultScheme = "B2C_OR_AAD";
    options.DefaultChallengeScheme = "B2C_OR_AAD";
})
.AddJwtBearer("B2C", jwtOptions =>
{
    jwtOptions.MetadataAddress = "B2C-MetadataAddress";
    jwtOptions.Authority = "B2C-Authority";
    jwtOptions.Audience = "B2C-Audience";
})
.AddJwtBearer("AAD", jwtOptions =>
{
    jwtOptions.MetadataAddress = "AAD-MetadataAddress";
    jwtOptions.Authority = "AAD-Authority";
    jwtOptions.Audience = "AAD-Audience";
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudiences = builder.Configuration.GetSection("ValidAudiences").Get<string[]>(),
        ValidIssuers = builder.Configuration.GetSection("ValidIssuers").Get<string[]>()
    };
})
.AddPolicyScheme("B2C_OR_AAD", "B2C_OR_AAD", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string authorization = context.Request.Headers[HeaderNames.Authorization];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            var token = authorization.Substring("Bearer ".Length).Trim();
            var jwtHandler = new JwtSecurityTokenHandler();

            return (jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Equals("B2C-Authority"))
                ? "B2C" : "AAD";
        }
        return "AAD";
    };
});

builder.Services.AddAuthentication()
        .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
```