> introduce: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio
> demo: https://learn.microsoft.com/en-us/training/modules/secure-aspnet-core-identity/
> **`Lockout`** - refers to a security feature that **temporarily disables a user's account** after **a certain number of failed login attempts**

> ClaimPricipal là cái quần què gì ?

===========================================================================
# ASP.NET Core Identity
* -> ASP.NET Core Identity provides **a framework** for **`managing and storing user accounts in ASP.NET Core apps`**
* _**Identity is added to our project** when **`Individual User Accounts`** is selected as the authentication mechanism_
* _by default, Identity makes use of an **`Entity Framework (EF) Core data model`**_

=====================================================================
# Identity on ASP.NET Core - ASP.NET Core Identity
* -> is the **`membership system`** for building ASP.NET Core web applications, including **membership**, **login**, and **user data**
* -> is **`an API`** that **supports user interface (UI) login functionality**
* -> **`manages users, passwords, profile data, roles, claims, tokens, email confirmation, ...`**

* -> **Identity (ASP.NET Core Identity)** is typically configured using a **`SQL Server database`** to store **`user names, passwords, and profile data`**
* -> _ASP.NET Core Identity_ **`adds user interface (UI) login functionality to ASP.NET Core web apps`**

## Web APIs with SPAs
*  -> _to secure this model, use one of these options: **`Duende Identity Server`**, **`Microsoft Entra ID`**, **`Azure Active Directory B2C`**_