> demo: https://learn.microsoft.com/en-us/training/modules/secure-aspnet-core-identity/

========================================================================
# ASP.NET Core security features
* -> ASP.NET Core provides many tools and libraries to **`secure ASP.NET Core apps`**
* -> such as **built-in identity providers** and **third-party identity services** such as _Facebook, Twitter, and LinkedIn_
* -> ASP.NET Core provides several approaches to **`store app secrets`**

===========================================================================
# Identity on ASP.NET Core - ASP.NET Core Identity
* -> **Identity (ASP.NET Core Identity)** provides **a framework** for **`managing and storing user accounts in ASP.NET Core apps`**
* -> is the **`membership system`** for building ASP.NET Core web applications, including **membership**, **login**, and **user data**

## Implement
* -> is **`an API`** that **supports user interface (UI) login functionality**
* -> **`manages users, passwords, profile data, roles, claims, tokens, email confirmation, ...`**

## Start project
* -> **Identity is added to our project** when **`Individual User Accounts`** is selected as the authentication mechanism
* -> by default, Identity makes use of an **`Entity Framework (EF) Core data model`**

* ->  is typically configured using a **`SQL Server database`** to store **`user names, passwords, and profile data`**
* -> _ASP.NET Core Identity_ **`adds user interface (UI) login functionality to ASP.NET Core web apps`**

## auth actions
* -> "ASP.NET Core Identity" sẽ cung cấp cho ta 3 lớp để thực hiện các auth actions: **`UserManager`**, **`SignInManager`**, **`RoleManager`**

## Web APIs with SPAs
*  -> _to secure this model, use one of these options: **`Duende Identity Server`**, **`Microsoft Entra ID`**, **`Azure Active Directory B2C`**_

=====================================================================




