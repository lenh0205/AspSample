============================================================================
# Secure user data
* -> **`all the major steps to create the secure user data app`**

## Tie the contact data to the user
* -> use **the ASP.NET Identity `user ID`** to **ensure users can edit their data**, but **`not other users data`**
* -> add **`OwnerID`** and **`ContactStatus`** to the **Contact** model
* -> **OwnerID** is the **`user's ID from the 'AspNetUser' table in the 'Identity' database`**
* -> the **Status** field determines if **`a contact is viewable by general users`**

```cs - "Contact" model
public class Contact
{
    public int ContactId { get; set; }

    public string? OwnerID { get; set; } // this one

    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public ContactStatus Status { get; set; } // this one
}

public enum ContactStatus
{
    Submitted,
    Approved,
    Rejected
}
```

```bash - create a new migration and update the database:
dotnet ef migrations add userID_Status
dotnet ef database update 
```

## Add Role services to Identity
* -> append **`AddRoles`** to add **`Role services`**:
```cs
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // this one
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

============================================================================
# Require authenticated users

## using 'AuthorizationOptions.FallbackPolicy'
* -> set the **`fallback authorization policy`** to **`requires all users to be authenticated`**, except for **Razor Pages**, **controllers**, or **action methods** with **`an authorization attribute`**
* -> **`.RequireAuthenticatedUser`** adds **DenyAnonymousAuthorizationRequirement** to the current instance, which **`enforces that the current user is authenticated`**

* _Ex: Razor Pages, controllers, or action methods with [AllowAnonymous] or [Authorize(PolicyName="MyPolicy")] use the **applied authorization attribute** rather than the fallback authorization policy_

* _is **`applied to all requests`** that **don't explicitly specify an authorization policy**_
* -> for **requests served by endpoint routing**, this includes **`any endpoint that doesn't specify an authorization attribute`**
* -> for **requests served by other middleware after the authorization middleware** (_such as **`static files`**_), this applies the policy to **`all requests`**

* -> setting the _fallback authorization policy_ to **require users to be authenticated** **`protects newly added Razor Pages and controllers`**
* -> having **authorization required by default** is **`more secure than`** relying on new controllers and Razor Pages to **include the [Authorize] attribute**

* -> the **AuthorizationOptions** class also contains **`AuthorizationOptions.DefaultPolicy`**
* -> the **DefaultPolicy** is **`the policy used with the [Authorize] attribute when no policy is specified`**
* _[Authorize] doesn't contain a named policy, unlike [Authorize(PolicyName="MyPolicy")]_

```cs
// ....

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// sets the "fallback authorization policy"
builder.Services.AddAuthorization(options => // this one
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

## using 'authorization filter' 
* -> an **`alternative way` for MVC controllers and Razor Pages to `require all users be authenticated`** is adding **`an authorization filter`**:
```cs
builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// uses an authorization filter, setting the fallback policy uses endpoint routing
builder.Services.AddControllers(config => // this one
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();
```

## [AllowAnonymous] for not requiring authenticated users
* -> add **`AllowAnonymous`** to the **Index** and **Privacy** pages so **`anonymous users can get information about the site before they register`**:
```cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactManager.Pages;

[AllowAnonymous] // this one
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
```
