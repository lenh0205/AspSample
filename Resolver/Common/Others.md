================================================================
# Access API Route information

## get Controller name and Action name in MVC
```cs
// VD: a partial view responsible for rendering the website's menu links
// for every page in website, the links are prepared and passed to the view from an action called "SiteMenuPartial" in "LayoutController"
// when we load up /Home/Index, the layout page is retrieved, the SiteMenuPartial method is called by the layout page, and the SiteMenuPartial.cshtml partial view is returned
// if inside that partial view, we do:

@{ 
    // return name of Controller handling the view where the code is executed
    // Output: Layout
    var controllerName = this.ViewContext.RouteData.Values["controller"].ToString(); // MVC 4
    var controllerName = ViewContext.Controller.ValueProvider.GetValue("controller").RawValue // MVC 3
}

@{ 
    // return name of controller requested in the URL
    // in case we are inside a partial view belonging to a different controller and want to get the name of the controller "higher-up" in the chain
    // Output: Home
    var controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
}

<%= ViewContext.RouteData.Values["Controller"] %>
<%= ViewContext.RouteData.Values["Action"] %> // get "Action" name

<script>var controllerName = @controllerName</script>
<script>var controllerName = @iewContext.RouteData.Values["controller"]</script>
```