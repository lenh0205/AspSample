# Content
* _`/Login` action kiểm `usename-password` gửi tới với cái đã lưu trong `Database`_
* _nếu oke thì generate 1 cái `Token` từ `Claims`_
* _và set `Token` vào `cookie` với tên "jwtCookie"; những request phía sau sẽ có cookie này và `server chỉ cần kiểm tra cookie này là được`_
* _khi ta `decode` token này ra ta sẽ thấy 3 phần `Header.Payload.Signature`_

# Setup NuGet package
* -> install **Microsoft.AspNetCore.Authentication.JwtBearer**
* -> íntall **System.IdentityModel.Tokens.Jwt**

```cs - Startup.cs
services
    .AddAuthentication(option => {
        // import "JwtBearerDefaults" from "Microsoft.AspNetCore.Authentication.JwtBearer"
        options.DefaultAuthentcateSheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthentcationScheme;
    })
    .AddJwtBearer(options => 
    { // validate token
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://abc",
            ValidAudience = "https://abc",
            ClockSkew = TimeSpan.Zero; // force token to expire exactly at token expiration time
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SomethingSecret"));
        };
    });


app.UseAuthentication();
app.UseAuthorization();
```

# Login Action

```cs - Users.cs
public class Users
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
```

```cs
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public IActionResult Login(string username, string password) 
    {
        Users loginUser = CreateDummyUsers()
            .Where(a => a.UserName == username && a.Password == password).FirstOrDefault();
        if (loginUser == null) return new JsonResult("Login Failed");

        var claims = new[] 
        {
            new Claim(ClaimTypes.Role, loginUser.Role)
        };
        var accessToken = GenerateJSONWebToken(claims);
        setJWTCookie(accessToken);

        return new JsonResult(accessToken);
    }

    private string GenerateJSONWebToken(Claim[] claims)
    {
        // import "SymmetricSecurityKey", "SymmetricSecurityKey" from "System.IdentityModel.Tokens"
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SomethingSecret")); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // signing

        // import "JwtSecurityToken", "JwtSecurityTokenHandler" from "System.IdentityModel.Tokens.Jwt"
        var token = new JwtSecurityToken   
        {
            issuer: "https://abc",
            audience: "https://abc",
            expires: DateTime.Now.AddHours(3),
            SigningCredentials: credentials,
            claims: claims
        }
        return new JwtSecurityTokenHandler().WriteToken(token);
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
```

```js - send request
axios.get('https://url', {}, { 
    Authorization: `Bearer ${token}` 
});
```

# Create a custom "Middleware" to modify request at server - create 'Authorization' header from 'cookie' header
* _vì ta đã sử dụng `HttpOnly`, nên các client app không thể sử dụng `Javascript` để đọc giá trị từ `cookie`, vậy nên cũng không thể tạo ra `Authorization header - Bearer Token` được_
* _mà server thì đang kiểm tra token thông qua `Authorization Bearer`_
* _tức là hiện tại user muốn truy cập vào `protected resouces`, thì đòi hỏi họ phải tự vào DevTook copy giá trị của Token trong Cookie, rồi dùng 1 công cụ gọi API như `Postman` để paste token vô mới gọi được_
* => _vậy nên tại server trước khi kiểm tra token, nó sẽ can thiệp vào `HttpRequest` object trước đó - bằng cách tạo thêm 1 `Authorization header - Bearer token` rồi nhét giá trị của của cookie vào_

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

```cs - Startup.cs
app.Routing();
app.UseMiddleware<JWTInHeaderMiddleware>();
```

```cs
[Authorize(Roles = "Admin")] // require "Authentication"; and "Authorization" with Role = "Admin"
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
}
// -> vậy nên giờ để truy cập được action trong "AdminController" đòi hỏi ta phải login bằng "username" là "a" và "password" là "a"
// -> nếu không nó sẽ trả về 403: Forbidden
```