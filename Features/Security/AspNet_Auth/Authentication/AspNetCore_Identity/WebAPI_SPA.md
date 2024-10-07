=====================================================================
# using Identity to secure a Web API backend for SPAs
* -> ta sẽ sử dụng **ASP.NET Core templates** that offer **`authentication in Single Page Apps (SPAs)`** using the **support for API authorization (`API Authorization`)**
* -> in this project, the **`ASP.NET Core Identity`** is for **authenticating** and **storing users**, combined with **`Duende Identity Server`** for **implementing OpenID Connect**

* => tức là ta dựng 1 project WebAPI đóng cả vai trò là **`Resource Server`** và **`Authorization Server`**
* _sử dụng JWT_
* _nhưng những **ASP.NET Core support** này chỉ tập trụng vào **`first party`**;_
* _nó sẽ không hỗ trợ những thứ như **consent** or **federation**; để hỗ trợ những thứ như này ta cần 1 **`IdentityServer`** thực sự_

=====================================================================
# Create an app with API authorization support
```r
dotnet new react -au Individual
```

## Program.cs - configuration
* -> _these code will reply on **`Microsoft.AspNetCore.ApiAuthorization.IdentityServer`** NuGet package - nó sẽ cung cấp cho ta 2 extension method_
* -> configure **`API authentication`** using **.AddApiAuthorization()** 
* -> **`authorization`** using **.AddIdentityServerJwt**

* -> the **authentication middleware** validating the **`request credentials`** and **`setting the user on the request context`**
* -> the **IdentityServer middleware** that **`exposes the OpenID Connect endpoints`**

```cs
// Identity with the default UI:
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ".AddIdentityServer()" is extension method of "Duende.IdentityServer" 
// IdentityServer with an additional "AddApiAuthorization" helper method 
builder.Services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

// "authentication" with an additional AddIdentityServerJwt helper method
builder.Services.AddAuthentication().AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ....

app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();
```

=====================================================================
# ASP.NET Core components of the app

## .AddApiAuthorization() - API Authorization
* -> configures **`IdentityServer`** to use our **supported configuration** - **`a set of conventions and configuration options`** is provided to us as **a good starting point**

* => it help us to avoid the **unnecessary complexity exposing by IdentityServer** for handling app security concerns in most of common scenarios
* => but still, when our authentication needs change the full power of IdentityServer is still available to **customize authentication to suit our needs**

## .AddIdentityServerJwt()
* -> it configures **`a policy scheme`** for the app as the **`default authentication handler`**
* -> the **policy**  is configured to let **`Identity handle all requests routed to any subpath in the Identity URL space "/Identity"`**
* -> the **JwtBearerHandler** **`handles all other requests`**
* -> additionally, this method **registers `an <<ApplicationName>>API API resource` with `a default scope of <<ApplicationName>>API` to IdentityServer**
* -> and configures the **JWT Bearer token middleware** to **`validate JWT tokens issued by IdentityServer for the app`**

## Authentication middleware - .UseAuthentication()
* -> responsible for **`validating the request credentials`** and **`setting the user on the request context`**

## IdentityServer middleware - .UseIdentityServer()
* -> **`exposes the OpenID Connect endpoints`**

## OidcConfigurationController
* -> the **endpoint** that's provisioned to **`serve the OIDC parameters`** that **the `client` needs to use**

## [Authorize] attribute
* -> indicates that the **user needs to be `authorized` based on the `default policy` to access the resource**

* -> **the default authorization policy** happens to be configured to use the **`default authentication scheme`** (_which is set up by `AddIdentityServerJwt` to the `policy scheme` that was mentioned above_)
* -> making the **JwtBearerHandler** configured by such helper method **`the default handler for requests to the app`**

## ApiAuthorizationDbContext
* -> the same DbContext is extends **`ApiAuthorizationDbContext`** (_a more derived class from **IdentityDbContext**_) to include **`the schema for IdentityServer`**

* -> to **`gain full control of the database schema`**, inherit from one of the available **Identity DbContext classes** and **configure the context to include the Identity schema** 
* -> by calling **`builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value)`** on the **OnModelCreating** method

## appsettings.json
* -> in the "appsettings.json" file of the project root, there's a **IdentityServer section** **`describes the list of configured clients`**

