

# SQL Injection
* -> Risk: Attackers can manipulate SQL queries to access or modify database data.


## Mitigation:
* -> **`Entity Framework`** prevents SQL injection when **`using LINQ queries`**
* -> always use **`parameterized queries`** if **`raw queries`** are necessary, Ex: **Dapper** 
* -> avoid raw SQL unless necessary.
* -> use **`Stored Procedures`** 

```cs
// Vulnerable Code: If you use raw SQL queries with string concatenation, an attacker can inject SQL code
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    var query = $"SELECT * FROM Users WHERE Id = {id}"; // BAD PRACTICE
    var user = _context.Users.FromSqlRaw(query).FirstOrDefault();
    return Ok(user);
}

// Secure Implementation (Using Parameterized Queries in Entity Framework):
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    var user = _context.Users.FirstOrDefault(u => u.Id == id);
    return user != null ? Ok(user) : NotFound();
}
```

# Cross-Site Scripting (XSS)
* -> Risk: Attackers inject malicious scripts into web pages viewed by other users

## Mitigation:
* -> Sanitize and escape user input before rendering in the frontend
* -> Use React’s built-in XSS protection (React escapes content by default)
* -> Implement Content Security Policy (CSP) to restrict script execution

# Cross-Site Request Forgery (CSRF)
Risk: Attackers trick users into performing actions they didn’t intend.
Mitigation:
Use anti-CSRF tokens (ASP.NET Core provides built-in support for CSRF protection).
Implement SameSite cookies (Strict or Lax mode).
Require re-authentication for sensitive actions.

# Broken Authentication & Session Management
Risk: Weak authentication mechanisms allow unauthorized access.
Mitigation:
Use ASP.NET Core Identity or a secure authentication provider like OAuth2/OIDC.
Enforce multi-factor authentication (MFA) where applicable.
Secure authentication cookies (HttpOnly, Secure, SameSite=Strict).
Implement proper session expiration and logout mechanisms.

# Insecure Direct Object References (IDOR)
Risk: Attackers manipulate object IDs to access unauthorized data.
Mitigation:
Validate user authorization before accessing any sensitive resources.
Use ASP.NET Core Authorization Policies for fine-grained access control.
Avoid exposing database IDs directly (use GUIDs or hashed IDs).

# Security Misconfigurations
Risk: Default settings, unnecessary services, or debug logs expose vulnerabilities.
Mitigation:
Disable detailed error messages in production.
Configure HTTPS (SSL/TLS) for secure communication.
Harden server configurations (disable unused endpoints, secure headers).
Implement security headers like X-Content-Type-Options, X-Frame-Options, and Strict-Transport-Security.




2. Cross-Site Scripting (XSS)
❌ Vulnerable Code (React)

jsx
Copy
Edit
const userInput = "<script>alert('Hacked!')</script>";
return <div dangerouslySetInnerHTML={{ __html: userInput }} />; // BAD PRACTICE
✅ Secure Implementation React automatically escapes content, but avoid dangerouslySetInnerHTML unless absolutely necessary. If you must use it, sanitize input:

jsx
Copy
Edit
import DOMPurify from 'dompurify';

const safeInput = DOMPurify.sanitize(userInput);
return <div dangerouslySetInnerHTML={{ __html: safeInput }} />;
🔧 Tools/Libraries:

React default escaping
DOMPurify (npm install dompurify) for sanitization
Content Security Policy (CSP) in web.config (ASP.NET Core) or HTTP headers:
csharp
Copy
Edit
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'");
    await next();
});
3. Cross-Site Request Forgery (CSRF)
❌ Vulnerable API (No CSRF Protection) If you allow state-changing requests (POST, PUT, DELETE) without CSRF protection, an attacker can trick a logged-in user into making unauthorized changes.

csharp
Copy
Edit
[HttpPost("update-profile")]
public IActionResult UpdateProfile([FromBody] UserProfile profile)
{
    // No CSRF protection - BAD PRACTICE
    _context.Users.Update(profile);
    _context.SaveChanges();
    return Ok();
}
✅ Secure Implementation (Using Anti-CSRF Tokens)

Enable CSRF protection in ASP.NET Core:

csharp
Copy
Edit
services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Token sent via header
});
Generate a CSRF token and send it to the frontend:

csharp
Copy
Edit
[HttpGet("csrf-token")]
public IActionResult GetCsrfToken([FromServices] IAntiforgery antiforgery)
{
    var tokens = antiforgery.GetAndStoreTokens(HttpContext);
    return Ok(new { token = tokens.RequestToken });
}
In React, send the CSRF token with each request:

jsx
Copy
Edit
fetch("/update-profile", {
    method: "POST",
    headers: {
        "Content-Type": "application/json",
        "X-CSRF-TOKEN": csrfToken // Retrieved from API
    },
    body: JSON.stringify(profile)
});
🔧 Tools/Libraries:

ASP.NET Core Antiforgery Middleware
SameSite Cookies (Lax or Strict)
Frontend: Axios/Fetch API to send CSRF token
4. Broken Authentication & Session Management
❌ Vulnerable Code (No JWT, No Expiration)

csharp
Copy
Edit
[HttpPost("login")]
public IActionResult Login([FromBody] LoginModel model)
{
    var user = _context.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);
    if (user == null) return Unauthorized();

    return Ok(new { Token = "hardcoded-token" }); // BAD PRACTICE
}
✅ Secure Implementation (Using JWT Authentication)

Install JWT Authentication:

sh
Copy
Edit
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
Configure authentication in Program.cs:

csharp
Copy
Edit
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourapp.com",
            ValidAudience = "yourapp.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"))
        };
    });
Generate a JWT token on login:

csharp
Copy
Edit
var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.UTF8.GetBytes("YourSecretKey");
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Email) }),
    Expires = DateTime.UtcNow.AddHours(1),
    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
};
var token = tokenHandler.CreateToken(tokenDescriptor);
return Ok(new { Token = tokenHandler.WriteToken(token) });
Use authentication in protected endpoints:

csharp
Copy
Edit
[Authorize]
[HttpGet("secure-data")]
public IActionResult GetSecureData()
{
    return Ok("This is secured data");
}
🔧 Tools/Libraries:

ASP.NET Core Authentication Middleware
JWT (JSON Web Token)
Secure Cookies (HttpOnly, Secure, SameSite=Strict)
5. Insecure Direct Object References (IDOR)
❌ Vulnerable API (No Authorization Check)

csharp
Copy
Edit
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    var user = _context.Users.FirstOrDefault(u => u.Id == id);
    return Ok(user); // BAD PRACTICE: Users can access others' data
}
✅ Secure Implementation (Check Ownership)

csharp
Copy
Edit
[Authorize]
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    if (userId != id) return Forbid(); // Prevent unauthorized access

    var user = _context.Users.FirstOrDefault(u => u.Id == id);
    return Ok(user);
}
🔧 Tools/Libraries:

ASP.NET Core Authorization Middleware
Claims-based authorization (User.Identity)
Use GUIDs instead of incremental IDs for sensitive data
Final Thoughts
Security is an ongoing process. Regularly review your code and follow best practices: ✅ Use parameterized queries to prevent SQL Injection
✅ Sanitize user input and enable CSP to prevent XSS
✅ Implement CSRF protection using antiforgery tokens
✅ Secure authentication with JWT and session management
✅ Enforce authorization checks to prevent IDOR attacks