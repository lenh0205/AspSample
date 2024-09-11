=====================================================================
# using Identity to secure a Web API backend for SPAs
* -> ta sẽ sử dụng **`ASP.NET Core templates`** that offer **authentication in Single Page Apps (SPAs)** using the **support for API authorization**
* -> in this project, the **`ASP.NET Core Identity`** is for **authenticating** and **storing users**, combined with **`Duende Identity Server`** for **implementing OpenID Connect**

* => tức là ta dựng 1 project WebAPI đóng cả vai trò là **`Resource Server`** và **`Authorization Server`**

=====================================================================
# Create an app with API authorization support
```r
dotnet new react -au Individual
```

## Identity with the default UI
```cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

## configure API authentication on IdentityServer
* -> **IdentityServer** with an additional **AddApiAuthorization** helper method that **`sets up some default ASP.NET Core conventions`** on top of **`IdentityServer`**

* -> the **AddApiAuthorization** helper method is the supported configuration of Microsoft for IdentityServer that are considered **`a good starting point`**
* -> it's a **set of conventions and configuration options** that **`reducing exposing unnecessary complexity for the most common scenarios`**
* -> however, once our authentication needs change, the **`full power of IdentityServer is still available`** to customize authentication to suit your needs

```cs
builder.Services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
```

## configure token validation 
* -> **Authentication** with an additional **AddIdentityServerJwt** helper method that configures the app to **`validate JWT tokens produced by IdentityServer`**

* -> the **AddIdentityServerJwt** helper method **`configures a policy scheme for the app`** as the **`default authentication handler`**
* -> the **policy**  is configured to let **`Identity handle all requests routed to any subpath in the Identity URL space "/Identity"`**
* -> the **JwtBearerHandler** **`handles all other requests`**
* -> additionally, this method **`registers an <<ApplicationName>>API API resource with IdentityServer`** with a **`default scope of <<ApplicationName>>API`**
* -> and configures the **`JWT Bearer token middleware`** to **validate tokens issued by IdentityServer for the app**

```cs
builder.Services.AddAuthentication().AddIdentityServerJwt();
```

## Authentication middleware
* -> responsible for **`validating the request credentials`** and **`setting the user on the request context`**

```cs
app.UseAuthentication();
```

## IdentityServer middleware
* -> **`exposes the OpenID Connect endpoints`**

```cs
app.UseIdentityServer();
```