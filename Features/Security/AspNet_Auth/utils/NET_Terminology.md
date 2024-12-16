> **`Lockout`** - refers to a security feature that **temporarily disables a user's account** after **a certain number of failed login attempts**

=======================================================================
# claims-based access control
* -> with the rise of `ASP.NET Core over ASP.NET 4.x`, the built in authentication has undergone a shift from **role-based access control (RBAC)** to **claim-based access control (CBAC)**
* -> the most notable change is the **`User` property on `HttpContext`** is now of type **ClaimsPrincipal** instead of **IPrincipal**

* -> regardless of whether it's **cookie-based** or **token-based** auth, when **`user is authenticated`** the **authentication middleware** will creates a **`ClaimPrincipal`** that represent authenticated user
* -> the **ClaimPrincipal** and **ClaimIdentity** class are fundamental to the **`claims-based identity model`** used in ASP.NET Core for processing auth

* => provides **`a consistent way`** to **handle `user identity` and `claims` across different `authentication schemes`**

# role-based access control
* -> before ASP.NET 4.5, ASP.NET primarily used a role-based access control
* -> there key interface are **`IPrincipal`** and **`IIdentity`**

```cs
if (User.IsInRole("Admin"))
{
    // Perform admin actions
}
```

## "ClaimsPrincipal" and "ClaimsIdentity"
* -> is indeed implemented the existing **IPrincipal** and **IIdentity** interfaces for **`backwards compatibility`**

* -> however, it's more flexible as **claims** can **`represent any type of user information, not just roles`**
* -> easier to **add custom claims** without **`modifying the underlying security system`**
* -> is modern **standardization** as Claims-based identity aligns with modern authentication protocols like **`OAuth`** and **`OpenID Connect`**

```cs
// use the same method of "role-based access control" in "claims-based access control"
if (User.IsInRole("Admin")) // This actually checks for a role claim
{
    // Perform admin actions
}
```

=======================================================================
# Claim, Identity and Principal

## Claim
* -> are **name-key values** and are represented via the **System.Security.Claim** class
* -> _a claim_ is simply **`a piece of information about a subject`** (_include user, application, service, device_); a claim **does not dictate what a subject can, or cannot do**
* => claims are the foundation behind **`claims-based authentication`** 

```cs
public class Claim {
  public string Type { get; } // used to identify the claim
  public string Value { get; } // holds the data of the claim

  // if the data type of Value is not a string then the ValueType property can be set 
  // so the claim consumer knows how to interpret the Value
  public string ValueType { get; } 

  // .... some properties have been omitted
}
```

```cs - Ex: 
// represent a claim for a user's Id 
Claim idClaim = new Claim("Id", "1", "Integer");

// use for a user’s birthday
Claim dobClaim = new Claim("dob", "04/20/1969", "Date");
```

## Identities - ClaimsIdentity
* -> **`claims representing the same subject`** can be **`grouped together`** and placed in **a ClaimsIdentity**

```cs
public class ClaimsIdentity {
  public string Name { get; }
  public IEnumerable<Claim> Claims { get; }

  // holds the authentication method used (such as "Bearer" or "Basic")
  public string AuthenticationType { get; }

  // returns true as long as "AuthenticationType" is not null
  public bool IsAuthenticated { get; }

  // .... some properties have been omitted
}
```

```cs - Ex: Usage
// For example, we we’re working on an API where "users" are identified via their "Id" and "Name"
// -> after validating a bearer token (JWT, etc…) recieved from the user,
// -> we could create a ClaimsIdentity to represent them:

ClaimsIdentity userIdentity = new ClaimsIdentity(
  new Claim[] {
    new Claim("Id", "1"),
    new Claim("Username", "Bert")
  },
  "Bearer" // => userIdentity.IsAuthenticated == true since we passed "Bearer" as AuthenticationType
);
```

## Principals
* -> **a ClaimsIdentity** is convenient for **representing a subject via a collection of claims** but what if we want to **`assign more than one identity to a subject`**
* -> there's case that we might want **seperate into different identities** - this allows us to **`indicate that each can exist without the other`**
* -> then we can use **`ClaimPrincipal`** to **express the relation between these indentities**
* => **a principal object** represents the **`security context of the user`** on whose behalf the code is running (_including that `user’s identity`_)

* -> _ClaimsPrincipal_ provides a handful of **helper methods / properties to check things** (_such as if a claim exists using **HasClaim()**_) in **`any of the associated identities`**
* -> when working **within an API controller** in ASP.NET we can **`access the current principal`** via the **`User` property**

