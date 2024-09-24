> the demo code is setup using ASP.NET Core with Identity and Razor Pages

==============================================================================
# Multi-factor authentication in ASP.NET Core
* -> **Multi-factor authentication (MFA)** is a process in which **`a user is requested during a sign-in event for additional forms of identification`**
* -> this prompt could be to **enter a code from a cellphone** (_use a FIDO2 key_), or to **provide a fingerprint scan**
* -> when we require a second form of authentication, security is enhanced; the additional factor isn't easily obtained or duplicated by an attacker

==============================================================================
# MFA, 2FA
* -> **MFA** requires **``at least two or more types of proof for an identity`** like **something you know**, **something you possess**, or **biometric validation** for the user to authenticate.
* -> **Two-factor authentication (2FA)** is like **`a subset of MFA`**, but the difference being that MFA can require two or more factors to prove the identity

* -> **`2FA is supported by default`** when using **`ASP.NET Core Identity`**
* -> to enable or disable 2FA for a specific user, set the **`IdentityUser<TKey>.TwoFactorEnabled`** property
* -> **the ASP.NET Core Identity Default UI** includes **`pages for configuring 2FA`**

## MFA TOTP (Time-based One-time Password Algorithm)
* -> **MFA using TOTP** is **`supported by default`** when using **`ASP.NET Core Identity`**
* -> this approach can be used together with any compliant authenticator app, including **Microsoft Authenticator** and **Google Authenticator**
* _xem `~\Features\Security\AspNet_Auth\Authentication\AspNetCore_Identity\QR_Code.md`_

* -> to **disable support for MFA TOTP**, configure authentication using **`AddIdentity`** instead of **`AddDefaultIdentity`**
* -> **AddDefaultIdentity** calls **`AddDefaultTokenProviders`** internally, which **registers multiple token providers including one for MFA TOTP**
* -> to **register only specific token providers**, call **`AddTokenProvider`** for **each required provider**

## MFA passkeys/FIDO2 or passwordless
* -> **passkeys/FIDO2** is **`currently the most secure way of achieving MFA`**; protects against **`phishing attacks`** (_as well as certificate authentication and Windows for business_)
* -> at present, **ASP.NET Core** **`doesn't support passkeys/FIDO2 directly`**; **Passkeys/FIDO2** can be used for **MFA** or **passwordless flows**
* _**other forms of passwordless MFA** **`do not or may not protect against phishing`**_

* -> **`Microsoft Entra ID`** provides support for **passkeys/FIDO2** and **passwordless flows**

## MFA SMS
* -> **MFA with SMS** increases security massively compared with password authentication (single factor)
* -> however, using SMS as a second factor is **`no longer recommended`**
* -> **`too many known attack vectors exist`** for this type of implementation

==============================================================================
# Configure MFA for "administration pages" using 'ASP.NET Core Identity'
* -> **`MFA could be forced on users`** to **access sensitive pages** within an _ASP.NET Core Identity app_
* -> this could be useful for apps where **`different levels of access exist for the different identities`**
* _For example, users might be able to view the profile data using a password login, but `an administrator` would be required to use MFA to access the administrative pages_

## Extend the login with an MFA claim
* -> the **AddIdentity** method is used instead of **AddDefaultIdentity** one, 
* -> so an **`IUserClaimsPrincipalFactory implementation`** can be used to **`add claims to the identity`** after a **successful login**
```cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
		options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, 
    AdditionalUserClaimsPrincipalFactory>();

builder.Services.AddAuthorization(options =>
    options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));

builder.Services.AddRazorPages();
```

* -> the **'AdditionalUserClaimsPrincipalFactory' class** adds the **`amr` claim** to the **`user claims`** only after a successful login
* -> **`the claim's value is read from the database`**
* -> **the claim is added** here because the **user should only access the higher protected view** if **`the identity has logged in with MFA`**
* -> if the **`database view`** is **read from the database directly** instead of **using the claim**, it's **`possible to access the view without MFA directly after activating the MFA`**
```cs
namespace IdentityStandaloneMfa
{
    public class AdditionalUserClaimsPrincipalFactory : 
        UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        public AdditionalUserClaimsPrincipalFactory( 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();

            if (user.TwoFactorEnabled)
            {
                claims.Add(new Claim("amr", "mfa"));
            }
            else
            {
                claims.Add(new Claim("amr", "pwd"));
            }

            identity.AddClaims(claims);
            return principal;
        }
    }
}
```

* -> because **the Identity service setup changed in the 'Startup' class**, the **`layouts of the Identity need to be updated`**
* _first, scaffold the Identity pages into the app_
* _define the **layout** in the `Identity/Account/Manage/_Layout.cshtml file`:_
```cs
@{
    Layout = "/Pages/Shared/_Layout.cshtml";
}
```
* -> also assign the layout for all the manage pages from the Identity pages:

## Validate the MFA requirement in the administration page
* -> **the administration Razor Page** **`validates that the user has logged in using MFA`**
* -> in the **OnGet** method, **`the identity is used to access the user claims`**
* -> the **`amr` claim** is **`checked for the value 'mfa'`**
* -> **if the identity is missing this claim or is false**, **`the page redirects to the "Enable MFA page"`** (_this is possible because the user has logged in already, but without MFA_)

```cs
namespace IdentityStandaloneMfa
{
    public class AdminModel : PageModel
    {
        public IActionResult OnGet()
        {
            var claimTwoFactorEnabled = 
                User.Claims.FirstOrDefault(t => t.Type == "amr");

            if (claimTwoFactorEnabled != null && 
                "mfa".Equals(claimTwoFactorEnabled.Value))
            {
                // You logged in with MFA, do the administrative stuff
            }
            else
            {
                return Redirect(
                    "/Identity/Account/Manage/TwoFactorAuthentication");
            }

            return Page();
        }
    }
}
```

## UI logic to toggle user login information
* -> **`an authorization policy`** was **added at startup**; **`the policy requires the 'amr' claim with the value 'mfa'`**
```cs
services.AddAuthorization(options =>
    options.AddPolicy("TwoFactorEnabled",
        x => x.RequireClaim("amr", "mfa")));
```

* -> **this policy can then be used in the '_Layout' view** to **`show or hide the Admin menu with the warning`**:
```cs
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Identity
@inject SignInManager<IdentityUser> SignInManager
@inject UserManager<IdentityUser> UserManager
@inject IAuthorizationService AuthorizationService
```

* -> if **the identity has logged in using MFA**, **`the Admin menu is displayed without the tooltip warning`** 
* -> when **the user has logged in without MFA**, **`the Admin (Not Enabled) menu is displayed along with the tooltip that informs the user (explaining the warning)`**
```cs
@if (SignInManager.IsSignedIn(User))
{
    @if ((AuthorizationService.AuthorizeAsync(User, "TwoFactorEnabled")).Result.Succeeded)
    {
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-page="/Admin">Admin</a>
        </li>
    }
    else
    {
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-page="/Admin" 
               id="tooltip-demo"  
               data-toggle="tooltip" 
               data-placement="bottom" 
               title="MFA is NOT enabled. This is required for the Admin Page. If you have activated MFA, then logout, login again.">
                Admin (Not Enabled)
            </a>
        </li>
    }
}
```

==============================================================================
# Send MFA sign-in requirement to OpenID Connect server

## OpenID Connect ASP.NET Core client

## Example OpenID Connect Duende IdentityServer server with ASP.NET Core Identity

==============================================================================
# Force ASP.NET Core OpenID Connect client to require MFA
