> https://auth0.com/blog/id-token-access-token-what-is-the-difference/

# ID Token
* -> an artifact that **proves that the user has been authenticated**
* -> was introduced by **OpenID Connect (OIDC)** (_an **`open standard for authentication`** used by many **`identity providers`** such as Google, Facebook, and, of course, Auth0_)

* -> the **`result of that authentication process based on OpenID Connect`** is the **ID token**, which is **`passed to the application`** as proof that the user has been authenticated
* -> _a user with their browser authenticates against an OpenID provider and gets access to a web application_

## Structure
* -> an ID token is **`encoded`** as **a JSON Web Token** (JWT) - a standard format 
* -> the ID token is **signed** by the issuer with its **private key**; verify by using the issuer's public key
* -> allows our application to **`easily inspect its content`**, and make sure it **comes from the expected issuer** and that **no one else changed it**

* -> have to **`decode to see the content`** JWT hold (_JSON object_)
* -> _JSON properties_ are called **claims** - **`declarations about the user and the token`** itself 
* -> _however, the OpenID Connect specifications **`don't require`** the ID token to have user's claims_

* one **`important claim`** is the **"aud" claim** - defines the **`audience of the token`**
* -> For example, the web application that is meant to be **the final recipient of the token** 
* -> _in the case of the ID token_, its value is the **`client ID`** of the application that should consume the token


```json 
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbXktZG9tYWluLmF1dGgwLmNvbSIsInN1YiI6ImF1dGgwfDEyMzQ1NiIsImF1ZCI6IjEyMzRhYmNkZWYiLCJleHAiOjEzMTEyODE5NzAsImlhdCI6MTMxMTI4MDk3MCwibmFtZSI6IkphbmUgRG9lIiwiZ2l2ZW5fbmFtZSI6IkphbmUiLCJmYW1pbHlfbmFtZSI6IkRvZSJ9.bql-jxlG9B_bielkqOnjTY9Di9FillFb6IMQINXoYsw

{ 
  "iss": "http://my-domain.auth0.com", 
  "sub": "auth0|123456", 
  "aud": "1234abcdef", 
  "exp": 1311281970, 
  "iat": 1311280970, 
  "name": "Jane Doe", 
  "given_name": "Jane", 
  "family_name": "Doe"
}
```

## Purpose
* -> demonstrates that the user has been **`authenticated by an entity (the OpenID provider) we trust`**  and so we **can trust the claims about their identity**
* ->  our application can **personalize the user’s experience** by **`using the claims about the user`** that are included in the ID token
* _we don't need to make additional requests to display username,...; means we may get **`a little gain in performance`** for our application_

=============================================================
# Access Token
* -> is the artifact issued by the **authorization server**, that **`allows`** the **client application** to access **resource** (_throught **`resource server`**_) of the **user**  
* -> after **successfully authenticating the user** and **obtaining their consent**

## delegated authorization scenario
* in the **OAuth 2** context, the _access token_ **allows a client application to access a specific resource to perform specific actions on behalf of the user**
* -> that is what is known as a **delegated authorization scenario**: the **`user delegates a client application`** to **`access a resource on their behalf`**
* -> the **`delegate limitation`** is very important in a delegated authorization scenario and is achieved through **scopes**
* -> **Scopes** are a mechanism that allows the user to **`authorize a third-party application`** to **`perform only specific operations`**

```r
// we can authorize our LinkedIn app to access Twitter’s API on our behalf to cross-post on both social platforms
// however, we only authorize LinkedIn to publish our posts on Twitter; we dont authorize it to delete them or change our profile’s data or do other things, too
```

## Resource Server recieve Access Token
* **`the API receiving the access token`** must be sure that it actually is **a valid token issued by the authorization server that it trusts** 
* -> and make **`authorization`** decisions **based on the information associated with it**
* -> _in other words, the API needs to somehow use that token in order to `authorize the client application` to perform the desired operation on the resource_

## How the access token should be used
* -> in order to make authorization decisions depends on many factors: **`the overall system architecture, the token format,....`**
```r - For example
// an access token could be a key that allows the API to retrieve the needed information from a database shared with the authorization server
// or it can directly contain the needed information in an encoded format
// =>  means that understanding how to retrieve the needed information to make authorization decisions is an "agreement between the authorization server and the resource server (Ex: the API)" 
```

## Access Token format
* -> **`OAuth 2 core specifications`** say nothing about the **`access token format`**
* -> it can be **a string in any format**
* -> a common format used for access tokens is **`JWT`**

============================================================
# Cautious - Not suitable for

## ID Token

### Common mistake
* one of the **`most common mistakes`** developers make with an ID token is **using ID token to call an API**
* -> an ID token proves that a **`user has been authenticated`**
* -> in a **`first-party scenario`**, we may **decide that our ID token is good to make authorization decisions**; because **`maybe all we need to know is the user identity`**
* _Ex: in a scenario where the client and the API are both controlled by us_
* -> but even in this scenario, the **security of your application** (_consisting of the **`client`** and the **`API`**_), may be at **`risk`**
* -> n fact, there is **`no mechanism that ties the ID token to the client-API channel`**; if an attacker manages to **steal our ID token**, they can use it to **call our API like a legitimate client**

### Access Token
* on the other hand, for the **Access Token**
* -> there is a set of techniques, collectively known as **sender constraint**, that allow us to **`bind an access token to a specific sender`**
* => this guarantees that **`even if an attacker steals an access token`**, they **can't use it to access our API** since the **`token is bound to the client that originally requested it`**

### delegated authorization scenario
* -> in a delegated authorization scenario where a third-party client wants to call our API, we **must not use an ID token to call the API**
* -> in addition to the **`lack of mechanisms to bind it to the client`**, there are several other reasons 
* -> if our API **`accepts an ID token as an authorization token`**, we are **ignoring the intended recipient** stated by the **`audience claim`** 
* -> the **`audience claim`** says that it is **meant for our client application**, **not for the resource server** (_Ex: the API_)
* -> it's **`not just a formality`**, but there are **security implications** here

* the first validation checks should be that **`API shouldn’t accept a token that is not meant for it`** 
* -> if it does, its **security is at risk**
* -> in fact, if our API doesn't care if a token is meant for it, **`an ID token stolen from any client application can be used to access your API`**
* -> and **`checking the audience`** is just one of the checks that our API should do to prevent unauthorized access

* our **ID token will not have granted scopes**
* -> as said before, **`scopes`** allow **`the user to restrict the operations our client application can do on their behalf`**
* -> those **scopes are associated with the access token** so that your **`API knows`** what the client application can do and what it can't do
* -> if our **`client application uses an ID token to call the API`**, we ignore this feature and **potentially allow the application to perform actions that the user has not authorized**

## Access Token
*  -> the _Access Token_ was conceived to demonstrate that we are **`authorized to access a resource`** (_Ex: to call an API_)
* -> our **`client application`** should use it only for this reason; in other words, the **access token should not be inspected by the client application**
* -> it is **intended for the resource server**, and your **`client application`** should **`treat access tokens as strings with no specific meaning`**
* -> even if we know the access token format, we shouldn’t try to interpret its content in our client application
* => the _access token format_ is **an agreement between the authorization server and the resource server**, and the client application should not intrude