> những page của IdentityServer mà SPA redirect đến để thực hiện Auth đều là **ASP.NET Core Identity Default UI**
> redirect được thực hiện bằng **window.location.replace(redirectUrl);** để đảm bảo nó không ghi Browser history

> vẫn chưa biết IdentityServer trả về authorization code cho client nhận và lưu như thế nào ?
> hiện tại không biết có sử dụng "id_token" không ?
> vẫn chưa biết client lấy access_token ? và client lưu cái access_token như thế nào và ở đâu ?
> Resource server muốn validate token thì cần có key, cái key này đang ở đâu?

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

app.UseAuthentication(); // authentication middleware
app.UseIdentityServer(); // IdentityServer middleware
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
> ta sẽ cần cài thư viện **`oidc-client`**: https://authts.github.io/oidc-client-ts/classes/UserManager.html#signinCallback

# SPA Client - React App

## Introduce
* -> the support for authentication and API authorization in the React template resides in the **`ClientApp/src/components/api-authorization`** directory

* _4 components:_
* -> **Login.js** - handles the **`app's login flow`**
* -> **Logout.js** - handles the **`app's logout flow`**
* -> **LoginMenu.js** - a widget that displays one of these sets of links: **`User profile management and log out links`** when the **user is authenticated**, **`Registration and log in links`** when the **user isn't authenticated**
* -> **AuthorizeRoute.js** - a route component that requires a user to be authenticated before rendering the component indicated in the Component parameter

* an exported **'authService' instance** of class AuthorizeService (_AuthorizeService.js_)
* -> handles the **`lower-level details of the authentication process`**
* -> and **`exposes information about the authenticated user`** to the rest of the app for consumption

## Routes
* -> về cơ bản thì app chia thành 3 loại route cơ bản **public** (_VD: /Home_), **protected** (_Ex: /fetch-data_) và **ApiAuthorzationRoutes** (_Ex: /authentication/login, /authentication/login-callback_)

### Protect a client-side route - for accessing protected resources
* -> **protect a client-side route** by using the **`AuthorizeRoute` component** (_custom component_) instead of the **plain 'Route' component (react-router-dom)**

* _just the client, this **doesn't protect the actual endpoint** (which still requires an **`[Authorize] attribute`** applied to it)_
* _**user-friendly only** - prevents the user from navigating to the given client-side route when it `isn't authenticated`_

```js - For example: the "fetch-data" route is configured within the "App" component
// Usage
<AuthorizeRoute path='/fetch-data' component={FetchData} />

// maintain state
this.state = {
    ready: false,
    authenticated: false
};
componentDidMount() {
    this._subscription = authService.subscribe(() => {
        this.setState({ ready: false, authenticated: false });

        const authenticated = await authService.isAuthenticated();
        this.setState({ ready: true, authenticated });
    });
    
    // populate authentication state:
    const authenticated = await authService.isAuthenticated();
    this.setState({ ready: true, authenticated });
}

// protect component - render UI
render() {
    const { ready, authenticated } = this.state;

    // create 'redirectUrl' after 'login'
    var link = document.createElement("a");
    link.href = this.props.path; // Ex: '/fetch-data'
    const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
    const redirectUrl = `/authentication/login?returnUrl=${encodeURIComponent(returnUrl)}`;

    if (!ready) {
      return <div></div>;
    } else {
      const { element } = this.props;
      return authenticated 
        ? element // directly render component if user is authenticated
        : <Navigate replace to={redirectUrl} />; // redirect user to "login" page if user haven't authenticated
    }
  }
```

```js - VD: protected "FetchData" component try to access protected resource
[Authorize]
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{}

// so it need to pass "AccessToken" in the Authorization header Bearer scheme of request
componentDidMount() {
    const token = await authService.getAccessToken();
    const response = await fetch('weatherforecast', {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
}
```

### ApiAuthorzationRoutes - interact with IdentityServer
* -> hầu hết các **ApiAuthorzationRoutes** sẽ trả về **`Login`** hoặc **`Logout`** component

