https://stackoverflow.com/questions/9270023/how-to-determine-if-an-exception-is-of-a-particular-type
https://stackify.com/csharp-exception-handling-best-practices/
https://dev.to/ephilips/better-error-handling-in-c-with-result-types-4aan

=================================================================

# "Endpoint Routing" feature of ASP.NET Core 3.0

## Prior to "Endpoint Routing" - the ASP.NET Core MVC middleware
* -> before the **`routing resolution for an ASP.NET Core application`** was done in the **`ASP.NET Core MVC middleware`** **at the end of the HTTP request processing pipeline (middleware pipeline)** 

* -> means that **route information** (_Ex: controller, action, ... would be executed_), **was not available to middleware** that **`processed the request before the MVC middleware`**
* -> it's _particularly useful to have this route information_ available in a **`CORS`** or **`authorization`** middleware to use the information as a factor in the authorization process 

* -> _Endpoint routing_ also allows to **decouple the route matching logic** from the **`MVC middleware`** and have its **`own middleware`**
* -> it allows the **MVC middleware** to focus on its responsibility of **`dispatching the request to the particular controller action method`** that is **`resolved`** by the **endpoint routing middleware**

## The Endpoint Routing middleware
* -> allow **the route resolution to happen earlier** in the pipeline in **`a separate endpoint routing middleware`**
* => this middleware can be **`placed at any point in the pipeline`**, after which **other middleware in the pipeline can access the resolved route data**

* _về cơ bản tức là cho phép xác định endpoint sẽ được dispatch sớm hơn trong pipeline, từ đó các middleware ở sau có thể sử dụng những thông tin đó để xây dựng những chức năng không thể ở phiên bản trước đó_
* _tách biệt `route matching` và `resolution` khỏi `endpoint dispatch`, vốn trước đó là 1 khổi nằm trong `MVC middle`_

## ASP.NET Core 3.0
* -> the **`ASP.NET Core 2.2`** adds a **new endpoint route resolution middleware**, but **keeps the endpoint dispatch in the MVC middleware** at the end of the pipeline
* -> in **`ASP.NET Core 3.0`**, this will change - the **`endpoint dispatch`** will happen in a separate **Endpoint Dispatch middleware** that will **`replace the MVC middleware`**

=================================================================
# Main concepts of "Enpoint Routing"

## Endpoint Route Resolution
* -> the concept of **looking at the incoming request** and **mapping the request to an endpoint** using **`route mappings`**
* -> **`an endpoint`** represents the **controller action** that **`the incoming request resolves to`**, along with **other metadata** **`attached to the route`** that matches the request 

* -> _the job of the route resolution middleware_ is to **construct "Endpoint" object** using the **`route information`** from **`the route that it resolves based on the route mappings`**
* -> the middleware then **places "Endpoint" object into the http context** 
* => where _other middleware the `come after the endpoint routing middleware` in the pipeline_ **can access the Endpoint object** and **use the route information within**

## Endpoint Dispatch
* -> the process of **invoking the controller action method** that **`corresponds to the endpoint`** that was **`resolved by the "Endpoint Routing Middleware"`**

* -> the **Endpoint Dispatch Middleware** is the **`last middleware in the pipeline`**
* ->  that **`grabs the "Endpoint" object from the http context`** and **`dispatches to particular controller action`** that the **resolved endpoint** specifies
* -> in ASP.NET Core 3.0, the **MVC middleware is removed**; instead the **`endpoint dispatch`** happens **at the end of the middleware pipeline by default** 
* _tức là ở ASP.NET Core 3.0 preview, "resolved endpoint" is implicitly dispatched at the end of the pipeline, no no explicit call to a "Endpoint Dispatcher Middleware"_

* -> because the MVC middleware is removed, the **route map configuration** that is usually passed to the **`MVC middleware`** may instead passed to the **`Endpoint Route Resolution middleware`**
* -> but no, the ASP.NET Core have a a **new endpoint routing middleware** that is **`placed at the end of the pipeline`** to **make the endpoint dispatch explicit again** (_instead of default implicit_)
* -> the **`route map configuration`** will be passed to this new middleware (_in `ASP.NET Core 3.0 final`_) instead of the Endpoint route resolution middleware (_in `ASP.NET Core 3.0 preview`_)

