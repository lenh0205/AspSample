> build an ASP.NET Core app that enables users to **sign in using `OAuth 2.0` with credentials** from **external authentication providers**
> Enabling users to **sign in with their existing credentials** is **`convenient for the users`** and **`shifts many of the complexities of managing the sign-in process onto a third party`**

> **Facebook, Twitter, Google, and Microsoft providers** are covered
> other providers are available in **third-party packages** such as **`OpenIddict`**, **`AspNet.Security.OAuth.Providers`** and **`AspNet.Security.OpenId.Providers`**

====================================================================
# Forward request information with a proxy or load balancer
* -> if the **app is deployed behind a `proxy server` or `load balancer`**, **`some of the original request information might be forwarded to the app in request headers`**
* -> this information usually includes the **secure request scheme (https)**, **host**, and **client IP address**

* -> **`apps don't automatically read these request headers`** to **discover and use the original request information**
* -> **the scheme is used in link generation** that **`affects the authentication flow with external providers`**
* -> **losing the secure scheme (https)** results in **`the app generating incorrect insecure redirect URLs`**

* => use **`Forwarded Headers Middleware`** to **make the original request information available to the app for request processing**

====================================================================
# Use 'SecretManager' to store tokens assigned by login providers
* -> **social login providers** assign **`Application Id`** and **`Application Secret tokens`** during the **registration process** (_the `exact token names` vary by provider_)
* -> these **`tokens represent the credentials`** our app uses to **access their API**
* -> the **tokens** constitute the **user secrets** that can be **`linked to your app configuration with the help of 'Secret Manager'`**

* _i **`User secrets`** are a more secure alternative to storing the tokens in a **configuration file**, such as appsettings.json_

====================================================================
# Setup login providers required by our application

====================================================================
# Multiple authentication providers
* -> when the **app requires multiple providers**, **`chain the provider extension methods from 'AddAuthentication'`**:

```cs
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var connectionString = config.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                                 options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
   .AddGoogle(options =>
   {
       IConfigurationSection googleAuthNSection =
       config.GetSection("Authentication:Google");
       options.ClientId = googleAuthNSection["ClientId"];
       options.ClientSecret = googleAuthNSection["ClientSecret"];
   })
   .AddFacebook(options =>
   {
       IConfigurationSection FBAuthNSection =
       config.GetSection("Authentication:FB");
       options.ClientId = FBAuthNSection["ClientId"];
       options.ClientSecret = FBAuthNSection["ClientSecret"];
   })
   .AddMicrosoftAccount(microsoftOptions =>
   {
       microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"];
       microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"];
   })
   .AddTwitter(twitterOptions =>
   {
       twitterOptions.ConsumerKey = config["Authentication:Twitter:ConsumerAPIKey"];
       twitterOptions.ConsumerSecret = config["Authentication:Twitter:ConsumerSecret"];
       twitterOptions.RetrieveUserDetails = true;
   });

var app = builder.Build();

// ....
```

====================================================================
# Optionally set password
* -> when we **register with an external login provider**, we don't have **`a password registered with the app`**
* -> this **alleviates us from creating and remembering a password for the site**, but it also makes us **`dependent on the external login provider`**
* -> if the **external login provider is unavailable**, we **`won't be able to sign in to the web site`**

* -> ta sẽ vào trang **~/Manage** để tạo password dựa trên email đã đăng ký

====================================================================
# Create the Google OAuth 2.0 Client ID and secret
* ->  integrating Google Sign-In into our web app: **`https://developers.google.com/identity/sign-in/web/sign-in`**
* -> Google API & Services: **`https://console.cloud.google.com/apis/dashboard`**
* -> create a project and go to **Dashboard**

* -> in the "Oauth consent screen" of the "Dashboard":
* _select **`User Type - External`** and CREATE_
* _in the **App information** dialog, Provide an **app name** for the app, **user support email**, and **developer contact information**_
* _step through the **`Scopes`** step_
* _step through the **Test users** step_
* _review the **`OAuth consent screen`** and go back to the app "Dashboard"

* -> in the **Credentials tab** of the application "Dashboard", select **`CREATE CREDENTIALS > OAuth client ID`**

* -> select **`Application type > Web application`**, choose a name

* -> in the **`Authorized redirect URIs`** section, select **ADD URI** to set the redirect URI
* _Example redirect URI: `https://localhost:{PORT}/signin-google`, where the {PORT} placeholder is the app's port_

* -> select the "CREATE" button

* -> save the **`Client ID`** and **`Client Secret`** for use in the **app's configuration**

* -> when **`deploying the site`**, either:
* _i **`update the app's redirect URI`** in the **Google Console** to the app's deployed redirect URI_
* _i **`create a new Google API registration`** in the **Google Console** for the production app with its production redirect URI_

## Store the Google "client ID" and "secret"
* _store **sensitive settings** such as the **`Google client ID and secret values`** with **Secret Manage**_
* -> initialize the project for **secret storage per the instructions** at https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage
* -> store the **sensitive settings** in the **local secret store** with the secret keys **`Authentication:Google:ClientId`** and **`Authentication:Google:ClientSecret`**

```cs
dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<client-secret>"
```

## Configure Google authentication
* -> add the **Microsoft.AspNetCore.Authentication.Google** NuGet package to the app
* -> add the **`Authentication service`** to the "Program":

```cs
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});
```

* -> the call to **`AddIdentity`** configures the **`default scheme settings`**
* -> the **AddAuthentication(IServiceCollection, String)** overload sets the **`DefaultScheme`** property
* -> the **AddAuthentication(IServiceCollection, Action<AuthenticationOptions>)** overload **`allows configuring authentication options`**
* _which can be used to **set up default authentication schemes for different purposes**_
* -> **subsequent calls to AddAuthentication** **`override previously configured 'AuthenticationOptions' properties`**

* -> **'AuthenticationBuilder' extension methods** that register an authentication handler **`may only be called once per authentication scheme`**
* -> overloads exist that allow **configuring the scheme properties, scheme name, and display name**

## Sign in with Google
* -> run the app and select "Log in"; an option to sign in with Google appears.
* -> select the "Google button", which **`redirects to Google for authentication`**
* -> after entering our "Google credentials", we are redirected back to the web site

## Change the default callback URI
* -> the URI segment **/signin-google** is set as the **`default callback of the Google authentication provider`**
* -> we can change the default callback URI while configuring the Google authentication middleware via the inherited **`RemoteAuthenticationOptions.CallbackPath`** property of the GoogleOptions class

## Troubleshooting
If the sign-in doesn't work and you aren't getting any errors, switch to development mode to make the issue easier to debug.
If Identity isn't configured by calling services.AddIdentity in ConfigureServices, attempting to authenticate results in ArgumentException: The 'SignInScheme' option must be provided. The project template used in this tutorial ensures Identity is configured.
If the site database has not been created by applying the initial migration, you get A database operation failed while processing the request error. Select Apply Migrations to create the database, and refresh the page to continue past the error.
HTTP 500 error after successfully authenticating the request by the OAuth 2.0 provider such as Google: See this GitHub issue.
How to implement external authentication with Google for React and other SPA apps: See this GitHub issue.

## Deploy to Azure
* -> Once you publish the app to Azure, reset the ClientSecret in the Google API Console.
* -> Set the Authentication:Google:ClientId and Authentication:Google:ClientSecret as application settings in the Azure portal. The configuration system is set up to read keys from environment variables




