> để Authorize được thì ta cần add role service (**`.AddRole`**) vào Identity (**.AddIdentity**)
> khi cấu hình **.AddAuthorization** middleware, ta cần nên thêm option **AuthorizationOptions.FallbackPolicy** với **`.equireAuthenticatedUser()`** để require authenticated user cho mọi request
> và sử dụng **`[AllowAnonymous] attribute`** cho những request không yêu cầu authenticated

============================================================================
# Authorization in ASP.NET Core
* -> **Authorization** is **`orthogonal and independent`** from **Authentication**. However, **authorization** requires **`an authentication mechanism`**
* -> **ASP.NET Core authorization** provides a simple, declarative **`role`** and a rich **`policy-based`** model

* -> Authorization is expressed in **`requirements`**, and **`handlers evaluate a user's claims`** against **requirements**
* -> **`imperative checks` can be based on** **`simple policies`** or **`policies which evaluate both the user identity and properties of the resource`** that the user is attempting to access

* -> Authorization components, including the **`AuthorizeAttribute`** and **`AllowAnonymousAttribute`** attributes, are found in the **Microsoft.AspNetCore.Authorization** namespace

============================================================================
# Create an ASP.NET Core web app with user data protected by authorization
* -> create an **ASP.NET Core web app** with **`user data protected by authorization`**
* -> it displays **a list of contacts that authenticated (registered) users have created**

* _there are 3 security groups:_
* -> **`Registered users`** can **view all the approved data** and **can edit/delete their own data**
* -> **`Managers`** can **approve or reject contact data**; only **approved contacts are visible to users**
* -> **`Administrators`** can **approve/reject and edit/delete any data**

```r - in the Example:
//  user "Rick" (rick@example.com) is signed in
// in the "/Contacts" page of Rick, it display a list of contacts; only the record created by Rick displays "Edit" and "Delete" button 
// all records will have "Detail" button but when go to "Contacts/Detail" there is no "Approve" or "Reject" button
// Rick also have a "Create" button to create a new record
// a user can only see contact records of other users until a "manager" or "administrator" changes the status to "Approved"

// for "manager" role account (manager@contoso.com) signed in
// each record only have "Detail" button so "manager" cannot "Edit" or "Delete" record; 
// they will go to "Contacts/Detail" page to "Approve" or "Reject" record

// for "administrator" role account (admin@contoso.com) signed in
// have all privileges - can read, edit, or delete any contact and change the status of contacts
```

## Sample App
* -> the app will follow **Contact** model:
```cs
public class Contact
{
    public int ContactId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
```

* _contains the following **authorization handlers**:_
* -> **`ContactIsOwnerAuthorizationHandler`** - ensures that **a user can only edit their data**
* -> **`ContactManagerAuthorizationHandler`** - allows **managers to approve or reject contacts**
* -> **`ContactAdministratorsAuthorizationHandler`** - allows **administrators to approve or reject contacts and to edit/delete contacts**

============================================================================
# The starter and completed app
* starter app: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authorization/secure-data/samples/starter6
* completed app: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authorization/secure-data/samples/final6 

* -> project sử dụng .NET 6, ASP.NET Core Identity; tạo sẵn model và page **Contact**
* -> ta chỉ cần chạy migration trước và start app lên là được

============================================================================
# Secure user data
* -> **`all the major steps to create the secure user data app`**

## Tie the contact data to the user
* -> use **the ASP.NET Identity `user ID`** to **ensure users can edit their data**, but **`not other users data`**
* -> add **`OwnerID`** and **`ContactStatus`** to the **Contact** model
* -> **OwnerID** is the **`user's ID from the 'AspNetUser' table in the 'Identity' database`**
* -> the **Status** field determines if **`a contact is viewable by general users`**

