======================================================================
# Authentication Scheme

## Definition
* _an authentication scheme is a name that corresponds to an **authentication handler**, **Options** for configuring that specific instance of the handler_

## Specific 'Authentication Scheme' by registering authentication service  
* -> by **`calling a scheme-specific extension method`** after a call to **AddAuthentication** (_such as **`AddJwtBearer`** or **`AddCookie`**_) or **`calling AuthenticationBuilder.AddScheme directly`**
* -> these "extension methods" use **`AuthenticationBuilder.AddScheme()`** to **register `schemes` with appropriate `settings`** 

```cs - register "authentication service" with multiple "schemes"
// registers authentication "services" and "handlers" for "cookie" and "JWT bearer" authentication schemes
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, // JWT bearer authentication scheme
        options => builder.Configuration.Bind("JwtSettings", options)) 
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, // cookie authentication scheme
        options => builder.Configuration.Bind("CookieSettings", options));

// "JwtBearerDefaults" là của thư viện "Microsoft.AspNetCore.Authentication.JwtBearer"

// "CookieAuthenticationDefaults.AuthenticationScheme" is a predefined constant with clarity and consistency for default name of the cookie authentication scheme; 
// -> it value is just a string "Cookies"; so we could provide a different name when calling ".AddCookie()" 
// -> Ex: we can config ".AddCookie("MyCustomCookieScheme", ...)" and add "[Authorize(AuthenticationSchemes = "MyCustomCookieScheme")]" to action to use it

// "options => builder.Configuration.Bind("JwtSettings", options))" 
// -> get configuration settings from our application's configuration sources - like "appsettings.json"
// -> bind it to the "JwtBearerOptions options" object
// -> this typically includes settings like the "Issuer", "Audience", "Key", and others that are required for "JWT authentication"
// -> so our "appsettings.json" might look like this:
{
  "JwtSettings": {
    "Issuer": "yourIssuer",
    "Audience": "yourAudience",
    "Key": "yourSecretKey",
    "ExpireMinutes": 60
  }
}
{
  "CookieSettings": {
    "CookieName": "YourAppName.Cookie", // name of cookie used for authentication
    "ExpireTimeSpan": "00:30:00", // 30 minutes is duration for which the cookie is valid
    "SlidingExpiration": true, // the cookie expiration time is extended on each request, as long as the user is active
    "RequireHttps": true, // specifies whether the cookie should only be sent over HTTPS
    "LoginPath": "/Account/Login", // path to redirect users when they need to log in
    "LogoutPath": "/Account/Logout", // path to redirect users when they log out
    "AccessDeniedPath": "/Account/AccessDenied", // path to redirect users when they are denied access
    "CookieSecurePolicy": "Always" // controls when the cookie should be sent - Always, None, SameAsRequest
  }
}
```

* -> in some cases, the **call to `AddAuthentication` is automatically made by other extension methods**
* -> for example, when using **`ASP.NET Core Identity`**, **AddAuthentication is called internally**

```cs - ".AddIdentity" internally calls "AddAuthentication" with a default scheme "cookie authentication"
services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
```

## DefaultScheme
* -> when **configuring authentication**, it's **`common to specify the default authentication scheme`** (_the default scheme is used unless a resource requests a specific scheme)

### Single Authentication scheme as 'DefaultScheme'
* _when there is **`only a single authentication scheme registered`**, the single authentication scheme:_
* -> is automatically used as the **`DefaultScheme`**
* -> **`eliminates the need to specify the DefaultScheme`** in **AddAuthentication(IServiceCollection)** (_or AddAuthenticationCore(IServiceCollection)_)
* => to **`disable automatically` using the `single authentication scheme` as the `DefaultScheme`**, call **AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressAutoDefaultScheme")**

### use multiple default scheme
* -> it's also possible to **`specify different default schemes`** to use for **authenticate**, **challenge**, and **forbid** actions (xem `~\Features\Security\AspNet_Auth\NET_Terminology.md` để hiểu)

### the default scheme is not specified
* -> there's **`no automatic probing of schemes`**; if **`the default scheme isn't specified`**, the **scheme must be specified in the `authorize attribute`**
* -> otherwise, the following error is thrown: 
```r
InvalidOperationException: No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).
```

## Multiple Schemes
* -> _if `multiple schemes` are used_, **`authorization policies`** (or **`authorization attributes`**) can **specify the authentication scheme (or schemes)** they depend on to authenticate the user
* -> _when `a specific scheme` isn't requested_, the **`AddAuthentication parameter`** is the name of the **scheme that will be use by default**
* -> _if `multiple schemes` are registered and the `default scheme isn't specified`_, **scheme must be specified in the authorize attribute**, otherwise an **`error is thrown`**

* -> we can **combine multiple schemes into one** using **`policy schemes`**

```cs - Ex: follow the example above, using "Authorization Attributes" in our controller
[Authorize] // no scheme specified, uses the JWT by default
// -> because our config ".AddAuthentication(JwtBearerDefaults.AuthenticationScheme)"
public IActionResult Get()
{
    return Ok("This is a protected resource.");
}

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // specific scheme Cookie
// because our config "CookieAuthenticationDefaults.AuthenticationScheme" as cookie authentication scheme name
public IActionResult GetCookieProtected()
{
    return Ok("This is a protected resource using Cookie Authentication.");
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // specific scheme JWT
public IActionResult GetJwtProtected()
{
    return Ok("This is a protected resource using Jwt Authentication.");
}
```

```cs - using "Authorization Policies"
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtPolicy", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));

    options.AddPolicy("CookiePolicy", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme));
});

// Applying policies
[Authorize(Policy = "JwtPolicy")]
public IActionResult GetWithJwtPolicy()
{
    return Ok("Protected by JWT policy.");
}
```
