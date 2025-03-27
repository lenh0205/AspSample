> https://frontegg.com/blog/oauth-grant-types
> deciding which one is suited for our use case **`depends mostly on our application type`**, but other parameters weigh in as well, like the **level of trust for the client**, or the **experience we want our users to have**

================================================================
# Authentication and Authorization Flows
* the **`Identity platform`** usually uses the **OpenID Connect (OIDC) Protocol** and **OAuth 2.0 Authorization Framework** 
* -> to **`authenticate users`** and get their authorization to **`access protected resources`**
* -> support **`different flows`** in our own applications and APIs **`without worrying about OIDC/OAuth 2.0 specifications`** or **`other technical aspects of authentication and authorization`**
* -> support scenarios for server-side, mobile, desktop, client-side, machine-to-machine, and device applications

# OAuth 2.0 Grant Types - flows to get an access token (4 flows)
* _deciding the right flow will depends mostly on our **`application type`**_
* -> **`Authorization Code Flow`** - used by **Web Apps executing on a server** (_SSR_); also used by **mobile apps, using the Proof Key for Code Exchange (PKCE) technique**
* -> **`Implicit Flow with Form Post`** - used by **JavaScript-centric apps (Single-Page Applications)** executing on the user's browser.
* -> **`Resource Owner Password Flow`** - used by **highly-trusted apps**
* -> **`Client Credentials Flow`** - used for **machine-to-machine communication**

# OpenID Connect Grant Types - flows to get an ID Token
* -> the **`Authorization Code Flow`** and **`Implicit Flow`** similar like **OAuth 2.0**
* -> but it also has the **`Hydrid Flow`** - combine steps from **Implicit Flow with Form Post** and **Authorization Code Flow**

================================================================
# Authorization Code Flow - - is the Client a web app executing on the server?
* if the **Client** is a **`regular web app executing on a server`**, then the "Authorization Code Flow" is the flow we should use
* -> using this the "Client" can exchanges an **Authorization Code** an **`Access Token`** and, optionally, a **`Refresh Token`**
* -> it's considered the **safest choice** since the **`Access Token is passed directly to the web server hosting the Client`**, without going through the user's web browser and risking exposure; also **the source code is not publicly exposed**

# Authorization Code Flow with Proof Key for Code Exchange (PKCE)
* -> during **authentication**, **`mobile and native applications`** can use the **Authorization Code Flow**, but they **require additional security** 
* -> additionally, **single-page apps** have **`special challenges`**
* -> to mitigate these, **OAuth 2.0** provides a version of the Authorization Code Flow which makes use of **`a Proof Key for Code Exchange (PKCE)`**

# Authorization Code Flow with enhanced privacy protection
* -> during the **`authentication and authorization process`**, some use cases such as **transactional authorization** exchange **`contextual information`**, which may contain **sensitive data**
* -> _to protect data and sensitive information_, we can use **`different protocol improvements`** for the authorization code flow:

* -> **`Authorization Code Flow`** with **Rich Authorization Requests (RAR)**
* -> **`Authorization Code Flow`** with **Pushed Authorization Requests (PAR)**
* -> **`Authorization Code Flow`** with **JWT-Secured Authorization Requests (JAR)**
* -> **`Authorization Code Flow`** with **PAR and JAR**
* -> **JSON Web Encryption (JWE)**

================================================================
# 'Authorization Code Flow with Proof Key for Code Exchange' vs 'Implicit Flow with Form Post'
* -> if the **Client is a Single-Page App (SPA)**, an application running in a browser using a scripting language like JavaScript
* -> there are **two grant options**: the "Authorization Code Flow with Proof Key for Code Exchange" and the "Implicit Flow with Form Post"
* -> for most cases, it's **`recommended using the Authorization Code Flow with PKCE`** because **the Access Token is not exposed on the client side**, and this flow can **return Refresh Tokens**
* -> if our SPA **doesn't need an Access Token**, we can use the **`Implicit Flow with Form Post`**

# Implicit Flow with Form Post - Is the Client a Single-Page App?
* -> as an **`alternative to the Authorization Code Flow`**, OAuth 2.0 provides the Implicit Flow
* -> which is intended for **`public clients`**, or applications which are **`unable to securely store Client Secrets`**
* -> while this is **`no longer considered a best practice`** for requesting **Access Tokens**
* -> when used with **Form Post response mode**, it does **offer a streamlined workflow** if the application needs only an **ID token** to perform **user authentication**

================================================================
## Client Credentials Flow - is the Client the Resource Owner?
* -> the first decision point is about whether **the party that requires access to resources** is **`a machine`**
* -> in the case of **`machine-to-machine (M2M) applications`** (_such as **CLIs, daemons, or services running on our back-end**_),
* -> the **`Client`** is also the **`Resource Owner`**, so **no end-user authorization is needed**

```bash
$ Example: **a cron job** that uses an API to import information to a database
$ in this case, the cron job is "the Client" and "the Resource Owner" 
$ since it holds the "Client ID" and "Client Secret" and uses them to get an "Access Token" from the "Authorization Server"
```

================================================================
## Hybrid Flow
* -> applications that are **`able to securely store Client Secrets`** may **`benefit from`** the use of the **`Hybrid Flow`**
* -> which **combines features** of the **`Authorization Code Flow`** and **`Implicit Flow`** with **`Form Post`**
* -> to allow our application to have **immediate access** to an **`ID token`** while still providing for **secure and safe retrieval** of **`access and refresh tokens`**
* -> this can be useful in situations where our application needs to **`immediately access information about the user`**, but must **`perform some processing before gaining access`** to protected resources for an extended period of time

================================================================
## Device Authorization Flow
* -> with **input-constrained devices** that **`connect to the internet`**
* -> rather than **`authenticate the user directly`**, the device asks the **user to go to a link on their computer or smartphone** and **`authorize the device`**
* -> this avoids a **poor user experience** for **`devices that do not have an easy way to enter text`**
* -> to do this, device apps use the _Device Authorization Flow_ (drafted in **`OAuth 2.0`**). 
* -> for use with **mobile/native applications**

================================================================
## Resource Owner Password Flow - is the Client absolutely trusted with user credentials?
* though it's **not recommend it**, **`highly-trusted applications`** can use the _Resource Owner Password Flow_
* -> in this flow, the **`end-user is asked to fill in credentials`** (username/password), typically using an **interactive form**
* -> this information is **`sent to the backend and from there to the Identity Platform`**
* -> it is therefore **`imperative`** that the **Client is absolutely trusted with this information** 
* => the _Resource Owner Password Flow_ should only be used when **redirect-based flows (like the Authorization Code Flow) cannot be used**