## Endpoint Route Mapping
* -> when we **`define route middleware`**, we can optionally **`pass in a lambda function`**
* -> that contains **route mappings that override the default route mapping** that **`ASP.NET Core MVC middleware extension method`** specifies 

* -> **Route mappings** are used by the **`route resolution process`** to **`match the incoming request parameters`** to **`a route specified in the route map`**
* -> _in ASP.NET 3.0 final_, the **`route mapping configuration lambda`** is being moved from the **Route Resolution Middleware** to the **Endpoint Dispatcher middleware**

* -> its important to note that the **`endpoint resolution`** happens during **`runtime request handling`** **after the route mapping is setup during application startup configuration**
* -> therefore, the **`route resolution middleware has access to the route mappings during request handling`** **regardless of which middleware the route map configuration is passed to**

=================================================================
# Accessing the resolved endpoint
* -> **any Middleware** after the **`endpoint route resolution middleware`** will be **`able to access the resolved endpoint`** through the **HttpContext** (_the **IEndpointFeature**_)

```c# - Example: in our custom middleware

app.Use((context, next) =>
{
    var endpointFeature = context.Features[typeof(Microsoft.AspNetCore.Http.Features.IEndpointFeature)]
                                           as Microsoft.AspNetCore.Http.Features.IEndpointFeature;

    Microsoft.AspNetCore.Http.Endpoint endpoint = endpointFeature?.Endpoint;

    //Note: endpoint will be null, if there was no
    //route match found for the request by the endpoint route resolver middleware
    if (endpoint != null)
    {
        var routePattern = (endpoint as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern
                                                                                   ?.RawText;

        Console.WriteLine("Name: " + endpoint.DisplayName);
        Console.WriteLine($"Route Pattern: {routePattern}");
        Console.WriteLine("Metadata Types: " + string.Join(", ", endpoint.Metadata));
    }
    return next();
});
```

==================================================================

# Endpoint routing configuration
* -> create the middleware pipeline **`Endpoint Route Resolver middleware`**, **`Endpoint Dispatcher middleware`** and **`Endpoint Route Mapping lambda`**
* -> setup in the **Startup.Configure** method of the **Startup.cs** file of **ASP.NET Core** project

* _there're configuration changed between versions, a general form of `endpoint routing middleware configuration` pseudo code base on 3 concept:_
```c#
//psuedocode that passes route map to endpoint resolver middleware
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    //middleware configured before the UseEndpointRouteResolverMiddleware middleware
    //that does not have access to the endpoint object
    app.UseBeforeEndpointResolutionMiddleware();

    // ------> the 'Endpoint Route Resolver Middleware'
    // middleware that inspects the incoming request, have access to "route mappings" 
    // resolves a match to the route map
    // construct "endpoint" object with route parameters and set into the "httpcontext"
    app.UseEndpointRouteResolverMiddleware()

    // middleware after configured after the UseEndpointRouteResolverMiddleware middleware
    // that can access to the "endpoint" object via HttpContext
    app.UseAfterEndpointResolutionMiddleware();

    // ------> the 'Endpoint Dispatch Middleware'
    // the middleware at the end of the pipeline that dispatches the controler action method
    // will replace the current MVC middleware
    // can access the resolved endpoint object via HttpContext
    app.UseEndpointDispatcherMiddleware(routes =>
    {
        //This is the route mapping configuration passed to the endpoint resolver middleware
        routes.MapControllers();
    });
}
```

## Endpoint routing in version 2.2
```c# -  create a Web API project using version 2.2 of the .NET Core SDK

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseHsts();

    // by default, endpoint routing is not added; MVC middleware also handles the route resolution
    // added Endpoint Routing - Endpoint Resolution Middleware, that will resolve the 'Endpoint' object
    // use the 'route mappings' configured by the 'MVC middleware'
    // all middlewares after this middleware will have access to the 'resolved Endpoint object'
    app.UseEndpointRouting(); 

    app.UseHttpsRedirection();

    //our custom middlware
    app.Use((context, next) =>
    {
        // if we enabled 'endpoint routing' we can inspect the resolved endpoint object
        var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
        var endpoint = endpointFeature?.Endpoint;

        //note: endpoint will be null, if there was no resolved route 
        // or the resolver was not able to match the request to a mapped route
        if (endpoint != null)
        {
            var routePattern = (endpoint as RouteEndpoint)?.RoutePattern
                                                          ?.RawText;

            Console.WriteLine("Name: " + endpoint.DisplayName);
            Console.WriteLine($"Route Pattern: {routePattern}");
            Console.WriteLine("Metadata Types: " + string.Join(", ", endpoint.Metadata));
        }
        return next();
    });

    // the MVC middleware - acts as the 'endpoint dispatcher middleware'
    // internally configures the "default route mapping" at startup configuration time
    // dispatches the controller action during request handling
    app.UseMvc();
}
```