```cs
public class ClaimsPrincipal 
{
  public IEnumerable<ClaimsIdentity> { get; } // a collection of ClaimsIdentity objects
  public IEnumerable<Claim> Claims { get; } //  all of the claims from all of the claims identities
  public ClaimsIdentity Identity { get; }

  // .... some properties have been omitted
}
```

```r - Practical Example:
// Principal = "User"
// Identity = "Driver's License, Passport, Credit Card, Google Account, Facebook Account, RSA SecurID, Finger print, Facial recognition, etc"

// if we are pulled over by the police, they dont verify us are who us claim to be, based on our driver license alone
// they also need to see our face, otherwise we could show anyones else driver license

// => hence it makes sense, why authentication can and sometimes should be based on multiple identities
// => that is why 1 ClaimsPrincipal can have any number of ClaimsIdentity
```

```r - Code Example:
// returning back to our previous example of the API, 
// what would happen if we wanted to also "identify the device being used by the user" to ensure it is whitelisted
// -> sure we could add a new Claim with device IP address, or agent string to the user identity, 

// but what if the user accesses the API from more than one device?
// -> to handle this use case without "duplicating user information" we’d be best off to "create a new identity to represent the device" that holds the IP address, and agent string.
```

```cs - seperating the "user claims" from the "device claims" into two seperate identities
ClaimsIdentity deviceIdentity = new ClaimsIdentity(
  new Claim[] {
    new Claim("IP", "192.168.1.1"),
    new Claim("Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0")
  }
);
```

```cs - create a ClaimsPrincipal
// -> group the "user identity" and "device identity" into one context 
// -> without having to duplicate any info
var principal = new ClaimsPrincipal(new IIdentity[] { userIdentity, deviceIdentity });
```

```cs - access current "ClaimPricipal" in Controller
if (HttpContext.Current.User is ClaimsPrincipal principal)
{
   foreach (Claim claim in principal.Claims)
   {
      Response.Write("CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value + "</br>");
   }
}
```

```cs - access a specific claim in action controller
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var email = User.FindFirst(ClaimTypes.Email)?.Value;
```

=======================================================================
# Authenticate, Chanllenge and Forbid

```cs - Ex: Authentication Service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer()
.AddCookie();
```

```r - Example:
// client request protected resource, server "authenticate" it using "AuthenticateScheme" - the handler will process it
// -> if success, handler create an authenticated user and server response with protected resource
// -> if fail, server use the "ChanllengeScheme" then the handler will return a challenge response
```

## Authenticate
* -> **an authentication scheme's `authenticate` action** is responsible for **`construct user's identity`** based on **request context** (_authenticate user on accessing protected resource_)

* -> it returns an **`AuthenticateResult`** indicating whether **authentication was successful**, 
* -> if so, the **user's identity** in **`an authentication ticket`**

* -> return **`no result`** or **`failure`** if **authentication is unsuccessful**
* -> have methods for **`challenge`** and **`forbid`** actions for **when users attempt to access resources**

```r 
// Authenticate examples include:
// -> "a cookie authentication scheme" constructing the "user's identity" from "cookies"
// -> "a JWT bearer scheme" deserializing and validating "a JWT bearer token" to construct the "user's identity"
```

## Challenge
* -> "an authentication challenge" is **`invoked by Authorization`** when an **`unauthenticated user` requests an endpoint that requires `authentication`**

* -> **Authorization invokes a challenge** using the **`specified authentication scheme(s)`** (_or the default if none is specified_)
* -> **a challenge action** should let the user know what **`authentication mechanism to use to access the requested resource`**

```r - Ex:
// an authentication challenge is issued when "authentication fails" or when "an unauthenticated user" tries to access a protected resource 

// Authentication challenge examples include:
// -> "a cookie authentication" scheme redirecting the user to a login page
// -> "a JWT bearer" scheme returning "a 401 result with a www-authenticate: bearer header"
```

## Forbid
* -> "an authentication scheme's forbid action" is **`called by Authorization`** when an **`authenticated user` attempts to access a resource they're `not permitted to access`**
* => **a forbid action** can let the user know that **`user're authenticated`** or they're **`not permitted to access the requested resource`**

```r - Authentication forbid examples include:
// "a cookie authentication" scheme "redirecting the user to a page indicating access was forbidden"
// "a JWT bearer" scheme "returning a 403 result"
// "a custom authentication" scheme "redirecting to a page where the user can request access to the resource"
```

=======================================================================
