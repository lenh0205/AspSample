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
* -> include: **/Identity/Account/Login**, **/Identity/Account/Logout**, **/Identity/Account/Manage**

## Apply migrations
* -> trong thư mục **Data/Migrations** đã có sẵn 1 số migrations; ta chỉ cần **`Update-Database`**
* -> sau đó ta có thể view database để xem các bảng **`AspNetRoles`**, **`AspNetUsers`**, **`AspNetRoleClaims`**, **`AspNetUserClaims`**, **`AspNetUserLogins`**, **`AspNetUserRoles`**, **`AspNetUserTokens`**

* -> giờ ta thử run the app and `register a user`, ta sẽ thấy bảng **AspNetUsers** được thêm vào 1 dòng

## Configure Identity services
* -> **UseRouting**, **UseAuthentication**, and **UseAuthorization** must be **`called in the order`**

```cs
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// .....

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
```

## Scaffold Register, Login, LogOut, and RegisterConfirmation