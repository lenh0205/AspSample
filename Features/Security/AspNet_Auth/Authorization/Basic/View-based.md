
# View-based authorization in ASP.NET Core MVC
* -> a developer often wants to **`show, hide, or otherwise modify a UI based on the current user identity`**
* -> however, don't rely on toggling visibility of our app's UI elements as **`the sole authorization check`**
* -> we can **`access the authorization service`** within MVC views via **dependency injection**

* -> to inject the "authorization service" into a **`Razor view`**, use the **@inject directive**:_
```cs
@using Microsoft.AspNetCore.Authorization
@inject IAuthorizationService AuthorizationService
```

* -> if we want the **`authorization service in every view`**, place the **@inject directive** into the **`_ViewImports.cshtml`** file of the Views directory

* -> use the injected authorization service to invoke "AuthorizeAsync" in **exactly the same way** we would check during **`resource-based authorization`**:
```cs
@if ((await AuthorizationService.AuthorizeAsync(User, "PolicyName")).Succeeded)
{
    <p>This paragraph is displayed because you fulfilled PolicyName.</p>
}
```

* -> in some cases, **`the resource will be your view model`** - invoke "AuthorizeAsync" in exactly the same way we would check during resource-based authorization:
```cs
@if ((await AuthorizationService.AuthorizeAsync(User, Model, Operations.Edit)).Succeeded)
{
    <p><a class="btn btn-default" role="button"
        href="@Url.Action("Edit", "Document", new { id = Model.Id })">Edit</a></p>
}
```