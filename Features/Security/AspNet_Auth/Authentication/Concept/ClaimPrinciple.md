> with the rise of `ASP.NET Core over ASP.NET 4.x`, the built in authentication has undergone a shift from **role-based access control (RBAC)** to **claim-based access control (CBAC)**
> the most notable change is the **`User` property on `HttpContext`** is now of type **ClaimsPrincipal** instead of **IPrincipal**

https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-8.0
https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal.claims?view=net-8.0
https://stackoverflow.com/questions/32584074/whats-the-role-of-the-claimsprincipal-why-does-it-have-multiple-identities
https://eddieabbondanz.io/post/aspnet/claims-based-authentication-claims-identities-principals/

=======================================================================
# Claim
* -> are **name-key values** and are represented via the **System.Security.Claim** class

```cs
public class Claim {
  public string Type { get; } // used to identify the claim
  public string Value { get; } // holds the data of the claim

  // if the data type of Value is not a string then the ValueType property can be set 
  // so the claim consumer knows how to interpret the Value
  public string ValueType { get; } 

  // some properties have been omitted
}
```

```cs - Ex: 
// represent a claim for a user's Id 
Claim idClaim = new Claim("Id", "1", "Integer");

// use for a user’s birthday
Claim dobClaim = new Claim("dob", "04/20/1969", "Date");
```

=======================================================================
