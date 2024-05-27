
# Routing in ASP.NET Core
* -> process of **mapping** a **request URL path** (_such as /Orders/1_) to some **handler that generates a response**
* -> this is primarily used with the **MVC middleware** for **`mapping requests to controllers and actions`**, but it is used in other areas too
* -> includes functionality for the reverse process: **generating URLs** that will **`invoke a specific handler`** with a given set of parameters

====================================================
# In ASP.NET Core 2.1

## implement routing
* -> **`routing`** was handled by implementing the **"IRouter" interface** to **`map incoming URLs to handlers`**
* -> however, rather than implementing the interface directly, we would typically rely on the **"MvcMiddleware" implementation** added to **`the end of middleware pipeline`**
* -> _once a request reached the `MvcMiddleware`_, routing was applied to **`determine which controller and action`** the incoming request URL path corresponded to

## MVC filters
* -> **`request`** then went through **various MVC filters** before **`executing the handler`**

* -> these filters formed another **pipeline**, **`similar to the middleware pipeline`** 
* -> in some cases had to **duplicate the behaviour of certain middleware**

* -> **a canonical example** of this is **`CORS policies`** 
* -> in order to **enforce different CORS policies** per **`MVC action`**, as well as other **`branches`** of middleware pipeline
* -> a **`certain amount of duplication`** was required internally

## Branching Middleware Pipeline - .Map()
* -> **Branching** the **`middleware pipeline`** was often used for **pseudo-routing**

* -> **`using extension methods`** like **Map()** in our middleware pipeline
* -> would allow us to **conditionally execute some middleware** when the incoming path had a **`given prefix`**

* _Ex: the following "Configure()" method from a "Startup.cs" class **`branches the pipeline`**_
* _so that when the incoming path is "/ping", the **`terminal middleware executes`** (written inline using Run())_
```cs
public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();

    app.UseCors();

    // branch
    app.Map("/ping", 
        app2 => app2.Run(async context =>
        {
            await context.Response.WriteAsync("Pong");
        })
    );

    app.UseMvcWithDefaultRoute();
}
```

* -> in this case, the **`Run() method`** is a **terminal middleware**, because it **`returns a response`**
* -> but in a sense, **`the whole Map branch`** corresponds to **`an endpoint`** of the application
* -> especially as we're not doing anything else in the `app2 branch` of the pipeline

## Problem
* -> the problem is that **`this endpoint`** is a bit of **a second-class citizen** when compared to the **`endpoints in the MvcMiddleware`** (i.e. controller actions)
* -> **extracting values** from the incoming route are a pain and we have to **manually implement any authorization requirements** ourself

* -> another problem is that there's **`no way to know`** which **branch will be run until we're already on it**
* -> For example, **`when the request reaches the UseCors() middleware`** from the above example it would be **useful to know which branch/endpoint is going to be executed** 
* -> maybe the "/ping endpoint" allows cross-origin requests, while the "MVC middleware" doesn't
* (_ở đây ta cần hiểu "biết được endpoint nào" tức là "biết được endpoint cũng như metadata được config của nó thông qua attribute,... lên nó"_)

## Introduce 'EndpointRouting' in ASP.NET
* -> in **`ASP.NET Core 2.2`**, Microsoft introduced the **`endpoint routing`** as the **new routing mechanism for MVC controllers**
* -> this implementation was essentially **internal to the MvcMiddleware**, so on the face of it, it wouldn't solve the issues described above
* -> _however, the intention was always to **trial** the implementation there and to expand it to be the **`primary routing mechanism in ASP.NET Core 3.0`**_

* -> the _Endpoint routing_ separates the **`routing of a request (selecting which handler to run)`** from the **`actual execution of the handler`**
* -> this means you can know ahead of time **`which handler will execute`**, and **`our middleware can react accordingly`**
* => this is aided by the new ability to **attach extra metadata to our endpoints**, such as **`authorization requirements`** or **`CORS policies`**

============================================================

# In ASP.NET Core 3.0
* -> in ASP.NET Core 3.0, it use endpoint routing, so the **`routing`** step is **`separate from the invocation of the endpoint`**
* -> in practical terms that means we have two pieces of middleware: **`EndpointRoutingMiddleware`**, **`EndpointMiddleware`**

## Endpoint Routing middleware
* -> **EndpointRoutingMiddleware** that does the actual routing - **`calculating which endpoint will be invoked`** for a given request URL path.
* -> **EndpointMiddleware** that **`invokes the endpoint`**

* -> these are **added at two distinct points** in the middleware pipeline, as they serve **`two distinct roles`**
* -> we want the **`routing middleware to be early in the pipeline`**, so that subsequent middleware has access to the information about the endpoint that will be executed
* -> **`the invocation of the endpoint should happen at the end`** of the pipeline

## Extension method
* the **UseRouting()** extension method 
* -> **`adds the EndpointRoutingMiddleware`** to the pipeline

* the **UseEndpoints()** extension method 
* -> **`adds the EndpointMiddleware`** to the pipeline
* -> also where **`register all the endpoints`** for our application (in the example above, we register our MVC controllers only).

