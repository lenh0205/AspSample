============================================================================
# Federated identity 
* https://www.okta.com/identity-101/what-is-federated-identity/

============================================================================
# Social Authentication
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/social-without-identity?view=aspnetcore-6.0

============================================================================
# Google
* -> **Google's OAuth 2.0 APIs** can be used for both **`authentication`** and **`authorization`**

## Authentication
* -> Google OAuth 2.0 implementation for authentication conforms to the OpenID Connect specification and is OpenID Certified

## Authorization
* -> https://developers.google.com/identity/oauth2/web/guides/overview
* -> https://developers.google.com/identity/protocols/oauth2 - lets us obtain an access token for use with Google APIs, or to access user data

## Sign in with Google
* -> helps us to **`quickly manage user authentication on our website`**
* -> **`users sign into a Google Account`**, **`provide their consent`**, and securely **`share their profile information`** with our platform
* => **customizable buttons** and **multiple flows** are supported for user sign-up and sign-in

* -> to provide a "Sign-in with Google" button for our website, use **Google Identity Services** - Google sign-in client library built on the OpenID Connect protocol provides OpenID Connect formatted ID Tokens
* -> however, **Google Identity Services** is migrating to **`FedCM APIs`** (_https://developers.google.com/identity/gsi/web/guides/fedcm-migration_)

## Sign-up
* -> refers to the steps to **`obtain a Google Account holder's consent`** to **`share their profile information with our platform`**
* -> typically, **`a new account is created on our site using this shared data`** (_but this is not a requirement_)

## Sign-in
* -> refers to **`logging users into our website`** using their **`active Google Account`** with a **personalized sign-in button** or **One Tap** and **Automatic sign-in** for users already logged in to their Google Account

============================================================================
# Spring Config
* https://docs.spring.io/spring-security/reference/servlet/oauth2/login/core.html
