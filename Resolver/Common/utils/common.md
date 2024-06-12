===================================================================
# Parse an unvalid DateTime to valid DateTime
```cs
string date = "20121004";

string result = DateTime
                  .ParseExact(date, "yyyyMMdd",CultureInfo.InvariantCulture)
                  .ToString("yyyy-MM-dd");
```

===================================================================
# check if 'file' or 'directory' exist

```c# - 
// check đường dẫn đến thư mục chứa file xem có chưa để tạo trước khi bỏ file vô
if (!Directory.Exists(pathToSaveSatus)) Directory.CreateDirectory(pathToSaveSatus);

// kiểm trả đường dẫn tới file có tồn tại không
File.Exists(path)
```

# indicate a method in a controller is "not an action method" ?
```cs
// using "NonAction" attribute
[NonAction]
public IActionResult UnprocessedEntityResult() {
    return StatusCode(StatusCodes.Status422UnprocessableEntity);
}

// or using "protected" - visible to derived types but wont confuse the route table
protected IActionResult UnprocessedEntityResult() {
    return StatusCode(StatusCodes.Status422UnprocessableEntity);
}
```

===================================================================
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

===================================================================
# Handle null value in "Deserialize"
```cs
var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

var jsonModel = JsonConvert.DeserializeObject<Customer>(jsonString, settings);
```

```cs
public class Response
{
    public string Status;

    public string ErrorCode;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ErrorMessage;
}


var response = JsonConvert.DeserializeObject<Response>(data);
```

===================================================================
# Check if an Exception is of a particular type
```cs
catch(DbUpdateException ex)
{
  if(ex.InnerException is UpdateException)
  {
    // do what you want with ex.InnerException...
  }
}
catch(Exception ex)
{
  if(ex is DbUpdateException)
  {
    // do what you want with ex.InnerException...
  }
}

// In C# 6
catch(DbUpdateException ex) when (ex.InnerException is UpdateException)
{
    // do what you want with ex.InnerException...
}
```

===================================================================
# Define an Enum
```cs
/// <summary>
/// EMyEnums.cs
/// </summary>
public enum EMyEnums
{
    All = 0,
    ChoXuLy = 1,
    DaXuLy = 2,
    SapDenHan = 3,
    QuaHan = 4,
}
```