

# SQL Injection
* -> Attackers manipulate user input in a way that alters your SQL query logic
* -> Risk: Attackers can manipulate SQL queries to access or modify database data.

## Method
```cs
// our vulnerable API
var query = $"SELECT * FROM Users WHERE Id = {id}";
```
```sql
-- Suppose a user enters "1; DROP TABLE Users; --" as input
-- Database will execute:
SELECT * FROM Users WHERE Id = 1; DROP TABLE Users;
-- => this drops your entire Users table
```

## Mitigation:
* -> always use **`parameterized queries`** if **`raw queries`** are necessary (Ex: **Dapper** use @parameter); this treat inputs as data, not executable SQL
* -> **Entity Framework** prevents SQL injection when **`using LINQ queries`**
* -> avoid raw SQL unless necessary
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
* -> Attackers inject JavaScript that executes in the victim’s browser
* -> Risk: Attackers inject malicious scripts into web pages viewed by other users

## Method
* -> A user submits <script>alert('Hacked!');</script> in a comment section.
* -> If your app does not sanitize it, the script executes for any user viewing that comment

## Mitigation:
* -> **Sanitize and escape user input before rendering in the frontend**
* => **React have built-in XSS protection** that automatically escapes content by default;
* => but avoid **dangerouslySetInnerHTML** unless absolutely necessary, sanitize input (_using **DOMPurity**_) if we must use it 

```js
// ❌ Vulnerable Code (React)
const userInput = "<script>alert('Hacked!')</script>";
return <div dangerouslySetInnerHTML={{ __html: userInput }} />; // BAD PRACTICE

// ✅ Secure Implementation React  escapes content, 
import DOMPurify from 'dompurify';

const safeInput = DOMPurify.sanitize(userInput);
return <div dangerouslySetInnerHTML={{ __html: safeInput }} />;
```

* -> implement **`Content Security Policy (CSP)`** to restrict script execution (_blocks inline scripts unless explicitly allowed_)
* => "Content Security Policy (CSP)" in **web.config (ASP.NET Core) or HTTP headers**:
```cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'");
    // context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'nonce-random123';"); 
    await next();
});
```

* -> consider **HTTP security headers middleware**
```cs
app.UseHsts();
app.UseXfo(options => options.Deny());
app.UseXXssProtection(options => options.EnabledWithBlockMode());
```

# Cross-Site Request Forgery (CSRF)
* -> A malicious website tricks a logged-in user into sending a request on their behalf
* -> Risk: Attackers trick users into performing actions they didn’t intend

## Method:
* -> User logs into your banking site.
* -> Without logging out, they visit evil.com.
* -> "evil.com" executes:
```html
<img src="http://yourbank.com/transfer?amount=5000&to=hacker_account">
```
* -> Since the user is logged in, the request is processed!

## Mitigation:
* -> use **`anti-CSRF tokens`** (_ASP.NET Core provides built-in support for CSRF protection_) to ensure every request is intentional (the hacker won’t have the token)
* -> implement **`SameSite cookies`** (**Strict** or **Lax** mode) to prevent unauthorized cross-site requests
* -> require **re-authentication** for sensitive actions to stop critical actions from being executed without user confirmation
* -> use **`Double Submit Cookie pattern`** for APIs - set a CSRF token in a secure cookie and validate it in a custom middleware before executing state-changing requests

```cs
// ❌ Vulnerable API (No CSRF Protection)
// -> if we allow state-changing requests (POST, PUT, DELETE) without CSRF protection, an attacker can trick a logged-in user into making unauthorized changes.
[HttpPost("update-profile")]
public IActionResult UpdateProfile([FromBody] UserProfile profile)
{
    // No CSRF protection - BAD PRACTICE
    _context.Users.Update(profile);
    _context.SaveChanges();
    return Ok();
}

// ✅ Secure Implementation (Using Anti-CSRF Tokens)
// -> Enable CSRF protection by ASP.NET Core Antiforgery Middleware:
services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Token sent via header
});

// -> Generate a CSRF token and send it to the frontend:
[HttpGet("csrf-token")]
public IActionResult GetCsrfToken([FromServices] IAntiforgery antiforgery)
{
    var tokens = antiforgery.GetAndStoreTokens(HttpContext);
    return Ok(new { token = tokens.RequestToken });
}
```
```js
// -> in React, send the CSRF token with each request:
fetch("/update-profile", {
    method: "POST",
    headers: {
        "Content-Type": "application/json",
        "X-CSRF-TOKEN": csrfToken // Retrieved from API
    },
    body: JSON.stringify(profile)
});
```

# Broken Authentication & Session Management
* -> Weak authentication allows attackers to impersonate users
* -> risk: Weak authentication mechanisms allow unauthorized access

## Method:
* -> If passwords are stored in plain text, a database breach exposes all user credentials.
* -> If JWT tokens never expire, a stolen token can be reused indefinitely

## Mitigation:
* -> use ASP.NET Core Authentication Middleware with **`JWT`**, ASP.NET Core Identity or a secure authentication provider like OAuth2/OIDC
* -> enforce **`multi-factor authentication (MFA)`** where applicable to make password leaks useless to attackers
* -> **`secure authentication cookies`** (HttpOnly, Secure, SameSite=Strict) to protect against theft via JavaScript
* -> implement proper **`session expiration, token expiration, rotate tokens periodically, refresh tokens and logout mechanisms`** to prevent long-term unauthorized access
* -> **`hashing passwords (BCrypt, Argon2)`** prevents password leaks
* -> implement **`rate limiting`** to prevent **brute-force attacks** (attacker tries many password combinations)

