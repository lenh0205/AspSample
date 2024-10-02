> để check Authorize thì ta "thường" gọi **isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Create);**
> đầu tiên nó sẽ tìm những handlers có khả năng handle specified **requirements** ta đang cần trong những **`builder.Services.AddScoped<IAuthorizationHandler, ....>`** mà ta đã đăng ký
> tất cả handlers sẽ được gọi và gọi theo thứ tự; 
> nó sẽ gọi đến method **`HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Contact resource)`** của **AuthorizationHandler** đó
> ta sẽ dùng **`AuthorizationHandlerContext context`** để biểu thị kết quả xử lý của handler bằng cách gọi **context.Succeed(requirement);**, **context.Fail(requirement);**, **return Task.CompletedTask;**
> đồng thời **AuthorizationHandlerContext context** còn cho ta truy cập **`ClaimPrincipal user`** được truyền từ **IAuthorizationService** bằng **`context.User`**

> ở đây ta sử dụng built-in **`OperationAuthorizationRequirement`** để tạo các constant **requirements** vì nó derive từ **`IAuthorizationRequirement`** và rất đơn giản để khởi tạo chỉ cần "Name" property là đủ

============================================================================
# Review the contact "operations requirements" class (định nghĩa các requirements)
* -> review the **ContactOperations** class - this class contains the **`requirements`** the app supports:
```cs
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ContactManager.Authorization
{
    public static class ContactOperations
    {
        public static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name=Constants.CreateOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=Constants.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=Constants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=Constants.DeleteOperationName};
        public static OperationAuthorizationRequirement Approve = 
          new OperationAuthorizationRequirement {Name=Constants.ApproveOperationName};
        public static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=Constants.RejectOperationName};
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string ContactAdministratorsRole = "ContactAdministrators";
        public static readonly string ContactManagersRole = "ContactManagers";
    }
}
```

============================================================================
# Create owner, manager, and administrator authorization handlers
* -> create a **ContactIsOwnerAuthorizationHandler** class in the **Authorization folder**
* -> the **ContactIsOwnerAuthorizationHandler** **`verifies`** that the **`"user acting on a resource" owns the resource`**
* -> the **ContactIsOwnerAuthorizationHandler** calls **`context.Succeed`** if **`the "current authenticated user" is the "contact owner"`**

* _**Authorization handlers** generally:_
* -> call **`context.Succeed`** when the **requirements are met**
* -> return **`Task.CompletedTask`** when **requirements aren't met**

* _**returning `Task.CompletedTask` without a prior call to `context.Success` or `context.Fail`** it **`allows other authorization handlers to run`**
* _If we need to explicitly fail, call context.Fail_

## Create a "Contact Owner" authorization handler
* -> the app **allows contact owners to edit/delete/create their own data**; 
* -> _ContactIsOwnerAuthorizationHandler_ **doesn't need to check the `operation` passed in the `requirement` parameter**

```cs
namespace ContactManager.Authorization
{
    public class ContactIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        UserManager<IdentityUser> _userManager;

        public ContactIsOwnerAuthorizationHandler(UserManager<IdentityUser> 
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
                                    AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement,
                                    Contact resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName 
                && requirement.Name != Constants.ReadOperationName  
                && requirement.Name != Constants.UpdateOperationName 
                && requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
```

## Create a "manager" authorization handler
* -> create a **ContactManagerAuthorizationHandler** class in the **Authorization folder**
* -> the **ContactManagerAuthorizationHandler** **`verifies the user acting on the resource is a "manager"`**
* -> only managers can **`approve`** or **`reject`** content changes (new or changed)
```cs
namespace ContactManager.Authorization
{
    public class ContactManagerAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Contact resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can approve or reject.
            if (context.User.IsInRole(Constants.ContactManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
```

## Create an "administrator" authorization handler
* -> create a **`ContactAdministratorsAuthorizationHandler`** class in the **Authorization folder**
* -> the **ContactAdministratorsAuthorizationHandler** **`verifies the user acting on the resource is an "administrator"`** 
* -> _Administrator can do all operations_
```cs
namespace ContactManager.Authorization
{
    public class ContactAdministratorsAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement, 
                                     Contact resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(Constants.ContactAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
```

============================================================================
# Register the authorization handlers
* -> Services using **`Entity Framework Core`** must be registered for **dependency injection** using **`AddScoped`**
* -> the **ContactIsOwnerAuthorizationHandler** uses **`ASP.NET Core Identity`**, which is **built on Entity Framework Core**
* -> **register the handlers with the service collection** so they're **`available to the "ContactsController"`** through dependency injection

* -> **ContactAdministratorsAuthorizationHandler** and **ContactManagerAuthorizationHandler** are added as **`singletons`** because **they don't use EF** 
* -> **all the information needed is already in the `Context` parameter** of the HandleRequirementAsync method

```cs - ConfigureServices:
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Authorization handlers.
builder.Services.AddScoped<IAuthorizationHandler, // this one
                      ContactIsOwnerAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler, // this one
                      ContactAdministratorsAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler, // this one
                      ContactManagerAuthorizationHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    // requires using Microsoft.Extensions.Configuration;
    // Set password with the Secret Manager tool.
    // dotnet user-secrets set SeedUserPW <pw>

    var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");

   await SeedData.Initialize(services, testUserPw);
}
```
