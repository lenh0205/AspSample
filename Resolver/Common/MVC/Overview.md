===========================================================================
# Supported Features
* https://www.tutorialsteacher.com/mvc/asp.net-mvc-version-history
* -> những chức năng được hỗ trợ trong việc `viết HTML markup cùng server-side code`: **HTML Helper**, **Ajax Helper**, ....

===========================================================================
# Basic ASP.NET MVC 5 Folder Structure

## App_Data
* -> **IIS will never serve files from App_Data folder**
* -> can contain **`application data files`** like _LocalDB, .mdf files, XML files, and other data related files_

## App_Start
* -> contain **class files that will be executed when the application starts**
* -> typically, these would be **`config files`** - **by default of MVC 5** are **`BundleConfig.cs`**, **`FilterConfig.cs`**, **`RouteConfig.cs`**; or some others common like **`AuthConfig`**, ... 

## Content
* -> contains **static files** like **`CSS files, images, and icons files`**
* -> **by default of MVC 5 application** includes **`bootstrap.css, bootstrap.min.css, and Site.css`**

## Models
* -> contains **model class files**
* -> typically model class **includes public properties**, which will be used by the application to **`hold and manipulate application data`**

## Scripts
* -> contains **JavaScript** (_or VBScript_) files for the application
* -> **by default of MVC 5** includes javascript files for **`bootstrap, jquery 1.10, and modernizer`**

## Views
* -> contains **views files (.cshtml file)** for the application - where we write **`HTML and C# (or VB.NET code)`**
* -> **includes a separate folder correspond to each controller**
* _Ex: all the .cshtml files, which will be rendered by HomeController will be in View > Home folder_

* -> the **Shared** folder under the View folder **`contains all the views shared among different controllers`** (_Ex: layout files_)

## Controller
* -> contains **class files for the controllers** - handles user's request and returns a response
* -> **MVC requires** the name of all controller files to **end with `Controller`**

## fonts
* -> contains **custom font files** for our application

===========================================================================
# ASP.NET MVC 5 configuration files

## Global.asax
* -> allows us to **write code that runs in response to application-level events** 
* -> such as **Application_BeginRequest**, **application_start**, **application_error**, **session_start**, **session_end**, ...

## Packages.config
* -> **`is managed by NuGet`** to **track what packages and versions** we have **`installed in the application`**

## Web.config
* -> contains **application-level configurations**

===========================================================================
# Routing
* -> Routing is about **maps URL to physical file or class**
* -> Routing process includes defining **`configured routes of an application`** - the **URL pattern** and **handler** information
* -> these config will be stored in **RouteTable** and will be **used by the Routing engine** to **`determine appropriate handler class or file`** for an **`incoming request`**

* -> we will **`configure all these route (register route) in 'RouteConfig' class`** - the **RouteConfig.cs** under **App_Start** folder
* -> then we need to register **`RouteConfig`** in the **Application_Start() event** in the _Global.asax_ so that it **includes all our routes into the RouteTable**

* -> MVC framework **evaluates each route in sequence** (_so the **`order of configured route does matter`**_)

```cs - Ex:
// ~/App_Start/RouteConfig.cs:
public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes) 
    { // "RouteCollection" is property of "RouteTable"
        routes.Ignore("*{resource}.axd/{*pathInfo}"); // Route to ignore

        routes.MapRoute(
            name: "Student",
            url: "students/{id}",
            // -> not specific "action" in URL - any URL starts with "domainName/students" 
            // -> always use the "Index()" action of the "StudentController" class

            defaults: new { controller = "Student", action = "Index"},
            constraints: new { id = @"\d+" } // Route - restrict the value of the parameter 
            // -> limit that the "id" parameter must be numeric
        );

        routes.MapRoute(
            name: "Default", // Route name
            url: "{controller}/{action}/{id}", // URL pattern
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } 
            // -> default parameter for URL pattern
        )
    }
}
```

## URL Pattern
* -> the URL pattern is considered **only after the domain name part** in the URL; if not existed, then the **`default controller and action method will be used`** to handle the request

```r - Ex:
// the "http://localhost:1234" would be handled by the "HomeController" and the "Index()" method as configured in the default parameter

___________________________________________________________________
|              URL                | Controller    | Action |  Id  |
|---------------------------------|---------------|--------|------|
| http://localhost/home           | HomController | Index  | null |
|---------------------------------|---------------|--------|------|
| http://localhost/home/index/123 | HomController | Index  |  123 |
|---------------------------------|---------------|--------|------|
```

## From "WebForm" to "MVC"
* -> in the **`ASP.NET Web Forms application`**, **every URL must match with a specific .aspx file** - **request handler is .aspx file**
* _Ex: a URL `http://domain/studentsinfo.aspx` must match with the file `studentsinfo.aspx` that contains code and markup for rendering a response to the browser_

* -> ASP.NET introduced **`Routing`** to **eliminate the needs of mapping each URL with a physical file**
* -> instead, map the URL pattern to the **`request handler`** - the **Controller** class and **Action** method

```r
// the URL: "http://domain/students"
// -> map to "http://domain/studentsinfo.aspx" in ASP.NET Webforms
// -> map to "StudentController" and "Index" action in ASP.NET MVC
```
