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
# using @ before string
* -> to **`interpret the string literally`**
* _nói chung là string của ta thế nào thì nó sẽ interpret như vậy không cần xử lý những **escape sequences**_

```cs
var path = @"\\servername\share\folder"; 
// instead of:
var path = "\\\\servername\\share\\folder";
```

===================================================================
# Add MVC to existing Web API project
```cs
public void ConfigureServices(IServiceCollection services)
{
    //services.AddControllers(); // for Web API
    services.AddControllersWithViews();
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection(); // not require for MVC enable, but for security of web app

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        //endpoints.MapControllers();
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");             
    });
}
```

===================================================================
# Tạo 1 IEnumerable<int> trong khoảng 2 con số x và y

```cs
var xRange = Enumerable.Range(0, 3); // sẽ bao gồm (0, 1, 2)
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
===================================================================
# Tạo "mảng" từ "string" và tạo "string" từ "mảng"
```cs
var str = "The quick brown fox jumps over the lazy dog";

// nếu không truyền tham số thì mặc định nó sẽ split() bằng " "
var result1 = str.Split(); // ["The", "quick", "brown", "fox", "jumps","over", "the", "lazy", "dog"]
var result2 = str.Split(" "); // ["The", "quick", "brown", "fox", "jumps","over", "the", "lazy", "dog"]

var str = "Davis, Clyne, Fonte";
string[] arr = str.Split(", ");
var str2 = string.Join(", "); // Davis, Clyne, Fonte
```

# Cắt string
```cs
string text = "Hello, World!";

// extracts a portion of a string based on "start index" and "length"
string cut = text.Substring(7, 5); // "World"
string cut = text.AsSpan(7, 5).ToString(); // the same as ".Substring()" but works better with large strings in performance-critical (avoiding extra string allocations) applications

// extract from "start index" to "end index"
string cut = text[7..12]; // "World"
string cut = text[..5]; // "Hello"
string lastPart = text[7..]; // "World!"

// removes a portion of a string starting from an index
string cut = text.Remove(5); // "Hello"
```

===================================================================
# get time span between 2 timeline contain minutes and seconds

```cs
public IActionResult LayTungKhoangThoiGian()
{
    var str = "00:45,01:32,02:18,03:01,03:44,04:31,05:19,06:01,06:47,07:35";
    var arr = str.Split(",");

    var result = arr.Select((item, index) =>
    {
        var currentTime = DateTime.ParseExact(item, "mm:ss", CultureInfo.InvariantCulture);
        var previousTime = DateTime.ParseExact(index == 0 ? "00:00" : arr[index - 1], "mm:ss", CultureInfo.InvariantCulture);
        var timeSpan = currentTime - previousTime;
        return timeSpan.TotalSeconds;
    });

    return Ok(string.Join(", ", result));
}
```

===================================================================
# Caculate "Age" of a person base on birthdate

```cs
public static int GetAge(this DateTime dateOfBirth)
{
    DateTime today = DateTime.Today;
    int age = today.Year - dateOfBirth.Year;
    if (dateOfBirth > today.AddYears(-age)) age--;
    return age;
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
# get MIME type of an image

```cs
Image imgStrm = System.Drawing.Image.FromStream(stream);
var mimeType = ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == imgStrm.RawFormat.Guid).MimeType;

// hoặc
string extension = Path.GetExtension(myFileStream.Name);
```
