> My Handbook: https://assets.ctfassets.net/2ntc334xpx65/o5J4X472PQUI4ai6cAcqg/13a2611de03b2c8edbd09c3ca14ae86b/jwt-handbook-v0_14_1.pdf

====================================================
# The idea of JWT
* -> a standard way to **`securely transmitting infomation`** between parties as **`JSON objects`**
* -> **JSON** is a **`lightweight data exchange`** interchange format; easy for human to **`read and write`**; simple for machines to **`parse and generate`**
* => JSON will be used to **`represent`** the **payload** - store the data for transmit

* JWTs are commonly used in standards like **OAuth2** and **OpenId connect** for authentication and authorization

====================================================
# JSON Web Token
* -> is an **`open standard`** (RFC 7519) that defines a **`compact`** and **`self-contained`** way for **securely transmitting information between parties** as a **`JSON object`**
* -> this information **`can be verified and trusted`** because it is digitally signed 
* -> JWTs can be **signed** using **a secret (_with the HMAC algorithm_)** or **a public/private key pair (_using RSA or ECDSA_)**

* _if sign with shared secret, signature is used to verify the message wasn't changed along the way_ 
* _and, in the case of tokens signed with a private key, it can also verify that the sender of the JWT is who it says it is_

## Sign or Encrypt
* -> although JWTs **`can be encrypted`** to also provide secrecy between parties, we will **`focus on signed tokens`**
* -> **`signed tokens`** can **verify the integrity of the claims** contained within it, while **`encrypted tokens`** **hide those claims from other parties**
* -> when tokens are **`signed using public/private key pairs`**, the signature also certifies that only **`the party holding the private key is the one that signed it`**

====================================================
# JSON Web Token Usage
* JWTs provide _a scalable way_ to handle **Information Exchange**, **Authorization**
* => it let our **`identity travel the web securely`**
* => JWTs are **`self-contained, portable and don't require server-side storage`**

## Authorization:
* -> this is the **most common scenario for using JWT**
* -> once the user is **`logged in`**, each **`subsequent`** request'll include the JWT, allowing the user to **`access routes, services, and resources`** that are permitted with that token
* -> **Single Sign On** is a feature that widely uses JWT nowadays, because of its small overhead and its ability to be **`easily used across different domains`**

## Information Exchange: 
* JSON Web Tokens are a good way of **securely transmitting information between parties** because **`JWTs can be signed`**, we can be sure the senders are who they say they are
* _for example, using public/private key pairs_
* -> additionally, as the signature is calculated using the **`header and the payload`**, we can also verify that the **`content hasn't been tampered with`**

====================================================
# JWT structure
* _3 parts_: **Header**, **Payload**, **Signature**; each section is **`base64Url encoded`**
* -> **Header** - claims about itself, typically consists of the **`token type`** (JWT) and **`sign or encrypted algorithm`** being used (_like HMAC SHA256, RSA_), ....
* -> **Payload** - store **claims**
* -> **Signature** - we have to take **`the encoded header, the encoded payload`**, **`a secret`**, **`the algorithm`** specified in the header, and **sign** that

