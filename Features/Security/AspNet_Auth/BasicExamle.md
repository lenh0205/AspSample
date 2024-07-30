=======================================================================
> Ví dụ này đòi hỏi ta phải thao tác tay với Postman để copy paste token
> để có thể `Authenticate` và `Authorize`, thì JWT đòi hỏi phải chứa **user info**, **role**, **expire time**

# Token-based Authentication

## Content
* _`/Login` action kiểm `usename-password` gửi tới với cái đã lưu trong `Database`_
* _nếu oke thì generate 1 cái `Token` cho user; user sau này khi gửi request sẽ đính kèm `token` vào **Authorization header**_
* _server sẽ có 1 **`middleware`** sử dụng **secret key** để kiểm tra tính hợp lệ của token này_
* _nếu token is **invalid** hoặc **expired** thì nó sẽ trả về **`403: UnAuthorized`**_
* _khi ta `decode` token này ra ta sẽ thấy 3 phần `Header.Payload.Signature`_

## Thêm 'secret key' vào config appsetting

```json - appsettings.json
{
    "AppSettings": {
        "Secret": "this is the secret string with 32 characters"
    },
    "JWT": {
        "ValidIssuer": "https://localhost:7014",
        "ValidAudience": "client",
        "Secret": "this is the secret string with 32 characters"
    }
}
```

```cs - định nghĩa 1 class để đọc "AppSetting"
// ~/Helpers/AppSettings
public class AppSettings
{
    public string Secret { get; set; }
}
```

```cs - đăng ký "AppSettings"
services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
```

## Đăng ký Auth
* -> install **System.IdentityModel.Tokens.Jwt**
* -> install **Microsoft.AspNetCore.Authentication.JwtBearer** 

```cs - Startup.cs
// -> thuật toán chỉ chạy trên bit nên ta sẽ cần Encode 'secret key'
var secretKey = Configuration["JWT:Secret"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services
    .AddAuthentication(option => { // sử dụng lại Authen nào ? (cookie-base, jwt bearer, ...)
        // import "JwtBearerDefaults" from "Microsoft.AspNetCore.Authentication.JwtBearer"
        options.DefaultAuthentcateSheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthentcationScheme;
    })
    // hoặc ".AddAuthentication(JwtBearerDefaults.AuthenticationScheme)" cho ngắn gọn
    .AddJwtBearer(options => 
    { // validate token
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        // import "TokenValidationParameters" from "Microsoft.IdentityModel.Tokens"
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ClockSkew = TimeSpan.Zero, // force token to expire exactly at token expiration time
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            // ValidateIssuerSigningKey = true, // related to X509 certificate

        };
    });

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```


## Login Action

```cs - ~/Data/Users.cs - table "Users" in database
[Table("NguoiDung")]
public class Users
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    // ta sẽ cần cấu hình thêm fluent API để đảm bảo "UserName" là unique
    public string UserName { get; set; }

    [Required]
    [MaxLength(250)]
    public string Password { get; set; }
    public string Role { get; set; }

    public string Email { get; set; }
}

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>(entity => {
            entity.HasIndex(e => e.UserName).IsUnique();
        })
    }
}
```

```cs - ~/Models/LoginModel.cs
public class LoginModel
{
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [MaxLength(250)]
    public string Password { get; set; }
}
```