## Endpoint routing in ASP.NET Version 3 preview 3
* -> in this version, the **Endpoint Routing** will **`become a full fledged citizen of ASP.NET Core`**; 
* -> and we will finally have **separation** between the **`MVC controller action dispatcher`** and the **`Route Resolution middleware`**

```cs - endpoint startup configuration 

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseHsts();

    app.UseHttpsRedirection();

    // configures "Endpoint Route Resolution middleware" to resolve the incoming request endpoint
    // takes a anonymous "lambda function" that configures the "route mappings" 
    app.UseRouting(routes =>
    {
        routes.MapControllers(); // configures the default MVC routes
    });

    // have access the httpcontext "Endpoint" object set by the "endpoint routing middleware"
    app.UseAuthorization();

    //our custom middleware
    app.Use((context, next) =>
    {
        var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
        var endpoint = endpointFeature?.Endpoint;

        //note: endpoint will be null, if there was no
        //route match found for the request by the endpoint route resolver middleware
        if (endpoint != null)
        {
            var routePattern = (endpoint as RouteEndpoint)?.RoutePattern
                                                          ?.RawText;

            Console.WriteLine("Name: " + endpoint.DisplayName);
            Console.WriteLine($"Route Pattern: {routePattern}");
            Console.WriteLine("Metadata Types: " + string.Join(", ", endpoint.Metadata));
        }
        return next();
    });

    // At the end of pipeline after all other middleware configuration 
    // no need to have a dispatcher middleware or any MVC middleware
    // the resolved endpoint is "implicitly dispatched" to a controller action at the end of pipeline
    // if an endpoint was not able to be resolved, a 404 not found is returned at the end of pipeline
}
```

## Endpoint routing in ASP.NET 3.0 final
* -> **make "Endpoint Routing" more explicit** by adding back in the call to **`Endpoint Dispatcher middleware`** configuration
* -> also **moving back the "route mapping" configuration option** to the **`dispatcher middleware configuration method`**

```cs
public void Configure(IApplicationBuilder app)
{
    // Configure Session.
    app.UseSession();

    // Add static files to the request pipeline
    app.UseStaticFiles();

    // Add the endpoint routing matcher middleware to the request pipeline
    // still have access to the mappings to resolve the endpoint at request handling time
    // although it's passed to the "UseEndpoints middleware" during startup configuration
    app.UseRouting();

    // Add cookie-based authentication to the request pipeline
    app.UseAuthentication();

    // Add the authorization middleware to the request pipeline
    app.UseAuthorization();

    // an explicit endpoint dispatch method provides the "endpoint dispatch implementation"
    // add endpoints to the request pipeline
    app.UseEndpoints(endpoints =>
    {
        // "route mapping configuration lambda" have been moved
        // from the "UseRouting" middleware to the new "UseEndpoints" middleware
        endpoints.MapControllerRoute(
            name: "areaRoute",
            pattern: "{area:exists}/{controller}/{action}",
            defaults: new { action = "Index" });

        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action}/{id?}",
            defaults: new { controller = "Home", action = "Index" });

        endpoints.MapControllerRoute(
            name: "api",
            pattern: "{controller}/{id?}");
    });
}
```

=============================================================
# DI services for "Endpoint routing middleware" 
* -> to use **endpoint routing**, we also need to add some services to DI container in the **`Startup.ConfigureServices`** method

## ConfigureServices in Version 2.2
* -> need to **explicitly add** the call to **services.AddRouting()** method to add **`endpoint routing feature`** to the _DI container_
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddRouting() // 
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
}
```

## ConfigureServices in Version 3 preview 3
* -> **`endpoint routing`** is **already configured** in the _DI container_ under the covers in the **AddMvc()** extension method
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddNewtonsoftJson();
}
```

