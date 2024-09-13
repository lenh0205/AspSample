===========================================================================
# Preparation
* -> Create project với template  **ASP.NET Core Web Application (Razor page)** > Identity Scaffold > override trang **`Account/Register`** và **`Account/Manage/Index`** + Add **DbContext** và **User class** mới
* -> thêm <partial name="_LoginPartial" /> to the **layout**; thêm **`UseAuthentication`** vào **pipeline**
* -> Register a user > confirm register > Login as a user > truy cập trang "Account/Manage/Index" > chọn tab "Personal Data"
* -> Test: chọn nút "Download" để tải file **PersonalData.json** (chứa thông tin các thông tin của user)
* -> Test: chọn nút "Delete" để **`xoá user data và close tài khoản`**

===========================================================================
# Add custom user data to the Identity DB
* -> ta sẽ update the **IdentityUser** derived class (_Areas/Identity/Data/WebApp1User.cs_) with **`custom properties`** 
* _Ex: ta thêm 2 trường "Name" and "DOB" vào class Identity user của ta, nhớ update lại một số page không là nó sẽ tèo_
```cs
public class WebApp1User : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; }
}
```

* _"Properties" with the **`'PersonalData' attribute`** are:
* -> **`deleted`** when the **Areas/Identity/Pages/Account/Manage/DeletePersonalData.cshtml** Razor Page (_trang được điều hướng tới sau khi ta nhấn nút "Delete"_) calls **UserManager.Delete** (_khi ta nhấn "Delete data and close my account"_)
* -> **`included in the downloaded data`** by the **Areas/Identity/Pages/Account/Manage/DownloadPersonalData.cshtml** Razor Page 

## Update Identity pages with new custom user data

* _update **Areas/Identity/Pages/Account/Manage/Index.cshtml.cs**_
```cs
public class InputModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Full name")]
    public string Name { get; set; } // this one

    [Required]
    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; } // this one

    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
}

private async Task LoadAsync(WebApp1User user)
{
    var userName = await _userManager.GetUserNameAsync(user);
    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

    Username = userName;

    Input = new InputModel
    {
        Name = user.Name, // this one
        DOB = user.DOB, // this one
        PhoneNumber = phoneNumber
    };
}

public async Task<IActionResult> OnPostAsync()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
    }

    if (!ModelState.IsValid)
    {
        await LoadAsync(user);
        return Page();
    }

    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
    if (Input.PhoneNumber != phoneNumber)
    {
        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
        if (!setPhoneResult.Succeeded)
        {
            StatusMessage = "Unexpected error when trying to set phone number.";
            return RedirectToPage();
        }
    }

    if (Input.Name != user.Name)
    {
        user.Name = Input.Name; // this one
    }

    if (Input.DOB != user.DOB)
    {
        user.DOB = Input.DOB; // this one
    }

    await _userManager.UpdateAsync(user); // this one
        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
```

* _update the **Areas/Identity/Pages/Account/Manage/Index.cshtml**_
```cs
<form id="profile-form" method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
    <div class="form-floating">
        <input asp-for="Username" class="form-control" disabled />
        <label asp-for="Username" class="form-label"></label>
    </div>
    <div class="form-floating"> // this one
        <input asp-for="Input.Name" class="form-control" />
        <label asp-for="Input.Name" class="form-label"></label>
    </div>
    <div class="form-floating"> // this one
        <input asp-for="Input.DOB" class="form-control" />
        <label asp-for="Input.DOB" class="form-label"></label>
    </div>
    <div class="form-floating">
        <input asp-for="Input.PhoneNumber" class="form-control" />
        <label asp-for="Input.PhoneNumber" class="form-label"></label>
        <span asp-validation-for="Input.PhoneNumber" class="text-danger"></span>
    </div>
    <button id="update-profile-button" type="submit" class="w-100 btn btn-lg btn-primary">Save</button>
</form>
```


* _update the **Areas/Identity/Pages/Account/Register.cshtml.cs** page_
```cs
public class InputModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Full name")]
    public string Name { get; set; } // this one

    [Required]
    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; } // this one

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}

public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    if (ModelState.IsValid)
    {
        var user = CreateUser();

        user.Name = Input.Name; // this one
        user.DOB = Input.DOB; // this one

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, Input.Password);

        // .....
    }

    // If we got this far, something failed, redisplay form
    return Page();
}
```

* _update the **Areas/Identity/Pages/Account/Register.cshtml**_
```cs
<form id="registerForm" asp-route-returnUrl="@Model.ReturnUrl" method="post">
    <h2>Create a new account.</h2>
    <hr />
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>

    <div class="form-floating">
        <input asp-for="Input.Name" class="form-control" />
        <label asp-for="Input.Name"></label>
        <span asp-validation-for="Input.Name" class="text-danger"></span>
    </div>
    <div class="form-floating">
        <input asp-for="Input.DOB" class="form-control" />
        <label asp-for="Input.DOB"></label>
        <span asp-validation-for="Input.DOB" class="text-danger"></span>
    </div>

    <div class="form-floating">
        <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" />
        <label asp-for="Input.Email"></label>
        <span asp-validation-for="Input.Email" class="text-danger"></span>
    </div>
    <div class="form-floating">
        <input asp-for="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" />
        <label asp-for="Input.Password"></label>
        <span asp-validation-for="Input.Password" class="text-danger"></span>
    </div>
    <div class="form-floating">
        <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" />
        <label asp-for="Input.ConfirmPassword"></label>
        <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
    </div>
    <button id="registerSubmit" type="submit" class="w-100 btn btn-lg btn-primary">Register</button>
</form>
```

## Add a migration for the custom user data
```bash - Package Manager Console:
Add-Migration CustomUserData
Update-Database
```

## Test App - create, view, download, delete custom user data
* -> **`Create`** - register a new user
* -> **`Read`** - **view the custom user data** on the "/Identity/Account/Manage page"; **Download and view the users personal data** from the "/Identity/Account/Manage/PersonalData" page
* -> **`Delete`**

