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
// if the user logs in without MFA, the warning: Admin(Not Enabled)
// if then user click one the "Admin" link, they is redirected to the "MFA enable view"

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
* -> the **`acr_values` parameter** can be used to **pass the mfa required value from the client to the server in an authentication request**
* -> the **'acr_values' parameter** needs to be **`handled on the OpenID Connect server`**

## OpenID Connect ASP.NET Core client
* -> **the ASP.NET Core Razor Pages OpenID Connect client app** uses the **`AddOpenIdConnect`** method to **`login to the OpenID Connect server`**
* -> the **`acr_values` parameter** is set with the **`mfa`** value and sent with the **authentication request**
* -> the **`OpenIdConnectEvents`** is used to add this
* _for **recommended acr_values parameter values**: https://datatracker.ietf.org/doc/html/draft-ietf-oauth-amr-values-08_

```cs
build.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
	options.SignInScheme =
		CookieAuthenticationDefaults.AuthenticationScheme;
	options.Authority = "<OpenID Connect server URL>";
	options.RequireHttpsMetadata = true;
	options.ClientId = "<OpenID Connect client ID>";
	options.ClientSecret = "<>";
	options.ResponseType = "code";
	options.UsePkce = true;	
	options.Scope.Add("profile");
	options.Scope.Add("offline_access");
	options.SaveTokens = true;
	options.Events = new OpenIdConnectEvents
	{
		OnRedirectToIdentityProvider = context =>
		{
			context.ProtocolMessage.SetParameter("acr_values", "mfa");
			return Task.FromResult(0);
		}
	};
});
```

## Example OpenID Connect Duende IdentityServer server with ASP.NET Core Identity
* -> on **`the OpenID Connect server`**, which is implemented using **ASP.NET Core Identity** with **Razor Pages**, a new page named **`ErrorEnable2FA.cshtml`** is created
* -> the view displays if the **Identity** comes from an **`app that requires MFA`** but the **`user hasn't activated this in Identity`**
* -> also **`informs the user`** and **`adds a link to activate this`**

```cs
@{
    ViewData["Title"] = "ErrorEnable2FA";
}

<h1>The client application requires you to have MFA enabled. Enable this, try login again.</h1>

<br />

You can enable MFA to login here:

<br />

<a href="~/Identity/Account/Manage/TwoFactorAuthentication">Enable MFA</a>
```

* -> in the **Login** method, the **IIdentityServerInteractionService interface** implementation **`_interaction`** is used to **`access the OpenID Connect request parameters`**
* -> the **'acr_values' parameter** is accessed using the **`AcrValues`** property
* -> as the **`client sent this with 'mfa' set`**, this **can then be checked**

* -> if **MFA is required**, and the user in ASP.NET Core Identity has **MFA enabled**, then **`the login continues`**
* -> when the user has **no MFA enabled**, the **user is redirected to the custom view `ErrorEnable2FA.cshtml`**
* -> then ASP.NET Core Identity signs the user in

* _the `Fido2Store` is used to check if the user has activated MFA using a `custom FIDO2 Token Provider`_
```cs
public async Task<IActionResult> OnPost()
{
	// check if we are in the context of an authorization request
	var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

	var requires2Fa = context?.AcrValues.Count(t => t.Contains("mfa")) >= 1;

	var user = await _userManager.FindByNameAsync(Input.Username);
	if (user != null && !user.TwoFactorEnabled && requires2Fa)
	{
		return RedirectToPage("/Home/ErrorEnable2FA/Index");
	}

	// code omitted for brevity

	if (ModelState.IsValid)
	{
		var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberLogin, lockoutOnFailure: true);
		if (result.Succeeded)
		{
			// code omitted for brevity
		}
		if (result.RequiresTwoFactor)
		{
			var fido2ItemExistsForUser = await _fido2Store.GetCredentialsByUserNameAsync(user.UserName);
			if (fido2ItemExistsForUser.Count > 0)
			{
				return RedirectToPage("/Account/LoginFido2Mfa", new { area = "Identity", Input.ReturnUrl, Input.RememberLogin });
			}

			return RedirectToPage("/Account/LoginWith2fa", new { area = "Identity", Input.ReturnUrl, RememberMe = Input.RememberLogin });
		}

		await _events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
		ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
	}

	// something went wrong, show form with error
	await BuildModelAsync(Input.ReturnUrl);
	return Page();
}
```

