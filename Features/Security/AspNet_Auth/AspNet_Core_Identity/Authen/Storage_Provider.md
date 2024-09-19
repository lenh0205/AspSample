> **ASP.NET Core Identity** enables us to **`create a custom storage provider and connect it to our app`**
> ta sẽ run .NET CLI  **dotnet new mvc -au Individual** start MVC project với "Individual User Accounts" option 

## Introduce
* -> **by default**, the ASP.NET Core Identity system **`stores user information in a SQL Server database using Entity Framework Core`**; for many apps, this approach works well
* -> however, we may prefer to use a **`different persistence mechanism or data schema`**
* _For example: using another **data store** (such as  `Azure Table Storage`), our database **tables have a different structure**, use a different **data access approach** (such as `Dapper`)_
* -> in each of these cases, we can **write a customized provider for our storage mechanism** and **plug that provider into our app**

## The ASP.NET Core Identity architecture
* -> _ASP.NET Core Identity_ consists of classes called **`managers`** and **`stores`**

* -> **Managers** are "high-level classes" which **`an app developer uses to perform operations`**, _such as `creating an Identity user`_ 
* -> **Managers** are **`decoupled from stores`**, which means we can **replace the persistence mechanism without changing our `application code` (except for configuration)**

* -> **Stores** are "lower-level classes" that **`specify how entities (such as users and roles) are persisted`**
* -> **Stores** follow the **`repository pattern`** and are **`closely coupled with the persistence mechanism`**

* => to **`create a custom storage provider`**, create **the data source**, **the data access layer**, and **the store classes** that interact with this data access layer
* => the **web app** interacts with the **managers** and we **`don't need to customize the managers or our app code`** that interacts with them 
* -> when creating a new instance of **UserManager** or **RoleManager** we **`provide the type of the user class`** and **`pass an instance of the store class as an argument`**
* -> this approach enables us to **plug our customized classes into ASP.NET Core**

## The data access layer
* -> 