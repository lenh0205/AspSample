=======================================================================
# Custom Authorization Policy Providers using IAuthorizationPolicyProvider in ASP.NET Core
* -> typically when using **policy-based authorization**, **`policies are registered`** by calling **AuthorizationOptions.AddPolicy** as part of authorization service configuration
* -> however in some scenarios, it may **`not be possible (or desirable) to register all authorization policies in this way`**
* => in those cases, we can use a custom **`IAuthorizationPolicyProvider`** to **control how authorization policies are supplied**

## Use cases
* -> using **`an external service to provide policy evaluation`**
* -> using **`a large range of policies`** (for different room numbers or ages, for example), so it doesn't make sense to **add each individual authorization policy** with an AuthorizationOptions.AddPolicy call
* -> creating **`policies at runtime`** based on **information in an external data source** (like a database) or determining **authorization requirements dynamically through another mechanism**

=======================================================================
# The completed custom 'IAuthorizationPolicyProvider' sample
* -> https://github.com/dotnet/aspnetcore/tree/v3.1.3/src/Security/samples/CustomPolicyProvider

=======================================================================
# Customize policy retrieval
* -> **ASP.NET Core apps** use an implementation of the **`IAuthorizationPolicyProvider`** interface to **`retrieve authorization policies`**
* -> **by default**, **`DefaultAuthorizationPolicyProvider`** is **registered and used**
* -> **DefaultAuthorizationPolicyProvider** **`returns policies from the AuthorizationOptions`** provided in an **IServiceCollection.AddAuthorization** call

* => customize this behavior by **`registering a different 'IAuthorizationPolicyProvider' implementation`** in the app's dependency injection container

* _the **IAuthorizationPolicyProvider** interface contains three APIs; by implementing these APIs, we can `customize how authorization policies` are provided._
* -> **`GetPolicyAsync`** returns **an authorization policy for a given name**
* -> **`GetDefaultPolicyAsync`** returns **the default authorization policy** (_the policy used for **[Authorize] attributes** without a policy specified_)
* -> **`GetFallbackPolicyAsync`** returns **the fallback authorization policy** (_the policy used by the **Authorization Middleware** when no policy is specified_)

=======================================================================
# Parameterized authorize attribute example
* -> one **scenario where IAuthorizationPolicyProvider is useful** is **`enabling custom [Authorize] attributes whose requirements depend on a parameter`**

```r - For example:
// in policy-based authorization documentation, an age-based "AtLeast21" policy was used as a sample
// if different controller actions in an app should be made "available to users of different ages", it might be useful to have "many different age-based policies"
// -> instead of "registering all the different age-based policies" that the application will need in AuthorizationOptions, we can generate the "policies dynamically" with a custom IAuthorizationPolicyProvider
// -> to make using the policies easier, we can annotate actions with "custom authorization attribute" like "[MinimumAgeAuthorize(20)]"
```

=======================================================================
# Custom Authorization attributes
* -> **Authorization policies** are **`identified by their names`**
* -> the **custom "MinimumAgeAuthorizeAttribute"** described previously needs to **`map arguments into a string`** that can be used to **`retrieve the corresponding authorization policy`**
* -> we can do this by **`deriving from AuthorizeAttribute`** and making the "Age" property **`wrap the 'AuthorizeAttribute.Policy' property`**

```cs
// this attribute type has a "Policy string" based on the hard-coded prefix ("MinimumAge") and an integer passed in via the constructor
internal class MinimumAgeAuthorizeAttribute : AuthorizeAttribute
{
    const string POLICY_PREFIX = "MinimumAge";

    public MinimumAgeAuthorizeAttribute(int age) => Age = age;

    // Get or set the Age property by manipulating the underlying Policy property
    public int Age
    {
        get
        {
            if (int.TryParse(Policy.Substring(POLICY_PREFIX.Length), out var age))
            {
                return age;
            }
            return default(int);
        }
        set
        {
            Policy = $"{POLICY_PREFIX}{value.ToString()}";
        }
    }
}

// apply it to actions in the same way as other Authorize attributes except that it takes an integer as a parameter
[MinimumAgeAuthorize(10)]
public IActionResult RequiresMinimumAge10()
```

=======================================================================
# Custom 'IAuthorizationPolicyProvider'
* -> the **custom MinimumAgeAuthorizeAttribute** makes it easy to **`request authorization policies for any minimum age desired`**
* -> the next problem to solve is **`making sure that authorization policies are available for all of those different ages`**

* => this is where an **`IAuthorizationPolicyProvider`** is useful

