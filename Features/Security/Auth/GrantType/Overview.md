https://frontegg.com/blog/oauth-grant-types

================================================================
# OAuth 2.0 Grant Types - flows to get an access token (4 flows)
* _deciding the right flow will depends mostly on our **`application type`**_
* -> **`Authorization Code Flow`** - used by **Web Apps executing on a server** (_SSR_); also used by **mobile apps, using the Proof Key for Code Exchange (PKCE) technique**
* -> **`Implicit Flow with Form Post`** - used by **JavaScript-centric apps (Single-Page Applications)** executing on the user's browser.
* -> **`Resource Owner Password Flow`** - used by **highly-trusted apps**
* -> **`Client Credentials Flow`** - used for **machine-to-machine communication**

## OpenID Connect Grant Types - flows to get an ID Token
* -> the **`Authorization Code Flow`** and **`Implicit Flow`** similar like **OAuth 2.0**
* -> but it also has the **`Hydrid Flow`** - combine steps from **Implicit Flow with Form Post** and **Authorization Code Flow**

================================================================
# Authentication and Authorization Flows
* -> the **`Identity platform`** usually uses the **OpenID Connect (OIDC) Protocol** and **OAuth 2.0 Authorization Framework** 
* -> to authenticate users and get their authorization to access protected resources
* -> support **`different flows`** in our **`own applications and APIs`** **`without worrying about OIDC/OAuth 2.0 specifications`** or **`other technical aspects of authentication and authorization`**
* -> support scenarios for server-side, mobile, desktop, client-side, machine-to-machine, and device applications


## Authorization Code Flow
* -> because **`regular web apps`** are **`server-side apps`** where **the source code is not publicly exposed** 
* -> they can use the **`Authorization Code Flow`**, which **exchanges an Authorization Code for a token**


## Authorization Code Flow with Proof Key for Code Exchange (PKCE)
* -> during **`authentication`**, **mobile and native applications** can use the **`Authorization Code Flow`**, but they **`require additional security`** 
* -> additionally, **single-page apps** have **`special challenges`**
* -> to mitigate these, **OAuth 2.0** provides **`a version of the Authorization Code Flow`** which makes use of **a Proof Key for Code Exchange (PKCE)**


## Authorization Code Flow with enhanced privacy protection
* -> during the **`authentication and authorization process`**, some use cases such as **transactional authorization** exchange **`contextual information`**, which may contain **sensitive data**
* -> _to protect data and sensitive information_, we can use **`different protocol improvements`** for the authorization code flow:

* -> **`Authorization Code Flow`** with **Rich Authorization Requests (RAR)**
* -> **`Authorization Code Flow`** with **Pushed Authorization Requests (PAR)**
* -> **`Authorization Code Flow`** with **JWT-Secured Authorization Requests (JAR)**
* -> **`Authorization Code Flow`** with **PAR and JAR**
* -> **JSON Web Encryption (JWE)**


## Implicit Flow with Form Post
* -> as an **`alternative to the Authorization Code Flow`**, **OAuth 2.0** provides the **`Implicit Flow`**
* -> which is intended for **Public Clients**, or **`applications`** which are **`unable to securely store`** **Client Secrets**
* -> while this is **`no longer considered a best practice`** for requesting **Access Tokens**
* -> when used with **Form Post response mode**, it does **offer a streamlined workflow** if the application needs only an **ID token** to perform **user authentication**


## Hybrid Flow
* -> applications that are **`able to securely store Client Secrets`** may **`benefit from`** the use of the **`Hybrid Flow`**
* -> which **combines features** of the **`Authorization Code Flow`** and **`Implicit Flow`** with **`Form Post`**
* -> to allow our application to have **immediate access** to an **`ID token`** while still providing for **secure and safe retrieval** of **`access and refresh tokens`**
* -> this can be useful in situations where our application needs to **`immediately access information about the user`**, but must **`perform some processing before gaining access`** to protected resources for an extended period of time


## Client Credentials Flow
* -> with **machine-to-machine (M2M) applications**, such as **`CLIs, daemons, or services running on our back-end`**, the **system** **`authenticates and authorizes`** **the app rather than a user**
* -> for this scenario, typical authentication schemes like **`username + password or social logins don't make sense`**
* -> instead, M2M apps use the Client Credentials Flow (defined in OAuth 2.0 RFC 6749, section 4.4).


## Device Authorization Flow
* -> with **input-constrained devices** that **`connect to the internet`**
* -> rather than **`authenticate the user directly`**, the device asks the **user to go to a link on their computer or smartphone** and **`authorize the device`**
* -> this avoids a **poor user experience** for **`devices that do not have an easy way to enter text`**
* -> to do this, device apps use the _Device Authorization Flow_ (drafted in **`OAuth 2.0`**). 
* -> for use with **mobile/native applications**


## Resource Owner Password Flow
* though it's **not recommend it**, **highly-trusted applications** can use the _Resource Owner Password Flow_
* -> which requests that **`users provide credentials`** (username and password), typically **using an interactive form**
* -> the _Resource Owner Password Flow_ should only be used when **redirect-based flows (like the Authorization Code Flow) cannot be used**