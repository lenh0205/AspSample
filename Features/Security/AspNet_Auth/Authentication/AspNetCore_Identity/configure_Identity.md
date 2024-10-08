
# Configure ASP.NET Core Identity
* -> _ASP.NET Core Identity_ uses **default values** for settings such as **`password policy, lockout, and cookie configuration`**
* -> these settings can be overridden at application startup

========================================================================
# Identity options
* -> the **IdentityOptions** class represents the options that can be used to **`configure the Identity system`**
* -> IdentityOptions must be set after calling **`AddIdentity`** or **`AddDefaultIdentity`**

## Claims Identity
* -> **`IdentityOptions.ClaimsIdentity`** specifies the **ClaimsIdentityOptions** with these properties:

## Lockout
* -> **Lockout** is **`set in the 'PasswordSignInAsync' method`**

```cs
public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/");

    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    if (ModelState.IsValid)
    {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await _signInManager.PasswordSignInAsync(Input.Email,
             Input.Password, Input.RememberMe,
             lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return LocalRedirect(returnUrl);
        }
        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }

    // If we got this far, something failed, redisplay form
    return Page();
}
```

```cs - "Lockout options" are set in Program.cs
builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// sets the IdentityOptions "LockoutOptions" with default values 
builder.Services.Configure<IdentityOptions>(options =>
{
    // the amount of time a user is locked out when a lockout occurs
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    // number of failed access attempts until a user is locked out, if lockout is enabled.
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    // determines if a new user can be locked out
    options.Lockout.AllowedForNewUsers = true;
});
```

## Password
* -> **`by default`**, **Identity requires that passwords** contain an **`uppercase character`**, **`lowercase character`**, **`a digit`**, and **`a non-alphanumeric character`**
* -> Passwords must be **`at least six characters long`**

* _Passwords are **configured** with:_
* -> **`PasswordOptions`** in **Program.cs**
* -> **`[StringLength] attributes`** of **Password properties** if Identity is scaffolded into the app. * -> **`InputModel Password`** properties are found in the following files: **Areas/Identity/Pages/Account/Register.cshtml.cs**, **Areas/Identity/Pages/Account/ResetPassword.cshtml.cs**

```cs
// Default Password settings
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true; // requires a number between 0-9 in the password
    options.Password.RequireLowercase = true; // requires a lowercase character in the password
    options.Password.RequireNonAlphanumeric = true; // Requires a non-alphanumeric character in the password
    options.Password.RequireUppercase = true; // requires an uppercase character in the password
    options.Password.RequiredLength = 6; // the minimum length of the password
    options.Password.RequiredUniqueChars = 1; // requires the number of distinct characters in the password (only applies to ASP.NET Core 2.0 or later)
});
```

## Sign-in
* -> **`IdentityOptions.SignIn`** specifies the **SignInOptions**

```cs
// this sets SignIn settings (to default values)
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default SignIn settings.
    options.SignIn.RequireConfirmedEmail = false; // requires a confirmed email to sign in
    options.SignIn.RequireConfirmedPhoneNumber = false; // requires a confirmed phone number to sign in
});
```

## Tokens
* -> **`IdentityOptions.Tokens`** specifies the **TokenOptions**

```cs
+------------------------------+-------------------------------------------------------------------------+
Property                       |                              Description                                |
+------------------------------+-------------------------------------------------------------------------+
AuthenticatorTokenProvider     | Gets or sets the AuthenticatorTokenProvider used to validate two-factor | 
                               | sign-ins with an authenticator                                          |
+------------------------------+-------------------------------------------------------------------------+
ChangeEmailTokenProvider       | Gets or sets the ChangeEmailTokenProvider used to generate tokens used  |
                               | in email change confirmation emails                                     |
+------------------------------+-------------------------------------------------------------------------+
ChangePhoneNumberTokenProvider | Gets or sets the ChangePhoneNumberTokenProvider used to generate tokens |
                               | used when changing phone numbers                                        |
+------------------------------+-------------------------------------------------------------------------+
EmailConfirmationTokenProvider | Gets or sets the token provider used to generate tokens used in account |
                               | confirmation emails                                                     |
+------------------------------+-------------------------------------------------------------------------+
PasswordResetTokenProvider     | Gets or sets the IUserTwoFactorTokenProvider<TUser> used to generate    |
                               | tokens used in password reset emails                                    |
+------------------------------+-------------------------------------------------------------------------+
ProviderMap                    | Used to construct a User Token Provider with the key used as the        | 
                               | provider name                                                           |
+------------------------------+-------------------------------------------------------------------------+
```

