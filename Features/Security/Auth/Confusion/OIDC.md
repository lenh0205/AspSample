# OIDC - OpenID Connect 
* -> is **an identity layer built on top of the OAuth 2.0 framework**
* -> it **`allows third-party applications`** to **verify the identity of the end-user** and to **obtain basic user profile information**
* -> OIDC **`uses JSON web tokens (JWTs)`** - obtain when using flows conforming to the OAuth 2.0 specifications. See our OIDC Handbook for more details.

# OpenID vs. OAuth2
* -> while **OAuth 2.0** is about **`resource access and sharing`**, **OIDC** is about **`user authentication`** 
* -> its purpose is to **give us one login for multiple sites**
* -> each time we need to **`log in to a website using OIDC`**, we are **redirected to your OpenID site** where we log in, and **`then taken back to the website`** 

* _For example, if we chose to sign in to Twitter using your Google account **then we used OIDC**_
* -> once we successfully **`authenticate with Google`** and **`authorize Twitter`** to access our information
* -> Google **sends information back** to Twitter about the **`user and the authentication performed`** 
* -> this information is returned in a **JWT**; we'll receive an **access token** and if requested and an **ID token**

# OpenID and JWTs
* -> **JWTs** **`contain claims`**, which are statements (_such as name or email address_) about an **`entity`** (_typically, the user_) and **`additional metadata`**

* -> the **OpenID Connect** specification **`defines a set of standard claims`**. 
* _the `set of standard claims` include: name, email, gender, birth date, and so on_ 
* _however, if we want to capture information about a user and there currently isn't a standard claim that best reflects this piece of information, we can create **`custom claims`** and add them to our tokens_

