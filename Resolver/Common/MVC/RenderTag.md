===================================================================================
# Layout
* -> define a **common template** - **`inherit a consistent look and feel`** across all the views/pages of ASP.NET MVC application
* -> the **_ViewStart.cshtml** within the `Views` folder for defining the **default Layout page** for our ASP.NET MVC application

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
# Styles.Render 
* -> used to **render a bundle of CSS files** defined within **BundleConfig.cs** files
* -> by **`creates style tags for the CSS bundle`**

# Scripts.Render
* -> used to **render a bundle of Script files** defined within **BundleConfig.cs** files
* -> by **`create script tags for the Script bundle`**

# Note:
* -> _`Styles.Render` and `Scripts.Render`_ **generates `multiple` style and script tags for each item** in the CSS bundle and Script bundle when **optimizations are disabled**
* -> when **optimizations are enabled**, _`Styles.Render` and `Scripts.Render`_ **generates a `single` style and script tag to a version-stamped URL** which **`represents the entire bundle for CSS and Scripts`**

* -> **`to enable/disable optimizations`** by setting the **EnableOptimizations** property of the **BundleTable** class to true/false within **`Global.asax.cs file`**:
```cs
protected void Application_Start()
{
    //Other code has been removed for clarity
    System.Web.Optimization.BundleTable.EnableOptimizations = false;
}
```

# BundleConfig
```cs 
// App_Start/BundleConfig.cs
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"));

        bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"));

        bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"));

        bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js"));

        bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/site.css"));
    }
} 

// Global.asax.cs
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
}
```

```cs - _Layout.cshtml
@Styles.Render("~/Content/css")

@Scripts.Render("~/bundles/modernizr")

@Scripts.Render("~/bundles/jquery")

@Scripts.Render("~/bundles/bootstrap")
```

```html - chạy website lên và view page source:
<link href="/Content/bootstrap.css" rel="stylesheet"/>
<link href="/Content/site.css" rel="stylesheet"/>

<script src="/Scripts/modernizr-2.8.3.js"></script>

<script src="/Scripts/jquery-3.4.1.js"></script>

<script src="/Scripts/bootstrap.js"></script>
```

===================================================================================
# "Html.ActionLink" and "Html Anchor Tag" 

## "Html.ActionLink" and "
* -> we should use **`Html.ActionLink()` to generate anchor tags** and **`Url.Action()` to generate URL** 

* -> as it will make sure to **`generate correct relative URL`** **from the action and controller name**
* -> because it **use current routing configuration in `Global.asax.cs`** to **`prepare hyperlink at runtime`**
* => _so when we `modify our routing setting in said global file` then Html.ActionLink directly `reference modified routing settings`; ensure that the links won't be broken_

```cs - VD:
// we have a view:  Views/Home/Partials/_Index.cshtml
// if we want to go to /About/Index (AboutController and Index action)

<a href="/About/Index">About</a> // wrong

<a href="@Url.Action("Index","About")">About</a> // <a href="../About/Index">About</a> // correct
Html.ActionLink(linkText:="About", controllerName:="About", actionName:="Index")  // <a href="../About/Index">About</a> // correct
```

## Anchor tag
* -> we may make mistak in providing URL
* -> _i **`changing configuration in Global.asax.cs`** can cause our **`link broken at runtime`**; to avoid this we have to **change all anchor tags manually**_

