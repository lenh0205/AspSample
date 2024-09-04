===========================================================================
# Overview
* _it's recommended that **`all production ASP.NET Core web apps`** call:_
* -> _UseHttpsRedirection()_ - **the HTTPS Redirection Middleware** to **`redirect all HTTP requests to HTTPS`**
* -> _UseHSTS()_ - **HTTP Strict Transport Security Protocol** to **`add HSTS response header which the client is supposed to obey`**

* _they don't block the "http" request, to do that we need config on our hosting (IIS, Apache, ...)_

## Problem
* -> **`HTTPS is mandatory to grant security to our web application`**, regardless of the programming framework we are using
* -> but what happens if a client **calls our web app with 'HTTP' instead of 'HTTPS'**

* => so we need to force client to use HTTPS

## HTTPS Redirection and Reverse Proxies
* -> all the above makes sense if **our ASP.NET Core application is directly exposed to the Internet**
* -> if our application is deployed in an environment with **`a reverse proxy that handles connection security`**, there is **no need to use HTTPS redirection or HSTS middleware**
* _the same applies to **ASP.NET Core Web API** application as well, we don't need to create a custom middleware to deny HTTP requests_
* => we **`delegate "HTTP to HTTPS switching and control" to the reverse proxy`**

===========================================================================
# Using HTTPS Redirection
* -> if a client calls our application using HTTP, **`our application redirects it to the same URL starting with HTTPS`**
* -> **`URL redirection`** is a well-known approach - the web application **creates an HTTP response** with **`a status code starting with 3`** and **`a Location header`**
```r
HTTP/1.1 301 Moved Permanently
Location: https://www.auth0.com/
```

## use the 'RequireHttps' attribute 

```cs - in "Razor Pages applications"
// if this page is called through HTTP, an "HTTPS redirection response" will be automatically created
// apply only to classes inheriting from 'PageModel', cannot apply to the "class methods"

[RequireHttps]
public class PrivacyModel : PageModel
{
  //...existing code...
}
```

```cs - in "ASP.NET Core MVC applications"
// -> can attached to class inherited from 'Controller', the HTTP redirection is applied to any view returned by it
// -> can also apply to a specific view

[RequireHttps]
public class HomeController : Controller
{
  // ...existing code...
}

public class HomeController : Controller
{
    //.....

    [RequireHttps]
    public IActionResult Privacy()
    {
        return View();
    }
}
```

## use 'UseHttpsRedirection' middleware
* -> this approach is better, _mixing HTTP and HTTPS pages_ can make web application is not secure because it is exposed to **HTTPS downgrade attacks**
* -> **`each request to our application`** will be inspected and possibly **`automatically redirected by the middleware`** to the **corresponding HTTPS-based URL if client request a page with HTTP**

```cs - Program.cs
var builder = WebApplication.CreateBuilder(args);

// ...existing code...

var app = builder.Build();

// ...existing code...

app.UseHttpsRedirection();  //👈 HTTPS redirection
app.UseStaticFiles();

app.UseRouting();

// ...existing code...
```

## 'UseHsts' middleware

### Problem - 'UseHttpsRedirection' middleware is not enough to prevent 'HTTPS downgrade attacks'
* -> **forcing a client** to **switch from HTTP to HTTPS** on **each request** might not be enough, the **`attacker could intercept the client's HTTP request before it switches to the corresponding HTTPS request`**
* => we need a way to tell the **`browser to mandatorily use HTTPS to request any resource of our web application`**

### Solution
* -> using the **`HSTS - HTTP Strict-Transport-Security header`**
* -> the browser will call our application using **HTTP only the very first time**; **`the subsequent requests against the same domain will be made using the HTTPS protocol`**, even in the presence of a URL using the HTTP scheme

```cs
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();    //👈 Enable HSTS middleware
}
```

### Notice
* -> if an ASP.NET Core application enables HSTS in **`development`**; in that case, it causes our **`browser to make HTTPS requests to any application hosted on 'localhost'`**
* _not just our ASP.NET Core application and not just ASP.NET Core applications in general_

* -> HSTS settings include **`an expiration time`**, which by default is **30 days for ASP.NET Core applications**

===========================================================================
# Type of Web Application
* -> the HTTPS redirection approach (_**RequireHttps**, **HTTPS redirection middleware**, **HSTS middleware**_) relies on **`sending back to the client a 301 or another 30* HTTP status code`**

* -> both approaches are **`well-understood by standard browsers`**
* -> so, application types whose **clients are browsers** (_such as ASP.NET Core MVC applications, Razor Pages applications, and Blazor Server applications_) can rely on these approaches

* -> however, those approaches are usually **`ignored by non-browser clients, such as API clients`**
* -> it's extremely rare for **a mobile app** or **a SPA** to take care of 301 status codes or HSTS headers

* => in this case, we have 2 alternative ways to deal with clients that make HTTP requests: **`Ignore HTTP requests`**, **`respond with a 400 Bad Request status code`**

## Ignore HTTP requests
* _this can be done in different ways_

* -> use the **`--urls`** flag of the **`dotnet run`** command - allows us to **`override the URL settings configured in the Properties/launchSettings.json file`** of your ASP.NET Core project
```bash
dotnet run --urls "https://localhost:7123"
```

* -> a more **production-oriented approach** to override those settings is using the **`ASPNETCORE_URLS`** environment variable
```bash
## PowerShell
$Env: ASPNETCORE_URLS = "https://localhost:7123"

## bash shell
export ASPNETCORE_URLS="https://localhost:7123"
```

## Treat HTTP requests as bad requests
* -> to implement the Bad Request approach, we need to **`create a custom middleware`** and use it to **`replace the HTTPS redirection and HSTS middleware`**

```cs
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// 👇 new code
//app.UseHttpsRedirection();
app.Use(async (context, next) => {
    if (!context.Request.IsHttps) { // check if current request uses HTTPS
        // if not, replies with a "400 Bad Request status code" 
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        // and a message warning that HTTPS is required
        await context.Response.WriteAsync("HTTPS required!");
    } else {
        await next(context);
    }
});
/ 👆 new code
  
app.UseStaticFiles();
app.UseRouting();
```

===========================================================================
# HTTPS Redirection and Reverse Proxies
* -> 