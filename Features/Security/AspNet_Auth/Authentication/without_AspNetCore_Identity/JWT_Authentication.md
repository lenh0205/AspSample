> vì project này là WebAPI và chưa có client - nên ví dụ này đòi hỏi ta phải thao tác tay với Postman để copy paste token
> để có thể `Authenticate` và `Authorize`, thì JWT đòi hỏi phải chứa **user info**, **role**, **expire time**
> Lưu ý: "refresh token" cần được lưu trong database còn "access token" thì không 

=======================================================================
# Token-based Authentication
* -> **Login action** sẽ kiểm tra `usename-password` gửi tới với cái đã lưu trong `Database`; nếu oke thì **`generate 1 Token cho user`** 
* -> user sau này khi gửi request sẽ đính kèm `token` vào **Authorization header**_
* -> server sẽ có 1 **middleware** sử dụng **`secret key`** để kiểm tra tính hợp lệ của token này

* _nếu token is **invalid** hoặc **expired** thì nó sẽ trả về **`403: UnAuthorized`**_
* _khi ta `decode` token này ra ta sẽ thấy 3 phần `Header.Payload.Signature`_

=======================================================================
# Lưu 'secret key' (sử dụng config appsetting và tạo class để truy xuất nó)

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

=======================================================================
# Cấu hình Authentication với JWT 
* -> install **System.IdentityModel.Tokens.Jwt**
* -> install **Microsoft.AspNetCore.Authentication.JwtBearer** 

```cs - Startup.cs
// -> thuật toán chỉ chạy trên bit nên ta sẽ cần Encode 'secret key'
var secretKey = Configuration["JWT:Secret"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services
    .AddAuthentication(option => { // sử dụng lại Authen nào ? (cookie-base, jwt bearer, ...)
        // import "JwtBearerDefaults" from "Microsoft.AspNetCore.Authentication.JwtBearer"
        options.DefaultAuthenticateSheme = JwtBearerDefaults.AuthenticationScheme;
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

=======================================================================
# Tạo "User" table

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

=======================================================================
# Login Action - generate token for user
* -> create a controller to handle Auth action

```cs - 
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
}
```

```cs - helper method
private string GenerateJSONWebToken(Claim[] claims)
{
    // import "SymmetricSecurityKey" from "System.IdentityModel.Tokens"
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)); 
    // hoặc: var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

    // import "SigningCredentials" from "System.IdentityModel.Tokens"
    var signingCredentials =  new SigningCredentials(
            new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature);// signing

    // import "SecurityTokenDescriptor" from "Microsoft.IdentityMode.Tokens"
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimIdentity(claims), // from "System.Security.Claims.ClaimsIdentity"
        Expires = DateTime.UtcNow.AddMinutes(20),
        SigningCredentials = signingCredentials
    };

    var tokenHandler = new JwtSecurityTokenHandler(); // import from "System.IdentityModel.Tokens.Jwt"
    var token = tokenHandler.CreateToken(tokenDescriptor);

    // hoặc
    // var token = new JwtSecurityToken  // import from "System.IdentityModel.Tokens.Jwt"
    // {
    //     issuer: "https://abc",
    //     audience: "https://abc",
    //     expires: DateTime.UtcNow.AddMinutes(20),
    //     SigningCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
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

=======================================================================
## Usage

```js - client send request
// gọi "/Login" action để lấy được token rồi copy bỏ vô Postman để gửi request:
axios.get('https://url', {}, { 
    Authorization: `Bearer ${token}` 
});
```

```cs - apply "authentication" to endpoint
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

```cs - apply authorize
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
> đây là phần mở rộng để tránh việc ta phải copy paste `token` để test
> ta sẽ sử dụng 'cookie' và 'middleware' để tự động set và check `token`

#  add 'token' to'Cookie' when 'login' success  

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

# Create a custom "Middleware" for processing "token" in cookie before authentication 
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

=======================================================================
# 'Refresh token' in Authentication
* -> client gửi **access token** cho server để trả về **protected resource**; nhưng token này có thời hạn nhất định khi đó thì **`đòi hỏi user login lại`**
* -> để tránh điều này, thì khi user (client) **`login success`** thì **Authorization server** sẽ trả về 1 bộ **access token & refresh token**
* -> khi cần **`tạo mới 1 bộ "access token - refresh token"`** thì client sẽ gửi **refresh token** này cho **Authorization Server**

* trong App client của ta (React, Vue, ...) ta sẽ cần kiểm tra thời gian expire của token để gửi lên

## Time to use "refresh token" in SPA + WebAPI
* -> **`SPA`** should be the one **handles the token refresh process**
* -> because **SPA** is responsible for **`storing the tokens`**; knows when it **`received the tokens`** and **`can track their expiration`**
* -> the **Web API** should remain **`stateless`** and only validate the tokens it receives

* -> time to use a refresh token is **`shortly before the access token expires`** - this approach, often called **proactive token refresh** to helps ensure uninterrupted user experience
* -> **SPA** will **`automatically initiate the refresh process`** when **the access token is close to expiring**

* -> for best practice, we should also have **Reactive (on-demand) Refresh token mechanism** 
* -> if **an API call fails due to an expired token** (_usually a 401 Unauthorized response_), the SPA should will **`attempt a token refresh`**, and then **`retry the original request`**

## Tạo Token Model
```cs - ~/Models/TokenModel.cs
public class TokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
```

## Tạo 'RefreshToken' Entity và lưu nó vào database
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

## update AuthController to integrate "refresh token"
* -> giờ **Login action** sẽ trả về 1 cặp **`Access Token - Refresh Token`**
* -> ta sẽ thêm 1 **endpoint** để **`renew và trả về cặp token`** - thằng này cần truyền vào cặp token cũ

```cs - main endpoint
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

    // method này làm nhiệm vụ validate token và generate bộ toke mới
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

            // check 5: check if "refresh token" is used / revoked ?
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
}
```

```cs - helper method
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
```

