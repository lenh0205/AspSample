> **`IAuthorizationService`** is registered in the **ASP.NET Core Dependency Injection (DI) container** **`by default`** (_so we don't need to register it manually_)
> để protect actions thì ta sẽ viết logic kiểm tra authorized ngay bên trong - có thể sử dụng **IAuthorizationService** dùng các handlers để kiểm tra, hoặc viết logic 1 cách manually

> **`ClaimPrincipal User`** là property của **PageModel** trong "Razor pages" nên ta có thể truy cập thoải mái
> **`resource`** thì ta sẽ query database, binding model từ form, ....
> **`requirment`** thì truyền action mà current user đang muốn thực hiện

> 1 service cần authorization của ta chỉ cần DI cơ bản 3 service **IAuthorizationService**, **ApplicationDbContext** và **UserManager<IdentityUser>** để có thể viết logic

> ta cần để ý **Edit.cshtml.cs**, thằng này phải xử lý phức tạp hơn xíu

============================================================================
# Support authorization
* _update the **`Razor Pages`** and add an **`operations requirements class`**_

## Create a base class for the Contacts Razor Pages
* -> create a base class that **`contains the services used in the contacts Razor Pages`**; the base class **`puts the initialization code in one location`**

* -> adds the **`IAuthorizationService`** service to **access to the authorization handlers**
* -> adds the Identity **`UserManager`** service
* -> add the **`ApplicationDbContext`**

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

============================================================================
# protected actions (Authorization) in Razor pages 
* -> **Create.cshtml.cs**, **Index.cshtml.cs**, **Edit.cshtml.cs**, **Delete.cshtml.cs**

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

            var isAuthorized = await AuthorizationService
                .AuthorizeAsync(User, Contact,ContactOperations.Create);

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

        var isAuthorized = await AuthorizationService
            .AuthorizeAsync(User, Contact, ContactOperations.Update);
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
        var contact = await Context.Contact.AsNoTracking().FirstOrDefaultAsync(m => m.ContactId == id);

        if (contact == null)
        {
            return NotFound();
        }

        var isAuthorized = await AuthorizationService
            .AuthorizeAsync(User, contact, ContactOperations.Update);
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
            var canApprove = await AuthorizationService
                .AuthorizeAsync(User, Contact, ContactOperations.Approve);

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

        var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Delete);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var contact = await Context.Contact.AsNoTracking().FirstOrDefaultAsync(m => m.ContactId == id);

        if (contact == null)
        {
            return NotFound();
        }

        var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact, ContactOperations.Delete);
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

## Update the DetailsModel

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