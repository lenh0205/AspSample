======================================================================
# "Authentication" and "Authorization"
* -> **Authentication** is the process of determining **`a user's identity`**
* -> **Authorization** is the process of determining whether **`a user has access to a resource`**

# "Authentication" in ASP.NET Core
* -> **Authentication** is responsible for **`providing the 'ClaimsPrincipal' for authorization`** to **make permission decisions against**
* -> there are multiple **authentication scheme approaches** to **`select which authentication handler`** is responsible for **`generating the correct set of claims`**: 
* -> **`Authentication scheme`**, **`default authentication scheme`**, **`directly set 'HttpContext.User'`**

======================================================================
# Authentication concept in ASP.NET Core

## Authentication Service
* -> in ASP.NET Core, **authentication** is **handled by the `authentication service`** - **`IAuthenticationService`**, which is used by **authentication middleware** 

## Authentication Handler
* -> the "authentication service" uses **registered authentication handlers** to **`complete authentication-related actions`**

```r - Examples of "authentication-related actions" include:
// Authenticating a user
// Responding when an unauthenticated user tries to access a restricted resource
```

## Authentication Scheme
* -> the **`registered authentication handlers`** and **`their configuration options`** are called **schemes**
* -> so the **authentication scheme** can **`select which authentication handler`** is responsible for **generating the correct set of claims**
* => "Schemes" are useful as **`a mechanism for referring`** to the **authentication, challenge, and forbid behaviors** of **`the associated handler`** (xem `~\Features\Security\AspNet_Auth\NET_Terminology.md` để hiểu)

```r - Example: 
// 1 "authorization policy" can use "scheme names" to "specify which authentication scheme (or schemes)"  should be "used to authenticate the user"
```

## Authentication Middleware
* -> the **Authentication middleware is added in 'Program.cs'** by calling **`UseAuthentication`**
* -> _calling .UseAuthentication()_ **registers the middleware** that **`uses the previously registered authentication schemes`**
* -> _calling .UseAuthentication()_ **`before any middleware that depends on users being authenticated`**

### Authentication Service
* -> _xem `~\Features\Security\AspNet_Auth\NET_Terminology.md` để hiểu về **authenticate**, **challenge**, **forbid**_
* -> còn **`DefaultScheme`** is **catch-all default** that sets the scheme for **all authentication actions (`authenticate, challenge, forbid, sign in, sign out`)** if a more specific default isn't set


```cs
options.DefaultScheme = "SomeScheme";
// <=>
options.DefaultAuthenticateScheme = "SomeScheme";
options.DefaultChallengeScheme = "SomeScheme";
options.DefaultForbidScheme = "SomeScheme";
options.DefaultSignInScheme = "SomeScheme";
options.DefaultSignOutScheme = "SomeScheme";
```

```cs - Ex:
services.AddAuthentication(options =>
{
    // JWT Bearer will be used for authentication:
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // Cookies will be used for challenges - might redirecting to login page
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie()
.AddJwtBearer();

services.AddAuthentication(options =>
{
    // JWT Bearer is used for authentication
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // OpenID Connect is used for challenges, which might redirect the user to an external identity provider
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddJwtBearer()
.AddOpenIdConnect();
```


======================================================================
# Authentication providers per tenant
* -> _ASP.NET Core_ **`doesn't have a built-in solution`** for **multi-tenant authentication** 
* _i **`multi-tenant`** refers to a single instance of an application serves multiple customers (tenants)_
* _i Ex: có nhiều company đang sử dụng application của ta, thì ta cần đảm bảo data of Company A should not be accessible to Company B_

* -> while it's possible for customers to **`write one using the built-in features`**, 
* -> it's recommend customers consider **Orchard Core**, **ABP Framework**, or **Finbuckle.MultiTenant** for "multi-tenant authentication"

