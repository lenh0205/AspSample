===================================================================================
# Layout
* -> define a **common template** - **`inherit a consistent look and feel`** across all the views/pages of ASP.NET MVC application
* -> the **_ViewStart.cshtml** within the `Views` folder for defining the **default Layout page** for our ASP.NET MVC application

* -> _Layout views_ are **`shared with multiple views`**, so it must be stored in the **Shared** folder
* -> _Layout view_ contain **`common portion`** (_Header, Left Menu, Right bar, and Footer sections_) and **`placeholder for the center section that changes dynamically`** 
* -> the placeholder can be render by using **RenderBody()** and **RenderSection**

```js - Basic structure of Layout page
<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width" />
        <title>@ViewBag.Title</title>
        @Styles.Render("~/Content/css")
        @Scripts.Render("~/bundles/modernizr")
    </head>
    <body>
        @RenderBody()

        @Scripts.Render("~/bundles/jquery")
        @RenderSection("scripts", required: false)
    </body>
</html>
```

===================================================================================
# Using Layout
* _specific the layout view for child view in `_ViewStart.cshtml`, in `child view` itself, or in an `action method`_

## ViewStart
* -> the **`default _ViewStart.cshtml file`** is included in the **Views** folder, but can also be created in **`all other folders/sub-folder of root (but not the root itself)`**
* -> it is used to **specify common settings for all the `views` under a folder and sub-folders where it is created**

* -> we can create **multiple "_ViewStart.cshtml" files** in **`different folder locations`** within the **`Views folder`** hierarchy
* -> when a view is rendered, ASP.NET will first look for a **`_ViewStart.cshtml`** file in the same folder as the view, and then **search up the folder hierarchy until it finds one**

* _For example, we render `Index.cshtml` view that's located inside `~/Views/Home` folder;_
* _then first, ASP.NET will find the `_ViewStart.cshtml` file in the `Home` folder; if not found, if we continue find it in next hierachy - `Views` folder; if not found, it stop because next one is the root_
* _so in this case, if we have only one `_ViewStart.cshtml` file locate in `~/Views/Shared` folder then the `Home/Index.cshtml` will render without any setting (Ex: Layout) take from `_ViewStart.cshtml` file_

```js - _ViewStart.cshtml
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
}
```

## Specify Layout View in a Child View
* -> **override the default layout view setting** of **`_ViewStart.cshtml`** by setting the **Layout** property in **`each child view`**

```js - Index.cshtml
@{
    ViewBag.Title = "Home Page";
    Layout = "~/Views/Shared/_myLayoutPage.cshtml";
    // "Index.cshtml" view uses the _myLayoutPage.cshtml even if _ViewStart.cshtml sets the _Layout.cshtml
}

<div class="jumbotron">
    <h1>ASP.NET</h1>
    <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS and JavaScript.</p>
    <p><a href="http://asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
</div>
```

## Specify Layout Page in Action Method
* -> specify the **`layout view name`** as **a second parameter in the `View()` method**

```cs
public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View("Index", "_myLayoutPage"); //set "_myLayoutView" as layout view
    }
}
```