```json - header and payload
/// HEADER:
{
    "alg": "HS256", // the Algorithm
    "typ": "JWT" // JWT token type
} // only useful for signature portion at the very end where we're verifying it 

/// PAYLOAD:
{
    "sub": "1234567890", // như là ID của User we're authenticating
    "name": "John Doe",
    "iat": 1516239022, // "issued at" - when token was created -> useful for expire token
    "exp": 1516239025 // when token expire at (invalid) -> useful when someone take your JWT
}
```
```js - signature
HMACSHA256(
    base64UrlEncode(header) + "." + base64UrlEncode(payload),
    secret  
)
```
```cs - Encoded JWT - send between server and client to Authorize user
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

## Claim
* -> **`statements about an entity`** (typically, the user) and **`additional data`**
* -> 3 types of claims: **Registered**, **Private**, **Public**

* **Registered Claims** - a set of **`predefined claims`** which are not mandatory but **`recommended`**, to provide a set of useful, interoperable claims
* _like: issuer, expiration time, subject_

* **Public claims** - can be **`defined at will`** by those using JWTs
* _but to avoid collisions they should be defined in the **ANA JSON Web Token Registry** or be defined as `a URI that contains a collision resistant namespace`_

* **Private claims** - are the **`custom claims`** created to **`share information between parties`** that agree on using them and are neither registered or public claims

## Security
* although **`JWT PAYLOAD`** can be encrypted using **`JWE - JSON Web Encryption`**, most implementations use **signed** but not encrypted tokens
* => this means while data is encoded, it's not encrypted and **can read if intercepted**
* => **sensitive information should never travel in a JWT PAYLOAD**, unless it's encrypted first 

## Signing Token
* _there're 2 main types of **`signing algorithms`**_

* **symmetric algorithms** 
* -> use **`a shared secret key`** for both signing and verification (_VD: HMAC, SHA256_)
* => **`quick and simple`**, but the **`secret key must be shared`** between parties ahead of time

* **asymmetric algorithms** 
* -> use **`a public/private key pair`** where the private key signs the token and the public key verifies it (_VD: RSA_)
* => allow **`verification of creator without sharing private`** keys but are **`slower`**

* => **signed JWTs** provide  **`Authorization`**, **`Sercure Information Exchange`**

======================================================
# How do JSON Web Tokens work ?
* -> in **authentication**, when the **`user successfully logs in`** using their credentials 
* -> a **JSON Web Token** will be returned 
* -> _since tokens are credentials, great care must be taken to prevent security issues_ 
* -> _in general, we should **`not keep tokens longer`** than required_
* -> _we also should **`not store sensitive session data in browser storage`** due to lack of security_
* -> Whenever the user wants to **`access a protected route or resource`**, the user agent should **`send the JWT`**, typically in the **Authorization** header using the **Bearer** schema

## In a "a stateless authorization mechanism" case
* -> the server's protected routes will **`check for a valid JWT in the Authorization header`**
* -> if it's present, the user will be **`allowed to access protected resources`** 
* -> if the JWT contains the **necessary data, the need to query the database** for certain operations may be reduced (_though this may not always be the case_)

* -> the application or client requests authorization to the **authorization server** (_VD: Auth0 by Okta_) (_this is performed through one of the **`different authorization flows`**_)
* _Ex: a typical **OpenID Connect** compliant web application will go through the /oauth/authorize endpoint using the **`authorization code flow`**_
* -> When the authorization is granted, the authorization server **`returns an access token`** to the application
* -> The application **`uses the access token to access a protected resource`** (like an API).

## Note
* if we send JWT tokens through **`HTTP headers`**, we should try to prevent them from getting too big
* -> some servers don't accept more than 8 KB in headers. 
* -> if we are trying to embed too much information in a JWT token, like by including all the user's permissions, we may need an alternative solution, like **Auth0 Fine-Grained Authorization**

* if the token is **`sent in the Authorization header`**, **Cross-Origin Resource Sharing (CORS) won't be an issue** as it **`doesn't use cookies`**

* with signed tokens, **`all the information`** contained within the token is **exposed to users or other parties**, even though they are **`unable to change`** it
* -> this means we should not put secret information within the token

======================================================
# Why using JSON Web Tokens - JWT vs SWT (Simple Web Tokens) & SAML (Security Assertion Markup Language Tokens)
* as **JSON is less verbose than XML**
* -> when it is **`encoded its size is also smaller`**, making JWT more compact than SAML
* => makes JWT a good choice to be **`passed in HTML and HTTP environments`**

* _Security-wise_
* -> **SWT can only be symmetrically signed** by a shared secret **`using the HMAC algorithm`**
* -> however, **`JWT and SAML tokens`** can **use a public/private key pair** in the form of a **`X.509 certificate`** for signing
* -> **signing XML with XML Digital Signature** without introducing **`obscure security holes`** is very difficult when compared to the simplicity of signing JSON

* **`JSON parsers`** are **common in most programming languages** because they **`map directly to objects`** 
* conversely, **`XML doesn't have a natural document-to-object mapping`**
* => this makes it easier to work with JWT than SAML assertions.

* _Regarding usage, JWT is used at Internet scale. This highlights the ease of client-side processing of the JSON Web token on multiple platforms, especially mobile_

