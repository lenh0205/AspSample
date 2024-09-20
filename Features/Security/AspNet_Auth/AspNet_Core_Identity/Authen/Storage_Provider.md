> **ASP.NET Core Identity** enables us to **`create a custom storage provider and connect it to our app`**
> ta sẽ run .NET CLI  **dotnet new mvc -au Individual** start MVC project với "Individual User Accounts" option 

=================================================================
# Introduce
* -> **by default**, the ASP.NET Core Identity system **`stores user information in a SQL Server database using Entity Framework Core`**; for many apps, this approach works well
* -> however, we may prefer to use a **`different persistence mechanism or data schema`**
* _For example: using another **data store** (such as  `Azure Table Storage`), our database **tables have a different structure**, use a different **data access approach** (such as `Dapper`)_
* -> in each of these cases, we can **write a customized provider for our storage mechanism** and **plug that provider into our app**

=================================================================
# The ASP.NET Core Identity architecture
* -> _ASP.NET Core Identity_ consists of classes called **`managers`** and **`stores`**

* -> **Managers** are "high-level classes" which **`an app developer uses to perform operations`**, _such as `creating an Identity user`_ 
* -> **Managers** are **`decoupled from stores`**, which means we can **replace the persistence mechanism without changing our `application code` (except for configuration)**

* -> **Stores** are "lower-level classes" that **`specify how entities (such as users and roles) are persisted`**
* -> **Stores** follow the **`repository pattern`** and are **`closely coupled with the persistence mechanism`**

* => to **`create a custom storage provider`**, create **the data source**, **the data access layer**, and **the store classes** that interact with this data access layer
* => the **web app** interacts with the **managers** and we **`don't need to customize the managers or our app code`** that interacts with them 
* -> when creating a new instance of **UserManager** or **RoleManager** we **`provide the type of the user class`** and **`pass an instance of the store class as an argument`**
* -> this approach enables us to **plug our customized classes into ASP.NET Core**

=================================================================
# The data access layer
* -> in our data access layer, we provide the **`logic to work with the structure of your storage implementation`**
* -> the data access layer provides the logic to **`save the data from ASP.NET Core Identity to a data source`**

* => these are some suggestions about design decisions when working with **ASP.NET Core Identity** that the **`data access layer` for our `customized storage provider`** might include:
* (_classes to store user and role information_)

## Context class
* -> **`encapsulates the information`** to **connect to our persistence mechanism** and **execute queries**
* -> _several data classes_ **`require an instance of this class`**, typically provided through **dependency injection**

## User Storage
* -> **`stores and retrieves user information`** (_such as **user name** and **password hash**_)

## Role Storage
* -> **`stores and retrieves role information`** (_such as the **role name**_)

## UserClaims Storage
* -> **`stores and retrieves user claim information`** (_such as the **claim type** and **value**_)

## UserLogins Storage
* -> **`stores and retrieves user login information`** (_such as **an external authentication provider**_)

## UserRole Storage
* -> **`stores and retrieves which roles are assigned to which users`** 

* -> in the **data access classes**, provide code to **`perform data operations for our persistence mechanism`**

## Data access example

```cs For example: within a custom provider, code to create a new user in the store class:
public async Task<IdentityResult> CreateAsync(ApplicationUser user, 
    CancellationToken cancellationToken = default(CancellationToken))
{
    cancellationToken.ThrowIfCancellationRequested();
    if (user == null) throw new ArgumentNullException(nameof(user));

    return await _usersTable.CreateAsync(user);
}
```

=================================================================
# Customize the user class
* -> when implementing a storage provider, **create a user class** which is **`equivalent to the IdentityUser class`**
* -> at a minimum, our **user class** must include an **`Id`** and a **`UserName`** property

* -> the **IdentityUser class** **`defines the properties that the 'UserManager' calls when performing requested operations`**
* -> the default type of the **Id property is a string**, but we can inherit from **`IdentityUser<TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>`** and specify a different type
* -> the framework **expects the storage implementation** to **`handle data type conversions`**

=================================================================
# Customize the user store
* -> create a **UserStore** class that **`provides the methods for all data operations on the user`**
* -> this class is equivalent to the **`UserStore<TUser>`** class
* -> in our UserStore class, **`implement IUserStore<TUser>`** and **`the optional interfaces required`** (_select based on the functionality provided in our app)

## Interfaces to implement when customizing user store

### IUserStore
* -> the "IUserStore<TUser>" interface is **`the only interface we must implement in the user store`**
* -> it **defines methods for creating, updating, deleting, and retrieving `users`**

### IUserClaimStore
* -> the "IUserClaimStore<TUser>" interface defines the methods we implement to **`enable user claims`**
* -> it contains methods for **adding, removing and retrieving `user claims`**

### IUserLoginStore
* -> the "IUserLoginStore<TUser>" defines the methods we implement to **`enable external authentication providers`**
* -> it contains methods for **adding, removing and retrieving `user logins`**, and a method for **retrieving a `user` based on the login information**

### IUserRoleStore
* -> the "IUserRoleStore<TUser>" interface defines the methods we implement to **`map a "user" to a "role"`**
* -> it contains methods to **add, remove, and retrieve a `user's roles`**, and a method to **check if a `user` is assigned to a `role`**

### IUserPasswordStore
* -> the "IUserPasswordStore<TUser>" interface defines the methods we implement to **`persist hashed passwords`**
* -> it contains methods for **getting and setting the `hashed password`**, and a method that **indicates whether the `user has set a password`**

