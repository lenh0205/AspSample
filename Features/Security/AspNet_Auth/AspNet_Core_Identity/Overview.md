> introduce: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio
> demo: https://learn.microsoft.com/en-us/training/modules/secure-aspnet-core-identity/

=====================================================================
# Identity on ASP.NET Core - ASP.NET Core Identity
* -> is the **`membership system`** for building ASP.NET Core web applications, including **membership**, **login**, and **user data**
* -> is **`an API`** that **supports user interface (UI) login functionality**
* -> **`manages users, passwords, profile data, roles, claims, tokens, email confirmation, ...`**

* -> **Identity (ASP.NET Core Identity)** is typically configured using a **`SQL Server database`** to store **`user names, passwords, and profile data`**
* -> _ASP.NET Core Identity_ **`adds user interface (UI) login functionality to ASP.NET Core web apps`**

* _to secure **web APIs and SPAs**, use one of these options: **`Duende Identity Server`**, **`Microsoft Entra ID`**, **`Azure Active Directory B2C`**_

=====================================================================
# Create a Web app with authentication

## Init project
* -> ta sẽ khởi tạo project sử dụng **template** là **`ASP.NET Core Web App (Model-Controller-View)`** và select **Authentication type** là **`Individual Accounts`** (_để add ASP.NET Core Identity_) 
* -> this project will provide ASP.NET Core Identity as **`a Razor class library`**

## Endpoint
* -> the Identity Razor class library exposes endpoints with the **`Identity`** area
* -> include: **/Identity/Account/Login**, **/Identity/Account/Logout**, **/Identity/Account/Manage**, ....
* -> _nhưng những trang mặc định này sẽ bị không hiển thị trực tiếp trong project, để ghi đè trang ta muốn thì xem phần **`Identity Scaffold`** bên dưới_ 

## Apply migrations
* -> trong thư mục **Data/Migrations** đã có sẵn 1 số migrations; ta chỉ cần **`Update-Database`**
* -> sau đó ta có thể view database để xem các bảng **`AspNetRoles`**, **`AspNetUsers`**, **`AspNetRoleClaims`**, **`AspNetUserClaims`**, **`AspNetUserLogins`**, **`AspNetUserRoles`**, **`AspNetUserTokens`**

* -> giờ ta thử run the app and `register a user`, ta sẽ thấy bảng **AspNetUsers** được thêm vào 1 dòng

## Configure Identity services
* -> "Identity" is enabled by calling **UseAuthentication** - add authentication middleware to the request pipeline
* -> **UseRouting**, **UseAuthentication**, and **UseAuthorization** must be **`called in the order`**

```cs
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// default option
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

## Scaffold Register, Login, LogOut, and RegisterConfirmation
* -> hiện tại các view như **`Register, Login, LogOut, and RegisterConfirmation`** ta đều đang sử dụng mặc định; để ghi đè nó thì ta sẽ sử dụng **Identity Scaffold** 
* -> để nó làm hiện ra những view mặc định này trong project của chúng ta (vì bình thường khi mới khởi tạo project chúng sẽ bị giấu đi), từ đó ta có thể sửa code trực tiếp trong những view này
* -> trước tiên để **`scaffold item`** ta cần install the **`Microsoft.VisualStudio.Web.CodeGeneration.Design`** NuGet package
* -> **Process**: right-click on the project > Add > New Scaffolded Item > chọn Identity > chọn Identity > rồi click "Add" và chờ vài giây > chọn view mà ta muốn override (hoặc có option để override tất cả)
* -> ngoài ra ta cần chọn **DbContext** để có thể Scaffold