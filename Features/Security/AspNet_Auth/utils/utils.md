=====================================================================
# Common

## Tìm user 
```cs
// find by 'UserName'
var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
var user = await userManager.FindByNameAsync(UserName);

// find by 'Id'
IdentityUser user;
string uid = user.Id;
var user = await userManager.FindByIdAsync(uid);
```

## Tạo user / Identity
```cs
// Tạo user trong database
var user = new IdentityUser
{
    UserName = UserName,
    EmailConfirmed = true
};
IdentityResult result = await userManager.CreateAsync(user, testUserPassword);
```

```cs
// khởi tạo instance cho 'ApplicationUser' derive from 'IdentityUser'
var user = Activator.CreateInstance<ApplicationUser>();
user.Name = Input.Name;
user.DOB = Input.DOB;
await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

// update to database
IdentityResult result = await _userManager.CreateAsync(user, Input.Password); 

// check xem có success không
if (result.Succeeded) {
    // gửi email xác nhận tài khoản, .....
}
```

## get 'ClaimPrincipal user'
```cs
// nếu trong razor pages hoặc Controller thì ta có thể truy cập "User" global variable
ClaimPrinciple user = User;

// nếu trong authorization handler
AuthorizationHandlerContext context;
ClaimPrincipal user = context.User;
```

## tìm 'User Id' từ 'ClaimPrincipal'
```cs
ClaimPrincipal user;
var userId = _userManager.GetUserId(user);
```

## Kiểm tra xem 'role' đã tồn tại chưa
```cs
string role;
if (!await roleManager.RoleExistsAsync(role))
{
}
```

## Tạo 'role'
```cs
string role;
var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
IdentityResult IR = roleManager.CreateAsync(new IdentityRole(role));
```

## Kiểm tra 'user' có đúng 'role' không ?
```cs
ClaimPrinciple user;
bool result = user.IsInRole(Constants.ContactAdministratorsRole);
```

## Attach "user" với "role"
```cs
IdentityUser user;
string role;
IdentityResult IR = await userManager.AddToRoleAsync(user, role);
```

## Mark the specified 'requirement' as "sucess" or "fail" in Authorization Handler
```cs
AuthorizationHandlerContext context;
OperationAuthorizationRequirement requirement;

context.Succeed(requirement);
context.Fail(requirement);
```

## check xem "user" có "Authorized" với action này không ?
* -> để sử dụng thằng này đòi hỏi ta phải đăng ký **`IAuthorizationHandler`**
```cs
var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Create);

var isAuthorized = User.IsInRole(Constants.ContactManagersRole) 
                        || User.IsInRole(Constants.ContactAdministratorsRole);
```

=====================================================================
# Tạo "ClaimPrincipal" và attach nó vào cơ chế authentication
* -> xem `~/Features\Security\AspNet_Auth\Authentication\without_AspNetCore_Identity\Cookie_Authentication.md` để hiểu thêm

```cs - for "cookie-based authentication"
var user = await CreateIdentityUser(Input.Email, Input.Password);
var claims = new List<Claim> 
{
    new Claim(ClaimTypes.Name, user.Email),
    new Claim("FullName", user.FullName),
    new Claim(ClaimTypes.Role, "Administrator"),
};
var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

var authProperties = new AuthenticationProperties {};

// default way to sign in a user when using cookie-based authentication 
// (without full identity system like using "ASP.NET Core Identity")
// to create and manage authentication cookies -> store it in server session -> send sessionID to the client in a cookie
await HttpContext.SignInAsync( 
    CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
```

============================================================
## Login / Sign In
* -> the **_signInManager.SignInAsync** or **_signInManager.PasswordSignInAsync** is indeed call **`HttpContext.SignInAsync`** under the hood (_after performing some additional operations_)

```cs
// this method sign in a user directly - used when we have already authenticated the user 
// and for "cookie-based authentication" only
// Ex: after user registration, external authentication providers, two-factor authentication
ApplicationUser user; // cần có đầy đủ thông tin
await _signInManager.SignInAsync(user, isPersistent: false);

// used for the traditional username/email and password login scenario
SignInResult result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
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
```

## Check xem có cần Confirm Account sau khi register không ?
```cs
// cấu hình trong Program.cs
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Usage:
if (_userManager.Options.SignIn.RequireConfirmedAccount) {}
```

## Gửi email để confim registration
```cs
// tạo callbackUrl - đường link với đầy đủ param để yêu cầu việc confirm registration
var userId = await _userManager.GetUserIdAsync(user);
var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
var callbackUrl = Url.Page(
    "/Account/ConfirmEmail",
    pageHandler: null,
    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
    protocol: Request.Scheme
);

// gửi email với title là "Confirm your email" và nội dung là link để 
await _emailSender.SendEmailAsync(
    Input.Email, 
    "Confirm your email",
    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
);
```

## Logout / Sign out
```cs
await _signInManager.SignOutAsync();
```

## Confirm Email Registration
```cs
code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
var result = await _userManager.ConfirmEmailAsync(user, code);
StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
```

============================================================
# Authorize against "scope" claim contain a list value

```cs
//  access the API controller with a JWT Token which contains this single scope 
services.AddAuthorization(options =>
{
    options.AddPolicy(
        "HasReadScope",
        builder.RequireClaim("scope", "read"));
    options.AddPolicy(
        "HasReadOrWriteScope",
        builder.RequireClaim("scope", "read", "write")); // "scope" claim have a single value with "read" or "write"
});
// apply both "HasReadScope" and "HasReadOrWriteScope" policy
[Authorize(Policy = "HasReadScope")]
[Authorize(Policy = "HasReadOrWriteScope")]
public IActionResult Get()
{
    return Ok();
}


// for access token contains a scope claim with a multi value like "read write delete"
services.AddAuthorization(options =>
{
    options.AddPolicy(
        "HasReadWriteScope",
        builder.RequireScope("read", "write"));
});
public static class ScopeAuthorizationRequirementExtensions
{
    public static AuthorizationPolicyBuilder RequireScope(
        this AuthorizationPolicyBuilder authorizationPolicyBuilder,
        params string[] requiredScopes)
    {
        authorizationPolicyBuilder.RequireScope((IEnumerable<string>) requiredScopes);
        return authorizationPolicyBuilder;
    }
 
    public static AuthorizationPolicyBuilder RequireScope(
        this AuthorizationPolicyBuilder authorizationPolicyBuilder,
        IEnumerable<string> requiredScopes)
    {
        authorizationPolicyBuilder.AddRequirements(new ScopeAuthorizationRequirement(requiredScopes));
        return authorizationPolicyBuilder;
    }
}
public class ScopeAuthorizationRequirement : AuthorizationHandler<ScopeAuthorizationRequirement>, IAuthorizationRequirement
{
    public IEnumerable<string> RequiredScopes { get; }
 
    public ScopeAuthorizationRequirement(IEnumerable<string> requiredScopes)
    {
        if (requiredScopes == null || !requiredScopes.Any())
        {
            throw new ArgumentException($"{nameof(requiredScopes)} must contain at least one value.", nameof(requiredScopes));
        }
 
        RequiredScopes = requiredScopes;
    }
 
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationRequirement requirement)
    {
        if (context.User != null)
        {
            var scopeClaim = context.User.Claims.FirstOrDefault(
                c => string.Equals(c.Type, "scope", StringComparison.OrdinalIgnoreCase));
 
            if (scopeClaim != null)
            {
                var scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (requirement.RequiredScopes.All(requiredScope => scopes.Contains(requiredScope)))
                {
                    context.Succeed(requirement);
                }
            }
        }
 
        return Task.CompletedTask;
    }
}
```