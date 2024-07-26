> việc trỗi dậy của WebPI giúp ta đỡ sự phụ thuộc của client vào backend về: công nghệ sử dụng, logic, ....

# Install
* -> cài **`Nuget package`** **Microsoft ASP.NET Web API** vào MVC project (_trong trường hợp ban đầu ta init project bằng template `WebApi` thì có sẵn rồi không cần cài nữa_)

# Register
```cs - ~/App_Start/WebApiConfig.cs
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // TODO: Add any additional configuration code.

        // Web API routes
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

    // WebAPI when dealing with JSON & JavaScript
    // Setup json serialization to serialize classes to camel (std. Json format)
    var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
    formatter.SerializerSettings.ContractResolver =
        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    }
}
```

```cs - Global.asax
protected void Application_Start() // the order routes is critical
{
    // Default 
    AreaRegistration.RegisterAllAreas();

    // manually installed WebAPI 2.2 
    GlobalConfiguration.Configure(WebApiConfig.Register); // NEW 
    //WebApiConfig.Register(GlobalConfiguration.Configuration); // DEPRECATED

    // Default 
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}
```

# Usage
```cs
// -> ta có thể xây dựng project với đồng thời MVC và WebAPI với ASP.NET MVC

// Web API
public class ValuesController : ApiController 
{
    // GET api/<controller>
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<controller>/5
    public string Get(int id)
    {
        return "value";
    }
}

// Web Page
public class HomeController : Controller
{
    public ActionResult Index()
    {
        ViewBag.Title = "Home page";
        return View();
    }
}
```

# Create fast Client for check API - WebAPI Help
* -> **`Install-Package Microsoft.AspNet.WebApi.HelpPage`**
* -> https://learn.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/creating-api-help-pages