* -> in the following example, there's **a single client**
* -> the **client name** corresponds to the app name and is mapped by convention to the OAuth **`ClientId`** parameter
* -> the **profile** indicates the **`app type`** being configured; it's used internally to drive conventions that **`simplify the configuration process for the server`**
* -> there are several profiles available như **SPA**, **IdentityServerJwt** (_xem phần **`Application profiles`** bên dưới để hiểu thêm_)

```json
{
    "IdentityServer": {
        "Clients": {
            "IdentitySPA": {
            "Profile": "IdentityServerSPA"
            }
        }
    }
}
```

## appsettings.Development.json
* -> there's an **IdentityServer section** in the "appsettings.Development.jsonn" file of the project root that describes the **`key used to sign tokens`**
* -> when deploying to **`production`**, a key needs to be provisioned and **deployed alongside the app**

```cs
"IdentityServer": {
  "Key": {
    "Type": "Development"
  }
}
```

=====================================================================
# Customize the API authentication handler
* -> to customize the configuration of the **`API's JWT handler`**, configure its **JwtBearerOptions** instance
* -> the **API's JWT handler raises events that enable control over the authentication process** using **`JwtBearerEvents`**
* -> to provide support for **`API authorization`**, **AddIdentityServerJwt registers its own event handlers**

* => to **customize the handling of an event**, **`wrap the existing event handler with additional logic`** as required:

```cs - Ex:
builder.Services.AddAuthentication()
    .AddIdentityServerJwt();  // registers the API's JWT handler (JWT authentication handler for our API)

builder.Services.Configure<JwtBearerOptions>(
    IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
    options =>
    {
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
> Other configuration options

# Application profiles
* -> "Application profiles" are the **predefined configurations for apps** that **`further define their parameters`**

## IdentityServerSPA - represents a 'SPA' hosted alongside 'IdentityServer' as a single unit
* -> the **`redirect_uri`** defaults to **/authentication/login-callback**
* -> the **`post_logout_redirect_uri`** defaults to **/authentication/logout-callback**
* -> the **set of scopes** includes the **`openid, profile, and every scope defined for the APIs in the app`**
* -> the **set of allowed OIDC response types** is **`id_token token or each of them individually (id_token, token)`**
* -> the **`allowed response mode`** is **fragment**

## SPA - represents a 'SPA' that isn't hosted with 'IdentityServer'
* -> the **set of scopes** includes the **`openid, profile, and every scope defined for the APIs in the app`**
* -> the **set of allowed OIDC response types** is **`id_token token or each of them individually (id_token, token)`**
* -> the **`allowed response mode`** is **fragment**

## IdentityServerJwt - represents an 'API' that is hosted alongside with 'IdentityServer'
* -> the app is configured to have **`a single scope`** that **defaults to the app name**

## API - represents an 'API' that isn't hosted with 'IdentityServer'
* -> the app is configured to have **`a single scope`** that **defaults to the app name**


# Configuration through 'AppSettings'
* -> configure the apps through the **`configuration system`** by adding them to the list of **Clients** or **Resources**

* -> configure each **`client`**'s **redirect_uri** and **post_logout_redirect_uri** property
```json 
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

* -> when configuring **`resources`** (_resource server_), we can configure the **scopes** for the resource
```json - 
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

# Configuration through code
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

=====================================================================
# SPA Client - React App
* -> the support for authentication and API authorization in the React template resides in the **`ClientApp/src/components/api-authorization`** directory

* _4 components:_
* -> **Login.js** - handles the **`app's login flow`**
* -> **Logout.js** - handles the **`app's logout flow`**
* -> **LoginMenu.js** - a widget that displays one of these sets of links: **`User profile management and log out links`** when the **user is authenticated**, **`Registration and log in links`** when the **user isn't authenticated**
* -> **AuthorizeRoute.js** - a route component that requires a user to be authenticated before rendering the component indicated in the Component parameter

* an exported **'authService' instance** of class AuthorizeService (_AuthorizeService.js_)
* -> handles the **`lower-level details of the authentication process`**
* -> and **`exposes information about the authenticated user`** to the rest of the app for consumption


## Summary
* -> về cơ bản thì app chia thành 3 loại route cơ bản **public** (_VD: /Home_), **protected** (_Ex: /fetch-data_) và **ApiAuthorzationRoutes** (_Ex: /authentication/login, /authentication/login-callback_)