```cs
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

## Require authenticated users

### using 'AuthorizationOptions.FallbackPolicy'
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

### using 'authorization filter' 
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

### [AllowAnonymous] for not requiring authenticated users
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

## Configure the test account
* -> the **SeedData** class creates two accounts: **`administrator`** and **`manager`**
* -> use the **`Secret Manager tool`** to **set a password for these accounts**

* -> set the password from the **project directory** (the directory containing Program.cs)
* -> if a **weak password** is specified, an **`exception is thrown`** when **SeedData.Initialize** is called
```bash
dotnet user-secrets set SeedUserPW <PW>
```

* -> update the app to use the test password:
```cs
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
builder.Services.AddScoped<IAuthorizationHandler,
                      ContactIsOwnerAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      ContactAdministratorsAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      ContactManagerAuthorizationHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) // this one
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

## Create the test accounts and update the contacts
* -> update the **`Initialize`** method in the **SeedData** class to **`create the test accounts`**:
```cs
public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
{
    using (var context = new ApplicationDbContext(
        serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
    {
        // For sample purposes seed both with the same password.
        // Password is set with the following:
        // dotnet user-secrets set SeedUserPW <pw>
        // The admin user can do anything

        var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@contoso.com");
        await EnsureRole(serviceProvider, adminID, Constants.ContactAdministratorsRole);

        // allowed user can create and edit contacts that they create
        var managerID = await EnsureUser(serviceProvider, testUserPw, "manager@contoso.com");
        await EnsureRole(serviceProvider, managerID, Constants.ContactManagersRole);

        SeedDB(context, adminID);
    }
}

private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                            string testUserPw, string UserName)
{
    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

    var user = await userManager.FindByNameAsync(UserName);
    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = UserName,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, testUserPw);
    }

    if (user == null)
    {
        throw new Exception("The password is probably not strong enough!");
    }

    return user.Id;
}

private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                              string uid, string role)
{
    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

    if (roleManager == null)
    {
        throw new Exception("roleManager null");
    }

    IdentityResult IR;
    if (!await roleManager.RoleExistsAsync(role))
    {
        IR = await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

    //if (userManager == null)
    //{
    //    throw new Exception("userManager is null");
    //}

    var user = await userManager.FindByIdAsync(uid);

    if (user == null)
    {
        throw new Exception("The testUserPw password was probably not strong enough!");
    }

    IR = await userManager.AddToRoleAsync(user, role);

    return IR;
}
```

* -> add the **`administrator user ID`** and **`ContactStatus`** to the **contacts**
* -> make one of the contacts "Submitted" and one "Rejected"; add the "user ID" and "status" to all the contacts
* -> only one contact is shown:
```cs
public static void SeedDB(ApplicationDbContext context, string adminID)
{
    if (context.Contact.Any())
    {
        return;   // DB has been seeded
    }

    context.Contact.AddRange(
        new Contact
        {
            Name = "Debra Garcia",
            Address = "1234 Main St",
            City = "Redmond",
            State = "WA",
            Zip = "10999",
            Email = "debra@example.com",
            Status = ContactStatus.Approved, // this one
            OwnerID = adminID // this one
        },
    )
}
```

## Create owner, manager, and administrator authorization handlers
* -> create a **`ContactIsOwnerAuthorizationHandler`** class in the **Authorization folder**
* -> the **ContactIsOwnerAuthorizationHandler** **`verifies that the user acting on a resource owns the resource`**
* -> the **ContactIsOwnerAuthorizationHandler** calls **`context.Succeed`** if **`the current authenticated user is the contact owner`**

* -> the app **allows contact owners to edit/delete/create their own data**; _ContactIsOwnerAuthorizationHandler_ **`doesn't need to check the operation passed in the requirement parameter`**

* _**Authorization handlers** generally:_
* -> call **`context.Succeed`** when the **requirements are met**
* -> return **`Task.CompletedTask`** when **requirements aren't met**
* _**returning Task.CompletedTask without a prior call to `context.Success` or `context.Fail`** (_is not a success or failure_) it **`allows other authorization handlers to run`**
* _If you need to explicitly fail, call context.Fail_

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

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName   &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName )
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

