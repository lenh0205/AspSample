
======================================================================
# Authentication Handler

## Definition
* -> is **`a type`** that **`implements the behavior of a scheme`**
* -> is **derived** from **`IAuthenticationHandler`** or **`AuthenticationHandler<TOptions>`**
* -> has the **primary responsibility** to **`authenticate users`**

## Mechanism
* _based on the **authentication scheme's configuration** and the **incoming request context**, authentication handlers:_
* -> **construct `AuthenticationTicket` objects** representing the **`user's identity`** if authentication is successful
* -> return **no result** or **failure** if authentication is unsuccessful
* -> have methods for challenge and forbid actions for when users attempt to access resources: **`forbid`** - they're **authenticated but unauthorized to access**, **`challenge`** - when they're **unauthenticated**

## Authenticate, Chanllenge and Forbid
* xem `~\Features\Security\AspNet_Auth\NET_Terminology.md` để hiểu thêm

## RemoteAuthenticationHandler<TOptions>
* -> **JWT** and **cookies** don't use this since they **`can directly use the bearer header and cookie to authenticate`**
* -> **`OAuth 2.0`** and **`OIDC`** both will use this pattern

* -> **RemoteAuthenticationHandler<TOptions>** is the **`class for authentication that requires a remote authentication step`**
* -> when the **remote authentication step is finished**, the **`handler calls back to the 'CallbackPath' set by the handler`**
* -> the **handler finishes the authentication step** using the **`information passed to the 'HandleRemoteAuthenticateAsync' callback path`**  (Ex: authorization code or tokens)

* -> the **remotely hosted provider** in this case is **`the authentication provider`**
* -> examples include **Facebook, Twitter, Google, Microsoft, and any other OIDC provider** that handles authenticating users using the **`handlers mechanism`** (_nói về cơ chế khi tích hợp authentication provider like "Facebook, Google,..." với ASP.NET Core's authentication system_)
