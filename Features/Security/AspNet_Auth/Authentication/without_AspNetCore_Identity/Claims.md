===================================================================
# Mapping, customizing, and transforming claims in ASP.NET Core
* -> **Claims** can be **`created from any user or identity data`** which can be **issued using `a trusted identity provider` or `ASP.NET Core identity`**

* -> **a claim** is **`a name value pair`** that **`represents what the subject is`**, **not what the subject can do**

===================================================================
# Mapping claims using OpenID Connect authentication
* -> the **`profile claims`** can be returned in the **`id_token`**, which is returned after **a successful authentication**
* -> **the ASP.NET Core client app** **`only requires the profile scope`**
* -> when **using the `id_token` for `claims`**, **`no extra claims mapping is required`**

```cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options => // this one
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
   .AddCookie()
   .AddOpenIdConnect(options =>
   {
       options.SignInScheme = "Cookies";
       options.Authority = "-your-identity-provider-";
       options.RequireHttpsMetadata = true;
       options.ClientId = "-your-clientid-";
       options.ClientSecret = "-your-client-secret-from-user-secrets-or-keyvault";
       options.ResponseType = "code";
       options.UsePkce = true;
       options.Scope.Add("profile");
       options.SaveTokens = true;
   });

var app = builder.Build();
```

* -> another way to **`get the user claims`** is to use the **`OpenID Connect User Info API`**
* -> **the ASP.NET Core client app** uses the **`GetClaimsFromUserInfoEndpoint`** property to configure this
* -> one important difference from the first settings, is that we **`must specify the claims we require using the MapUniqueJsonKey method`**, 
* -> otherwise **`only the name`**, **`given_name` and `email` standard claims** will be available in the client app
* -> the **claims included in the id_token** are **`mapped per default`**
* -> this is the major difference to the first option, we must _explicitly define some of the claims we require_

```cs
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
   .AddCookie()
   .AddOpenIdConnect(options =>
   {
       options.SignInScheme = "Cookies";
       options.Authority = "-your-identity-provider-";
       options.RequireHttpsMetadata = true;
       options.ClientId = "-your-clientid-";
       options.ClientSecret = "-client-secret-from-user-secrets-or-keyvault";
       options.ResponseType = "code";
       options.UsePkce = true;
       options.Scope.Add("profile");
       options.SaveTokens = true;
       options.GetClaimsFromUserInfoEndpoint = true; // this one
       options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username"); // this one
       options.ClaimActions.MapUniqueJsonKey("gender", "gender"); // this one
   });
```

===================================================================
# Name claim and role claim mapping
* -> the **Name claim** and the **Role claim** are **`mapped to default properties in the ASP.NET Core HTTP context`**
* -> sometimes it is required to **use different claims for the default properties**, or **the name claim and the role claim do not match the default values**
* -> the claims can be mapped using the **`TokenValidationParameters`** property and **set to any claim as required**
* -> the **values from the claims** can be **used directly in the HttpContext `User.Identity.Name` property and the `roles`**

* -> if the **User.Identity.Name has no value** or **the roles are missing**, please check the **`values in the returned claims`** and **`set the 'NameClaimType' and the 'RoleClaimType' values`**
* -> the **returned claims from the client authentication** can be **`viewed in the HTTP context`**

```cs
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
  .AddCookie()
  .AddOpenIdConnect(options =>
  {
       // Other options...
       options.TokenValidationParameters = new TokenValidationParameters // this one
       {
          NameClaimType = "email"
          //, RoleClaimType = "role"
       };
  });
```

===================================================================
# Claims namespaces, default namespaces
* -> **ASP.NET Core adds default namespaces to some known claims**, which **`might not be required in the app`**
* -> so optionally, **`disable these added namespaces`** and **`use the exact claims that the OpenID Connect server created`**

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // this one

builder.Services.AddAuthentication(options =>
{
    // .....
})
```

* -> if we need to **`disable the namespaces per scheme`** and **not globally**, we can use the **`MapInboundClaims = false`** option
```cs
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
   .AddCookie()
   .AddOpenIdConnect(options =>
   {
       options.SignInScheme = "Cookies";
       options.Authority = "-your-identity-provider-";
       options.RequireHttpsMetadata = true;
       options.ClientId = "-your-clientid-";
       options.ClientSecret = "-your-client-secret-from-user-secrets-or-keyvault";
       options.ResponseType = "code";
       options.UsePkce = true;
       options.MapInboundClaims = false; // this one
       options.Scope.Add("profile");
       options.SaveTokens = true;
   });
```

===================================================================
# Extend or add custom claims using 'IClaimsTransformation'
* -> the **'IClaimsTransformation' interface** can be used to **`add extra claims`** to the **'ClaimsPrincipal' class**
* -> the interface requires a single method **`TransformAsync`**; this method might **get called multiple times**
* -> **`only add a new claim if it does not already exist in the 'ClaimsPrincipal'`**
* -> a **ClaimsIdentity** is created to **`add the new claims`** and this **`can be added to the ClaimsPrincipal`**

```cs
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class MyClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();
        var claimType = "myNewClaim";
        if (!principal.HasClaim(claim => claim.Type == claimType))
        {
            claimsIdentity.AddClaim(new Claim(claimType, "myClaimValue"));
        }

        principal.AddIdentity(claimsIdentity);
        return Task.FromResult(principal);
    }
}

// the IClaimsTransformation interface and the MyClaimsTransformation class can be registered as a service:
builder.Services.AddTransient<IClaimsTransformation, MyClaimsTransformation>();
```

