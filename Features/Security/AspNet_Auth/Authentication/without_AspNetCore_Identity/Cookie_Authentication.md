> cần nhớ authentication session tạo bởi việc gọi `HttpContext.SignInAsync` khác với tradition session của server

====================================================================
# Summary
* -> nói chung là ta sẽ cần **.AddCookie** để thêm cookie service
* -> ta sẽ login bằng **`HttpContext.SignInAsync()`** với tham số là **scheme** ta chọn và **ClaimPriciple**

====================================================================
# Use cookie authentication without ASP.NET Core Identity
* -> **ASP.NET Core Identity** is a **`complete, full-featured authentication provider`** for creating and maintaining logins
* -> however, a **`cookie-based authentication provider`** without **ASP.NET Core Identity** can be used

# Sample App:
```r
// the user account for the hypothetical user, Maria Rodriguez, is hardcoded into the app
// use the Email address "maria.rodriguez@contoso.com" and "any password" to sign in the user
// the user is authenticated in the "AuthenticateUser" method in the "Pages/Account/Login.cshtml.cs file"
```

====================================================================
# Add cookie authentication
* -> add **the Authentication Middleware services** with the **`AddAuthentication`** and **`AddCookie`** methods
* -> call **`UseAuthentication`** and **`UseAuthorization`** to set the **HttpContext.User** property and run the **Authorization Middleware** for requests
* -> _UseAuthentiction and UseAuthorization_ **`must be called before Map methods`** such as **MapRazorPages** and **MapDefaultControllerRoute**

```cs
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // this one
    .AddCookie();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); // this one
app.UseAuthorization(); // this one

app.MapRazorPages(); // this one
app.MapDefaultControllerRoute(); // this one

app.Run();
```

## Default Authentication scheme
* -> **AuthenticationScheme passed to AddAuthentication** sets the **`default authentication scheme`** for the app
* -> _AuthenticationScheme is useful_ when there are **`multiple instances of cookie authentication`** and the app needs to authorize with a specific scheme

* -> setting the **`AuthenticationScheme`** to **CookieAuthenticationDefaults.AuthenticationScheme** provides **a value of Cookies** (_a normal string_) for the scheme
* -> however, **`any string`** value can be used that **distinguishes the scheme**

* -> the **app's authentication scheme** is different from the **app's cookie authentication scheme**
* -> when **`a cookie authentication scheme isn't provided to AddCookie`**, it uses **CookieAuthenticationDefaults.AuthenticationScheme**

* -> the authentication cookie's **`IsEssential`** property is set to **true** by default
* -> **`Authentication cookies are allowed`** when a site visitor **hasn't consented to data collection**

```cs - configure 'CookieAuthenticationOptions' in the 'AddCookie' method
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
        //options.LoginPath = "/Account/Login";
        //options.LogoutPath = "/Account/Logout";
        //options.Cookie.Name = "YourAppCookie";
        //options.Cookie.HttpOnly = true;
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
```

====================================================================
# Cookie Policy Middleware
* -> the **Cookie Policy Middleware UseCookiePolicy** **`enables "cookie policy" capabilities`**

* _use **`CookiePolicyOptions`** provided to the Cookie Policy Middleware to_
* -> **`control global characteristics of cookie processing`** 
* -> and **`hook into cookie processing handlers when cookies are appended or deleted`**

## MinimumSameSitePolicy
* -> the **MinimumSameSitePolicy** can **`affect the setting of 'Cookie.SameSite' in 'CookieAuthenticationOptions'`**
* -> the default **MinimumSameSitePolicy** value is **`SameSiteMode.Lax`** to **`permit OAuth2 authentication`**

* -> set the **MinimumSameSitePolicy = SameSiteMode.Strict** to strictly enforce a **`same-site policy`**
* -> although **`this setting breaks OAuth2 and other cross-origin authentication schemes`**,
* -> it **`elevates the level of cookie security`** for other types of apps that **don't rely on cross-origin request processing**

```cs
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};

app.UseCookiePolicy(cookiePolicyOptions);
```

```cs
// tức là trong trường hợp cả "MinimumSameSitePolicy" và "Cookie.SameSite" đều setting cho SameSiteMode thì kết quả cuối cũng sẽ là
+-----------------------+-----------------------+-------------------------------------+
| MinimumSameSitePolicy |   Cookie.SameSite     |  Resultant Cookie.SameSite setting  |
+-----------------------+-----------------------+-------------------------------------+
|  SameSiteMode.None	|  SameSiteMode.None    |         SameSiteMode.None           |
|                       |  SameSiteMode.Lax     |         SameSiteMode.Lax            |
|                       |  SameSiteMode.Strict	|         SameSiteMode.Strict         |
+-----------------------+-----------------------+-------------------------------------+
|  SameSiteMode.Lax	    |  SameSiteMode.None    |         SameSiteMode.Lax            |
|                       |  SameSiteMode.Lax     |         SameSiteMode.Lax            |
|                       |  SameSiteMode.Strict	|         SameSiteMode.Strict         |
+-----------------------+-----------------------+-------------------------------------+
|  SameSiteMode.Strict	|  SameSiteMode.None    |         SameSiteMode.Strict         |
|                       |  SameSiteMode.Lax     |         SameSiteMode.Strict         |
|                       |  SameSiteMode.Strict	|         SameSiteMode.Strict         |
+-----------------------+-----------------------+-------------------------------------+
```