* _ví dụ với trang **/authentication/login**, component **`Login.js`** sẽ chạy logic login trong **ComponentDidMount**_
* _ngay khi user open app trong 1 tab, 1 component sẽ check xem user đã `authenticated` cũng như thử lấy `user information`; ở đây là component LoginMenu.js_
```js
componentDidMount() {
    this._subscription = authService.subscribe(() => this.populateState());
    this.populateState();
}
async populateState() {
    const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
    this.setState({
        isAuthenticated,
        userName: user && user.name
    });
}
```

```js - navigate to ApiAuthorzationRoutes
// App.js
<Route path="/authentication/login" element={<Login action={ELoginActions.Login} />}/>
<Route path="/authentication/register" element={<Login action={ELoginActions.Register} />}/>

// LoginMenu.js
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';

// -----> navigation bar for "anonymous user"
anonymousView() {
    return (
        <Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-dark" to='/authentication/register'>Register</NavLink>
            </NavItem>
            <NavItem>
                <NavLink tag={Link} className="text-dark" to='/authentication/login'>Login</NavLink>
            </NavItem>
        </Fragment>
    );
}

componentWillUnmount() {
    const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
    this.setState({
      isAuthenticated,
      userName: user && user.name
    });
}

// navigation bar for "authenticated user"
authenticatedView(userName, profilePath, logoutPath, logoutState) {
    return (
        <Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-dark" to='/authentication/profile'>
                    Hello {userName}
                </NavLink>
            </NavItem>
            <NavItem>
                <NavLink replace tag={Link} className="text-dark" to='/authentication/logout' state={{ local: true }}>
                    Logout
                </NavLink>
            </NavItem>
    </Fragment>);
}
``` 

## AuthorizeService
* -> ta sẽ tạo 1 class **`AuthorizeService`** để handle các logic liên quan đến **Auth**
* -> nó sẽ cần node package **`oidc-client`** - cung cấp **OpenID Connect (OIDC) and OAuth2 protocol support** cho `browser-based Javascript client application`
* -> cụ thể thì ta sẽ tạo instance của class **`UserManager`** cung cấp bởi thư viện "oidc-client", sau đó sử dụng những method của nó để thực hiện các Auth action

### UserManager ('Oidc-client' API support)
* -> ta sẽ cần gọi endpoint **`_configuration/{clientId}`** của **IdentityServer** để nó lấy những **`client parameters`** 
* -> ta sẽ cần pass **clientId** (`IdentitySPA` in this case) vào **`ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId)`;**
* -> ta sẽ dùng object chứa những client parameter được trả về này để khởi tạo **UserManager**

```js
let response = await fetch(ApplicationPaths.ApiAuthorizationClientConfigurationUrl);
if (!response.ok) {
    throw new Error(`Could not load settings for '${ApplicationName}'`);
}
let settings = await response.json();
settings.automaticSilentRenew = true;
settings.includeIdTokenInSilentRenew = true;
settings.userStore = new WebStorageStateStore({
    prefix: ApplicationName
});

this.userManager = new UserManager(settings);

this.userManager.events.addUserSignedOut(async () => {
    await this.userManager.removeUser();
    this.updateState(undefined);
});
```

### maintain "State"
* -> mục đích chính nhất của **AuthorizeService** vẫn là maintain các **auth state**
* -> state quan trọng nhất của **AuthorizeService** vẫn là **`user`**
```js 
// state
_callbacks = [];
_nextSubscriptionId = 0;
_user = null;
_isAuthenticated = false;

// update critical state:
updateState(user) {
    this._user = user;
    this._isAuthenticated = !!this._user;
    this.notifySubscribers();
}
```

```js - access user information
const userName = user.name;
```

```js - cách lấy "_user"
async getUser() { // cụ thể là "user profile"
    if (this._user && this._user.profile) {
      return this._user.profile;
    }

    await this.ensureUserManagerInitialized();
    const user = await this.userManager.getUser();
    return user && user.profile; 
}
```

```js - cách lấy "_isAuthenticated"
async isAuthenticated() {
    const user = await this.getUser();
    return !!user;
}
```

### AccessToken
```cs
async getAccessToken() {
    const user = await this.userManager.getUser();
    return user && user.access_token;
}
```

### Common Function

```js - ReturnUrl
getReturnUrl(state) {
    const queryString = window.location.search; // returns the querystring part of a URL
    const params = new URLSearchParams(queryString); // tranfer queryString into an oject
    const fromQuery = params.get("returnUrl"); // get value of "returnUrl" query parameter
    if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
        // This is an extra check to prevent open redirects.
        throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
    }
    return (state && state.returnUrl) || fromQuery || `${window.location.origin}/`;
}
```