## Create a manager authorization handler
* -> create a **`ContactManagerAuthorizationHandler`** class in the **Authorization folder**
* -> the **ContactManagerAuthorizationHandler** **`verifies the user acting on the resource is a "manager"`**
* -> only managers can "approve" or "reject" content changes (new or changed)
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

## Create an administrator authorization handler
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
* -> Services using **`Entity Framework Core`** must be registered for **dependency injection using AddScoped**
* -> the **ContactIsOwnerAuthorizationHandler** uses **`ASP.NET Core Identity`**, which is **built on Entity Framework Core**
* -> **register the handlers with the service collection** so they're **`available to the "ContactsController"`** through dependency injection

* -> **ContactAdministratorsAuthorizationHandler** and **ContactManagerAuthorizationHandler** are added as **`singletons`**
* -> they're singletons because **`they don't use EF`** and **`all the information needed is already in the 'Context' parameter`** of the HandleRequirementAsync method

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

============================================================================
# Support authorization

## Review the contact operations requirements class
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

        public static readonly string ContactAdministratorsRole = 
                                                              "ContactAdministrators";
        public static readonly string ContactManagersRole = "ContactManagers";
    }
}
```

## Create a base class for the Contacts Razor Pages
* -> create a base class that **`contains the services used in the contacts Razor Pages`**; the base class **`puts the initialization code in one location`**:
* __

```cs
namespace ContactManager.Pages.Contacts
{
    public class DI_BasePageModel : PageModel
    {
        // adds the IAuthorizationService service to access to the authorization handlers
        protected IAuthorizationService AuthorizationService { get; }
        protected ApplicationDbContext Context { get; } // add the ApplicationDbContext
        protected UserManager<IdentityUser> UserManager { get; } // adds the Identity UserManager service

        public DI_BasePageModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        } 
    }
}
```

## Update the CreateModel
* -> Update the **create page model**: 
* -> constructor to use the **`DI_BasePageModel`** base class, 
* -> **OnPostAsync** method to **`add the user ID to the Contact model`** and **call the authorization handler** to **`verify the user has permission to create contacts`**

```cs
namespace ContactManager.Pages.Contacts
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Contact.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Contact,
                                                        ContactOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Contact.Add(Contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
```

## Update the IndexModel
* -> update the **OnGetAsync** method so **`only approved contacts are shown to general users`**:
```cs
public class IndexModel : DI_BasePageModel
{
    public IndexModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
    {
    }

    public IList<Contact> Contact { get; set; }

    public async Task OnGetAsync()
    {
        var contacts = from c in Context.Contact
                       select c;

        var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                           User.IsInRole(Constants.ContactAdministratorsRole);

        var currentUserId = UserManager.GetUserId(User);

        // Only approved contacts are shown UNLESS you're authorized to see them
        // or you are the owner.
        if (!isAuthorized)
        {
            contacts = contacts.Where(c => c.Status == ContactStatus.Approved
                                        || c.OwnerID == currentUserId);
        }

        Contact = await contacts.ToListAsync();
    }
}
```

## Update the EditModel
* -> add **an authorization handler** to **`verify the user owns the contact`**
* -> because **resource authorization is being validated**, **`the [Authorize] attribute is not enough`**
* -> the app **`doesn't have access to the resource`** when **attributes are evaluated**

* -> **`resource-based authorization must be imperative`**
* -> **`checks must be performed`** once the **app has access to the resource**, either by **`loading it in the page model`** or by **`loading it within the handler itself`**
* -> we **frequently access the resource by passing in the resource key**

