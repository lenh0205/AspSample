===================================================================================
# Layout
* -> define a **common template** - **`inherit a consistent look and feel`** across all the views/pages of ASP.NET MVC application
* -> the **_ViewStart.cshtml** within the `Views` folder for defining the **default Layout page** for our ASP.NET MVC application

* -> _Layout views_ are **`shared with multiple views`**, so it must be stored in the **Shared** folder
* -> _Layout view_ contain **`common portion`** (_Header, Left Menu, Right bar, and Footer sections_) and **`placeholder for the center section that changes dynamically`** by using **RenderBody()**

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
# Specific the layout view for child view
* _in `_ViewStart.cshtml`, in `child view` itself, or in an `action method`_

## ViewStart
* -> the **`default _ViewStart.cshtml file`** is included in the **Views** folder, but can also be created in **`all other sub-folders of Views folder`**
* -> it is used to **specify common settings for all the `views` under a folder and sub-folders where it is created**

* -> set the **Layout** property to **`a particular layout view`** will be applicable to **all the child views under that folder and its sub-folders**
* -> _For example, the following `_ViewStart.cshtml` in the `Views` folder sets the `Layout property to "~/Views/Shared/_Layout.cshtml"`; so, the **`_layout.cshtml would be a layout view of all the views included in Views and its subfolders`**_

https://www.tutorialsteacher.com/mvc/layout-view-in-asp.net-mvc