### Sign In ('Oidc-client' API support)
* -> **`userManager.signinSilent`** sign in using **iframe** instead of visible UI changes, typically used to **renew tokens** (_or check if the **user is already authenticated**_); this maintaining a user's session without interrupting their experience
* -> **`userManager.signinPopup`** opens a **popup window** for authentication, this keep the user on the **current page while authenticating**; 
* -> **`userManager.signinRedirect`** - the most common approach for **traditional web applications**; redirects the user to the **identity provider's login page** (_after authentication, the user is redirected back to our application_)

* _but not all `browser / device` support **popup**, also some `enviroment and policy` might restrict **iframe** too; so the **redirect method** necessary_

```js - "signin" action
// ----------> Params:
createArguments(state) { // create parameter for signin methods of 'oidc-client'
    return { 
        // ensures that the redirect doesn't create a new entry in the browser's history stack
        // only need this field for processing "silent signin" and "popup signin" 
        useReplaceToNavigate: true, 

        // the value of "state" is basically { returnUrl }
        // the "redirect signin" of 'oidc-client" need this in addition to process properly
        data: state 
    };
}
const returnUrl = this.getReturnUrl();


// ----------> Main Execute:
const result = await authService.signIn({ returnUrl });

// ----------> Main Function:
// ----------> Target: update Authen state by call "updateState" method
async signIn(state) {
    await this.ensureUserManagerInitialized();
    try 
    {
      const silentUser = await this.userManager.signinSilent(this.createArguments());
      this.updateState(silentUser);
      return this.success(state);
    } 
    catch (silentError) 
    {
      // User might not be authenticated, fallback to popup authentication
      console.log("Silent authentication error: ", silentError);

      try 
      {
        if (this._popUpDisabled) {
          throw new Error('Popup disabled. Change \'AuthorizeService.js:AuthorizeService._popupDisabled\' to false to enable it.')
        }

        const popUpUser = await this.userManager.signinPopup(this.createArguments());
        this.updateState(popUpUser);
        return this.success(state);
      } 
      catch (popUpError) 
      {
        if (popUpError.message === "Popup window closed") {
          // The user explicitly cancelled the login action by closing an opened popup.
          return this.error("The user closed the window.");
        } else if (!this._popUpDisabled) {
          console.log("Popup authentication error: ", popUpError);
        }

        // PopUps might be blocked by the user, fallback to redirect 
        try 
        {
          // thằng này sẽ redirect trang của ta đến https://localhost:44486/Identity/Account/Login?ReturnUrl=..."
          // "ReturnUrl" sẽ là /connect/authorize/callback?client_id=IdentitySPA&redirect_uri=...&response_type=code&scope=...&code_challenge=...&code_challenge_method=...&response_mode=query
          // "redirect_uri" sẽ là "https://localhost:44486//authentication/login-callback"
          await this.userManager.signinRedirect(this.createArguments(state));
          return this.redirect(); // thực ra hiện tại hàm này không làm gì cả
        } 
        catch (redirectError) 
        {
          console.log("Redirect authentication error: ", redirectError);
          return this.error(redirectError);
        }
      }
    }
}

// ----------> Return (result):
success(state) { // for "Silent SignIn" and "Popup SignIn"
    return { status: AuthenticationResultStatus.Success, state };
}
error(message) { // for "Popup SignIn"
    return { status: AuthenticationResultStatus.Fail, message };
}
redirect() { // for "Redirect Login"
    return { status: AuthenticationResultStatus.Redirect };
}

// ----------> Business:
switch (result.status) {
    case AuthenticationResultStatus.Redirect:
        break;
    case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(returnUrl);
        break;
    case AuthenticationResultStatus.Fail:
        this.setState({ message: result.message }); // hiện message ra UI khi login fail
        break;
    default:
        throw new Error(`Invalid status result ${result.status}.`);
}
navigateToReturnUrl(returnUrl) {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    window.location.replace(returnUrl);
}
```

### Login Callback
* -> để redirect ngược từ trang của IdentityServer về trang của client (**`/authentication/login-callback`**) sau khi Login 