```cs
public class EditModel : DI_BasePageModel
{
    public EditModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
    {
    }

    [BindProperty]
    public Contact Contact { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contact? contact = await Context.Contact.FirstOrDefaultAsync(
                                                         m => m.ContactId == id);
        if (contact == null)
        {
            return NotFound();
        }

        Contact = contact;

        var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                  User, Contact,
                                                  ContactOperations.Update);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Fetch Contact from DB to get OwnerID.
        var contact = await Context
            .Contact.AsNoTracking()
            .FirstOrDefaultAsync(m => m.ContactId == id);

        if (contact == null)
        {
            return NotFound();
        }

        var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, contact,
                                                 ContactOperations.Update);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        Contact.OwnerID = contact.OwnerID;

        Context.Attach(Contact).State = EntityState.Modified;

        if (Contact.Status == ContactStatus.Approved)
        {
            // If the contact is updated after approval, 
            // and the user cannot approve,
            // set the status back to submitted so the update can be
            // checked and approved.
            var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                    Contact,
                                    ContactOperations.Approve);

            if (!canApprove.Succeeded)
            {
                Contact.Status = ContactStatus.Submitted;
            }
        }

        await Context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
```

## Update the DeleteModel
* -> update the delete page model to use the **authorization handler** to **`verify the user has delete permission on the contact`**
```cs
public class DeleteModel : DI_BasePageModel
{
    public DeleteModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
    {
    }

    [BindProperty]
    public Contact Contact { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contact? _contact = await Context.Contact.FirstOrDefaultAsync(
                                             m => m.ContactId == id);

        if (_contact == null)
        {
            return NotFound();
        }
        Contact = _contact;

        var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, Contact,
                                                 ContactOperations.Delete);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var contact = await Context
            .Contact.AsNoTracking()
            .FirstOrDefaultAsync(m => m.ContactId == id);

        if (contact == null)
        {
            return NotFound();
        }

        var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, contact,
                                                 ContactOperations.Delete);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        Context.Contact.Remove(contact);
        await Context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
```

============================================================================
# Inject the authorization service into the views
* -> _currently, the UI shows edit and delete links for contacts the user can't modify_
* -> **`inject the authorization service`** in the **Pages/_ViewImports.cshtml** file so it's **`available to all views`**:

```cs
@using Microsoft.AspNetCore.Identity
@using ContactManager
@using ContactManager.Data
@namespace ContactManager.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using ContactManager.Authorization; // this one
@using Microsoft.AspNetCore.Authorization // this one
@using ContactManager.Models // this one
@inject IAuthorizationService AuthorizationService // this one
```

* -> update the "Edit" and "Delete" links in _Pages/Contacts/Index.cshtml_ so they're **`only rendered for users with the appropriate permissions`**:
* -> _remember `hiding links` from users that don't have permission to change data **doesn't secure the app**. Hiding links makes the app more **user-friendly** by displaying only valid links_
```cs
@page
@model ContactManager.Pages.Contacts.IndexModel

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-page="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Name)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Address)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].City)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].State)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Zip)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Email)
            </th>
             <th> // this one
                @Html.DisplayNameFor(model => model.Contact[0].Status)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model.Contact) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Address)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.City)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.State)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Zip)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Email)
            </td>
                <td> // this one
                    @Html.DisplayFor(modelItem => item.Status)
                </td>
                <td>
                    @if ((await AuthorizationService.AuthorizeAsync(
                     User, item,
                     ContactOperations.Update)).Succeeded)
                    {
                        <a asp-page="./Edit" asp-route-id="@item.ContactId">Edit</a>
                        <text> | </text>
                    }

                    <a asp-page="./Details" asp-route-id="@item.ContactId">Details</a>

                    @if ((await AuthorizationService.AuthorizeAsync(
                     User, item,
                     ContactOperations.Delete)).Succeeded)
                    {
                        <text> | </text>
                        <a asp-page="./Delete" asp-route-id="@item.ContactId">Delete</a>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>
```

