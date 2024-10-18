=========================================================================
# Simple authorization in ASP.NET Core
* -> Authorization in ASP.NET Core is controlled with the **`[Authorize] attribute`** and its various **parameters**
* -> in its most basic form, applying the **[Authorize] attribute** to a component (controller, action, or Razor Page) **`limits access to that component to authenticated users`**

=========================================================================
# Use the [Authorize] attribute

* _**limits access** to the **`controller`** to **`authenticated users`**:_
```cs
[Authorize]
public class AccountController : Controller
{
}
```

* _apply **authorization** to an **`action`** - only **`authenticated users`** can access the action_
```cs
public class AccountController : Controller
{
   public ActionResult Login()
   {
   }

   [Authorize]
   public ActionResult Logout()
   {
   }
}
```

* _limits access to the **`Razor Page Model`** to **authenticated users**:_
```cs
[Authorize]
public class LogoutModel : PageModel
{
    public async Task OnGetAsync()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
    }
}
```

# Use the [AllowAnonymous] attribute
* -> to **allow access** by **`non-authenticated users`**
* _accessible by everyone, regardless of their authenticated or unauthenticated / anonymous status_

* -> [AllowAnonymous] **bypasses authorization statements**
* -> in the combination [AllowAnonymous] and an [Authorize] attribute, **`the [Authorize] attributes are ignored`**
* _Ex: if we apply [AllowAnonymous] at the controller level, any authorization requirements from [Authorize] attributes on the same controller or action methods on the controller are ignored_

* _allow **only authenticated users to the AccountController**, **`except for the "Login" action`**_
```cs
[Authorize]
public class AccountController : Controller
{
    [AllowAnonymous]
    public ActionResult Login()
    {
    }

    public ActionResult Logout()
    {
    }
}
```

=========================================================================
# Authorize attribute and Razor Pages
* -> **`the 'AuthorizeAttribute' can not be applied to Razor Page handlers`**
* _For example, [Authorize] can't be applied to OnGet, OnPost, or any other page handler_

* => consider using **`an ASP.NET Core MVC controller for pages`** with different **authorization requirements** for **different handlers**

* => if we decide **not to use an MVC controller**, the following two approaches can be used to **`apply authorization to Razor Page handler methods`**:

## use "separate pages" for "page handlers requiring different authorization*"
* -> move **shared content** into **`one or more partial views`** when possibleth (_this is the recommended approach_)

## For content that must share a common page
* -> write **`a filter that performs authorization`** as part of **IAsyncPageFilter.OnPageHandlerSelectionAsync**
* -> **the PageHandlerAuth GitHub project** demonstrates this approach: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authorization/simple/samples/3.1/PageHandlerAuth

* _the **AuthorizeIndexPageHandlerFilter** implements the authorization filter:_
```cs
public class AuthorizeIndexPageHandlerFilter : IAsyncPageFilter, IOrderedFilter
{
    private readonly IAuthorizationPolicyProvider policyProvider;
    private readonly IPolicyEvaluator policyEvaluator;

    public AuthorizeIndexPageHandlerFilter(
        IAuthorizationPolicyProvider policyProvider,
        IPolicyEvaluator policyEvaluator)
    {
        this.policyProvider = policyProvider;
        this.policyEvaluator = policyEvaluator;
    }

    // Run late in the selection pipeline
    public int Order => 10000;

    public Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next) => next();

    public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        var attribute = context.HandlerMethod?.MethodInfo?.GetCustomAttribute<AuthorizePageHandlerAttribute>();
        if (attribute is null)
        {
            return;
        }

        var policy = await AuthorizationPolicy.CombineAsync(policyProvider, new[] { attribute });
        if (policy is null)
        {
            return;
        }

        await AuthorizeAsync(context, policy);
    }

    #region AuthZ - do not change
    private async Task AuthorizeAsync(ActionContext actionContext, AuthorizationPolicy policy)
    {
        var httpContext = actionContext.HttpContext;
        var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, httpContext);
        var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, httpContext, actionContext.ActionDescriptor);
        
        if (authorizeResult.Challenged)
        {
            if (policy.AuthenticationSchemes.Count > 0)
            {
                foreach (var scheme in policy.AuthenticationSchemes)
                {
                    await httpContext.ChallengeAsync(scheme);
                }
            }
            else
            {
                await httpContext.ChallengeAsync();
            }

            return;
        }
        else if (authorizeResult.Forbidden)
        {
            if (policy.AuthenticationSchemes.Count > 0)
            {
                foreach (var scheme in policy.AuthenticationSchemes)
                {
                    await httpContext.ForbidAsync(scheme);
                }
            }
            else
            {
                await httpContext.ForbidAsync();
            }

            return;
        }
    }   
}
```

* _the **[AuthorizePageHandler] attribute** is applied to the OnPostAuthorized page handler:_
```cs
[TypeFilter(typeof(AuthorizeIndexPageHandlerFilter))]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public void OnPost()
    {

    }

    [AuthorizePageHandler]
    public void OnPostAuthorized()
    {

    }
}
```