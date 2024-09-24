============================================================================
# Authorization in ASP.NET Core
* -> **Authorization** is orthogonal and independent from **Authentication**. However, authorization requires an authentication mechanism
* -> **ASP.NET Core authorization** provides a simple, declarative **`role`** and a rich **`policy-based`** model

* -> Authorization is expressed in requirements, and **`handlers evaluate a user's claims`** against **requirements**
* -> **`imperative checks`** can be based on **simple policies** or **policies which evaluate both the user identity and properties of the resource** that the user is attempting to access

* -> Authorization components, including the **`AuthorizeAttribute`** and **`AllowAnonymousAttribute`** attributes, are found in the **Microsoft.AspNetCore.Authorization** namespace

============================================================================
# Create an ASP.NET Core web app with user data protected by authorization