## User
* -> **`IdentityOptions.User`** specifies the **UserOptions**

```cs
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default User settings

    // allowed characters in the username
    options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // requires each user to have a unique email
    options.User.RequireUniqueEmail = false;

});
```

## Cookie settings
* -> configure the app's cookie in **Program.cs**
* -> **`ConfigureApplicationCookie`** must be called after calling **AddIdentity** or **AddDefaultIdentity**

```cs
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "YourAppCookieName";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    // ReturnUrlParameter requires 
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});
```

========================================================================
# Password Hasher options
* -> _PasswordHasherOptions_ **gets and sets options** for **`password hashing`**

```r
+-------------------+------------------------------------------------------------------------------------------------------------+
| Option	        |                                              Description                                                   |
+-------------------+------------------------------------------------------------------------------------------------------------+
| CompatibilityMode	| The compatibility mode used when hashing new passwords. Defaults to IdentityV3. The first byte of a hashed | 
|                   | password, called a format marker, specifies the version of the hashing algorithm used to hash the password.|
|                   | When verifying a password against a hash, the VerifyHashedPassword method selects the correct algorithm    | 
|                   | based on the first byte. A client is able to authenticate regardless of which version of the algorithm was |
|                   | used to hash the password. Setting the compatibility mode affects the hashing of new passwords.            |
+-------------------+------------------------------------------------------------------------------------------------------------+
|  IterationCount	| the number of iterations used when hashing passwords using PBKDF2. This value is only used when the        |
|                   | CompatibilityMode is set to IdentityV3. The value must be a positive integer and defaults to 100000        |
+-------------------+------------------------------------------------------------------------------------------------------------+
```

========================================================================
# Globally require all users to be authenticated
* -> xem phần `~\Features\Security\AspNet_Auth\AspNet_Core_Identity\Authorize`

========================================================================
# 'ISecurityStampValidator' and 'SignOut' everywhere
* -> **Apps need to react to events** involving **`security sensitive actions`** by **`regenerating the users 'ClaimsPrincipal'`**
* _For example, the **ClaimsPrincipal should be regenerated** when **`joining a role`**, **`changing the password`**, or other security sensitive events_

* -> "Identity" uses the **`ISecurityStampValidator interface`** to **regenerate the 'ClaimsPrincipal'**
* -> the **`default implementation of Identity`** registers a **SecurityStampValidator** with the **`main application cookie`** and **`the two-factor cookie`**

* -> the **`validator`** hooks into **the `OnValidatePrincipal` event of each cookie** to call into Identity to **`verify that the user's security stamp claim is unchanged from what's stored in the cookie`**
* -> the validator calls in at **`regular intervals`**
* -> the "call interval" is a tradeoff between **hitting the datastore too frequently** and **not often enough**
* -> **checking with a long interval** results in **`stale claims`**

* -> call **userManager.UpdateSecurityStampAsync(user)** to **`force existing cookies to be invalided the next time they are checked`**
* -> **most of the Identity UI account and manage pages** call **userManager.UpdateSecurityStampAsync(user)** **`after changing the password or adding a login`**
* -> Apps can call **userManager.UpdateSecurityStampAsync(user)** to **`implement a sign out everywhere action`**

```cs
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Force Identity's security stamp to be validated every minute.
builder.Services.Configure<SecurityStampValidatorOptions>(o => 
                   o.ValidationInterval = TimeSpan.FromMinutes(1));

builder.Services.AddRazorPages();
```