```js
// ----------> Main Execute:
const url = window.location.href;
const result = await authService.completeSignIn(url);

// ----------> Main Function
async completeSignIn(url) {
    try {
        await this.ensureUserManagerInitialized();
        const user = await this.userManager.signinCallback(url);
        this.updateState(user);
        return this.success(user && user.state);
    } catch (error) {
        console.log('There was an error signing in: ', error);
        return this.error('There was an error signing in.');
    }
}

// ----------> Business:
switch (result.status) {
    case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as the only time completeSignIn finishes
        // is when we are doing a redirect sign in flow.
        throw new Error('Should not redirect.');
    case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(this.getReturnUrl(result.state));
        break;
    case AuthenticationResultStatus.Fail:
        this.setState({ message: result.message });
        break;
    default:
        throw new Error(`Invalid authentication result status '${result.status}'.`);
}
```

### Register
* -> redirect to **IdentityServer UI page**

```js
const apiAuthorizationPath = `Identity/Account/Register?returnUrl=${encodeURI('/authentication/login')}`
const redirectUrl = `${window.location.origin}/${apiAuthorizationPath}`;
// It's important that we do a replace here so that when the user hits the back arrow 
// on the browser they get sent back to where it was on the app instead of to 
// an endpoint on this component
window.location.replace(redirectUrl);
```

### User Profile
* -> redirect to **IdentityServer UI page**

```js
const apiAuthorizationPath = 'Identity/Account/Manage'
const redirectUrl = `${window.location.origin}/${apiAuthorizationPath}`;
// It's important that we do a replace here so that when the user hits the back arrow 
// on the browser they get sent back to where it was on the app instead of to 
// an endpoint on this component
window.location.replace(redirectUrl);
```

### Sign Out ('Oidc-client' API support)
* -> event **`userManager.events.addUserSignedOut`** sẽ cho phép ta làm hành động gì đó khi **a user `signs out` from the OP (OpenID Provider - Authorization Server)**
* -> method **`userManager.removeUser`** cho phép **remove from any storage the currently `authenticated user`**

```js - "signout" action
// ----------> Main Execute:
const isauthenticated = await authService.isAuthenticated();
if (isauthenticated) {
    const result = await authService.signOut({ returnUrl: this.getReturnUrl() });
}

// ----------> Main Function:
// ----------> Target: update Authen state by call "updateState" method
// We try to sign out the user in two different ways:
  // 1) We try to do a sign-out using a PopUp Window. This might fail if there is a
  //    Pop-Up blocker or the user has disabled PopUps.
  // 2) If the method above fails, we redirect the browser to the IdP to perform a traditional
  //    post logout redirect flow.
async signOut(state) {
    await this.ensureUserManagerInitialized();
    try {
        if (this._popUpDisabled) {
            throw new Error('Popup disabled. Change \'AuthorizeService.js:AuthorizeService._popupDisabled\' to false to enable it.')
        }

        await this.userManager.signoutPopup(this.createArguments());
        this.updateState(undefined);
        return this.success(state);
    } 
    catch (popupSignOutError) {
        console.log("Popup signout error: ", popupSignOutError);
        try {
            await this.userManager.signoutRedirect(this.createArguments(state));
            return this.redirect(); // thực ra hiện tại hàm này không làm gì cả
        } 
        catch (redirectSignOutError) {
            console.log("Redirect signout error: ", redirectSignOutError);
            return this.error(redirectSignOutError);
        }
    }
}

// ----------> Return (result):
success(state) { // for "Popup SignOut"
    return { status: AuthenticationResultStatus.Success, state };
}
error(message) { // for "Redirect SignOut"
    return { status: AuthenticationResultStatus.Fail, message };
}
redirect() { // for "Redirect SignOut"
    return { status: AuthenticationResultStatus.Redirect };
}

// ----------> Business:
switch (result.status) {
    case AuthenticationResultStatus.Redirect:
        break;
    case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(returnUrl);
        break;
    case AuthenticationResultStatus.Fail:
        this.setState({ message: result.message });
        break;
    default:
        throw new Error("Invalid authentication result status.");
    }
navigateToReturnUrl(returnUrl) {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    window.location.replace(returnUrl);
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
