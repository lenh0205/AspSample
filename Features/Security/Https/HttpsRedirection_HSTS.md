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

===========================================================================
# Solution 1 - HTTPS Redirection
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
* -> **`each request to our application`** will be inspected and possibly automatically redirected by the middleware to the **corresponding HTTPS-based URL if client request a page with HTTP**

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