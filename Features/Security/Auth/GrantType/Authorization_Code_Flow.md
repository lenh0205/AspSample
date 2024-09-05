> defined in OAuth 2.0 RFC 6749, section 4.1
> làm sao để **Resource Server** (Web API) validate **access token** ?

==============================================================
# Authorization Code Flow
* -> involves **`exchanging an 'Authorization Code' for a 'Token'`**
* -> this flow **`can only be used for "confidential applications"`** (such as **`Regular Web Applications`**) 
* _because the **application's authentication methods are included in the exchange** and **must be kept secure**_

## Steps in "Authorization Code Flow"
* -> **user** click the **`Login link`** of **application (Regular Web Application)**
* -> the **Application Identity middleware/SDK** redirects **user** to **Authorization Server** (_i **`make Authorization Code Request to "/authorize" endpoint`**_) 
* -> **Authorization Server** redirects **user** to **`login/authorization prompt`**
* -> **user** **`authenticates`** using one of the configured login options, and may see **`a consent prompt listing the permissions`** Authorization Server will give to the application
* -> **Authorization Server** redirects **user** **`back to Application with single-use authorization code`**
* -> **Application Identity middleware** sends **`authorization code, application's client ID, and application's credentials (such as client secret or Private Key JWT)`** to **Authorization Server** at **`"/oauth/token" endpoint`** for application credentials
* -> **Authorization Server** **`verifies authorization code, application's client ID, and application's credentials`**
* -> **Authorization Server** responds with an **`ID token`** and **`access token`** (and optionally, a **`refresh token`**)
* -> **Application** can **`use the access token`** to call an **API (a Web API)** to **`access information about the user`**
* -> API responds with requested data

==============================================================
# Authorization Code Flow with Proof Key for Code Exchange (PKCE)

## Problem with public client
* -> when _public clients_ (e.g., **native** and **single-page applications**) **`request access tokens`**, some **additional security concerns** are posed that are **not mitigated by the Authorization Code Flow alone**
* -> this is because:

### Native apps
* -> **cannot securely store a Client Secret** - **`decompiling the app will reveal the Client Secret`**, which is bound to the app and is the same for **all users and devices**
* -> may make use of a **custom URL scheme** to capture redirects (e.g., MyApp://) potentially allowing **`malicious applications to receive an Authorization Code from our Authorization Server`**

### Single-page apps
* -> **cannot securely store a Client Secret** because their **`entire source is available to the browser`**
 
## Solution
* ->  **OAuth 2.0** provides a version of the **Authorization Code Flow** which **`makes use of a Proof Key for Code Exchange (PKCE)`**
* -> the **PKCE-enhanced Authorization Code Flow** introduces **`a secret created by the calling application that can be verified by the authorization server`**; 
* -> this secret is called the **`Code Verifier`**
* -> additionally, the _calling app_ creates **a transform value of the Code Verifier** called the **`Code Challenge`** and sends this value over **HTTPS** to **`retrieve an Authorization Code`**
* -> this way, a malicious attacker **can only intercept the Authorization Code**, and they **`cannot exchange Authorization Code for a token without the Code Verifier`**

## Step
* -> the **user** clicks **`Login link`** within the **application** (**SPA** or **Native App**)
* -> **Application Identity middleware** creates a cryptographically-random **`code_verifier`** and from this generates a **`code_challenge`**
* -> **Application Identity middleware/SDK** redirects the **user** to the **Authorization Server** - **`Authorization Code Request`** + **`code_challenge`** to **`"/authorize" endpoint`**
* -> our **Authorization Server** redirects the **user** to the **`login and authorization prompt`**
* -> the **user** **`authenticates`** using one of the configured login options and may see a **`consent page listing the permissions`** **Authorization Server** will give to the **Application**
* -> our **Authorization Server** stores the **`code_challenge`** and redirects the user **`back to the application with an authorization code`**, which is good for one use
* -> **Application Identity middleware/SDK** **`sends this code and the 'code_verifier' to the Authorization Server`** at **`"/oauth/token" endpoint`**
* -> our **Authorization Server** **`verifies the 'code_challenge' and 'code_verifier'`**
* -> our **Authorization Server** responds with an **`ID token`** and **`access token`** (and optionally, a **`refresh token`**)
* -> our **Application** can use the **`access token to call an API to access information about the user`**
* -> the API responds with requested data