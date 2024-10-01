==============================================================================
# Resource-based authorization in ASP.NET Core
* -> **`Authorization approach depends on the resource`**

```r - For example:
// only the author of a document is authorized to update the document
// consequently, the document must be retrieved from the data store before authorization evaluation can occur
```

* -> **attribute evaluation** occurs **`before data binding`** and **`before execution of the page handler`** or **`action that loads the document`**
* => for these reasons, **declarative authorization with an [Authorize] attribute** **`doesn't suffice`**
* => instead, we can **invoke a custom authorization method** - a style known as **`imperative authorization`**

==============================================================================
# Sample Code:
* _an ASP.NET Core app with user data protected by authorization contains a sample app that uses resource-based authorization_
* https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authorization/resourcebased/samples/3_0

==============================================================================
# Use imperative authorization
* -> **`Authorization is implemented as an IAuthorizationService service`**
* -> is registered in the **service collection** at application startup; the service is made available via dependency injection to **page handlers** or **actions**

```cs
public class DocumentController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IDocumentRepository _documentRepository;

    public DocumentController(IAuthorizationService authorizationService,
                              IDocumentRepository documentRepository)
    {
        _authorizationService = authorizationService;
        _documentRepository = documentRepository;
    }
    // ....
}
```

* -> **IAuthorizationService** has two **`AuthorizeAsync`** method overloads: 
* -> **one accepting the `resource` and the `policy name`** 
* -> and **the other accepting the `resource` and `a list of requirements` to evaluate**
```cs
Task<AuthorizationResult> AuthorizeAsync(
                            ClaimsPrincipal user,
                            object resource,
                            IEnumerable<IAuthorizationRequirement> requirements
                        );
Task<AuthorizationResult> AuthorizeAsync(
                            ClaimsPrincipal user,
                            object resource,
                            string policyName
                        );
```

* _in the following example, the **resource to be secured** is loaded into a custom "Document" object_
* -> an **AuthorizeAsync overload** is invoked to determine whether **`the current user is allowed to edit the provided document`**
* -> a `custom "EditPolicy" authorization policy` is factored into the decision
```cs
public async Task<IActionResult> OnGetAsync(Guid documentId)
{
    Document = _documentRepository.Find(documentId);

    if (Document == null)
    {
        return new NotFoundResult();
    }

    var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, Document, "EditPolicy");

    if (authorizationResult.Succeeded)
    {
        return Page();
    }
    else if (User.Identity.IsAuthenticated)
    {
        return new ForbidResult();
    }
    else
    {
        return new ChallengeResult();
    }
}
```

==============================================================================
# Write a resource-based handler
* -> writing **a handler for resource-based authorization** isn't much different than writing **a plain requirements handler**
* -> **`create a custom requirement class`**, and **`implement a requirement handler class`**
* -> the handler class **`specifies both`** the **requirement** and **resource** type

```cs - For example: a handler utilizing a "SameAuthorRequirement" and a "Document resource" follows:
public class DocumentAuthorizationHandler : 
    AuthorizationHandler<SameAuthorRequirement, Document>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   SameAuthorRequirement requirement,
                                                   Document resource)
    {
        if (context.User.Identity?.Name == resource.Author)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class SameAuthorRequirement : IAuthorizationRequirement { }
```

* _in the preceding example, imagine that SameAuthorRequirement is a special case of a more generic SpecificAuthorRequirement class_
* _The SpecificAuthorRequirement class (not shown) contains a Name property representing the name of the author. The Name property could be set to the current user_

```cs - Register the "requirement" and "handler" in Program.cs:
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options => // this one
{
    options.AddPolicy("EditPolicy", policy =>
        policy.Requirements.Add(new SameAuthorRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>(); // this one
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationCrudHandler>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
```

## Operational requirements
If you're making decisions based on the outcomes of CRUD (Create, Read, Update, Delete) operations, use the OperationAuthorizationRequirement helper class. This class enables you to write a single handler instead of an individual class for each operation type. To use it, provide some operation names:

C#

Copy
public static class Operations
{
    public static OperationAuthorizationRequirement Create =
        new OperationAuthorizationRequirement { Name = nameof(Create) };
    public static OperationAuthorizationRequirement Read =
        new OperationAuthorizationRequirement { Name = nameof(Read) };
    public static OperationAuthorizationRequirement Update =
        new OperationAuthorizationRequirement { Name = nameof(Update) };
    public static OperationAuthorizationRequirement Delete =
        new OperationAuthorizationRequirement { Name = nameof(Delete) };
}
The handler is implemented as follows, using an OperationAuthorizationRequirement requirement and a Document resource:

C#

Copy
public class DocumentAuthorizationCrudHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Document>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   Document resource)
    {
        if (context.User.Identity?.Name == resource.Author &&
            requirement.Name == Operations.Read.Name)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
The preceding handler validates the operation using the resource, the user's identity, and the requirement's Name property.

==============================================================================
# Challenge and forbid with an operational resource handler




* -> to call **an operational resource handler**, **`specify the operation`** when **invoking AuthorizeAsync** in **our page handler or action**

* -> if **authorization succeeds**, the page for viewing the document is returned
* -> if **authorization fails** but the **`user is authenticated`**, returning **`ForbidResult`** informs **any authentication middleware that authorization failed**
* -> a **`ChallengeResult`** is returned when **authentication must be performed**
* _for interactive browser clients, it may be appropriate to `redirect the user to a login page`_

```cs - Ex: determines whether the authenticated user is permitted to view the provided document
public async Task<IActionResult> OnGetAsync(Guid documentId)
{
    Document = _documentRepository.Find(documentId);

    if (Document == null)
    {
        return new NotFoundResult();
    }

    var authorizationResult = await _authorizationService // this one
            .AuthorizeAsync(User, Document, Operations.Read);

    if (authorizationResult.Succeeded)
    {
        return Page();
    }
    else if (User.Identity.IsAuthenticated)
    {
        return new ForbidResult();
    }
    else
    {
        return new ChallengeResult();
    }
}
```
