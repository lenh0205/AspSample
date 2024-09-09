======================================================================
# Authentication in ASP.NET Core
* -> **Authentication** is responsible for **`providing the 'ClaimsPrincipal' for authorization`** to **make permission decisions against**

* _there are multiple **authentication scheme** approaches to select which **authentication handler** is responsible for **`generating the correct set of claims`**: **Authentication scheme**, **default authentication scheme, **directly set 'HttpContext.User'**_

## Service
* -> in ASP.NET Core, **authentication** is **`handled by the authentication service`** - **`IAuthenticationService`**, which is used by **authentication middleware** 

## Handler
* -> the **authentication service** uses **`registered authentication handlers`** to complete **authentication-related actions**

```r - Examples of authentication-related actions include:
// Authenticating a user.
// Responding when an unauthenticated user tries to access a restricted resource
```

### Definition
* -> is **`a type`** that **`implements the behavior of a scheme`**
* -> is **`derived`** from **IAuthenticationHandler** or **AuthenticationHandler<TOptions>**
* -> has the **primary responsibility** to **`authenticate users`**

### Mechanism
* _based on the **authentication scheme's configuration** and the **incoming request context**, authentication handlers:_

* -> **`an authentication scheme's authenticate action`** is responsible for construct **`user's identity`** based on **request context**
* _it returns an **`AuthenticateResult`** indicating whether **authentication was successful**, if so, the **`user's identity in an authentication ticket (AuthenticationTicket objects)`**_
* _a **cookie authentication scheme** constructing the **user's identity from cookies**_
* _a **JWT bearer scheme** deserializing and validating **a JWT bearer token to construct the user's identity**_

* -> return 'no result' or 'failure' if authentication is **unsuccessful**
* -> have **`methods for challenge and forbid actions`** for when users attempt to **access resources**: **`forbid`** - they're **unauthorized to access**, **`challenge`** - when they're **unauthenticated**

## Scheme
* -> _Schemes_ are useful as a mechanism for referring to the **`authentication`**, **`challenge`**, and **`forbid behaviors`** of the associated **handler**
* _ví dụ: 1 "authorization policy" can use `scheme names` to "specify which authentication scheme" (or schemes) should be used to authenticate the user_

### Definition
* -> the **`registered authentication handlers`** and **`their configuration options`** are called **schemes**
* _an authentication scheme is a name that corresponds to an **authentication handler**, **Options** for configuring that specific instance of the handler_

### Register authentication service with 'scheme' 
* _`Authentication schemes` are specified by `registering authentication services` in `Program.cs`_
* -> by **`calling a scheme-specific extension method`** after a call to **AddAuthentication** (_such as **AddJwtBearer** or **AddCookie**_)
* -> these _extension methods_ use **`AuthenticationBuilder.AddScheme()`** to **register schemes with appropriate settings** (_adds a **`AuthenticationScheme`** which can be used by **IAuthenticationService**_)
* -> less commonly, by calling _AuthenticationBuilder.AddScheme_ directly

```cs - register "authentication service" with multiple "schemes"
// registers authentication "services" and "handlers" for "cookie" and "JWT bearer" authentication schemes
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, // JWT bearer authentication scheme
        options => builder.Configuration.Bind("JwtSettings", options)) 
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, // cookie authentication scheme
        options => builder.Configuration.Bind("CookieSettings", options));

// "JwtBearerDefaults" là của thư viện "Microsoft.AspNetCore.Authentication.JwtBearer"

// "CookieAuthenticationDefaults.AuthenticationScheme" is a predefined constant with clarity and consistency for default name of the cookie authentication scheme; it value is just a string "Cookies"
// so we could provide a different name when calling ".AddCookie()" 
// Ex: we can config ".AddCookie("MyCustomCookieScheme", ...)" and add "[Authorize(AuthenticationSchemes = "MyCustomCookieScheme")]" to action to use it

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

* -> in some cases, the **call to AddAuthentication is automatically made by other extension methods**
* _For example, when using **`ASP.NET Core Identity`**, **AddAuthentication is called internally**_
```cs - ".AddIdentity" internally calls "AddAuthentication" with a default scheme "cookie authentication"
services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
```

### Specify 'scheme' usage
* -> _if `multiple schemes` are used_, **`authorization policies`** (or **`authorization attributes`**) can **specify the authentication scheme (or schemes)** they depend on to authenticate the user
* -> _when `a specific scheme` isn't requested_, the **`AddAuthentication parameter`** is the name of the **scheme that will be use by default**

* _if multiple schemes are registered and the default scheme isn't specified, **scheme must be specified in the authorize attribute**, otherwise an error is thrown_

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

### DefaultScheme
* _when there is **`only a single authentication scheme registered`**, the single authentication scheme:_
* -> is automatically used as the **`DefaultScheme`**
* -> **`eliminates the need to specify the DefaultScheme in AddAuthentication(IServiceCollection)`** (_or **AddAuthenticationCore(IServiceCollection)**_)

* -> to **`disable automatically`** using the **single authentication scheme as the DefaultScheme**, call **`AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressAutoDefaultScheme")`**

