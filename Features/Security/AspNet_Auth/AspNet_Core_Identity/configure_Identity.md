
# Configure ASP.NET Core Identity
* -> _ASP.NET Core Identity_ uses **default values** for settings such as **`password policy, lockout, and cookie configuration`**
* -> these settings can be overridden at application startup

========================================================================
# Identity options
* -> the **IdentityOptions** class represents the options that can be used to **`configure the Identity system`**
* -> IdentityOptions must be set after calling **`AddIdentity`** or **`AddDefaultIdentity`**

## Claims Identity
* -> **`IdentityOptions.ClaimsIdentity`** specifies the **ClaimsIdentityOptions** with these properties:
