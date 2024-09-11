=====================================================================
# using Identity to secure a Web API backend for SPAs
* -> ta sẽ sử dụng **`ASP.NET Core templates`** that offer **authentication in Single Page Apps (SPAs)** using the **support for API authorization**
* -> in this project, the **`ASP.NET Core Identity`** is for **authenticating** and **storing users**, combined with **`Duende Identity Server`** for **implementing OpenID Connect**

* => tức là ta dựng 1 project WebAPI đóng cả vai trò là **`Resource Server`** và **`Authorization Server`**
* _nhưng nó sẽ không hỗ trợ những thứ như **consent** or **federation**_

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

## configure API authentication on IdentityServer - .AddApiAuthorization()
* -> **IdentityServer** with an additional **AddApiAuthorization** helper method that **`sets up some default ASP.NET Core conventions`** on top of **`IdentityServer`**

* -> the **AddApiAuthorization** helper method is the supported configuration of Microsoft for IdentityServer that are considered **`a good starting point`**
* -> it's a **set of conventions and configuration options** that **`reducing exposing unnecessary complexity for the most common scenarios`**
* -> however, once our authentication needs change, the **`full power of IdentityServer is still available`** to customize authentication to suit your needs

```cs
builder.Services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
```

## configure token validation - .AddIdentityServerJwt()
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

## API Controller
* -> in the file, notice the **[Authorize] attribute** applied to the class that indicates that the **`user needs to be authorized based on the default policy`** to **access the resource**
* -> the **default authorization policy** happens to be configured to use the **`default authentication scheme`**,
* -> which is set up by **`AddIdentityServerJwt`** to the policy scheme (_mentioned above_), making the **`JwtBearerHandler`** configured by such helper method the **default handler for requests to the app**

## OidcConfigurationController
* -> the **endpoint** that's provisioned to **`serve the OIDC parameters that the client needs to use`**

## DbContext
* -> the same DbContext is used in Identity but it extends **`ApiAuthorizationDbContext`** (a more derived class from **`IdentityDbContext`**) to include the schema for IdentityServer

* -> to **`gain full control of the database schema`**, inherit from one of the available **Identity DbContext classes** 
* -> and **configure the context to include the Identity schema** by calling **`builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value)`** on the **OnModelCreating** method

## appsettings.json
* -> there's a **IdentityServer section** in the "appsettings.json" file of the project root that **`describes the list of configured clients`**

* -> in the following example, there's **a single client**
* -> the **client name** corresponds to the app name and is mapped by convention to the OAuth **`ClientId`** parameter
* -> the **profile** indicates the **`app type`** being configured; it's used internally to drive conventions that **`simplify the configuration process for the server`**
* -> there are several profiles available nhứ **SPA**, **IdentityServerJwt** (_xem phần **`Application profiles`** bên dưới để hiểu thêm_)

```json
"IdentityServer": {
  "Clients": {
    "angularindividualpreview3final": {
      "Profile": "IdentityServerSPA"
    }
  }
}
```

## appsettings.Development.json
* -> there's an **IdentityServer section** in the "appsettings.Development.jsonn" file of the project root that describes the **`key used to sign tokens`**

```cs
"IdentityServer": {
  "Key": {
    "Type": "Development"
  }
}
```

## SPA Client - React App
* -> the support for authentication and API authorization in the React template resides in the **`ClientApp/src/components/api-authorization`** directory

* _4 components:_
* -> **Login.js** - handles the **`app's login flow`**
* -> **Logout.js** - handles the **`app's logout flow`**
* -> **LoginMenu.js** - a widget that displays one of these sets of links: **`User profile management and log out links`** when the **user is authenticated**, **`Registration and log in links`** when the **user isn't authenticated**
* -> **AuthorizeRoute.js** - a route component that requires a user to be authenticated before rendering the component indicated in the Component parameter

* an exported **'authService' instance** of class AuthorizeService (_AuthorizeService.js_)
* -> handles the **`lower-level details of the authentication process`**
* -> and **`exposes information about the authenticated user`** to the rest of the app for consumption

=====================================================================
# Customize the API authentication handler
* -> 
```cs
builder.Services.AddAuthentication()
    .AddIdentityServerJwt();  // registers the API's JWT handler (JWT authentication handler for our API)

// Event - API's JWT handler raises "events" that enable control over the authentication process using JwtBearerEvents
// Event Handler - to provide support for API authorization, AddIdentityServerJwt registers its own event handlers
// => tức là nhờ việc gọi ".AddIdentityServerJwt()" nó cho phép ta vừa tạo event và handle những event đó luôn
// => và nhờ vào những "event" này mà ta có thể hook into the flow and customize behavior as needed

// to customize the configuration of the API's JWT handler, configure its "JwtBearerOptions" instance
builder.Services.Configure<JwtBearerOptions>(
    IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
    options =>
    {
        // To customize the handling of an event, wrap the existing "event handler" with additional logic as required:

        // Calls the original implementation provided by the API authorization support
        var onTokenValidated = options.Events.OnTokenValidated;

        // Run its own custom logic
        options.Events.OnTokenValidated = async context =>
        {
            await onTokenValidated(context);
            // ...
        }
    });
```

=====================================================================
# Protect a client-side route (React)
* -> **`protect a client-side route`** by using the **`AuthorizeRoute`** component (_custom component_) instead of the **plain 'Route' component (react-router-dom)**

* _just the client, this **doesn't protect the actual endpoint** (which still requires an **`[Authorize] attribute`** applied to it)_
* _only prevents the user from navigating to the given client-side route when it isn't authenticated_

```js - For example: the "fetch-data" route is configured within the "App" component
<AuthorizeRoute path='/fetch-data' component={FetchData} />
```

# Authenticate API requests (React)
* -> **authenticating requests with React** is done by first **importing the "authService" instance** from the AuthorizeService
* -> the **`access token`** is **retrieved from the "authService"** and is **attached to the request**
* _in React components, this work is typically done in the componentDidMount lifecycle method or as the result from some user interaction_

```cs - Retrieve and attach the access token to the response
async populateWeatherData() {
  const token = await authService.getAccessToken();
  const response = await fetch('api/SampleData/WeatherForecasts', {
    headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
  });
  const data = await response.json();
  this.setState({ forecasts: data, loading: false });
}
```

=====================================================================
# Deploy to production
* _to deploy the app to production, the following resources need to be provisioned:_
* -> **a database** to **`store the Identity user accounts`** and **`the IdentityServer grants`**

* -> **a production certificate** to **`use for signing tokens`**
* _there are no specific requirements for this certificate; it can be a **self-signed certificate** or a certificate provisioned through a **CA authority**_
* _it can be generated through standard tools like **PowerShell** or **OpenSSL**_
* _it can be **installed into the certificate store on the target machines** or **deployed as a .pfx file with a strong password**_

## Example: Deploy to a non-Azure web hosting provider

## Example: Deploy to Azure App Service

=====================================================================
# Other configuration options

## Application profiles
* -> these are the **`predefined configurations`** supported by Microsoft for apps that further define their parameters

### IdentityServerSPA - represents a 'SPA' hosted alongside 'IdentityServer' as a single unit
* -> the **`redirect_uri`** defaults to **/authentication/login-callback**
* -> the **`post_logout_redirect_uri`** defaults to **/authentication/logout-callback**
* -> the **set of scopes** includes the **`openid, profile, and every scope defined for the APIs in the app`**
* -> the **set of allowed OIDC response types** is **`id_token token or each of them individually (id_token, token)`**
* -> the **`allowed response mode`** is **fragment**

### SPA - represents a 'SPA' that isn't hosted with 'IdentityServer'
* -> the **set of scopes** includes the **`openid, profile, and every scope defined for the APIs in the app`**
* -> the **set of allowed OIDC response types** is **`id_token token or each of them individually (id_token, token)`**
* -> the **`allowed response mode`** is **fragment**

### IdentityServerJwt - represents an 'API' that is hosted alongside with 'IdentityServer'
* -> the app is configured to have **`a single scope`** that **defaults to the app name**

### API - represents an 'API' that isn't hosted with 'IdentityServer'
* -> the app is configured to have **`a single scope`** that **defaults to the app name**

## Configuration through 'AppSettings'
* -> configure the apps through the **`configuration system`** by adding them to the list of **Clients** or **Resources**

```json - Configure each client's "redirect_uri" and "post_logout_redirect_uri" property
{
    "IdentityServer": {
        "Clients": {
            "MySPA": {
                "Profile": "SPA",
                "RedirectUri": "https://www.example.com/authentication/login-callback",
                "LogoutUri": "https://www.example.com/authentication/logout-callback"
            }
        }
    }
}
```

```json - when configuring resources, we can configure the scopes for the resource
{
    "IdentityServer": {
        "Resources": {
            "MyExternalApi": {
                "Profile": "API",
                "Scopes": "a b c"
            }
        }
    }
}
```

## Configuration through code
* -> we can also configure the **clients** and **resources** through code using an overload of **`AddApiAuthorization`** that takes an action to configure options
```cs
AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
{
    options.Clients.AddSPA(
        "My SPA", spa =>
        spa.WithRedirectUri("http://www.example.com/authentication/login-callback")
           .WithLogoutRedirectUri(
               "http://www.example.com/authentication/logout-callback"));

    options.ApiResources.AddApiResource("MyExternalApi", resource =>
        resource.WithScopes("a", "b", "c"));
});
```