### Combine multiple scheme
* -> we can **combine multiple schemes into one** using **`policy schemes`**

## Middleware
* -> the **Authentication middleware is added in 'Program.cs'** by calling **`UseAuthentication`**
* -> _calling .UseAuthentication()_ **registers the middleware** that **`uses the previously registered authentication schemes`**
* -> _calling .UseAuthentication()_ **`before any middleware that depends on users being authenticated`**

======================================================================
# Other Authentication concepts

## RemoteAuthenticationHandler<TOptions>
* -> **JWT** and **cookies** don't use this since they **`can directly use the bearer header and cookie to authenticate`**
* -> **`OAuth 2.0`** and **`OIDC`** both will use this pattern

* -> **RemoteAuthenticationHandler<TOptions>** is the **`class for authentication that requires a remote authentication step`**
* -> when the **remote authentication step is finished**, the **`handler calls back to the 'CallbackPath' set by the handler`**
* -> the **handler finishes the authentication step** using the **`information passed to the 'HandleRemoteAuthenticateAsync' callback path`**  (Ex: authorization code or tokens)

* -> the **remotely hosted provider** in this case is **`the authentication provider`**
* -> examples include **Facebook, Twitter, Google, Microsoft, and any other OIDC provider** that handles authenticating users using the **`handlers mechanism`** (_nói về cơ chế khi tích hợp authentication provider like "Facebook, Google,..." với ASP.NET Core's authentication system_)

## Challenge
* -> _an authentication challenge_ is **`invoked by Authorization`** when an **unauthenticated user requests an endpoint** that requires **`authentication`**
* _for example, an authentication challenge is issued, when **an anonymous user** requests a **restricted resource** or **follows a login link**_

* -> _Authorization invokes a challenge_ using the **`specified authentication scheme(s), or the default if none is specified`**
* -> **a challenge action** should **`let the user know what authentication mechanism to use`** to **access the requested resource**

```r - Authentication challenge examples include:
// a cookie authentication scheme redirecting the user to a login page
// a JWT bearer scheme returning a 401 result with a "www-authenticate: bearer header"
```

## Forbid
* -> _an authentication scheme's forbid action_ is **`called by Authorization`** when an **`authenticated user`** attempts to **`access a resource`** they're not permitted to access

* -> **`a forbid action can let the user know`** that they're **authenticated** or they're **not permitted to access** the requested resource

```r - Authentication forbid examples include:
// a cookie authentication scheme redirecting the user to a page indicating access was forbidden.
// a JWT bearer scheme returning a 403 result.
// a custom authentication scheme redirecting to a page where the user can request access to the resource
```

## Authentication providers per tenant
* -> _ASP.NET Core_ **`doesn't have a built-in solution`** for **multi-tenant authentication** 
* _i **`multi-tenant`** refers to a single instance of an application serves multiple customers (tenants)_
* _i Ex: có nhiều company đang sử dụng application của ta, thì ta cần đảm bảo data of Company A should not be accessible to Company B_

* -> while it's possible for customers to **`write one using the built-in features`**, it's recommend customers consider **Orchard Core**, **ABP Framework**, or **Finbuckle.MultiTenant** for multi-tenant authentication