```cs - controller for handle Auth action
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly AppSettings _appSettings;

    public UserController(MyDbContext db, IOptions<AppSettings> appSettings)
    {
        _context = db;
        _appSettings = appSettings.Value; // ta có thể DI IConfiguration để lấy AppSettings cũng được
    }

    [HttpPost]
    public IActionResult Login(LoginModel model) 
    {
        string username = model.UserName;
        string password = model.Password;

        Users loginUser =  CreateDummyUsers() // mock data
            .Where(a => a.UserName == username && a.Password == password).FirstOrDefault();
        // Users loginUser = _context.Users.SingleOrDefault(
        //    p => p.UserName == username && p.Password == password);

        if (loginUser == null) return new JsonResult("Login Failed!");

        // generate token
        var claims = new Claim[]
        { // mấy thằng này sẽ tạo ra property trong phần "header" của chuỗi JWT
            new Claim(ClaimTypes.Name, user.UserName), // "unique_name": "Admin"
            new Claim(ClaimTypes.Email, user.Email), // "email": "admin@gmail.com"
            new Claim("UserName", user.UserName), // "UserName" "Admin"
            new Claim("id", user.Id), // "id": "1"
            new Claim(ClaimTypes.Role, loginUser.Role),
            new Claim("TokenId", Guid.NewGuid().ToString())
        };
        var accessToken = GenerateJSONWebToken(claims);

        return new JsonResult(accessToken);
    }

    private string GenerateJSONWebToken(Claim[] claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler(); // import from "System.IdentityModel.Tokens.Jwt"

        // import "SymmetricSecurityKey" from "System.IdentityModel.Tokens"
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)); 
        // hoặc: var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

        // import "SigningCredentials" from "System.IdentityModel.Tokens"
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // signing

        // import "SecurityTokenDescriptor" from "Microsoft.IdentityMode.Tokens"
        var tokenDescriptor = new SecurityTokenDescriptor
        {
           Subject = new ClaimIdentity(claims), // from "System.Security.Claims.ClaimsIdentity"
           Expires = DateTime.UtcNow.AddMinutes(20),
           SigningCredentials = new SigningCredentials(
               new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // hoặc
        // var token = new JwtSecurityToken  // import from "System.IdentityModel.Tokens.Jwt"
        // {
        //     issuer: "https://abc",
        //     audience: "https://abc",
        //     expires: DateTime.UtcNow.AddMinutes(20),
        //     SigningCredentials: credentials,
        //     claims: claims
        // };

        return tokenHandler.WriteToken(token);
    }

    private List<Users> CreateDummyUsers() // Database
    {
        List<Users> userList = new List<Users> 
        {
            new Users { UserName = "a", Password = "a", Role = "Admin" },
            new Users { UserName = "b", Password = "b", Role = "Manager" },
            new Users { UserName = "c", Password = "c", Role = "Developer" }
        }
        return userList;
    }
}
```

## Usage

```cs
[Authorize] // make every actions in HomeController require "Authentication"
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
}


[ApiController]
[Route("api/[controller]")]
public class LoaiController : ControllerBase
{
    [HttpPost]
    [Authorize] // auth specific action
    public IActionResult CreateNew(LoaiModel model)
    {
    }
}
```

```js - send request
// gọi "/Login" action để lấy được token rồi copy bỏ vô Postman để gửi request:
axios.get('https://url', {}, { 
    Authorization: `Bearer ${token}` 
});
```

=======================================================================
> ta sẽ sử dụng 'cookie' và 'middleware' để tự động set và check `token`

# combine 'Cookie' 

```cs - modify "Login" action
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public IActionResult Login(string username, string password) 
    {
        var accessToken = GenerateJSONWebToken(claims);
        setJWTCookie(accessToken);

        return new JsonResult("Login Success");
    }

    private void SetJWTCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddHours(3)
        };
        Response.Cookies.Append("jwtCookie", token, cookieOptions);
    }
}
```

# Create a custom "Middleware" for automatically check "token" in cookie
* -> _vì ta đã sử dụng **HttpOnly**, nên các client app không thể sử dụng `Javascript` để đọc giá trị từ `cookie`, vậy nên cũng không thể tạo ra `Authorization header - Bearer Token` được_
* -> _mà server thì đang kiểm tra token thông qua **Authorization Bearer**_
* -> _vậy nên tại server ,trước khi kiểm tra token nó sẽ can thiệp vào `HttpRequest` object trước đó 
* -> bằng cách tự tạo thêm 1 `Authorization header - Bearer token` và lấy giá trị của của cookie bỏ vào giá trị của nó_

```cs
public class JWTInHeaderMiddleware
{
    private readonly RequestDelegate _next;
    public JWTInHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var name = "jwtCookie";
        var cookie = context.Request.Cookies[name];

        if (cookie != null && !context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Append("Authorization", "Bearer " + cookie);
        }
        await _next.Invoke(context);
    }
}
```

```cs - đăng ký middleware
// Startup.cs
app.Routing();
app.UseMiddleware<JWTInHeaderMiddleware>();
```

```cs - check quyền "Admin"
[Authorize(Roles = "Admin")] // require "Authentication"; and "Authorization" with Role = "Admin"
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
}
// -> vậy nên giờ để truy cập được action trong "AdminController" đòi hỏi ta phải login bằng "username" là "a" và "password" là "a" (vì thằng "a" này đang có Role là "Admin")
// -> nếu không nó sẽ trả về 403: Forbidden
```

=======================================================================

