
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
    {
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
* _`/Login` action kiểm `usename-password` gửi tới với cái đã lưu trong `Database`_
* _nếu oke thì tạo 1 `Token` và set vào `cookie` với tên "jwtCookie"; những request phía sau sẽ có cookie này và `server chỉ cần kiểm tra cookie này là được`_
* _khi ta `decode` token này ra ta sẽ thấy 3 phần `Header.Payload.Signature`_

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

        var accessToken = GenerateJSONWebToken();
        setJWTCookie(accessToken);

        return new JsonResult(accessToken);
    }

    private string GenerateJSONWebToken()
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

    private List<Users> CreateDummyUsers()
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

public class Users
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
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