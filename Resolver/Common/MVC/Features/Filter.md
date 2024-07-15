=======================================================================
# ASP.NET MVC - Filters
* -> ASP.NET MVC Filter is **`a custom class`** allow us to **apply some logic to execute before or after an action method executes**

* -> Filters can be **`applied to`** an **action method** or **controller** in a **`declarative`** or **`programmatic`** way
* -> **Declarative** means by **`applying a filter attribute`** to an action method or controller class 
* -> **Programmatic** means by **`implementing a corresponding interface`**

```cs - built-in filters and interface that must be implemented to create custom filters
+-----------------+------------------------------------------+------------------------------------------+
| Filter Type     |      Description                         | Built-in Filter	 | Interface            |
+-----------------+------------------------------------------+------------------------------------------+
| Authorization   | Performs authentication and authorizes   | [Authorize],      | IAuthorizationFilter |
| filters         | before executing an action method        | [RequireHttps]    |                      |
+-----------------+------------------------------------------+------------------------------------------+
| Action filters  | Performs some operation before and       |                   | IActionFilter        |
|                 | after an action method executes          |                   |                      |
+-----------------+------------------------------------------+------------------------------------------+
| Result filters  | Performs some operation before or after  | [OutputCache]     | IResultFilter        |
|                 | the execution of the view                |                   |                      |
+-----------------+------------------------------------------+------------------------------------------+
| Exception       | Performs some operation if there is an   | [HandleError]     | IExceptionFilter     |
| filters         | unhandled exception thrown during the    |                   |                      |
|                 | execution of the ASP.NET MVC pipeline    |                   |                      |
+-----------------+------------------------------------------+------------------------------------------+
```

```cs - Ex: built-in Exception filter 
// 'HandleErrorAttribute' class is a built-in exception filter class 
// that renders the 'Error.cshtml' by default when an unhandled exception (not have try/catch block) occurs 

// web.config
<System.web>
    <customErrors mode="On" /> 
</System.web>

[HandleError] // this one
public class HomeController : Controller
{
    public ActionResult Index()
    {
        throw new Exception("This is unhandled exception");
        // error page Error.cshtml will be displayed if user access Index()
            
        return View();
    }

    public ActionResult About()
    {
        return View();
    }

    public ActionResult Contact()
    {
        return View();
    }        
}
```

=======================================================================
# Register Filters
* _Filters can be applied at three levels: `Global`, `Controller`, `Action Method`_

## Global Level Filters
* -> apply filters at a **`global`** level - applied to **all the controller and action methods**
* -> by using default **FilterConfig.RegisterGlobalFilters()** method in the **Application_Start** event of the **global.asax.cs** file 

```cs - global.asax.cs
// -> by default in every MVC application created using Visual Studio, the [HandleError] filter is applied globally in the MVC application

// MvcApplication class contains in Global.asax.cs file 
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    }
}

// FilterConfig.cs located in App_Start folder 
public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new HandleErrorAttribute());
    }
}
```

## Controller Level Filters
* -> Controller level filters are **`applied to all the action methods`**

```cs
[HandleError]
public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}
```

## Action Method Filters
* -> **`one or more filters`** can also applied to **`an individual action method`**

```cs
public class HomeController : Controller
{
    [HandleError]
    public ActionResult Index()
    {
        return View();
    }
}
```

=======================================================================
# Action Filters
* -> Action filter **executes before and after an action method executes** - can be applied to **`an individual action method or to a controller`**
* _Action filters are generally used to **apply cross-cutting concerns** such as **`logging`**, **`caching`**, **`authorization`**, ..._

```cs
// the "OutputCache" is a built-in action filter attribute 
// that can be applied to an action method for which we want to cache the output

[OutputCache(Duration=100)] // the output of this action method will be cached for 100 seconds
public ActionResult Index()
{
    return View();
}
```

# Create a Custom Action Filter
* _there're 2 ways to create a custom action filter_
* -> implementing the **`IActionFilter` interface and the `FilterAttribute` class**
* -> deriving the **ActionFilterAttribute** abstract class

## the 'IActionFilter' interface 
* _methods to `implement`:_
* -> **void OnActionExecuted(ActionExecutedContext filterContext)**
* -> **void OnActionExecuting(ActionExecutingContext filterContext)**

## the 'ActionFilterAttribute' abstract class 
* _methods to `override`:_
* -> **void OnActionExecuted(ActionExecutedContext filterContext)**
* -> **void OnActionExecuting(ActionExecutingContext filterContext)**
* -> **void OnResultExecuted(ResultExecutedContext filterContext)**
* -> **void OnResultExecuting(ResultExecutingContext filterContext)**

```cs - creating a custom action filter class for "Logging"
public class LogAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        Log("OnActionExecuted", filterContext.RouteData); 
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        Log("OnActionExecuting", filterContext.RouteData);      
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        Log("OnResultExecuted", filterContext.RouteData);      
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        Log("OnResultExecuting ", filterContext.RouteData);      
    }

    private void Log(string methodName, RouteData routeData)
    {
        var controllerName = routeData.Values["controller"];
        var actionName = routeData.Values["action"];
        var message = String.Format("{0}- controller:{1} action:{2}", methodName, 
                                                                    controllerName, 
                                                                    actionName);
        Debug.WriteLine(message);
    }
}

// Usage
[Log] // this one
public class HomeController : Controller
{
}
```