# 'refresh token' in Authentication
* -> client gửi **access token** cho server để trả về **protected resource**; nhưng token này có thời hạn nhất định khi đó thì **`đòi hỏi user login lại`**
* -> để tránh điều này, thì khi user (client) **`login success`** thì **Authorization server** sẽ trả về 1 bộ **access token & refresh token**
* -> khi cần **`tạo mới 1 bộ "access token - refresh token"`** thì client sẽ gửi **refresh token** này cho **Authorization Server**

* trong App client của ta (React, Vue, ...) ta sẽ cần kiểm tra thời gian expire của token để gửi lên

# Tạo token Model
```cs - ~/Models/TokenModel.cs
public class TokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
```

# Tạo Entity
```cs - ~/Data/RefreshToken.cs
[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public Users Users { get; set; }

    public string Token { get; set; } // value of Refresh Token
    public string JwtId { get; set; } // ID of Access Token
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpriredAt { get; set; }
}

public class MyDbContext : DbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
```

# action to create new "access token" and "refresh token" 
```cs
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model) 
    {
        // ......

        var token = await GenerateJSONWebToken(claims);

        return new JsonResult(accessToken);
    }

    [HttpPost("RenewToken")]
    public async Task<IActionResult> RenewToken(TokenModel model)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

        // tương tự như param ta bỏ trong service ".useAuthentication()"
        var tokenValidateParam = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://abc",
            ValidAudience = "https://abc",
            ClockSkew = TimeSpan.Zero, 
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

            ValidateLifeTime = false // important
            // -> để "jwtTokenHandler.ValidateToken" không kiểm tra token hết hạn
            // -> nếu không khi token hết hạn nó sẽ throw Exception ngay lập tức    
        };

        try 
        {
            // check 1: AccessToken valid format
            var tokenInVerification = jwtTokenHandler.ValidateToken(
                model.AccessToken, tokenValidateParam, out var validatedToken);
            
            // check 2: check 'algorithm' in token header
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                if (!result) return new JsonResult("Invalid Token");
            }

            // check 3: check if "access token" expired
            var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            if (expireDate > DateTime.UtcNow) return new JsonResult("Access Token has not yet expired");

            // check 4: check if "refresh token" existed in DB
            var storedRefreshToken = _context.RefreshTokens.FirstOrDefault(
                x => x.Token == model.RefreshToken);
            if (storedRefreshToken) return new JsonResult("Refresh token doesn't exist");

            // check 5: check if "refresh token is used / revoked ?
            if (storedRefreshToken.IsUsed) return new JsonResult("Refresh token has been used");
            if (storedRefreshToken.IsRevoked) return new JsonResult("Refresh token has been revoked");

            // check 6: AccessToken Id == RefreshToken JwtId
            var jti = tokenInVerification.Claims.FirstOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedRefreshToken.JwtId != jti) return new JsonResult("tokens doesn't match");

            // mark refresh token as used
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.IsUsed = true;
            _context.Update(storedToken);
            await _context.SaveChangesAsync();

            // create new "access token - refresh token"
            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == storedToken.UserId);
            var tokens = await GenerateJSONWebToken(user);
            return new JsonResult(tokens);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest("something went wrong")
        }
    }

    private async Task<TokenModel> GenerateJSONWebToken(Users user)
    {
        var claims = new Claim[]
        { 
            new Claim(ClaimTypes.Name, user.UserName), 
            new Claim(JwtRegisteredClaimNames.Email, user.Email), 
            new Claim(JwtRegisteredClaimNames.Sub, user.Email), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // access token Id
            new Claim("UserName", user.UserName),
            new Claim("Id", user.Id), 
            new Claim(ClaimTypes.Role, loginUser.Role),
        };

        // ......

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        // Lưu refresh token vào database
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            JwtId = token.Id, // lấy value ta set trong "JwtRegisteredClaimNames.Jti"
            UserId = user.Id,
            Token = refreshToken,
            IsUsed = false,
            IsRevoked = false,
            IssuedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddHours(1)
        };
        await _context.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new TokenModel
        {
            AccessToken =  accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateRefreshToken()
    { // tạo đại 1 chuỗi random
        var random = new byte[32];
        // import "RandomNumberGenerator" from "System.Security.Cryptography"
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }
    }

    private object ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
        return dateTimeInterval;
    }
}
```

=======================================================================
> ASP.NET Core Identity hỗ trợ 1 số UI; cung cấp các **`identity model`** cho phép ta customize 
> https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-8.0
> ta sẽ sử dụng Role-based authorization; và Identity đã hỗ trợ cho ta sẵn 1 số Role

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