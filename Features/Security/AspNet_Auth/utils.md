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

## Tạo user
```cs
// Tạo user trong database
var user = new IdentityUser
{
    UserName = UserName,
    EmailConfirmed = true
};
await userManager.CreateAsync(user, testUserPassword);
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
// nếu trong razor pages thì ta có thể truy cập nó global vì nó là thuộc tính của "PageModel"
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

## Login / Sign In
```cs
// nếu muốn mark user as logged in  - Ví dụ tự động login ngay sau khi register thành công:
ApplicationUser user; // cần có đầy đủ thông tin
await _signInManager.SignInAsync(user, isPersistent: false);

// nếu có trang "/Login" riêng:
var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
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