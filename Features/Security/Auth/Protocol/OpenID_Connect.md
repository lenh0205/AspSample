==================================================================

# OpenID Connect Protocol
* -> is **an identity layer** built on top of the **`OAuth 2.0 framework`**
* -> allows **`third-party applications to verify the identity of the end-user`** and to **`obtain basic user profile information`**
* -> OIDC uses **`JSON web tokens (JWTs)`**, which we can obtain using **`flows conforming to the OAuth 2.0 specifications`**

* => its purpose is to give us **`one login for multiple sites`**

## Mechanism
* -> each time we need to **log in to a website using OIDC**, we are **`redirected to our OpenID site`** where we **log in**, and then **`taken back to the website`**

## OpenID vs OAuth 2.0
* -> while **OAuth 2.0** is about **`resource access and sharing`**, **OIDC** is about **`user authentication`**

```r - For example:
// if we chose to sign in to websiteA using our "Google account" then we used OIDC
// once we successfully authenticate with Google and authorize websiteA to access our information, Google sends "information (the user and the authentication performed)" back to websiteA
// this information is returned in a "JWT"
// we will receive an "access token" and if requested, an "ID token"
```

## OpenID vs JWTs
* -> the **OpenID Connect specification** defines a set of **`standard claims`**; (_the set of standard claims include **name**, **email**, **gender**, **birth date**, ..._)
* -> **JWTs** contain **`claims`**, which are statements (such as name or email address) about an entity (typically, the user) and **`additional metadata`**

* -> if we want to capture information about a user and there currently isn't a standard claim that best reflects this piece of information, we can **create custom claims and add them to our tokens**

==================================================================
# OIDC Discovery
* -> so-called **discovery document** - **`a standard endpoint in identity servers`**
* -> this will be **`used by our clients and APIs`** to **`download the necessary configuration data`**

## Configure Applications with OIDC Discovery
* -> **OpenID Connect (OIDC) Discovery documents** contain **`metadata about the identity provider (IdP)`**
* -> **`adding discovery to our SDK`** to **point our application to the "./wellknown" endpoint** to **`consume information about our IdP`** could **help configure our integration with the IdP**