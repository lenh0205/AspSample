
# OAuth 2.0
* -> _OAuth 2.0 Authorization Framework_ is a protocol 
* -> that **`allows a user`** to **grant a third-party web site or application access** to the **`user's protected resources`**, **`without necessarily revealing their long-term credentials or even their identity`**

## 'Roles' in OAuth 2.0 flow
* -> **`Resource Owner`** - **`Entity`** that **can grant access to a protected resource**; typically, this is the **`end-user`**
* -> **`Resource Server`** - **`Server`** **hosting the protected resources** - this is the **API we want to access**
* -> **`Client`** - **`Application`** **requesting access to a protected resource** on **`behalf of the Resource Owner`**
* -> **`Authorization Server`** - **`Server`** that **authenticates the Resource Owner** and **issues access tokens after getting proper authorization**

* => the **client** requests **`access to resources`** **controlled by the resource owner** and **hosted by the resource server** and **`is issued a different set of credentials`** than those of the **resource owner** 

## 'Credential for Client' and 'Resource Owner's Credential'
* -> _OAuth_ introduces an **authorization layer** and **`separates`** the **role of the client** from **role of the resource owner**
* -> _instead of **using the resource owner's credentials** to access protected resources_, **`the third-party clients obtains an Access Token (a string denoting a specific scope, lifetime, and other access attributes)`**

## Access tokens 
* -> are **`issued by an authorization server`** with **`the approval of the resource owner`**
* -> then the **client** uses the **access token** to access the **protected resources** hosted by the **resource server**

## Scope
* -> is the **`permissions`** **represented by the access token** in OAuth terms
* -> when an application authenticates, it'll specifies the scopes it wants; then **if those scopes are authorized by the user**, then the **`access token will represent these authorized scopes`**

## Grant types - flows to get an access token (4 flows)
* _deciding the right flow will depends mostly on our **`application type`**_

* -> **`Authorization Code Flow`** - used by **Web Apps executing on a server**; also used by **mobile apps, using the Proof Key for Code Exchange (PKCE) technique**
* -> **`Implicit Flow with Form Post`** - used by **JavaScript-centric apps (Single-Page Applications)** executing on the user's browser.
* -> **`Resource Owner Password Flow`** - used by **highly-trusted apps**
* -> **`Client Credentials Flow`** - used for **machine-to-machine communication**

## Endpoints
* -> OAuth 2.0 uses two endpoints: the **`/authorize`** endpoint and the **`/oauth/token`** endpoint

### Authorization endpoint (/authorize)
* -> is used to **`interact with the resource owner`** and **`get the authorization to access the protected resource`**

```r - Ex:
// we want to log in to a service using our Google account
// First, the service "redirects us to Google in order to authenticate (if we're not already logged in)" 
// and then we will get "a consent screen", where we will be asked to "authorize the service to access some of our data (protected resources)" (for example, our email address and our list of contacts)
```

* -> the **`query parameters`** of the /authorize enpoint includes: **response_type**, **response_mode**, **client_id**, **redirect_uri**, **scope**, **state**, **connection**

* _this endpoint is used by the **`Authorization Code`** (_use **`response_type=code`** parameter_) and the **`Implicit`** (_use **`response_type=token`** to include **access token** or **`response_type=id_token`** to include both an **access token** and an **ID token**_) grant types_ 
* _depend on the grant type, the **Authorization Server** will **`issue different kind of credential`**_
* -> for the **Authorization Code grant**, it will **`issue an authorization code`** (which can later be **`exchanged for an access token`** at the **/oauth/token endpoint**)
* -> for the **Implicit** grant, it will **`issue an access token`** (_an opaque string - for example, a JWT_) that denotes who has authorized which **`permissions (scopes) to which application`**

* -> the **OAuth 2.0 Multiple Response Type Encoding Practices specification** define a (optional) parameter called **`response_mode`** to specifies how the **`result of the authorization request is formatted`**
* -> it can take one of these value: **query**, **fragment**, **form_post**, **web_message**

### 'state' parameters
* -> the primary reason for using the 'state' parameter is to **`mitigate CSRF attacks`**
* -> **Authorization protocols** provide a 'state' parameter that allows us to **restore the previous state of our application**
* -> the 'state' parameter **`preserves some state object set by the client in the Authorization request`** and **`makes it available to the client in the response`**

### Token endpoint (/oauth/token)
* -> is **`used by the application`** in order to **`get an access token or a refresh token`**

* -> it is **`used by all flows except for the Implicit Flow`** because in that case an access token is issued directly
* -> in the **Authorization Code Flow**, the application **`exchanges the authorization code it got from the authorization endpoint for an access token`**
* -> in the **Client Credentials Flow** and **Resource Owner Password Credentials Grant Exchange**, the application **`authenticates using a set of credentials and then gets an access token`**