## Update Details
* -> update the details view so **`managers can approve or reject contacts`**:
```cs
@*Preceding markup omitted for brevity.*@
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Contact.Email)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Contact.Email)
        </dd>
    <dt>
            @Html.DisplayNameFor(model => model.Contact.Status)
        </dt>
        <dd>
            @Html.DisplayFor(model => model.Contact.Status)
        </dd>
    </dl>
</div>

@if (Model.Contact.Status != ContactStatus.Approved)
{
    @if ((await AuthorizationService.AuthorizeAsync(
     User, Model.Contact, ContactOperations.Approve)).Succeeded)
    {
        <form style="display:inline;" method="post">
            <input type="hidden" name="id" value="@Model.Contact.ContactId" />
            <input type="hidden" name="status" value="@ContactStatus.Approved" />
            <button type="submit" class="btn btn-xs btn-success">Approve</button>
        </form>
    }
}

@if (Model.Contact.Status != ContactStatus.Rejected)
{
    @if ((await AuthorizationService.AuthorizeAsync(
     User, Model.Contact, ContactOperations.Reject)).Succeeded)
    {
        <form style="display:inline;" method="post">
            <input type="hidden" name="id" value="@Model.Contact.ContactId" />
            <input type="hidden" name="status" value="@ContactStatus.Rejected" />
            <button type="submit" class="btn btn-xs btn-danger">Reject</button>
        </form>
    }
}

<div>
    @if ((await AuthorizationService.AuthorizeAsync(
         User, Model.Contact,
         ContactOperations.Update)).Succeeded)
    {
        <a asp-page="./Edit" asp-route-id="@Model.Contact.ContactId">Edit</a>
        <text> | </text>
    }
    <a asp-page="./Index">Back to List</a>
</div>
```

## Update the details page model

```cs
public class DetailsModel : DI_BasePageModel
{
    public DetailsModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
    {
    }

    public Contact Contact { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contact? _contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

        if (_contact == null)
        {
            return NotFound();
        }
        Contact = _contact;

        var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                           User.IsInRole(Constants.ContactAdministratorsRole);

        var currentUserId = UserManager.GetUserId(User);

        if (!isAuthorized
            && currentUserId != Contact.OwnerID
            && Contact.Status != ContactStatus.Approved)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, ContactStatus status)
    {
        var contact = await Context.Contact.FirstOrDefaultAsync(
                                                  m => m.ContactId == id);

        if (contact == null)
        {
            return NotFound();
        }

        var contactOperation = (status == ContactStatus.Approved)
                                                   ? ContactOperations.Approve
                                                   : ContactOperations.Reject;

        var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact,
                                    contactOperation);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }
        contact.Status = status;
        Context.Contact.Update(contact);
        await Context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
```

============================================================================
# Add or remove a user to a role
* -> removing privileges from a user. For example, muting a user in a chat app.
* -> adding privileges to a user

* _https://github.com/dotnet/AspNetCore.Docs/issues/8502_

============================================================================
# Differences between Challenge and Forbid
* -> this **app sets the `default policy` to require authenticated users**

* _following code allows anonymous users:
* _anonymous users are allowed to show the differences between `Challenge` vs `Forbid`_

* -> when the **user is not authenticated**, a **`ChallengeResult`** is returned
* -> when **a ChallengeResult is returned**, **`the user is redirected to the "sign-in" page`**

* -> when the **user is authenticated, but not authorized**, a **`ForbidResult`** is returned
* -> when **a ForbidResult is returned**, **`the user is redirected to the "access denied" page`**

```cs
[AllowAnonymous]
public class Details2Model : DI_BasePageModel
{
    public Details2Model(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
    {
    }

    public Contact Contact { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contact? _contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

        if (_contact == null)
        {
            return NotFound();
        }
        Contact = _contact;

        if (!User.Identity!.IsAuthenticated)
        {
            return Challenge();
        }

        var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                           User.IsInRole(Constants.ContactAdministratorsRole);

        var currentUserId = UserManager.GetUserId(User);

        if (!isAuthorized
            && currentUserId != Contact.OwnerID
            && Contact.Status != ContactStatus.Approved)
        {
            return Forbid();
        }

        return Page();
    }
}
```

