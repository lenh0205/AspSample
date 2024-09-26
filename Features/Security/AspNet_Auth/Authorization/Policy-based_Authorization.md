# Policy-based authorization in ASP.NET Core
* **`an authorization policy`** consists of one or more **requirements**
* -> underneath the covers, **role-based authorization** and **claims-based authorization** use **`a requirement`**, **`a requirement handler`**, and **`a preconfigured policy`**
* => these building blocks support the **`expression of authorization evaluations`** in code which result in a richer, reusable, testable authorization structure

```c# - register it as part of the authorization service configuration, in the app's Program.cs file:
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AtLeast21", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(21)));
});
// -> an "AtLeast21" policy is created; it has a single requirement—that of a minimum age, which is supplied as a parameter to the requirement
```

## IAuthorizationService
* the primary service that determines **if authorization is successful** is _IAuthorizationService_
