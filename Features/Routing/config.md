# Configuration

=============================================================
## UseRouting
* -> **matches request to an endpoint** - looks at the **`set of endpoints defined in the app`**, and **`selects the best match`** based on the request
* -> in ASP.NET Core 6, there's **`no need to have explicit calls`** to _UseRouting_ register routes
* -> UseRouting can still be used to specify where route matching happens, but UseRouting doesn't need to be explicitly called if routes should be matched at the beginning of the middleware pipeline

## UseEndpoints
* -> **execute the matched endpoint** - **`runs the delegate associated with the selected endpoint`**
* -> in ASP.NET Core 6, there's **`no need to have explicit calls`** to _UseEndpoints_ to register routes

## Summary
* -> by seperate into 2 middleware, tt decouples the **`route matching`** and **`resolution functionality`** from the endpoint executing functionality
* -> this makes the ASP.NET Core framework more flexible and **allows other middlewares to act between UseRouting and UseEndpoints**

* => that allows those other middlewares to **`utilize the information from endpoint routing`**
* -> for example, **UseAuthentication** must go after UseRouting, so that **`route information is available`** for authentication decisions 
* -> and before UseEndpoints so that users are **`authenticated before accessing the endpoints`**

```cs
// Depending on where app.Use is called in the pipeline, there may not be an endpoint

app.Use(async (context, next) =>
{
    
    Console.WriteLine("Before routing runs, endpoint is always null here");
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "null"}");
    await next(context);
});

app.UseRouting();

app.Use(async (context, next) =>
{
    Console.WriteLine("After routing runs, endpoint will be non-null if routing found a match.");
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

## Note
* -> if the app calls **UseStaticFiles**, **`place UseStaticFiles before UseRouting`**
* -> it's important that you place the **Authentication** and **Authorization** middleware **`between UseRouting and UseEndPoints`**
* -> **any middleware that appears after the UseRouting()** call will **`know which endpoint will run eventually`** (_if it appears before the UseRouting(), it won't know which endpoint will run_)

=============================================================
## MapControllerRoute
* -> most often used in an **MVC application**
* -> uses **conventional routing**, and **`sets up the URL route pattern`**

* -> but it's entirely possible to use MapControllerRoute (and by proxy MapDefaultControllerRoute) **`along side attribute routing`** as well
* -> if the user does not supply attributes, it will **`use the defined default pattern`**
*_về cơ bản thì thằng này có thể làm hết, không cần `route attribute` và còn cho ta `default route` nếu không tìm thấy_

```cs - Example:
endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
```
* -> the above pattern is basically **`{{root_url}}/{{name_of_controller}}/{{name_of_action}}/{{optional_id}}`** (_if controller and action are not supplied, it defaults to home/index_)
* -> but we could set this to whatever we wanted (within reason) and our routes would follow this pattern

## MapDefaultControllerRoute
* -> this is the above, but it **`shorthands the configuration of the default pattern`** that we displayed above

## MapControllers
* -> most commonly used in **WebAPI controllers**
* -> this doesn't make any assumptions about routing and will **`rely on the user doing attribute routing`** to get requests to the right place