============================================================================
# Test the completed app
* -> uses the **`Secret Manager tool`** to **store the password for the seeded user accounts** (_sensitive data during local development_)
* -> choose a **strong password**: Use eight or more characters and at least one upper-case character, number, and symbol.
* -> **execute the following command** from the project's folder, where <PW> is the password:
```bash
dotnet user-secrets set SeedUserPW <PW>
```

* _if the app has contacts:_
* -> **delete all of the records** in the Contact table.
* -> **restart the app** to seed the database

* -> an easy way to test the completed app is to **launch three different browsers** (or incognito/InPrivate sessions)
* -> in one browser, register a new user (for example, test@example.com)
* -> Sign in to each browser with a different user

* _Verify the following operations:_
* -> Registered users can view all of the approved contact data
* -> Registered users can edit/delete their own data
* -> Managers can approve/reject contact data. The Details view shows Approve and Reject buttons
* -> Administrators can approve/reject and edit/delete all data

* _create a contact in the **administrator's browser**_
* -> copy the URL for delete and edit from the administrator contact
* -> Paste these links into the test user's browser to verify the test user can't perform these operations

============================================================================
# Create the starter app
* _Create a Razor Pages app named "ContactManager"_
* -> Create the app with Individual User Accounts
* -> Name it "ContactManager" so the namespace matches the namespace used in the sample.
* -> -uld specifies LocalDB instead of SQLite

```bash
dotnet new webapp -o ContactManager -au Individual -uld
```

* _Add Models/Contact.cs: secure-data\samples\starter6\ContactManager\Models\Contact.cs_
```cs
using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }
}
```

* -> Scaffold the Contact model.

* -> Create initial migration and update the database:
```bash
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet-aspnet-codegenerator razorpage -m Contact -udl -dc ApplicationDbContext -outDir Pages\Contacts --referenceScriptLibraries
dotnet ef database drop -f
dotnet ef migrations add initial
dotnet ef database update
```

* -> Update the ContactManager anchor in the Pages/Shared/_Layout.cshtml file:
```html
<a class="nav-link text-dark" asp-area="" asp-page="/Contacts/Index">Contact Manager</a>
```

* -> Test the app by creating, editing, and deleting a contact

============================================================================
# Seed the database
* -> add the **SeedData** class to the **Data folder**:
```cs
using ContactManager.Models;
using Microsoft.EntityFrameworkCore;

// dotnet aspnet-codegenerator razorpage -m Contact -dc ApplicationDbContext -udl -outDir Pages\Contacts --referenceScriptLibraries

namespace ContactManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw="")
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                SeedDB(context, testUserPw);
            }
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Contact.Any())
            {
                return;   // DB has been seeded
            }

            context.Contact.AddRange(
                new Contact
                {
                    Name = "Debra Garcia",
                    Address = "1234 Main St",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "debra@example.com"
                },
                new Contact
                {
                    Name = "Thorsten Weinrich",
                    Address = "5678 1st Ave W",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "thorsten@example.com"
                },
                new Contact
                {
                    Name = "Yuhong Li",
                    Address = "9012 State st",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "yuhong@example.com"
                },
                new Contact
                {
                    Name = "Jon Orton",
                    Address = "3456 Maple St",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "jon@example.com"
                },
                new Contact
                {
                    Name = "Diliana Alexieva-Bosseva",
                    Address = "7890 2nd Ave E",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "diliana@example.com"
                }
             );
            context.SaveChanges();
        }

    }
}
```

* -> call **SeedData.Initialize** from Program.cs:
```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) // this one
{
    var services = scope.ServiceProvider;

    await SeedData.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
```

* -> Test that the app seeded the database. If there are any rows in the contact DB, the seed method doesn't run.