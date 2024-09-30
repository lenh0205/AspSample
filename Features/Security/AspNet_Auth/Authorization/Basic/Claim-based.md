=================================================================================
# Claims-based authorization in ASP.NET Core

## Claims
* -> when **an identity is created** it may be **`assigned one or more claims issued by a trusted party`**
* -> **a claim** is a name value pair that **`represents what the subject is`**, not what the subject can do

```r - Ex:
// we may have a drivers license, issued by a local driving license authority
// our drivers license has our "date of birth" on it
// -> in this case the "claim" name would be "DateOfBirth", the claim value would be our date of birth, for example 8th June 1970 
// -> and the "issuer" would be the "driving license authority"
```

## Claim-based authorization
* -> at its **simplest**, **`checks the value of a claim`** and **`allows access to a resource based upon that value`**

```r - For example:
// if we want access to a night club, the authorization process might be:
// the door security officer would "evaluate the value of our date of birth claim" and "whether they trust the issuer" (the driving license authority) before granting us access
```

=================================================================================
# Adding claims checks - Claim-based authorization checks
* -> are declarative and applied to **Razor Pages**, **controllers**, or **actions** within a controller; can **not be applied at the Razor Page handler level**, they must be applied to the Page

* -> **claims in code** specify **`claims which the current user must possess`**, and optionally **`the value the claim must hold to access the requested resource`**
* -> **claims requirements** are **`policy based`**; the developer must **build and register a policy expressing the `claims requirements`**

## Authorization service configuration
* -> **the simplest type of claim policy** **`looks for the presence of a claim`** and **`doesn't check the value`**

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// build and register the policy and call "UseAuthorization"
builder.Services.AddAuthorization(options =>
{
    // the "EmployeeOnly" policy checks for the presence of an "EmployeeNumber" claim on the current identity
   options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
});

var app = builder.Build();
```

* -> most claims come with a value; we can **`specify a list of allowed values`** when creating the policy
```cs
builder.Services.AddAuthorization(options =>
{
    // only succeed for employees whose employee number was 1, 2, 3, 4, or 5
    options.AddPolicy("Founders", policy => policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));
});

```

## apply the policy
* -> using the **`Policy` property** on the **[Authorize] attribute** to specify the policy name
```cs
[Authorize(Policy = "EmployeeOnly")] // this one
public class VacationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public ActionResult VacationBalance()
    {
        return View();
    }

    [AllowAnonymous] // this one
    public ActionResult VacationPolicy()
    {
        return View();
    }
}
```

=================================================================================
# Multiple Policy Evaluation
* -> if **multiple policies** are applied at the controller and action levels, **`all policies must pass before access is granted`**

```cs
[Authorize(Policy = "EmployeeOnly")] // must fulfills the "EmployeeOnly" policy
public class SalaryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Payslip()
    {
        return View();
    }

    [Authorize(Policy = "HumanResources")] // must fulfill both the "EmployeeOnly" policy and the "HumanResources" policy
    public IActionResult UpdateSalary()
    {
        return View();
    }
}
```

```cs
[Authorize(Policy = "EmployeeOnly")]
[Authorize(Policy = "HumanResources")]
// both page handler methods must fulfill both the "EmployeeOnly" policy and the "HumanResources" policy
public class SalaryModel : PageModel
{
    public ContentResult OnGetPayStub()
    {
        return Content("OnGetPayStub");
    }

    public ContentResult OnGetSalary()
    {
        return Content("OnGetSalary");
    }
}
```