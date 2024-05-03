# Initial project
* -> chọn Template ASP.NET Core Web App (Model-View-Controller) 
* -> với "Authentication type", thì thường ta sẽ chọn "Individual Accounts" (_Identity library will be added to this project_)
* nhưng hiện tại ta sẽ chọn "None" và sẽ import library and UI later

# Add Identity Library to project
* right-click vào project -> Add -> New Scaffolded Item -> chọn "Identity" -> Add
* nó sẽ hiện cho ta 1 window bao gồm 1 list những Razor pages của ASP.NET Core Identity mà ta muốn customize;
* ta sẽ tạm chọn "Login" và "Register"; những pages còn lại sẽ được load trực tiếp từ Identity UI library với default behaviors and features
* tiếp theo trong window đó, ta sẽ dùng "Select a Layout Page" để chọn Layout cho những views mà ta đã chọn: chọn Views/Shared/_Layout.cshtml -> OK
* tiếp theo chọn DbContext -> click vào "+" -> cho nó cái tên (_VD: AuthDbContext_) -> Add
* tiếp chọn a "model" class cho user -> click vào "+" -> cho nó cái tên (_VD: ApplicationUser_) -> Add

* => h nó sẽ installing required packages bao gồm:
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.VisualStudio.Web.CodeGeneration.Design

* => Microsoft.AspNetCore.Identity.UI is the main package added to UI; contains the Razor pages nêu trên

* => có thêm thư mục Areas/Identity/Pages với những Razor views for respective views "Login.cshtml" và "Register.cshtml"
* => có thêm thư mục Areas/Identity/Data với 2 model "ApplicationUser.cs" và "AuthDbContext.cs"
* => file "program.cs" sẽ có thay đổi đôi chút

# Configure Program.cs
* -> add support for Razor pages (_in order to view Razor pages "/Login" and "/Register"_)
```cs
// by default, newly created MVC project only have "builder.Services.AddControllersWithViews()" for supporting the conventional Controller with seperate Views
builder.Services.AddRazorPages()
```

* to add routes for Identity UI Razor pages into application:
```cs
// by default, we have the routes for the default conventional MVC Controller:
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// add routes from Identity Razor pages:
// all of the routes for Identity UI Razor pages are already configure inside 
app.MapRazorPages();
```

* -> _h ta thử start project, rồi truy cập ".../identity/Account/Login" để thấy "Login" page; truy cập ".../identity/Account/Register" để thấy "Register" page

# Customize UI
```cs - ta sẽ bỏ cả "Login" form và "Register" form inside a bootstrap
// -> inside the "Areas/Identity/Pages", create a layout page specific to both the pages "Login" and "Register" containing a Tab control
// -> right-click on folder, Add -> New Item -> chọn "Razor Layout" -> nhập "_AuthLayout.cshtml"

// _AuthLayout.cshtml
@{
    // we will use the "_Layout.chtml" as a layout for this newly created layout
    Layout="/Views/Shared/_Layout.cshtml";
}

// ->  set "_AuthLayout.cshtml" as layout for "/Login" and "/Register"

// Register.cshtml
@{
    ViewData["Title"] = "Register";
    Layout = "~/Areas/Identity/Pages/_AuthLayout.cshtml";
}
// Login.cshtml
@{
    ViewData["Title"] = "Log in";
    Layout = "~/Areas/Identity/Pages/_AuthLayout.cshtml";
}
```


# Lấy Resorces
```js - Nhấn button để gọi 1 AJAX request, kết quả trả về được render ra màn hình
<div class="mt-4">
    <button 
        id="callProtectedResourceButton" 
        class="btn btn-warning" 
        onclick="getDetailedWeatherForcast();"
    >
        Get detailed weather forcast (protected API)
    </button>
</div>
<div class="mt-4">
    <textarea id="message" class="form-control" rows="10"></textarea>
</div>

function renderData(text) {
    document.getElementById("message").value += text + '\n';
}
async function getFreeWeatherForcast() {
    const response = await fetch('/api/weatherforcast/v1/free');
    const forcast = await response.text();

    renderData(forcast);
    return false;
}
```

```cs - định nghĩa AJAX request - tạo 1 request có Access Token gọi đến Web API Resource Server
[Route("/api/weatherforcast/v1/detailed")]
[HttpGet]
public async Task<IActionResult> GetDetailedWeatherForcastAsync()
{
    var httpClient = httpClientFactory.CreateClient();
    // send access_token 
    var accessTokenClaim = User.Claims.Where(c => c.Type == "access_token").FirstOrDefault();
    if (accessTokenClaim != null)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenClaim.Value);
    }

    // gọi đến Web API
    var response = await httpClient.GetAsync("https://localhost:7102/weatherforcast/detailed");
    if (response != null && response.IsSuccessStatusCode)
    {
        return Json(JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync()));
    }
    else
    {
        return Content(response?.ReasonPhrase ?? "Error");
    }
}
```