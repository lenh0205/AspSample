==================================================================
# Role-based authorization in ASP.NET Core

## Roles
* -> when **`an identity is created`** it may belong to one or more **roles**
* -> how these **roles are created and managed** depends on the **`backing store of the authorization process`**

## Code
* -> **Roles are exposed to the developer** through the **`.IsInRole`** method on the **`ClaimsPrincipal`** class
* -> for configuration, **`.AddRoles`** must be added to **Role services**

```r - For example:
// Tracy may belong to the "Administrator" and "User" roles 
// while Scott may only belong to the "User" role.
```

## Roles - Claims
* -> while **roles** are **`claims`**, not all claims are roles
* _depending on the identity issuer a role may be a `collection of users that may apply claims for group members`, as well as an `actual claim on an identity`_
* _tức là sử dụng 1 role cho 1 nhóm các users; hoặc mỗi user sẽ sở hữu 1 list roles_

* -> however, **claims** are meant to be **`information about an individual user`**
* -> **using roles to add claims to a user** can **`confuse the boundary between the user and their individual claims`**
* _tức là không biết được những claim ví dụ như `CanDelete`, `CanEdit` là thuộc về user hay được thêm bởi role_

## SPA 
* -> the confusion above is why the **SPA templates** are **`not designed around roles`**

* -> in addition, for **organizations migrating from an on-premises legacy system**; 
* -> the proliferation of roles over the years can mean **`a role claim may be too large to be contained within a token`** usable by SPAs

==================================================================
# Configuration - Add Role services to Identity
* -> **register `role-based authorization services`** in Program.cs by calling **`AddRoles`** with the **`role type`** in the **app's Identity configuration**

```cs - Ex: the role type "IdentityRole"
builder.Services.AddDefaultIdentity<IdentityUser>( ... )
    .AddRoles<IdentityRole>()
    ...
```

==================================================================
# Adding role checks - Role based authorization checks
* -> are **declarative** and **`specify roles which the current user must be a member of`** to access the requested resource

* -> are applied to **Razor Pages**, **controllers**, or **actions** within a controller.
* -> can not be applied at the **Razor Page handler level**, they must be applied to the Page

```cs - limits access to any actions on the "AdministrationController"
[Authorize(Roles = "Administrator")] // to users who are a member of the "Administrator" role
public class AdministrationController : Controller
{
    public IActionResult Index() =>
        Content("Administrator");
}
```

## multiple roles
* -> can be specified as **a comma separated list**:
```cs
// accessible by users who are members of the "HRManager" role or the "Finance" role
[Authorize(Roles = "HRManager,Finance")]
public class SalaryController : Controller
{
    public IActionResult Payslip() => Content("HRManager || Finance");
}
```

## mutiple attributes
* -> require **an accessing user** must be **`a member of all the roles specified`**
```cs
//  requires that a user must be a member of both the "PowerUser" and "ControlPanelUser" role
[Authorize(Roles = "PowerUser")]
[Authorize(Roles = "ControlPanelUser")]
public class ControlPanelController : Controller
{
    public IActionResult Index() =>
        Content("PowerUser && ControlPanelUser");
}
```

## Limit access to action
* -> by applying **`additional role` authorization attributes** at the **action level**

```cs
// members of the "Administrator" role or the "PowerUser" role can access the controller and the SetTime action
[Authorize(Roles = "Administrator, PowerUser")]
public class ControlAllPanelController : Controller
{
    public IActionResult SetTime() => Content("Administrator || PowerUser");

    // only members of the "Administrator" role can access the ShutDown action
    [Authorize(Roles = "Administrator")] 
    public IActionResult ShutDown() => Content("Administrator only");
}
```

## Allow Anonymous
* -> **a controller can be secured** but **`allow anonymous, unauthenticated access`** to **individual actions**:
```cs
[Authorize]
public class Control3PanelController : Controller
{
    public IActionResult SetTime() =>
        Content("[Authorize]");

    [AllowAnonymous] // this one
    public IActionResult Login() =>
        Content("[AllowAnonymous]");
}
```

## Razor pages
* ->  **[Authorize]** can be applied by either **`using a 'convention'`**, or **`applying the [Authorize] to the PageModel instance`**
* _can only be applied to PageModel and cannot be applied to specific page handler methods_

```cs
[Authorize(Policy = "RequireAdministratorRole")]
public class UpdateModel : PageModel
{
    public IActionResult OnPost() =>
         Content("OnPost RequireAdministratorRole");
}
```

==================================================================
# Policy based role checks
* ->  **Role requirements** can also be expressed using the **`Policy syntax`** 
* -> where a developer **`registers a policy`** at application startup as part of the **`Authorization service configuration`**

```cs - Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    // authorizes users who belong to the "Administrator", "PowerUser" or "BackupAdministrator" roles
    options.AddPolicy("ElevatedRights", policy =>
          policy.RequireRole("Administrator", "PowerUser", "BackupAdministrator"));
});

var app = builder.Build();
```

## Apply "policy"
* -> using the **`Policy`** property on the **[Authorize] attribute**

```cs
[Authorize(Policy = "RequireAdministratorRole")]
public IActionResult Shutdown()
{
    return View();
}
```