=============================================================
# Setting up "Endpoint Authorization" using "endpoint routing" and "route mappings"
* -> when working with the _version 3 preview 3 release_, we can **attach authorization metadata to an endpoint**
* -> we do this using the **route mappings configuration fluent API** **`RequireAuthorization`** method

* -> the **`endpoint routing resolver`** will **access this metadata** when **`processing a request`** and **add it to the "Endpoint" object** that it sets on the **httpcontext**
* -> **any middleware** in the pipeline **`after the route resolution middleware`** can **access this authorization data** by accessing the **`resolved Endpoint object`**
* => in particular the **`authorization middleware`** can use this data to **make authorization decisions**

```cs - ASP.NET version 3 preview 3 - added a new "/secret" route 
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseHsts();

    app.UseHttpsRedirection();

    app.UseRouting(routes =>
    {
        routes.MapControllers();

        // Mapped route that gets attached authorization metadata using the 'RequireAuthorization' method
        // the RequireAuthorization method to add an "AuthorizeAttribute" attribute to the "/secret" route
        // this metadata will be added to the resolved endpoint for this route by the endpoint resolver
        // app.UseAuthorization() middleware later in the pipeline will get the resolved endpoint
        // correspond the /secret route and use the authorization metadata attached to the endpoint
        routes.MapGet("/secret", context =>
        {
            return context.Response.WriteAsync("secret");
        }).RequireAuthorization(new AuthorizeAttribute(){ Roles = "admin" });
    });

    app.UseAuthentication();

    // a custom middleware to inspect the "resolved endpoint object" in "httpcontext"
    // so that we can inspect the "AuthorizeAttribute" added to the endpoint metadata
    app.Use((context, next) =>
    {
        var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
        var endpoint = endpointFeature?.Endpoint;

        //note: endpoint will be null, if there was no
        //route match found for the request by the endpoint route resolver middleware
        if (endpoint != null)
        {
            var routePattern = (endpoint as RouteEndpoint)?.RoutePattern
                                                          ?.RawText;

            Console.WriteLine("Name: " + endpoint.DisplayName);
            Console.WriteLine($"Route Pattern: {routePattern}");
            Console.WriteLine("Metadata Types: " + string.Join(", ", endpoint.Metadata));
            // the metadata will contains "AuthorizeAttribute" and "HttpMethodMetadata"
        }
        return next();
    });

    // the Authorization middleware check the resolved endpoint object
    // to see if it requires authorization. If it does as in the case of
    // the "/secret" route, then it will authorize the route, if it the user is in the admin role
    app.UseAuthorization();

    //the framework implicitly dispatches the endpoint at the end of the pipeline.
}
```

```cs - ASP.NET 3.0 final - Startup.cs in WebAPI project
//code from Startup.cs file in a webapi project template

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers()
            .AddNewtonsoftJson();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseHsts();

    app.UseHttpsRedirection();

    //add endpoint resolution middlware
    app.UseRouting();

    app.UseAuthorization();

    // explicitly add endpoint dispatch middleware
    app.UseEndpoints(endpoints =>
    {
        //route map configuration
        endpoints.MapControllers();

        //route map I added to show Authorization setup
        endpoints.MapGet("/secret", context =>
        {
            return context.Response.WriteAsync("secret");
        }).RequireAuthorization(new AuthorizeAttribute(){ Roles = "admin" });  
    });
}
```

=============================================================
# ASP.NET Core 6
* -> there's **no need to have explicit calls** to **`UseRouting`** or **`UseEndpoints`** to register routes
* -> if not explicitly call UseRouting() by default, **`routes should be matched at the beginning of the middleware pipeline`**
* -> however, **`UseRouting() can still be used`** to **specify where route matching happens**

```cs
app.Use(async (context, next) =>
{
    
    Console.WriteLine("this before routing runs, so endpoint is always null here");
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "null"}");
    await next(context);
});

app.UseRouting(); // routing runs

app.Use(async (context, next) =>
{
    Console.WriteLine("this after routing runs, so endpoint will be non-null if routing found a match.");
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "null"}");
    await next(context);
});

app.MapGet("/", (HttpContext context) =>
{
    Console.WriteLine("Runs when this endpoint matches");
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "null"}");
    return "Hello World!";
}).WithDisplayName("/");

app.UseEndpoints(_ => { });

app.Use(async (context, next) =>
{
    Console.WriteLine("Runs after UseEndpoints - will only run if there was no match.");
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "null"}");
    await next(context);
});
```