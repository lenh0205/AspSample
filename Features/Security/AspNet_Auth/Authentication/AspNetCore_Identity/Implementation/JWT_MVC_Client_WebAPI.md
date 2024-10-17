> project này là MVC nhưng tách biệt thành 2 phần là Client và Web API

===================================================================
# Initial project
* -> tạo project với **`ASP.NET Core Web App (Model-View-Controller)`** template không khởi tạo sẵn với **ASP.NET Core Identity**
* -> sau đó ta sẽ bắt đầu **Scaffold Identity** (xem `~\Features\Security\AspNet_Auth\Authentication\AspNetCore_Identity\Custom\Scaffold_Identity.md`)

* -> configure **Program.cs**
```cs
// add support for Razor pages of Identity (like "/Login" and "/Register")
// by default, newly created MVC project only have "builder.Services.AddControllersWithViews()" for supporting the conventional Controller with seperate Views
builder.Services.AddRazorPages()

// add routes for Identity UI Razor pages into application:
// by default, we have the routes for the default conventional MVC Controller:
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// add routes from Identity Razor pages:
// all of the routes for Identity UI Razor pages are already configure inside 
app.MapRazorPages();
```

===================================================================
# Customize UI
```cs - Tạo "_AuthLayout.cshtml" để bỏ cả "Login" form và "Register" form inside a bootstrap
// -> inside the "Areas/Identity/Pages", create a layout page specific to both the pages "Login" and "Register" containing a Tab control
// -> right-click on folder, Add -> New Item -> chọn "Razor Layout" -> nhập "_AuthLayout.cshtml"

// _AuthLayout.cshtml
@{
    // we will use the "_Layout.chtml" as a layout for this newly created layout
    Layout="/Views/Shared/_Layout.cshtml";
}

// ->  set "_AuthLayout.cshtml" as layout for "/Login"
// Register.cshtml
@{
    ViewData["Title"] = "Register";
    Layout = "~/Areas/Identity/Pages/_AuthLayout.cshtml";
}

// ->  set "_AuthLayout.cshtml" as layout for "/Register"
// Login.cshtml
@{
    ViewData["Title"] = "Log in";
    Layout = "~/Areas/Identity/Pages/_AuthLayout.cshtml";
}
```

===================================================================
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