### IUserSecurityStampStore
* -> the "IUserSecurityStampStore<TUser> interface defines the methods we implement to **use a `security stamp` for indicating whether the `user's account information has changed`**
* -> **this `stamp is updated` when a `user changes the password, or adds or removes logins`**
* -> it contains methods for **getting and setting the `security stamp`**

### IUserTwoFactorStore
* -> the "IUserTwoFactorStore<TUser>" interface defines the methods we implement to **support `two factor authentication`**
* -> it contains methods for **getting and setting whether two factor authentication is enabled for a user**

### IUserPhoneNumberStore
* -> the "IUserPhoneNumberStore<TUser>" interface defines the methods we implement to **`store user phone numbers`**
* -> it contains methods for **getting and setting the phone number and whether the phone number is confirmed**

### IUserEmailStore
* -> the "IUserEmailStore<TUser>" interface defines the methods we implement to **`store user email addresses`**
* -> it contains methods for **getting and setting the email address and whether the email is confirmed**

### IUserLockoutStore
* -> the "IUserLockoutStore<TUser>" interface defines the methods we implement to **`store information about locking an account`**
* -> it contains methods for **tracking `failed access` attempts and `lockouts`**

### IQueryableUserStore
* -> the IQueryableUserStore<TUser> interface defines the members you implement to provide a queryable user store.

### Example

```cs - Example:
public class CustomUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
{
}

public class UserStore : IUserStore<IdentityUser>,
                         IUserClaimStore<IdentityUser>,
                         IUserLoginStore<IdentityUser>,
                         IUserRoleStore<IdentityUser>,
                         IUserPasswordStore<IdentityUser>,
                         IUserSecurityStampStore<IdentityUser>
{
    // interface implementations not shown
}
```

* -> _within the **UserStore** class_, we **`use the data access classes that we created to perform operations`** (_these are passed in using dependency injection_)

```cs - For example: 
// in the "SQL Server" with "Dapper" implementation, the "UserStore" class has the "CreateAsync" method 
// which uses an instance of 'DapperUsersTable' to insert a new record:

public async Task<IdentityResult> CreateAsync(ApplicationUser user)
{
    string sql = "INSERT INTO dbo.CustomUser " +
        "VALUES (@id, @Email, @EmailConfirmed, @PasswordHash, @UserName)";

    int rows = await _connection.ExecuteAsync(sql, new { user.Id, user.Email, user.EmailConfirmed, user.PasswordHash, user.UserName });

    if(rows > 0)
    {
        return IdentityResult.Success;
    }
    return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.Email}." });
}
```

## IdentityUserClaim, IdentityUserLogin, and IdentityUserRole
* -> the **Microsoft.AspNet.Identity.EntityFramework** namespace contains **implementations** of the **`IdentityUserClaim`**, **`IdentityUserLogin`**, and **`IdentityUserRole`** classes
* -> if we are using these features, we may want to **create our own versions of these classes** and define the properties for our app
* -> however, sometimes it's **`more efficient to not load these entities into memory when performing basic operations`** (_such as adding or removing a user's claim_)
* -> instead, the **backend store classes** can **execute these operations directly on the data source**

* _For example, the `UserStore.GetClaimsAsync` method can call the `userClaimTable.FindByUserId(user.Id)` method to **execute a query on that table directly** and **return a list of claims**_

=================================================================
# Customize the role class
* -> when implementing a **role storage provider**, we can **`create a custom role type`**
* -> it need **not implement a particular interface**, but it must have an **`Id`** and typically it will have a **`Name`** property

```cs
using System;

namespace CustomIdentityProviderSample.CustomProvider
{
    public class ApplicationRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}
```

=================================================================
# Customize the role store
* -> we can create a **RoleStore** class that provides the methods for **`all data operations on roles`**
* -> this class is equivalent to the **`RoleStore<TRole>`** class
* -> in the RoleStore class, we implement the **`IRoleStore<TRole>`** and optionally the **`IQueryableRoleStore<TRole>`** interface

* -> **IRoleStore<TRole>** interface - defines the methods to implement in **`the role store class`**; it contains methods for **`creating, updating, deleting, and retrieving roles`**
* -> **RoleStore<TRole>** - to **`customize RoleStore`**, create a class that **`implements the IRoleStore<TRole> interface`**

=================================================================
# Reconfigure app to use a new storage provider
* _once you have implemented a storage provider, we configure our app to use it. If our app used the default provider, replace it with our custom provider_
* -> **`Remove the Microsoft.AspNetCore.EntityFramework.Identity`** NuGet package
* -> if the storage provider resides in a separate project or package, **add a reference to it**
* -> **`replace all references to Microsoft.AspNetCore.EntityFramework.Identity`** with a **using statement for the namespace of our storage provider**
* -> **`change the AddIdentity method to use the custom types`**; we can create our own extension methods for this purpose
* -> if we are using **Roles**, **`update the 'RoleManager' to use our RoleStore class`**
* -> update the **connection string and credentials** to our app's configuration

```cs - Example:
var builder = WebApplication.CreateBuilder(args);

// Add identity types
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddDefaultTokenProviders();

// Identity Services
builder.Services.AddTransient<IUserStore<ApplicationUser>, CustomUserStore>();
builder.Services.AddTransient<IRoleStore<ApplicationRole>, CustomRoleStore>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<SqlConnection>(e => new SqlConnection(connectionString));
builder.Services.AddTransient<DapperUsersTable>();

// additional configuration

builder.Services.AddRazorPages();

var app = builder.Build();
```