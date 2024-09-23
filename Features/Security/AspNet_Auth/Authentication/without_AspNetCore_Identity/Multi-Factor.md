> the demo code is setup using ASP.NET Core with Identity and Razor Pages

==============================================================================
# Multi-factor authentication in ASP.NET Core
* -> **Multi-factor authentication (MFA)** is a process in which **`a user is requested during a sign-in event for additional forms of identification`**
* -> this prompt could be to **enter a code from a cellphone** (_use a FIDO2 key_), or to **provide a fingerprint scan**
* -> when we require a second form of authentication, security is enhanced; the additional factor isn't easily obtained or duplicated by an attacker

==============================================================================
# MFA, 2FA
* -> **MFA** requires **``at least two or more types of proof for an identity`** like **something you know**, **something you possess**, or **biometric validation** for the user to authenticate.
* -> **Two-factor authentication (2FA)** is like **`a subset of MFA`**, but the difference being that MFA can require two or more factors to prove the identity

* -> **`2FA is supported by default`** when using **`ASP.NET Core Identity`**
* -> to enable or disable 2FA for a specific user, set the **`IdentityUser<TKey>.TwoFactorEnabled`** property
* -> **the ASP.NET Core Identity Default UI** includes **`pages for configuring 2FA`**

## MFA TOTP (Time-based One-time Password Algorithm)
* -> **MFA using TOTP** is **`supported by default`** when using **`ASP.NET Core Identity`**
* -> this approach can be used together with any compliant authenticator app, including **Microsoft Authenticator** and **Google Authenticator**
* _xem `~\Features\Security\AspNet_Auth\Authentication\AspNetCore_Identity\QR_Code.md`_

* -> to **disable support for MFA TOTP**, configure authentication using **`AddIdentity`** instead of **`AddDefaultIdentity`**
* -> **AddDefaultIdentity** calls **`AddDefaultTokenProviders`** internally, which **registers multiple token providers including one for MFA TOTP**
* -> to **register only specific token providers**, call **`AddTokenProvider`** for **each required provider**

## MFA passkeys/FIDO2 or passwordless
* -> **passkeys/FIDO2** is **`currently the most secure way of achieving MFA`**; protects against **`phishing attacks`** (_as well as certificate authentication and Windows for business_)
* -> at present, **ASP.NET Core** **`doesn't support passkeys/FIDO2 directly`**; **Passkeys/FIDO2** can be used for **MFA** or **passwordless flows**
* _**other forms of passwordless MFA** **`do not or may not protect against phishing`**_

* -> **`Microsoft Entra ID`** provides support for **passkeys/FIDO2** and **passwordless flows**

## MFA SMS
* -> **MFA with SMS** increases security massively compared with password authentication (single factor)
* -> however, using SMS as a second factor is **`no longer recommended`**
* -> **`too many known attack vectors exist`** for this type of implementation

==============================================================================
# Configure MFA for "administration pages" using 'ASP.NET Core Identity'
* -> **`MFA could be forced on users`** to **access sensitive pages** within an _ASP.NET Core Identity app_
* -> this could be useful for apps where **`different levels of access exist for the different identities`**
* _For example, users might be able to view the profile data using a password login, but `an administrator` would be required to use MFA to access the administrative pages_

## Extend the login with an MFA claim
* -> the **AddIdentity** method is used instead of **AddDefaultIdentity** one, so an **`IUserClaimsPrincipalFactory implementation`** can be used to **`add claims to the identity after a successful login`**

## Validate the MFA requirement in the administration page

## UI logic to toggle user login information

==============================================================================
# Send MFA sign-in requirement to OpenID Connect server

## OpenID Connect ASP.NET Core client

## Example OpenID Connect Duende IdentityServer server with ASP.NET Core Identity

==============================================================================
# Force ASP.NET Core OpenID Connect client to require MFA
