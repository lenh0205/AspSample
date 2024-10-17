> ASP.NET Core Identity hỗ trợ 1 số UI; cung cấp các **`identity model`** cho phép ta customize 
> https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-8.0
> ta sẽ sử dụng Role-based authorization; và Identity đã hỗ trợ cho ta sẵn 1 số Role

=======================================================================
# ASP.NET Core Identity
* -> install **Microsoft.AspNetCore.Identity**

## Tạo 'user' model

```cs - ~/Data/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
```

## Modify 'DbContext'
* -> install **Microsoft.AspNetCore.Identity.EntityFrameworkCore**

```cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class MyDbContext : IdentityDbContext<ApplicationUser> // chỉ định lớp quản lý user là 'ApplicationUser'
{
    public DbSet<Book> Books { get; set; }
}
```

## Cấu hình service sử dụng Identity
* -> install **Microsoft.AspNetCore.Authentication.JwtBearer**

```cs - program.cs
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>() // add "Identity" service
    .AddEntityFrameworkStores<MyDbContext>() // chứa 1 số bảng của "Identity"
    .AddDefaultTokenProvider();

builder.Services.AddDbContext<MyDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectString("myDb"))});

```

## Migration
* -> **Add-Migration AddIdentity** - ta sẽ thấy nó tạo 1 list bảng **`AspNetRoles`**, **`AspNetUsers`**, **`AspNetRoleClaims`**, **`AspNetUserClaims`**, **`AspNetUserLogins`**, **`AspNetUserRoles`**, **`AspNetUserTokens`**
* -> **update-database** - vào SSMS để xem diagram biểu thị mối quan hệ của những bảng này

=======================================================================
# Our project 

## Tạo Model cho "login" và "signup"

```cs - ~/Model/
public class SignInModel 
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class SignUpModel
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string ConfirmPassword { get; set; } = null!;
}
```

## Role
```cs - ~/Helper/AppRole.cs
public static class AppRole
{
    public const string Admin = "Administrator";
    public const string Customer = "Customer";
    public const string Manager = "Manager";
    public const string Accountant = "Accountant";
    public const string HR = "Human Resource";
    public const string Warehouse = "Warehose staff";
}
```

## Repositories

```cs - ~/Repositories/IAccountRepository.cs
public interface IAccountRepository
{
    public Task<IdentityResult> SignUpAsync(SignUpModel model);
    public Task<string> SignInAsync(SignInModel model); // trả về token 
}

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplciationUser> signInManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration configuration;

    // "UserManager", "SignInManager" là service cung cấp bởi Identity
    public AccountRepository(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplciationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration
    )
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.configuration = configuration;
    }

    public Task<string> SignInAsync(SignInModel model)
    {
        // kiểm tra tính hợp lệ của user credential
        var user = await userManager.FindByEmailAsync(model.Email);
        var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);
        if (user == null || !passwordValid) return string.Empty;
        
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // lấy ra list "role" gắn với user tướng ứng; 
        // từ đó tạo 1 list "claim" với type là "Role" và giá trị là từng role của user
        // thêm list claim đó vào list claim hiện có
        var userRoles = await userManager.GetRolesAsync(user);
        foreach(var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(20),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<IdentityResult> SignUpAsync(SignUpModel model)
    {
        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email
        };

        // 'userManager' tự động lưu vào "AspNetUsers" table 
        // nó cũng sẽ hash password cho ta luôn
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // add role default cho user là "Customer"
            // chỉ thêm duy nhất trong lần đầu có 1 request chạy vô đây
            // kiểm tra role "Customer" đã có (trong DB của Identity) chưa
            if (!await roleManager.RoleExistAsync(AppRole.Customer))
            {
                // chưa có thì tạo rồi lưu vào bảng 'AspNetRoles' của 'Identity'
                await roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
            }

            // thêm dữ liệu vào bảng 'AspNetUserRoles'
            await userManager.AddToRoleAsync(user, AppRole.Customer)
        }

        return result;
    }
}
```

```cs
// program.cs
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Controllers
[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository accountRepo;

    public AccountsController(IAccountRepository repo)
    {
        accountRepo = repo;
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp (SignUpModel signUpModel)
    {
        var result = await accountRepo.SignUpAsync(SignUpModel);
        if (result.Succeeded) return Ok(result.Succeeded);
        return StatusCode(500);
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(SignInModel signInModel)
    {
        var result = await accountRepo.SignInAsync(signInModel);
        if (string.IsNullOrEmpty(result)) return Unauthorized();
        return Ok(result);
    }
}
```

## Usage

```cs
[HttpGet]
[Authorize] // require "authen" only
public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
{
}

[HttpGet("{id}")]
[Authorize(Roles = AppRole.Customer)] // require "author" by role
public async Task<ActionResult<Book>> GetBook(int id)
{
}
```