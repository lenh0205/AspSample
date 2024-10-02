============================================================================
# Add or remove a user to a role
* _https://github.com/dotnet/AspNetCore.Docs/issues/8502_

* -> removing privileges from a user. For example, muting a user in a chat app
* -> adding privileges to a user


============================================================================
# Differences between 'Challenge' and 'Forbid'
* -> this **app sets the `default policy` to `require authenticated users`**

* -> when the **user is not authenticated**, a **`ChallengeResult`** is returned
* -> when **a ChallengeResult is returned**, **`the user is redirected to the "sign-in" page`**

* -> when the **user is authenticated, but not authorized**, a **`ForbidResult`** is returned
* -> when **a ForbidResult is returned**, **`the user is redirected to the "access denied" page`**

* _anonymous users are allowed to show the differences between `Challenge` vs `Forbid`_
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