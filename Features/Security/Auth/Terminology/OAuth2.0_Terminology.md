
# Resource Owner
* it's you; you're the owner of your identity, data, any actions that can be performed with your account

# Client
* is the **`application`** that wants to **access data or perform actions** on behaft of **`resource owner`**

# Authorization Server
* is the **`application`** that knows the resource owner where the resource owner already has an account

# Resource Server
* is the API or service the client want use on behalf of the resource owner
* sometimes, the Authorization Server and the Resource Server are the same server 
* however, there are cases where they will not be the same server or even part of the same organization (_the **`Authorization Server`** might be a **`third-party service`** the **`Resource Server trusts`**_)

# Redirect URI
* is the URL the Authorization Server will redirect the Resource Owner back to after granting permission to the client (_this is sometimes referred to as the Callback URL_)

# Response Type
* the type of information the Client expects to receive
* the most common response type is **Code** where the client expects to receive an **`Authorization Code`**

# Authorization Code
* is a **short-lived temporary code** the Authorization Server sends back to the Client
* the Client than privately sends the Authorization Code back to the Authorization Server long with the Client Secret in exchange for an Access Token
* the Access Token is the key the Client will use from that point forward to communicate with the Resource Server
* 

# Client Secret 
* the secret password that only the Client and the Authorization Server know
* allow them to securely share information privately behind the scenes

# Client ID
* this ID is used to identify the Client with the Authorization Server

# Scope
* are granular permission the Client wants such as access to data or to perform actions 

# Consent
* the Authorization Server takes the Scopes the Client is requesting and verifies with the Resource Owner whether or not they want to give the Client permission



