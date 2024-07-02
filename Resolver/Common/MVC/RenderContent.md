===================================================================================
# ASP.NET MVC 5 – RenderBody, RenderPage, RenderSection

## @RenderBody
* -> the _@RenderBody() method_ is **`used within a master page`** to indicate **where the content from the child view should be injected**

## "@RenderSection" and "@section" directive
* -> the contents define in **`@section` blocks of child view** will be insert into the **corresponding named sections (`@RenderSection`) in the master page**

* -> the **second parameter** is false (**`@RenderSection("Footer", false)`**) denotes that the section is optional and we can choose to use it or not
* -> if using **true** then it is **`compulsory to have this section defined in child page`** otherwise we will get the error: _[HttpException (0x80004005): Section not defined: "MySection"]_
* -> _ta có thể dùng method **IsSectionDefined()** để kiểm tra name section đã được khai báo hay chưa_

* _i **`order of definding @section`** doesn't matter_

```js
// master page (_Layout.cshtml)
<head>
    @if (IsSectionDefined("head"))
    {
        @RenderSection("head", required: true);
    }
    else
    {
        <span>This is The default Header</span>
    }
</head>
<body>
    @RenderBody()
    @RenderSection("scripts", required: false)
</body>

// child page (Home/Index.cshtml)
@section head {
    <link href="~/Content/Site.css" rel="stylesheet" type="text/css" />
}

<h2>This is the child view content</h2>

@section scripts {
    @Scripts.Render("~/bundles/jquery")
}
```

## @RenderPage
* -> a page use it to **`render other pages that exist in our application`**
* -> **first parameter** refers to the **`file's physical location`**
* -> **second parameter** (optional) is an array of objects/models that can be passed into the view

```js 
// create 2 partial views under the "Views/Shared" folder (to make it as shared): Shared/_Header.cshtml, Shared/_Footer.cshtml

// _Layout.cshtml
<!DOCTYPE html>
<html>
<head>
    <title>….</title>
</head>
<body>
    <div class="header-content">@RenderPage("~/shared/_Header.cshtml")</div>
    <div class="body-content">@RenderBody()</div>
    <div class="header-content">@RenderPage("~/shared/_Footer.cshtml")</div>
</body>
</html>
```