* -> if the user is already logged in, the client app still **validates the `amr` claim** and can **`set up the MFA with a link to the ASP.NET Core Identity view`**

==============================================================================
# Force ASP.NET Core OpenID Connect client to require MFA
* -> for **requiring that users have authenticated using MFA** in **`an ASP.NET Core Razor Page app`**, which **`uses OpenID Connect to sign in`**
* -> to **validate the MFA requirement**, an **`IAuthorizationRequirement`** requirement is created
* -> this will be added to **`the pages using a policy that requires MFA`**

```cs
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreRequireMfaOidc;

public class RequireMfa : IAuthorizationRequirement{}
```

* -> an **AuthorizationHandler is implemented** that will **`use the 'amr' claim`** and **`check for the value 'mfa'`**
* -> the **amr** is returned in the **`id_token`** of a successful authentication and can have many different values as defined in the **`Authentication Method Reference Values specification`**
* -> the **`returned value`** depends on **how the identity authenticated** and on **the OpenID Connect server implementation**
* -> the **AuthorizationHandler** **`uses the 'RequireMfa' requirement`** and **`validates the 'amr' claim`**

* -> the **`OpenID Connect server can be implemented`** using **`Duende Identity Server`** with **`ASP.NET Core Identity`**. 
* -> when a **user logs in using `TOTP`**, **the `amr` claim** is returned with **an `MFA` value**
* -> if using a different **OpenID Connect server implementation** or a different **MFA type**, **`the 'amr' claim will, or can, have a different value`**
* -> the code must be extended to accept this as well

```cs
public class RequireMfaHandler : AuthorizationHandler<RequireMfa>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context, 
		RequireMfa requirement)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));
		if (requirement == null)
			throw new ArgumentNullException(nameof(requirement));

		var amrClaim =
			context.User.Claims.FirstOrDefault(t => t.Type == "amr");

		if (amrClaim != null && amrClaim.Value == Amr.Mfa)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}
```

* -> in the program file, the **`AddOpenIdConnect`** method is used as the **`default challenge scheme`**
* -> the **authorization handler**, which is used to **`check the 'amr' claim`**, is added to the Inversion of Control container
* -> **`a policy is then created`** which **adds the `RequireMfa` requirement**
```cs
builder.Services.ConfigureApplicationCookie(options =>
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always);

builder.Services.AddSingleton<IAuthorizationHandler, RequireMfaHandler>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
	options.SignInScheme =
		CookieAuthenticationDefaults.AuthenticationScheme;
	options.Authority = "https://localhost:44352";
	options.RequireHttpsMetadata = true;
	options.ClientId = "AspNetCoreRequireMfaOidc";
	options.ClientSecret = "AspNetCoreRequireMfaOidcSecret";
	options.ResponseType = "code";
	options.UsePkce = true;	
	options.Scope.Add("profile");
	options.Scope.Add("offline_access");
	options.SaveTokens = true;
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireMfa", policyIsAdminRequirement =>
	{
		policyIsAdminRequirement.Requirements.Add(new RequireMfa());
	});
});

builder.Services.AddRazorPages();
```

* -> this **policy** is then **`used in the Razor page as required`**; the policy **`could be added globally for the entire app as well`**
```cs
[Authorize(Policy= "RequireMfa")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
```

* -> if **the user authenticates without MFA**, **the `amr` claim** will probably **have a `pwd` value**
* -> **`the request won't be authorized to access the page`**
* -> using the **default values**, **`the user will be redirected to the 'Account/AccessDenied' page`**
* _this behavior can be changed or we can implement our own custom logic here_
* _in this example, a link is added so that the `valid user can set up MFA for their account`_
```cs
@page
@model AspNetCoreRequireMfaOidc.AccessDeniedModel
@{
    ViewData["Title"] = "AccessDenied";
    Layout = "~/Pages/Shared/_Layout.cshtml";
}

<h1>AccessDenied</h1>

You require MFA to login here

<a href="https://localhost:44352/Manage/TwoFactorAuthentication">Enable MFA</a>
```

* -> now **`only users that authenticate with MFA can access the page or website`**
* -> if **different MFA types are used or if 2FA is okay**, **the `amr` claim** will **`have different values`** and needs to be **`processed correctly`**
* -> **different OpenID Connect servers** also return different values for this claim and might not follow the Authentication Method Reference Values specification