* -> ta sẽ tạo 1 class **`AuthorizeService`** để handle các logic liên quan đến **Auth**
* -> 

* -> hầu hết các **ApiAuthorzationRoutes** sẽ trả về **`Login`** hoặc **`Logout`** component
* -> với trang **/authentication/login**, component **`Login`** sẽ chạy logic login trong **ComponentDidMount** 

* -> **`protected route`** sẽ được bảo vệ bởi **`AuthorizeRoute`** component
* -> ngay khi component này **ComponentDidMount**

## Protect a client-side route (React)
* -> **`protect a client-side route`** by using the **"AuthorizeRoute" component** (_custom component_) instead of the **plain 'Route' component (react-router-dom)**

* _just the client, this **doesn't protect the actual endpoint** (which still requires an **`[Authorize] attribute`** applied to it)_
* _**user-friendly only** - prevents the user from navigating to the given client-side route when it `isn't authenticated`_

```js - For example: the "fetch-data" route is configured within the "App" component
<AuthorizeRoute path='/fetch-data' component={FetchData} />
```

## Authenticate API requests (React)
* -> **authenticating requests with React** is done by first **importing the `authService` instance** from the AuthorizeService
* -> the **`access token`** is **retrieved from the "authService"** and is **attached to the request**
* _in React components, this work is typically done in the `componentDidMount` lifecycle method or as the result from some `user interaction`_

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
# "Deploy to production" requirements
* -> **a database** to **`store the Identity user accounts`** and **`the IdentityServer grants`**
* -> **a production certificate** to **`use for signing tokens`**

* _there are no specific requirements for this certificate; it can be a **`self-signed` certificate** or **a certificate provisioned through a `CA authority`**_
* _it can be **`generated through standard tools`** like **PowerShell** or **OpenSSL**_
* _it can be **`installed into the certificate store` on the target machines** or **deployed as `a .pfx file` with a strong password**_

## Example: Deploy to a non-Azure web hosting provider
* -> in our **web hosting panel**, **`create or load our certificate`**
* -> then in the app's **appsettings.json** file, modify the **IdentityServer section** to **`include the key details`**

```json
{
    "IdentityServer": {
    "Key": {
        "Type": "Store",

        // "StoreName" - the name of the certificate store where the certificate is stored
        // in this case, it points to the web hosting store
        "StoreName": "WebHosting",

        // "StoreLocation" - where to load the certificate from (CurrentUser in this case)
        "StoreLocation": "CurrentUser",

        // "Name" corresponds with the distinguished subject for the certificate
        "Name": "CN=MyApplication"
    }
    }
}
```

## Example: Deploy to Azure App Service
* -> to deploying the **app to Azure App Service** using **a certificate** **`stored in the certificate store`**
* -> to modify the app to **load a certificate from the certificate store**, **`a Standard tier service`** plan or better is required (_when we `configure the app in the Azure portal` in a later step_)

* -> in the app's **appsettings.json** file, modify the IdentityServer section to **`include the key details`**:
```json
{
    "IdentityServer": {
    "Key": {
        "Type": "Store",

        // the store name represents the name of the certificate store where the certificate is stored
        // in this case, it points to the personal user store.
        "StoreName": "My",

        // the store location represents where to load the certificate from (CurrentUser or LocalMachine)
        "StoreLocation": "CurrentUser",

        // the name property on certificate corresponds with the distinguished subject for the certificate.
        "Name": "CN=MyApplication"
    }
    }
}
```

* -> to deploy to Azure App Service, follow the steps in **`Deploy the app to Azure`**: https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vs?view=aspnetcore-8.0&viewFallbackFrom=aspnetcore-6.0#deploy-the-app-to-azure
* _which explains how to **create the necessary Azure resources** and **deploy the app to production**_

* -> after following the preceding instructions, the **app is deployed to Azure** but **`isn't yet functional`**
* -> **`the certificate`** used by the app must be **`configured in the Azure portal`**

* -> **`locate the thumbprint`** for the certificate and follow the steps in Load your certificates: https://learn.microsoft.com/en-us/azure/app-service/configure-ssl-certificate-in-code?tabs=windows#load-the-certificate-in-code
* -> while these steps mention **SSL**, there's **`a Private certificates section in the Azure portal`** where we can **upload the provisioned certificate to use with the app**

* => after configuring the app and the app's settings in the Azure portal, **`restart the app in the portal`**