====================================================
# JWT process: 
* -> client send its **`login detail`** to server 
* -> instead of storing information on the Server inside the session memory, Server **`generates a JWT`**
* -> Json Web Token is created from all the **`user information`** by **`encode, serialize and sign`** it with its own **secret key** 
* -> then send JWT back to browser (**`Authorization: Bearer<JWT>`**)
* -> client normally keep it in **`local storage`** (_vẫn có thể để chỗ khác_)
* -> JWT will be ensured to **`included on future client requests's HTTP Header`** (**`Authoriztion: Bearer<JWT>`**) to access protected resources
* -> Server signed to that JWT with its own sercret key - it verifies if JWT has not been changed since the time that it signed it; 
* (_if client change JWT, change the user information in JWT; Server will know and say it invalid_)
* -> if it's not, Server will deserialize that JWT, take user information stored in token
* -> now Server know exactly what to do with that user and if that user is authorized to use that resource it'll send the response back down to the client


* _User Info is stored in the actual token which mean it's stored on the client; the Server then only needs to validate the signature_
* _Server doesn't have to store anything (no need to database lookup somewhere else in the infrastructure)_
* => that mean we can use the **`same JWT accross multiple Servers`** without run into problem where 1 server has a certain session and the other server doesn't 
* _it's more efficient when dealing with **`a distributed system`** in the cloud_

# Validate a JWT:
* when Server gets the JWT from the client 
* -> s/d Algorithm chỉ định trong header để hash - 1 function nhận 2 tham số
* -> **First parameter** - server sẽ decode 2 phần đầu và combine chúng: `base64UrlEncode(header) + "." + base64UrlEncode(payload)` 
* -> **Second parameter** - **`secret key`**
* => s/d kết quả trả vể so sánh với phần `Verify signature` của JWT gửi từ Client ; nếu match thì ta biết token vẫn nguyên vẹn từ lúc ta gửi cho client và nhận lại

* _User chỉ có thể fake when they have access to `secret key`. Nhưng nó sẽ không xảy ra as long as we safely store our secret key on our Server, unaccessible to other outside_
* _Cách nó hoạt động giống như Password Hashing: input như nhau thì hashing out sẽ như nhau và không thể bị unhashing_

# Using JWT - the common usecase:
* JWT is popular in the context of **micro-services**

```cs - VD: A common use case of JWT in a lot of larger scale industries, applications company
// a Bank have 2 server:
// a "bank server" runs all of their banking application, banking website,...
// "retirement server" - a seperate server take care of all retirement plans allow people to invest and do retirement plans

// But the Bank want their users that logged to the "bank server" to also be able to be automatically logged into the "retirement account" 
// so when they switch from the "bank server" to "retirement server" the user don't have to re-login
```

* **when using "session-based"**, the session is stored inside of "bank server" and not the "retirement server" 
* -> **`user have to re-login`** because they need their session stored in "retirement server"

* **when using "JWT"**, if we **`share the same secret`** key between both "bank server" and "retirement server"
* -> **`all user need to do is send the same JWT`** from the client to both servers
* -> user will be authenticated both times without having to re-login

# Risk of using JWT
* JWT can't be used to authenticate a user in the background on the server
* token hijacking - an attacker steals a valid JWT to impersonate a user
* JWTs also could be vunerable to cryptographic weaknesses if using weak hashing algorithms
* automated brute force attacks may try to crack token signatures
* JWT aren't ideal for managing user sessions since they are stateless
* revoking, invalidate JWT access can be challenging
* the payload can get quite large if too much information is included which can affect performance

# Best practices
* keeping JWT payloads compact with only the necessary user claims 
* using short token expiration times when possible
* storing tokens securely and invalidating any leaked tokens
* using strong signature algorithms

==================================================
# question
* để verify token, bình thường ta sẽ hash message và decrypt signature rồi so sánh với nhau; tại sao ta không hash message rồi decrypt nó và so sánh với signature ?
* làm sao để authorize với token ?
* làm sao để tấn công giả mạo user và tấn công brute force để crack token?
* client có cần lấy thông tin để làm gì không ? ví dụ expire time
