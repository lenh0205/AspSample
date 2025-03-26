===================================================================================
# Layout
* -> define a **common template** - **`inherit a consistent look and feel`** across all the views/pages of ASP.NET MVC application

## Structure of a Layout
* -> _Layout view_ contain **`common portion`** (_Header, Left Menu, Right bar, and Footer sections_) and **`placeholder for the center section that changes dynamically`** 
* -> the placeholder can be render by using **RenderBody()** and **RenderSection** (_đây cũng là cách ta nhận biết Layout view so với những view thường_)

## Apply "Layout" 
* -> the **_ViewStart.cshtml** within the `Views` folder for defining the **default Layout page** for our ASP.NET MVC application
* -> _Layout views_ are **`shared with multiple views`**, so it must be stored in the **Shared** folder

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
* -> specific the layout view for child view in **_ViewStart.cshtml**, in **child view** itself, or in an **action method**

## ViewStart
* _xem `~/Features/Common/Default_Behavior.md` để hiểu thêm_

```js - ~/Views/_ViewStart.cshtml
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
}
// all views under "Views" folder (if there is no other _ViewStart.cshml) will use the "_Layout.cshtml" as view
```

## Specify Layout View in a Child View
* -> **override the default layout view setting** of **`_ViewStart.cshtml`** by setting the **Layout** property in **`each child view`**

```js - Index.cshtml
@{
    ViewBag.Title = "Home Page";
    Layout = "~/Views/Shared/_myLayoutPage.cshtml";
    // "Index.cshtml" view uses the _myLayoutPage.cshtml even if _ViewStart.cshtml sets the _Layout.cshtml
}

@section footer {
    <p class="lead">
        This is footer section.
    </p>
}

<div class="jumbotron">
    <h1>ASP.NET</h1>
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
