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

## custom provider

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