====================================================================
# Create an authentication cookie - Login
* -> construct a **`ClaimsPrincipal`** **to `create a cookie` holding user information**; the **user information** is **`serialized and stored in the cookie`**
* -> create a **`ClaimsIdentity`** with any **`required Claims`** and call **`SignInAsync`** to **sign in the user**

* -> **`ASP.NET Core's Data Protection system`** is used for **encryption**
* -> for **an app hosted on multiple machines**, **load balancing across apps**, or **using a web farm**, configure data protection to **`use the same key ring and app identifier`**

## AuthenticationTicket - cookie-based auth tickets
* _a data structure that encapsulates the **user's identity** and **additional authentication-related information**_
* -> allows for **`stateless authentication`** - where the **server doesn't need to store session information**; by containing all necessary information within the ticket
* -> helps manage **`authenticated user sessions`** (_different from traditional server-side sessions_) by including information like "expiration time"

* _thằng này cũng hơi giống như JWT object trong JWT Authentication - cũng chứa claim và authen data; nhưng về cơ chế sử dụng thì khác nhau_

## Login
* -> **SignInAsync** **`creates an encrypted cookie`** and **`adds it to the current response`** (_if AuthenticationScheme isn't specified, the default scheme is used_)
* -> **RedirectUri** is only **`used on a few specific paths by default`**, for example, the login path and logout paths

```cs - Login.cshtml.cs
public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    ReturnUrl = returnUrl;

    if (ModelState.IsValid)
    {
        // Use Input.Email and Input.Password to authenticate the user
        // with our custom authentication logic.
        //
        // For demonstration purposes, the sample validates the user
        // on the email address maria.rodriguez@contoso.com with 
        // any password that passes model validation.

        var user = await AuthenticateUser(Input.Email, Input.Password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        var claims = new List<Claim> // this one
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, "Administrator"),
        };

        var claimsIdentity = new ClaimsIdentity( // this one
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties // this one
        {
            //AllowRefresh = <bool>,
            // Refreshing the authentication session should be allowed.

            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // The time at which the authentication ticket expires. A 
            // value set here overrides the ExpireTimeSpan option of 
            // CookieAuthenticationOptions set with AddCookie.

            //IsPersistent = true,
            // Whether the authentication session is persisted across 
            // multiple requests. When used with cookies, controls
            // whether the cookie's lifetime is absolute (matching the
            // lifetime of the authentication ticket) or session-based.

            //IssuedUtc = <DateTimeOffset>,
            // The time at which the authentication ticket was issued.

            //RedirectUri = <string>
            // The full path or absolute URI to be used as an http 
            // redirect response value.
        };

        // a new cookie is create -> cookie is configured with security settings (HttpOnly, Secure flag, ...)
        // create an authentication ticket -> serialize -> encrypted and signed -> store in cookie
        // cookie is added to the HTTP response
        await HttpContext.SignInAsync( // this one
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);

        _logger.LogInformation("User {Email} logged in at {Time}.", 
            user.Email, DateTime.UtcNow);

        return LocalRedirect(Url.GetLocalUrl(returnUrl));
    }

    // Something failed. Redisplay the form.
    return Page();
}
```

## Sign out
* -> to **`sign out the current user`** and **`delete their cookie`**, call **SignOutAsync**:
```cs
public async Task OnGetAsync(string returnUrl = null)
{
    if (!string.IsNullOrEmpty(ErrorMessage))
    {
        ModelState.AddModelError(string.Empty, ErrorMessage);
    }

    // Clear the existing external cookie
    await HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    ReturnUrl = returnUrl;
}
```

* -> if **CookieAuthenticationDefaults.AuthenticationScheme** (or "Cookies") isn't used as the "scheme", **`supply the scheme used when configuring the authentication provider`**
* -> otherwise, the **`default scheme is used`**
* _For example: if "ContosoCookie" is used as the scheme, supply the scheme used when configuring the authentication provider_

## Browser closed
* -> when the **`browser closes`** it **`automatically deletes session based cookies (non-persistent cookies)`**
* -> but **`no cookies are cleared when an individual tab is closed`**; the server is not notified of tab or browser close events

====================================================================
# React to back-end changes
* -> once a cookie is created, the **`cookie is the single source of identity`**; 

## Revoked users - a user account is disabled in back-end systems
* -> the **app's cookie authentication system** **`continues to process requests based on the authentication cookie`**
* -> **`the user remains signed into the app`** as long as **the authentication cookie is valid**
* _ý là việc authenticate các request khác vẫn hoạt động bình thường_

```r - Practical Example:
// "Account suspension": An admin disables a users account due to policy violations, but the user still has an active session.
// "Security breach": If a users account is compromised, it might be disabled as a precaution, but their existing sessions remain active
// "Employee termination": In a corporate setting, when an employee is terminated, their account might be disabled immediately, but their active sessions dont automatically end.
// "Subscription expiration": In a SaaS application, a users subscription might expire, requiring account deactivation, but their session remains active.
```

## cookie validation for "revoked user"
* -> **`ValidatePrincipal` event** can be used to intercept and override **`validation of the cookie identity`**
* -> **validating the cookie on every request** mitigates the risk of **`revoked users accessing the app`**
* => one approach to **cookie validation** is based on **`keeping track of when the user database changes`**
* => if the **`database hasn't been changed since the user's cookie was issued`**, there's **no need to re-authenticate the user if their `cookie is still valid`**

* -> however, validating authentication cookies for all users on every request can result in **a large `performance` penalty for the app**
* _vì mỗi lần nó validate request là mỗi lần nó phải query `User` table để lấy `LastChanged` field value_
* _rồi dùng nó só sánh với giá trị `LastChanged claim` của cookie (thời điểm mà cookie được issue)_
* _vì mỗi lần ta update user state thì giá trị của field `LastChanged` của `User` table sẽ được update, nên ta sẽ cần validate trên từng request_

## Sample Code - invalidate cookie of "revoked user"
* -> in the sample app, the **database** is implemented in "IUserRepository" and stores a **`LastChanged`** value 
* -> when **a user is updated in the database**, the "LastChanged value" is **`set to the current time`**

* -> in order to **`invalidate a cookie when the database changes`** based on the **LastChanged** value, we have to create the **cookie** with a **`LastChanged claim`** 
* -> that contain the **`current "LastChanged" value from the database`**

### Create authenticated cookie with "LastChanged" claim

```cs 
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Email),
    new Claim("LastChanged", {Database Value})
};

var claimsIdentity = new ClaimsIdentity(
    claims,
    CookieAuthenticationDefaults.AuthenticationScheme);

await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme, 
    new ClaimsPrincipal(claimsIdentity));
```

### define and register "ValidatePrincipal" event to Cookie Authentication Service

* _to implement an override for the **'ValidatePrincipal' event**, we create a class that derives from **`CookieAuthenticationEvents`**_
* _that overload a method with signature **ValidatePrincipal(CookieValidatePrincipalContext)**_
```cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly IUserRepository _userRepository;

    public CustomCookieAuthenticationEvents(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        ClaimsPrincipal userPrincipal = context.Principal;

        // Look for the LastChanged claim.
        var lastChanged = (from c in userPrincipal.Claims
                           where c.Type == "LastChanged"
                           select c.Value).FirstOrDefault();

        // check if user's data in databased has changed since the authentication cookie was issued
        if (string.IsNullOrEmpty(lastChanged) || !_userRepository.ValidateLastChanged(lastChanged))
        {
            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
```

* -> **register the events instance** during **`cookie service registration`**
* -> provide **`a scoped service registration`** for our **CustomCookieAuthenticationEvents** class:
```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // this one
    .AddCookie(options =>
    {
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

var app = builder.Build();
```

### Improve Performance approach
* -> Consider caching user data or using a more efficient storage mechanism for last-changed timestamps.
* -> track specific types of changes rather than any change

## update "user" without destructering principal 
* -> consider a situation in which the **user's name is updated** - **`a decision that doesn't affect security in any way`**
* -> if we want to **`non-destructively update the user principal`**, call **context.ReplacePrincipal** and set the **context.ShouldRenew** property to true

====================================================================
# Persistent cookies
* -> we may want **`the cookie to persist across browser sessions`**
* -> this persistence **`should only be enabled with explicit user consent with a "Remember Me" checkbox`** on **sign in** or **a similar mechanism**

* -> set **`IsPersistent`** to **true** in **`AuthenticationProperties`**
* -> **`any sliding expiration settings previously configured`** are **honored**
* -> if the **cookie expires while the browser is closed**, the **`browser clears the cookie once it's restarted`**
* -> creates **an identity** and **corresponding cookie** that survives through **browser closures**:
```cs
await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(claimsIdentity),
    new AuthenticationProperties
    {
        IsPersistent = true
    });
```

====================================================================
# Absolute cookie expiration
* -> an **absolute expiration time** can be set with **`ExpiresUtc`**
* -> to create **a persistent cookie**, **`IsPersistent`** must also be set
* -> otherwise, **`the cookie is created with a session-based lifetime`** and **`could expire either before or after the authentication ticket that it holds`**
* -> when **ExpiresUtc is set**, it **`overrides the value of the 'ExpireTimeSpan' option of 'CookieAuthenticationOptions'`**, if set

* _creates an **identity** and corresponding **cookie** that lasts for `20 minutes`:_
* _this ignores any sliding expiration settings previously configured_
```cs
await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(claimsIdentity),
    new AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
    });
```