* _when using **MinimumAgeAuthorizationAttribute**, the **`authorization policy names`** will follow **the pattern "MinimumAge" + Age**, so the custom IAuthorizationPolicyProvider should **`generate authorization policies by`**:_
* -> parsing the **age** from **`the policy name`**
* -> using **AuthorizationPolicyBuilder** to **`create a new AuthorizationPolicy`**
* _in this and following examples it will be assumed that the **user is authenticated via a `cookie`**_
* _the **AuthorizationPolicyBuilder** should either be constructed with **`at least one authorization scheme name`** or **`always succeed`**_
* _otherwise there is **`no information on how to provide a 'challenge' to the user`** and **an exception will be thrown**_
* -> **`adding requirements to the policy`** _based on the age with AuthorizationPolicyBuilder.AddRequirements_
* -> in other scenarios, we might use **RequireClaim**, **RequireRole**, or **RequireUserName** instead

```cs
internal class MinimumAgePolicyProvider : IAuthorizationPolicyProvider
{
    const string POLICY_PREFIX = "MinimumAge";

    // Policies are looked up by string name, so expect 'parameters' (like age)
    // to be embedded in the policy names. This is abstracted away from developers
    // by the more strongly-typed attributes derived from AuthorizeAttribute
    // (like [MinimumAgeAuthorize()] in this sample)
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(policyName.Substring(POLICY_PREFIX.Length), out var age))
        {
            var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
            policy.AddRequirements(new MinimumAgeRequirement(age));
            return Task.FromResult(policy.Build());
        }

        return Task.FromResult<AuthorizationPolicy>(null);
    }
}
```

=======================================================================
# Multiple authorization policy providers
When using custom IAuthorizationPolicyProvider implementations, keep in mind that ASP.NET Core only uses one instance of IAuthorizationPolicyProvider. If a custom provider isn't able to provide authorization policies for all policy names that will be used, it should defer to a backup provider.

For example, consider an application that needs both custom age policies and more traditional role-based policy retrieval. Such an app could use a custom authorization policy provider that:

Attempts to parse policy names.
Calls into a different policy provider (like DefaultAuthorizationPolicyProvider) if the policy name doesn't contain an age.
The example IAuthorizationPolicyProvider implementation shown above can be updated to use the DefaultAuthorizationPolicyProvider by creating a backup policy provider in its constructor (to be used in case the policy name doesn't match its expected pattern of 'MinimumAge' + age).

C#

Copy
private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

public MinimumAgePolicyProvider(IOptions<AuthorizationOptions> options)
{
    // ASP.NET Core only uses one authorization policy provider, so if the custom implementation
    // doesn't handle all policies it should fall back to an alternate provider.
    BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
}
Then, the GetPolicyAsync method can be updated to use the BackupPolicyProvider instead of returning null:

C#

Copy
...
return BackupPolicyProvider.GetPolicyAsync(policyName);

=======================================================================
# Default policy
In addition to providing named authorization policies, a custom IAuthorizationPolicyProvider needs to implement GetDefaultPolicyAsync to provide an authorization policy for [Authorize] attributes without a policy name specified.

In many cases, this authorization attribute only requires an authenticated user, so you can make the necessary policy with a call to RequireAuthenticatedUser:

C#

Copy
public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => 
    Task.FromResult(new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
As with all aspects of a custom IAuthorizationPolicyProvider, you can customize this, as needed. In some cases, it may be desirable to retrieve the default policy from a fallback IAuthorizationPolicyProvider.

=======================================================================
# Fallback policy
A custom IAuthorizationPolicyProvider can optionally implement GetFallbackPolicyAsync to provide a policy that's used when combining policies and when no policies are specified. If GetFallbackPolicyAsync returns a non-null policy, the returned policy is used by the Authorization Middleware when no policies are specified for the request.

If no fallback policy is required, the provider can return null or defer to the fallback provider:

C#

Copy
public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => 
    Task.FromResult<AuthorizationPolicy>(null);

=======================================================================
# Use a custom IAuthorizationPolicyProvider
To use custom policies from an IAuthorizationPolicyProvider, you must:

Register the appropriate AuthorizationHandler types with dependency injection (described in policy-based authorization), as with all policy-based authorization scenarios.

Register the custom IAuthorizationPolicyProvider type in the app's dependency injection service collection in Startup.ConfigureServices to replace the default policy provider.

C#

Copy
services.AddSingleton<IAuthorizationPolicyProvider, MinimumAgePolicyProvider>();