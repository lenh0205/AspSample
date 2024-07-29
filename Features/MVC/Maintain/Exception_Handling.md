====================================================================
# Exception Handling in ASP.NET MVC

## Problem
* -> we may **`handle all possible exceptions`** in the action methods using **try-catch blocks**
* -> however, there can be **some unhandled exceptions** that we want to **log** and **display custom error messages or custom error pages to `users`**

* -> _when we create an MVC application in Visual Studio_, it **`doesn't implement any exception handling technique out of the box`**
* -> it will **`display an error page when an exception occurred`** - **Yellow Screen of Death** 
* -> shows _exception details_ such as **`exception type, line number and file name where the exception occurred, and stack trace`**

## Solution:
* _`ASP.NET` provides some ways to handle exceptions:_
* -> using **<customErrors> element in `web.config`**
* -> using **HandleErrorAttribute**
* -> **overriding Controller.OnException** method
* -> using **`Application_Error` event of HttpApplication**

* => in most web applications, we should ideally **log the exceptions** (_Ex: by using global **`Application_Error`** event_) and also **show appropriate error messages or pages** to the users (_Ex: using **`customErrors`** element_)
* _to return specific error code in response then we have to use **<httpErrors>** element in web.config_

====================================================================
# <customErrors> Element in web.config
* -> _the `customErrors` element_ under **`system.web`** in **`web.config`** is used to **configure error code to a custom page**
* -> it can be used to **`configure custom pages`** for any error code **4xx** or **5xx**
* -> however, it **cannot be used to `log exception` or perform any other `action` on exception**

* -> we also need to **add 'HandleErrorAttribute' filter** in the **`FilterConfig.cs file`**
* -> _after enabling the customErrors mode to "On"_, the _HandleErrorAttribute filter_ will set **~/Views/Shared/Error.cshtml** as the **`default custom error page`** to display on an error occurred

```xml - web.config
<system.web> 
    <customErrors mode="On"></customErrors>
</system.web> 
```
```cs - FilterConfig.cs
public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new HandleErrorAttribute());
    }
}
```

```cs - Ex:
public class HomeController : Controller
{
        public ActionResult Contact()
        {
            string msg = null;
            ViewBag.Message = msg.Length; // this will throw an exception
            return View();
        }
}
// -> when navigating to "/home/contact" in browser, we will get the "Error.cshtml" page
```

# HandleErrorAttribute
* -> an attribute that can be **`used to handle exceptions`** **thrown by an action method or a controller**
* -> we can use it to **display a custom view on a specific exception** occurred in **`an action method or in an entire controller`**
* -> however, it can only be used to handle the exception with **status code 500** and also it **`does not provide a way to log exceptions`**

```cs - Ex
public class HomeController : Controller
{
    [HandleError]
    [HandleError(ExceptionType =typeof(NullReferenceException), View ="~/Views/Error/NullReference.cshtml")]
    public ActionResult Contact()
    {
        string msg = null;
        ViewBag.Message = msg.Length; // this will throw an exception
        return View();
    }
}
// [handleError(...)] will display "NullReference.cshtml" when "Contact()" throw "NullReferenceException"
// [HandleError] will display the Error.cshtml view as default view for any other exceptions on "Contact()" action method
```

# Overriding 'Controller.OnException' Method
* -> this method **`handles all our unhandled errors`** with **error code 500**
* -> allows us to **log an exception** and **redirect to the specific view**
* -> **`doesn't require to enable the <customErrors> config in web.config`**

```cs
public class HomeController : Controller
{
    public ActionResult Contact()
    {
        string msg = null;
        ViewBag.Message = msg.Length;
            
        return View();
    }
    
    protected override void OnException(ExceptionContext filterContext)
    {
        filterContext.ExceptionHandled = true;

        //Log the error!!
     
        //Redirect to action
        filterContext.Result = RedirectToAction("Error", "InternalError");

        // OR return specific view
        filterContext.Result = new ViewResult
        {
            ViewName = "~/Views/Error/InternalError.cshtml"
        };
   }
} 
```

# using 'Application_Error' event of 'HttpApplication' in 'global.asax'
* -> this is **the ideal way to log exception** occurred in **`any part of our MVC application`**

```cs - global.asax
public class MvcApplication : System.Web.HttpApplication
{
    //other code removed for clarity

    protected void Application_Error()
    {
        var ex = Server.GetLastError();
        //log an exception
    }
}
```