```bash
# Secure cookies 
options.Cookie.HttpOnly = true;
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
```

```cs
// ❌ Vulnerable Code (No JWT, No Expiration)
[HttpPost("login")]
public IActionResult Login([FromBody] LoginModel model)
{
    var user = _context.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);
    if (user == null) return Unauthorized();

    return Ok(new { Token = "hardcoded-token" }); // BAD PRACTICE
}

// ✅ Secure Implementation (Using JWT Authentication)

// -> Install JWT Authentication:
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
// configure authentication in "Program.cs":
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

// -> generate a JWT token on login:
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

// -> Apply authentication in protected endpoints:
[Authorize]
[HttpGet("secure-data")]
public IActionResult GetSecureData()
{
    return Ok("This is secured data");
}
```

# Insecure Direct Object References (IDOR)
* -> Attackers manipulate IDs to access unauthorized data
* -> Risk: Attackers manipulate object IDs to access unauthorized data

## Method
* -> User John fetches their profile with /user/123.
* -> They modify the URL to /user/124 and see another user’s data

## Mitigation:
* -> validate user **`authorization`** before accessing any sensitive resources
* -> use **ASP.NET Core Authorization Policies** for fine-grained access control (_Ex: use Authorization Middleware to apply Claims-based authorization_)
* -> avoid exposing database IDs directly; **`use GUIDs or hashed IDs`** (_use GUIDs instead of incremental IDs for sensitive data like user references to make enumeration/guessing IDs harder_)

```cs
// ❌ Vulnerable API (No Authorization Check)
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    var user = _context.Users.FirstOrDefault(u => u.Id == id);
    return Ok(user); // BAD PRACTICE: Users can access others' data
}

// ✅ Secure Implementation (Check Ownership)
[Authorize]
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    if (userId != id) return Forbid(); // Prevent unauthorized access

    var user = _context.Users.FirstOrDefault(u => u.Id == id);
    return Ok(user);
}
```

# Security Misconfigurations
* -> Misconfigured servers, frameworks, or default settings expose vulnerabilities
* -> Risk: Default settings, unnecessary services, or debug logs expose vulnerabilities.

## Method
* -> Running in development mode shows detailed error messages with stack traces (useful to attackers).
* -> Allowing HTTP instead of enforcing HTTPS lets attackers intercept data

## Mitigation:
* -> **`disable detailed error messages`** in production to hides internal logic
```cs
if (!app.Environment.IsDevelopment()) // production enviroment
{
    // Generic error handling for production
    app.UseExceptionHandler("/Error"); 
    app.UseHsts(); // Enforce HTTPS
}
else
{
    // Enable developer-friendly error pages only in development
    // Show detailed errors only in development
    app.UseDeveloperExceptionPage(); 
}
```
```json
// Ensure detailed error logging is disabled in "appsettings.Production.json":
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "DetailedErrors": false
}
```

* -> Configure **`HTTPS (SSL/TLS)`** to ensure encrypted communication
* _for production, ensure your web server (IIS, Nginx, Apache) enforces HTTPS and disables weak TLS versions (like TLS 1.0 and 1.1)_
```cs
// Force "HTTPS" redirection in Program.cs:
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443; // Ensure HTTPS is used
});

var app = builder.Build();

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
```
```json
// In appsettings.json, configure Kestrel to use HTTPS:
"Kestrel": {
  "Endpoints": {
    "HttpsInlineCertFile": {
      "Url": "https://localhost:443",
      "Certificate": {
        "Path": "certificate.pfx",
        "Password": "your-secure-password"
      }
    }
  }
}
```

* -> **`harden server configurations`** (disable unused endpoints, secure headers)
```cs
// remove default endpoints like Swagger in production:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// restrict CORS (Cross-Origin Resource Sharing) to allow requests only from trusted origins:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins("https://your-frontend.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
```

* -> implement **`security headers`** like **X-Content-Type-Options**, **X-Frame-Options**, and **Strict-Transport-Security**
* _help prevent attacks like **clickjacking, MIME-sniffing, and downgrade attacks**_
```cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Prevent MIME sniffing
    context.Response.Headers.Add("X-Frame-Options", "DENY"); // Prevent clickjacking
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload"); // Enforce HTTPS
    context.Response.Headers.Add("Referrer-Policy", "no-referrer"); // Restrict referrer info
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=()"); // Limit browser permissions

    await next();
});
```

# Denial of Service (DoS)
Attackers send excessive requests to slow down or crash a server.

# Method:
* -> a bot sends millions of requests that overwhelms the server


## Mitigation:
* -> **`rate limiting`** in ASP.NET Core:
```cs
services.Configure<IpRateLimitOptions>(options => 
{ 
    options.GeneralRules = new List<RateLimitRule> 
    {
        new RateLimitRule { Endpoint = "*", Limit = 100, Period = "1m" }
    }; 
});
```

* -> Use **Cloudflare** or **WAF** to **`block attack patterns`**