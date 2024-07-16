======================================================================
# Bundling and Minification
* -> _bundling and minification techniques_ were introduced in MVC 4 to **improve request load time**
* -> **`bundling`** allows us to **load the bunch of static files from the server in a single HTTP request**
* _tức là thay vì mỗi request lấy 1 file .js thì gom tất cả file .js thành 1 rồi request file đó thôi_

## Minification
* ->  **removing unnecessary `white space` and `comments` and `shortening variable names` to one character**
* => **`optimizes script or CSS file size`** - improve the loading time of the page

```js
// normally
sayHello = function(name){
    //this is comment
    var msg = "Hello" + name;
    alert(msg);
}

// minify version
sayHello=function(n){var t="Hello"+n;alert(t)}
```

## Bundle Types
* _MVC 5 includes following bundle classes in **`System.web.Optimization`** namespace:_
* -> **ScriptBundle** - responsible for **`JavaScript minification of single or multiple script files`**
* -> **StyleBundle** - responsible for **`CSS minification of single or multiple style sheet files`**
* -> **DynamicFolderBundle** - represents **`a Bundle object`** that ASP.NET creates from a folder that contains **`files of the same type`**

## Create bundle
* -> we can **`create style or script bundles`** in **BundleConfig** class under **App_Start** folder in an ASP.NET MVC project
* - _we can create our own custom class instead of using BundleConfig class, but it is recommended to follow standard practice_

======================================================================
# ScriptBundle - combine multiple javascript files
* -> **`ScriptBundle`** class represents a bundle that does **JavaScript minification and bundling**

* -> inside **RegisterBundles** method of **BundleConfig** class, we **`created a new bundle`** by creating an instance of the **ScriptBundle** class - **`specific the virtual path`** in constructor
* -> then add the new bundle into the **BundleCollection**
* -> **`by default`**, the bundle file will be created in the **release mode**, but we can see bundles in the **debug mode** by using **BundleTable.EnableOptimizations = true**
* -> then calls the **BundleConfig.RegisterBundle()** from the **Application_Start** event in the **Global.asax.cs** file to all the bundles at the starting of an application
* -> in view, we use **Scripts.Render()** to use it

```cs - ~/App_Start/BundleConfig.cs
using System.Web;
using System.Web.Optimization;

public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {   
        // add 2 js file "bootstrap.js" and "jquery-3.3.1.js" in this bundle
        // this will create a bundle with the name "bs-jq-bundle"
        bundles.Add(new ScriptBundle("~/bundles/bs-jq-bundle").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery-3.3.1.js"));

        BundleTable.EnableOptimizations = true; // creates bundles in debug mode
    }
}
```

```cs - Usage
// when we run the application, we will see the bundle is created and loaded in a single request
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title</title>
    @Scripts.Render("~/bundles/bs-jq-bundle") // this one
</head>
<body>
    @*html code removed for clarity *@
</body>
</html>
```

## Include a Directory in Bundle
* -> to **`add all the files under a particular directory in a bundle`**

```cs
public static void RegisterBundles(BundleCollection bundles)
{            
    bundles.Add(new ScriptBundle("~/bundles/scripts")
                    .IncludeDirectory("~/Scripts/","*.js", true));
}
```

## Using WildCards
* -> most third party JavaScript files **`include a version in the name`**
* * -> the wildcard **{version}** will **`automatically pick up an available version file`**

```js
// For example, jQuery includes the version in the file name
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {            
        bundles.Add(new ScriptBundle("~/bundles/jquery")
               .Include( "~/Scripts/jquery-{version}.js"));
    }
}
```

## Using CDN
* -> we can also create a bundle of the files from **CDN (Content Delivery Network)**
```js
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {            
        var cdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js";

        bundles.Add(new ScriptBundle("~/bundles/jquery", cdnPath)
               .Include( "~/Scripts/jquery-{version}.js"));
    }
}
```

======================================================================
# StyleBundle
* -> **`StyleBundle`** class that does **CSS minification and bundling**
* -> it should be created in the **BundleConfig** class
* _we can use `.IncludeDirectory()`, version wildcard `{version}`, and `CDN path` same way as ScriptBundle_

```cs - combine multiple CSS files into a bundle
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {            
        // combine multiple CSS files into a bundle
        // create a bundle with the name "css"
        bundles.Add(new StyleBundle("~/bundles/css").Include(
                                                    "~/Content/bootstrap.css",
                                                    "~/Content/site.css"
                                                ));
    }
}
```

```cs - Usage in view:
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - My ASP.NET Application</title>
    @Styles.Render("~/bundles/css") // this one
</head>
<body>
    @*html code removed for clarity *@
</body>
</html>
```