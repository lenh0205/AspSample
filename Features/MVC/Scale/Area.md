=========================================================================
# Area in ASP.NET MVC
* -> _usually use in **`large ASP.NET MVC application`**_

## Reason
* -> the large ASP.NET MVC application includes **`many controllers, views, and model classes`**
* -> so it can be difficult to maintain it with **`the default ASP.NET MVC project structure`**
* => ASP.NET MVC introduced a new feature called **Area** for this
* => allow **multiple developers to work on the same web application** without **`interfering with one another`**

## Purpose
* -> allows us to **partition the large application into smaller `units`**
* -> each **`unit`** contains **a separate MVC folder structure** (_same as the default MVC folder structure_)

```js - For example:
// a large enterprise application may have different modules like admin, finance, HR, marketing, ...
// so an "Area" can contain a separate MVC folder structure for all these modules

root
|
|-Areas
    |- admin
    |--Controller
    |--Models
    |--Views
    |    |-Shared
    |    |-web.config
    |--adminAreaRegistration.cs
    |
    |- finance
    |--Controller
    |--Models
    |--Views
    |    |-Shared
    |    |-web.config
    |--financeAreaRegistration.cs
    |
    |- HR
    |--Controller
    |--Models
    |--Views
    |    |-Shared
    |    |-web.config
    |--HRAreaRegistration.cs
```

=========================================================================
# Creating and Register Area
* -> open Visual Studio -> right-click on the project -> Add -> Area... -> enter Area name
* -> sau đó ta có thể thêm vào Area ta vừa tạo các controller, view, model

* -> **all the areas must be registered in the `Application_Start` event** in **`Global.asax.cs`** as **AreaRegistration.RegisterAllAreas();**
* _`order of registering the Areas` must be **on top** so that all of the settings, filters, and routes registered for the applications will also apply to the Areas_

```cs - Global.asax.cs
protected void Application_Start()
{
 // Register all application Areas
 AreaRegistration.RegisterAllAreas();
 
 WebApiConfig.Register(GlobalConfiguration.Configuration);
 FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
 RouteConfig.RegisterRoutes(RouteTable.Routes);
 BundleConfig.RegisterBundles(BundleTable.Bundles);

}
```

=========================================================================
# Structure
* -> each `Area` includes the **AreaRegistration** class - override the **RegisterArea** method to **`map the routes for the Area`**

```cs - adminAreaRegistration.cs
public class adminAreaRegistration : AreaRegistration 
{
    public override string AreaName 
    {
        get 
        {
            return "admin";
        }
    }

    public override void RegisterArea(AreaRegistrationContext context) 
    {
        // any URL that starts with the "admin" 
        // will be handled by the controllers included in the admin folder structure under the "Area" folder
        // Ex: http://localhost/admin/profile will be handled by the "ProfileController"
        // included in the "Areas/admin/controller/ProfileController folder"
        context.MapRoute(
            "admin_default",
            "admin/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
            // -> ta có thể sửa thành: new { action = "Index", controller = "Home", id = UrlParameter.Optional }
            // -> để chỉ cần truy cập "http://localhost/admin" thì nó sẽ nhảy đến "Index.cshtml" của "Home"
        );
    }
}
```

## Folder Structure
* -> each `Area folder` by default will include _`Controllers`, `Models`, `Views` folder_ and _a `AreaRegistration.cs` file_  
