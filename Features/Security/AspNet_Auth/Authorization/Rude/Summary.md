============================================================================
# Summary
* -> the **`[Authorize] Attribute`** **can not be applied to Razor Page handlers** (_xem `Features\Security\AspNet_Auth\Authorization\Basic\Simple.md` để hiểu_)

* -> nói chung là để authorize thì ta sẽ thường check quyền với 1 action bằng **`var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Create);`**
* -> **.AuthorizeAsync()** sẽ check xem 1 **`User`** có thể thực hiện action **`ContactOperations.Create`** lên resource **`Contact`**, đơn giản là thế
* -> tuy nhiên, với trường hợp read all record và cần lọc qua 1 số điều kiện thì logic check **authorize** có thể hơi khác chút 

* -> để enable Authorize được thì ta cần add role service (**`.AddRole`**) vào Identity (**.AddIdentity**)
* -> khi cấu hình **.AddAuthorization** middleware, ta cần nên thêm option **AuthorizationOptions.FallbackPolicy** với **`.equireAuthenticatedUser()`** để require authenticated user cho mọi request
* -> và sử dụng **`[AllowAnonymous] attribute`** cho những request không yêu cầu authenticated
* -> ta sẽ sử dụng **Secret Manager tool** để chứa **`password`** dùng cho việc development
* -> tạo 1 **`Scope Service`** để chạy **`SeedData.cs`** để tạo 2 user từ **UserName** và **password** với 2 role là **manager** và **admin** rồi tạo thêm các record **Contact** với chứa **`userId`** của owner; rồi update database
* -> tiếp theo define các handler kế thừa **`IAuthorizationHandler`** (_register và DI nó cho IAuthorizationService s/d_) và các **`IAuthorizationRequirement`** cho handers

============================================================================
# Authorization in ASP.NET Core
* -> **Authorization** is **`orthogonal and independent`** from **Authentication**; however, **authorization** requires **`an authentication mechanism`**
* -> **ASP.NET Core authorization** provides a simple, declarative **`role`** and a rich **`policy-based`** model

* -> Authorization is expressed in **`requirements`**, and **`handlers evaluate a user's claims`** against **requirements**
* -> **`imperative checks` can be based on** **`simple policies`** or **`policies which evaluate both the user identity and properties of the resource`** that the user is attempting to access

* -> Authorization components, including the **`AuthorizeAttribute`** and **`AllowAnonymousAttribute`** attributes, are found in the **Microsoft.AspNetCore.Authorization** namespace

============================================================================
# Sample app - Create an ASP.NET Core web app with user data protected by authorization
* -> create an **ASP.NET Core web app** with **`user data protected by authorization`**
* -> it displays **a list of contacts that authenticated (registered) users have created**

* _there are 3 security groups:_
* -> **`Registered users`** can **view all the approved data** and **can edit/delete their own data**
* -> **`Managers`** can **approve or reject contact data**; only **approved contacts are visible to users**
* -> **`Administrators`** can **approve/reject and edit/delete any data**

```r - in the Example:
//  user "Rick" (rick@example.com) is signed in
// in the "/Contacts" page of Rick, it display a list of contacts; only the record created by Rick displays "Edit" and "Delete" button 
// all records will have "Detail" button but when go to "Contacts/Detail" there is no "Approve" or "Reject" button
// Rick also have a "Create" button to create a new record
// a user can only see contact records of other users until a "manager" or "administrator" changes the status to "Approved"

// for "manager" role account (manager@contoso.com) signed in
// each record only have "Detail" button so "manager" cannot "Edit" or "Delete" record; 
// they will go to "Contacts/Detail" page to "Approve" or "Reject" record

// for "administrator" role account (admin@contoso.com) signed in
// have all privileges - can read, edit, or delete any contact and change the status of contacts
```

* _contains the following **authorization handlers**:_
* -> **`ContactIsOwnerAuthorizationHandler`** - ensures that **a user can only edit their data**
* -> **`ContactManagerAuthorizationHandler`** - allows **managers to approve or reject contacts**
* -> **`ContactAdministratorsAuthorizationHandler`** - allows **administrators to approve or reject contacts and to edit/delete contacts**

============================================================================
# The starter and completed app
* starter app: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authorization/secure-data/samples/starter6
* completed app: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authorization/secure-data/samples/final6 

* -> project sử dụng .NET 6, ASP.NET Core Identity; tạo sẵn model và page **Contact**
* -> ta chỉ cần chạy migration trước và start app lên là được

## Create project from scratch
* _hoặc ta có thể tạo project từ đầu cũng được:_
```bash
dotnet new webapp -o ContactManager -au Individual -uld

// -> create a Razor Pages app named "ContactManager"
// -> create the app with "Individual User Accounts"
// -> name it "ContactManager" so the namespace matches the namespace used in the sample
// -> -uld specifies LocalDB instead of SQLite
```

* _Add Models/Contact.cs: secure-data\samples\starter6\ContactManager\Models\Contact.cs_
```cs
using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }
}
```

* -> Scaffold the Contact model
```bash
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet-aspnet-codegenerator razorpage -m Contact -udl -dc ApplicationDbContext -outDir Pages\Contacts --referenceScriptLibraries
```

* -> Create initial migration and update the database:
```bash
dotnet ef database drop -f
dotnet ef migrations add initial
dotnet ef database update
```

* -> Update the ContactManager anchor in the Pages/Shared/_Layout.cshtml file:
```html
<a class="nav-link text-dark" asp-area="" asp-page="/Contacts/Index">Contact Manager</a>
```

============================================================================
# Test the completed app
* -> an easy way to test the completed app is to **launch three different browsers** (or incognito/InPrivate sessions) with 3 different **role** account
* -> sign in to each browser with a different user (in one browser, register a new user)

* _Verify the following operations:_
* -> Registered users can view all of the approved contact data
* -> Registered users can edit/delete their own data
* -> Managers can approve/reject contact data. The Details view shows Approve and Reject buttons
* -> Administrators can approve/reject and edit/delete all data

* _create a contact in the **administrator's browser**_
* -> copy the URL for delete and edit from the administrator contact
* -> Paste these links into the test user's browser to verify the test user can't perform these operations