## Middleware pipeline order
* => generally best practice to place the **static files middleware** **`before the Routing middleware`** to avoids the overhead of routing when requesting static files

* => it's important that you place the **Authentication** and **Authorization** middleware **`between UseRouting and UseEndPoints`**

* -> **any middleware that appears after the UseRouting()** call will **`know which endpoint will run eventually`** 

* -> **any middleware that appears before the UseRouting()** call **`won't know which endpoint will run eventually`**

## Usage Example:

```cs - config routing in ASP.NET Core 2.0
// Example: create a custom middleware that return FileVersion of application
// => CORS middleware (added using UseCors()) can't know which endpoint will ultimately be executed

public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();

    app.UseCors();

    // request prefixed with /version (e.g. /version or /version/test)
    // always get the same response, the version of the app
    app.Map("/version", versionApp => versionApp.UseMiddleware<VersionMiddleware>()); 

    // request with any other path
    app.UseMvcWithDefaultRoute();
}

public class VersionMiddleware
{
    readonly RequestDelegate _next;
    static readonly Assembly _entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
    static readonly string _version = FileVersionInfo.GetVersionInfo(_entryAssembly.Location).FileVersion;

    public VersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(_version);
        
        //we're all done, so don't invoke next middleware
    }
}
```

```cs - config routing in ASP.NET Core 3.0
// Move the registration of the "/version" endpoint into the UseEndpoints() call

public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();

    // Add the EndpointRoutingMiddleware
    app.UseRouting();

    // All middleware from here onwards know which endpoint will be invoked
    app.UseCors();

    // Execute the endpoint selected by the routing middleware
   app.UseEndpoints(endpoints =>
    {
        // Add a new endpoint that uses the VersionMiddleware
        endpoints.Map("/version", endpoints.CreateApplicationBuilder()
            .UseMiddleware<VersionMiddleware>()
            .Build())
            .WithDisplayName("Version number");

        // register MVC controllers
        endpoints.MapDefaultControllerRoute();
    });
}
```

===========================================================

# "Endpoint Routing" important points
* -> a **RequestDelegate** is building using the **`IApplicationBuilder()`**
* -> there's **`no longer match based on a route prefix`**, but on the **complete route**
* -> be able to **set an informational name** for the **`endpoint`** (_Version number_)
* -> be able to **attach additional metadata to the endpoint**

## Benefit
* -> can **`attach metadata to endpoints`** so intermediate middleware (e.g. Authorization, CORS) can **know what will be eventually executed**
* -> can **use routing templates in your non-MVC endpoints**, so we get **`route-token parsing features`** that were **`previously limited to MVC`**
* -> more easily **generate URLs to non-MVC endpoints**

## RequestDelegate
* -> now, the **`Map() method`** here requires **a RequestDelegate** instead of **an Action<IApplicationBuilder>**

* -> the downside to this is that visually it's much harder to see what's going on (_nói chung là khó đọc hơn do cú pháp rườm rà, dài dòng hơn_)
* -> we can work around easily by creating a small **`extension method`**
```cs
public static class VersionEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapVersion(this IEndpointRouteBuilder endpoints, string pattern)
    {
        var pipeline = endpoints.CreateApplicationBuilder()
            .UseMiddleware<VersionMiddleware>()
            .Build();

        return endpoints.Map(pattern, pipeline).WithDisplayName("Version number");
    }
}

// than the config will be:
public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors();

    // Execute the endpoint selected by the routing middleware
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapVersion("/version");
        endpoints.MapDefaultControllerRoute();
    });
}
```

## Complete route behavior
* -> in **`ASP.NET Core 2.x`**, the custom middleware branch would execute for any requests that have a specific **prefix**
* -> with **`endpoint routing`**, we're not specifying a prefix for the URL, we're specifying the whole **pattern**
* -> that means we can have **`route parameters`** in all of our endpoint routes
* _this is more powerful than the previous version, but we need to be aware of it_

```r - VD:
## ASP.NET Core 2.x
// -> a "/version" segment prefix will match "/version", "/version/123", "/version/test/oops", ...

## Endpoint Routing
endpoints.MapVersion("/version/{id:int?}");
// -> match both "/version" and "/version/123" URLs, but not "/version/test/oops"
```

## Attach Metadata
* -> one import feature of endpoints is the ability to **`attach metadata`** to them, then **other middleware can interrogate it**
* -> some casual information like **`authorization policies`**, **`CORS policies`**

* -> when **`a request to the endpoint arrives`**, 
* -> the **`routing middleware`** **selects the suitable endpoint**, and **makes its metadata available for subsequent middleware** in the pipeline
* -> the according middleware can see that there are associated metadata (_policies,..._) and act, **`before the endpoint is executed`**

```cs
public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        // add "AllowAllHosts" CORS policy and "AdminOnly" and "authorization policy"
        // to the "/version" endpoint
        endpoints.MapVersion("/version")
            .RequireCors("AllowAllHosts")
            .RequireAuthorization("AdminOnly");
    
        endpoints.MapDefaultControllerRoute();
    });
}
```
