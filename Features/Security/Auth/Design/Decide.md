# Choosing OAuth 2.0 Flow
* -> the **OAuth 2.0 Authorization Framework** supports several **`different flows (or grants)`**
* -> **Flow** are ways of retrieving an **Access Token**
* -> deciding which one is suited for our use case **`depends mostly`** on our **application type**
* -> but other parameters weigh in as well, like the **`level of trust for the client`**, or the **`experience we want our users to have`**

## Client Credentials Flow - is the Client the Resource Owner?
* -> the **first decision point** is about whether **`the party that requires access to resources`** is **a machine**
* -> in the case of **`machine-to-machine authorization`**, the **Client** is also the **Resource Owner**, so **`no end-user authorization is needed`**

* -> an example is **a cron job** that uses an API to import information to a database
* -> in this example, the cron job is **`the Client and the Resource Owner`** since it holds the **Client ID** and **Client Secret** 
* -> and uses them to get an **Access Token** from the **Authorization Server**

## Authorization Code Flow - is the Client a web app executing on the server?
* -> if the **Client is a regular web app executing on a server**, then the **`Authorization Code Flow`** is the flow we should use
* -> using this the **`Client`** can retrieve an **`Access Token`** and, optionally, a **`Refresh Token`**
* -> it's considered the **safest choice** since the **`Access Token is passed directly to the web server hosting the Client`**, without going through the user's web browser and risking exposure

## Resource Owner Password Flow - is the Client absolutely trusted with user credentials?
* -> this decision point may result in the **`Resource Owner Password Credentials Grant`**
* -> in this flow, the **`end-user is asked to fill in credentials`** (username/password), typically using an **interactive form**
* -> this information is **`sent to the backend and from there to the Identity Platform`**
* -> it is therefore **`imperative`** that the **Client is absolutely trusted with this information**
* -> this grant should **`only be used`** when **redirect-based flows (like the Authorization Code Flow) are not possible**

## PKCE / Implicit Flow with Form Post - Is the Client a Single-Page App?
* -> if the **Client is a Single-Page App (SPA)**, an application **`running in a browser`** using a **`scripting language`** like JavaScript
* -> there are two grant options: the **`Authorization Code Flow with Proof Key for Code Exchange (PKCE)`** and **`the Implicit Flow with Form Post`**
* -> for most cases, it's **recommended** using the **`Authorization Code Flow with PKCE`** because **the Access Token is not exposed on the client side**, and this flow can **return Refresh Tokens**
* -> if our SPA **doesn't need an Access Token**, we can use the **`Implicit Flow with Form Post`**

## Authorization Code Flow with Proof Key for Code Exchange (PKCE) - Is the Client a Native/Mobile App?
* if the Application is a **native app**, then **`use the Authorization Code Flow with Proof Key for Code Exchange (PKCE)`**

## Different Authorization Flow - I have an application that needs to talk to different resource servers
* -> if **`a single application needs access tokens for different resource servers`**
* -> then **`multiple calls to /authorize`** (that is, **multiple executions** of the same or different **Authorization Flow**) needs to be performed
* -> **`each authorization`** will use **a different value for audience**, which will result in **a different access token** at the end of the flow

## can I try the endpoints before I implement my application?
* [Authentication API Debugger Extension](https://auth0.com/docs/customize/extensions/authentication-api-debugger-extension)
* detailed instructions per /grant endpoint at [Authentication API Reference](https://auth0.com/docs/api/authentication)
* For the Authorize endpoint, go to [Authorize Application](https://auth0.com/docs/api/authentication#authorize-application); then "Test this endpoint" for the grant we want to test
* For the Token endpoint, go to [Get Token](https://auth0.com/docs/api/authentication#get-token); the  "Test this endpoint" section for the grant we want to test