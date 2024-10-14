
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

## Authenticate
* -> **an authentication scheme's `authenticate action`** is responsible for **`construct user's identity`** based on **request context**
* -> it returns an **`AuthenticateResult`** indicating whether **authentication was successful**, if so, the **`user's identity in an authentication ticket (AuthenticationTicket objects)`**_

```r Authenticate examples include:_
// "a cookie authentication scheme" constructing the "user's identity from cookies"
// "a JWT bearer scheme" deserializing and validating "a JWT bearer token" to construct the "user's identity"
```
* -> return 'no result' or 'failure' if authentication is **unsuccessful**
* -> have **`methods for challenge and forbid actions`** for when users attempt to **access resources**: **`forbid`** - they're **unauthorized to access**, **`challenge`** - when they're **unauthenticated**

## RemoteAuthenticationHandler<TOptions>
* -> **JWT** and **cookies** don't use this since they **`can directly use the bearer header and cookie to authenticate`**
* -> **`OAuth 2.0`** and **`OIDC`** both will use this pattern

* -> **RemoteAuthenticationHandler<TOptions>** is the **`class for authentication that requires a remote authentication step`**
* -> when the **remote authentication step is finished**, the **`handler calls back to the 'CallbackPath' set by the handler`**
* -> the **handler finishes the authentication step** using the **`information passed to the 'HandleRemoteAuthenticateAsync' callback path`**  (Ex: authorization code or tokens)

* -> the **remotely hosted provider** in this case is **`the authentication provider`**
* -> examples include **Facebook, Twitter, Google, Microsoft, and any other OIDC provider** that handles authenticating users using the **`handlers mechanism`** (_nói về cơ chế khi tích hợp authentication provider like "Facebook, Google,..." với ASP.NET Core's authentication system_)

## Challenge
* -> _an authentication challenge_ is **`invoked by Authorization`** when an **unauthenticated user requests an endpoint** that requires **`authentication`**
* _for example, an authentication challenge is issued, when **an anonymous user** requests a **restricted resource** or **follows a login link**_

* -> _Authorization invokes a challenge_ using the **`specified authentication scheme(s), or the default if none is specified`**
* -> **a challenge action** should **`let the user know what authentication mechanism to use`** to **access the requested resource**

```r - Authentication challenge examples include:
// a cookie authentication scheme redirecting the user to a login page
// a JWT bearer scheme returning a 401 result with a "www-authenticate: bearer header"
```

## Forbid
* -> _an authentication scheme's forbid action_ is **`called by Authorization`** when an **`authenticated user`** attempts to **`access a resource`** they're not permitted to access

* -> **`a forbid action can let the user know`** that they're **authenticated** or they're **not permitted to access** the requested resource

```r - Authentication forbid examples include:
// "a cookie authentication" scheme "redirecting the user to a page indicating access was forbidden"
// "a JWT bearer" scheme "returning a 403 result"
// "a custom authentication" scheme "redirecting to a page where the user can request access to the resource"
```
