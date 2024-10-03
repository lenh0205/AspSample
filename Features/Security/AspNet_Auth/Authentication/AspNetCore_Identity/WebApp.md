> ASP.NET Core Identity sử dụng **`cookie-based authentication`** by default - rất phù hợp với **Web Application**
> vậy nên nó sẽ cấu hình sẵn những option mặc định về **Identity** và **cookie** ta có thể ghi đè bằng **`builder.Services.Configure<IdentityOptions>()`** và **`builder.Services.ConfigureApplicationCookie()`**

=====================================================================
# Create a Web app with authentication

## Init project
* -> ta sẽ khởi tạo project sử dụng **template** là **`ASP.NET Core Web App (Model-Controller-View)`** và select **Authentication type** là **`Individual Accounts`** (_để add ASP.NET Core Identity_) 
* -> ASP.NET Core Identity will be provided as **`a Razor class library`**

## Endpoint
* -> the Identity Razor class library exposes endpoints with the **`Identity`** area
* -> include: **/Identity/Account/Login**, **/Identity/Account/Logout**, **/Identity/Account/Manage**, ....
* -> _nhưng những trang mặc định này sẽ bị không hiển thị trực tiếp trong project, để ghi đè trang ta muốn thì xem phần **`Identity Scaffold`** bên dưới_ 

## Apply migrations
* -> trong thư mục **Data/Migrations** đã có sẵn 1 số migrations; ta chỉ cần **`Update-Database`**
* -> sau đó ta có thể view database để xem các bảng **`AspNetRoles`**, **`AspNetUsers`**, **`AspNetRoleClaims`**, **`AspNetUserClaims`**, **`AspNetUserLogins`**, **`AspNetUserRoles`**, **`AspNetUserTokens`**

* -> giờ ta thử run the app and `register a user`, ta sẽ thấy bảng **AspNetUsers** được thêm vào 1 dòng

## Configure Identity services
* -> calling **`AddDefaultIdentity`** is similar to calling the **`AddIdentity`**, **`AddDefaultUI`**, **`AddDefaultTokenProviders`**
* -> "Identity" is enabled by calling **UseAuthentication** - add authentication middleware to the request pipeline
* -> **UseRouting**, **UseAuthentication**, and **UseAuthorization** must be **`called in the order`**

```cs
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// ta có thể tách code ra như này hoặc config trực tiếp trong .AddIdentity()
// this is the default option of Identity (tức là ta không cấu hình thì mặc định Identity đã cấu hình vậy rồi)
// the "builder.Services.Configure" will DI a "Singleton" instance of "IdentityOptions" and allow to config it in here
// this service will be used internally by Identity to store configuration options for identity system
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

// "ConfigureApplicationCookie" is an extension of ASP.NET Core Identity to config cookie option
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// .....

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
```

## Usage:
```cs
[Authorize]
public class HomeController : Controller
{
}
```

=====================================================================
> scaffold lại những page của Identity nếu ta muốn custom chúng

# Scaffold Register, Login, LogOut, and RegisterConfirmation
* -> ta sẽ ghi đề lại các page **Register, Login, LogOut, and RegisterConfirmation** sử dụng **`Identity Scaffolder`** (_xem `Features/Security/AspNet_Auth/AspNet_Core_Identity/Scaffold_Identity.md` để hiểu_)

## Register
* -> when a user clicks the _"Register" button_ on the _Register page_, the **`RegisterModel.OnPostAsync`** action is invoked
* -> đầu tiên từ những form Input (_ngoại trừ `password`_) ta tạo 1 object **`IdentityUser`**
* -> rồi pass **IdentityUser** này cùng **password** vào **`UserManager.CreateAsync()`** để create **user** đó trong backing store (_database mà chứa mấy cái bảng của ASP.NET Core Identity như "AspNetUsers",..._)
* -> nếu success thì ta sẽ dùng **UserManager** để lấy **`userId`** và **`email confirm token`** của **user** đó, rồi sử dụng làm **query param** để tạo đường dẫn với enpoint là **`/Account/ConfirmEmail`**
* -> rồi gửi **email** theo địa chỉ user đã nhập với nội dung là 1 lời nhắn là "please confirm your account by clicking" **đường dẫn ta vừa tạo**
* -> đồng thời nó sẽ redirect ta tới page **`/Account/RegisterConfirmation`** với 2 query params là **email** và **returnUrl**

## Disable default account verification
* -> user is redirected to the **RegisterConfirmation.OnGetAsync** where they can **`select a link to have the account confirmed`** (_tạo bởi EmailConfirmationUrl = Url.Page("/Account/ConfirmEmail", .....)_)
* -> nhưng ta sẽ cần bỏ cái link này đi; nó chỉ dùng cho testing vì nó là automatic account verification 
* -> trên production, it require a confirmed account and prevent immediate login at registration
* _tức là nếu ta dùng cái link được tạo mặc định thì sau khi `Register` ta `Login` thì nó sẽ success ngày; nếu không có cái link mà ta `Login` ngay thì nó sẽ báo lỗi_

```cs
DisplayConfirmAccountLink = false;
```

## Log in
* -> when the form on the Login page is submitted, the **`OnPostAsync`** action is called
* -> nó sẽ chạy **`_signInManager.PasswordSignInAsync`** để login với **email** và **password**; kết quả sẽ có 3 trường hợp
* -> case 1 **`result.Succeeded`**, nó sẽ redirect tới **returnUrl**
* -> case 2 **`result.RequiresTwoFactor`**, redirect tới trang **/LoginWith2fa**
* -> case 3 **`result.IsLockedOut`**, redirect tới trang **/Lockout**

## Log out
* -> the Log out link invokes the **LogoutModel.OnPost** action
* -> nó sẽ chạy **`_signInManager.SignOutAsync()`** để **`clear the user's claims stored in a cookie`**
* -> ta sẽ cần return 1 **redirect action** để browser có thể performs a new request and the **`identity for the user gets updated`**

## Test Identity
* -> giờ ta thử log out ra rồi thêm **`[Authorize]`** attribute vào "Privacy" page, thì khi ta truy cập trang "Privacy" nó sẽ redirect ta tới trang **`Login`**
* -> sau khi ta đăng nhập thì ta có thể vào lại trang "Privacy"

## Prevent publish of static Identity assets
* -> để cho an toàn thì ta có thể prevent publishing static **`Identity assets`** (stylesheets and JavaScript files for Identity UI) **`to the web root`** 
* -> bằng cách add **ResolveStaticWebAssetsInputsDependsOn** property and **RemoveIdentityAssets** target to the app's project file

```xml
<PropertyGroup>
  <ResolveStaticWebAssetsInputsDependsOn>RemoveIdentityAssets</ResolveStaticWebAssetsInputsDependsOn>
</PropertyGroup>

<Target Name="RemoveIdentityAssets">
  <ItemGroup>
    <StaticWebAsset Remove="@(StaticWebAsset)" Condition="%(SourceId) == 'Microsoft.AspNetCore.Identity.UI'" />
  </ItemGroup>
</Target>
```