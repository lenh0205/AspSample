# Idea of JWT
* to create a standard way to **securely communicate** between 2 parties (exchange information/**`claims`**)
* But then **JWT is so widely used for Authorization**
* Some **Authorization strategies** include **`Session token`**, **`JSON web Token`**

* To accessing a simple static HTML page from a server, ta chỉ cần gửi URL of the page -> we need another page, just send another URL
* so technically, the server doen't need to remember you have previous request

====================================================
# Token-based Authentication - JSON Web Token
* **JWT is just for `Authorization` not Authentication**

* **`Authentication`** - take username and password then autheticating to make sure username and password is correct (login)

* **`Authorization`** 
* -> make sure the user send request to our Server is the same user that actually logged in during the Authentication process; 
* -> it's authorizing that this user has access to this particular system
* -> normally done by using session


# JWT process: 
* -> Client send its login detail to Server 
* -> instead of storing information on the Server inside the session memory, Server generates a JWT 
* -> Json Web Token is created from all the information of user by encode, serialize and sign it with its own **secret key** 
* -> then send JWT back to browser where it's normally kept in `local storage` (_vẫn có thể để chỗ khác_)
* -> on future Client request, JWT will be ensured to included to the `Authorization header` prefix by `Bearer` (Authoriztion: Bearer<token>) 
* -> Server signed to that JWT with its own sercret key - it verifies if JWT has not been changed since the time that it signed it; 
* (_if client change JWT, change the user information in JWT; Server will know and say it invalid_)
* -> if it's not, Server will deserialize that JWT, take user information stored in token
* -> now Server know exactly what to do with that user and if that user is authorized to use that resource it'll send the response back down to the client


* User Info is stored in the actual token which mean it's stored on the client; the Server then only needs to validate the signature
* => Server doesn't have to store anything (no need to database lookup somewhere else in the infrastructure)
* => that mean we can use the same JWT accross multiple Servers without run into problem where 1 server has a certain session and the other server doesn't (it's more efficient when dealing with a distributed system in the cloud)

# How JWT signs its Tokens and store User Information
```cs - Encoded JWT - send between server and client to Authorize user
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```
* Gồm 3 phần: **`HEADER`**, **`PAYLOAD`**, **`VERIFY SIGNATURE`**

* **HEADER**: point out the Algorithm we using to encode and decode token
* -> just a base64 encoded
* -> content:
```json
{
    "alg": "HS256", // the Algorithm
    "typ": "JWT" // JWT token type
} // only useful for signature portion at the very end where we're verifying it 
```

* **PAYLOAD**: put all information, data for application to store in token
```json
{
    "sub": "1234567890", // như là ID của User we're authenticating
    "name": "John Doe",
    "iat": 1516239022, // "issued at" - when token was created -> useful for expire token
    "exp": 1516239025 // when token expire at (invalid) -> useful when someone take your JWT
}
```

* **VERIFY SIGNATURE**: allow us to verify that token hasn't been changed by client (user didn't tamper token before send back to server)
* -> it take the first 2 portion `header` and `payload` of token đã được encode bằng base64 và combine them with period "."
* -> use the Algorithm which defined in the header to hash using **`secret key`**
```js
HMACSHA256(
    base64UrlEncode(header) + "." + base64UrlEncode(payload),
    ourSecretKey  
)
```

# Flow of validate a JWT:
* when Server gets the JWT from the client 
* -> s/d Algorithm chỉ định trong header để hash - 1 function nhận 2 tham số
* -> **First parameter** - server sẽ decode 2 phần đầu và combine chúng: `base64UrlEncode(header) + "." + base64UrlEncode(payload)` 
* -> **Second parameter** - **`secret key`**
* => s/d kết quả trả vể so sánh với phần `Verify signature` của JWT gửi từ Client ; nếu match thì ta biết token vẫn nguyên vẹn từ lúc ta gửi cho client và nhận lại

* User chỉ có thể fake when they have access to `secret key`. Nhưng nó sẽ không xảy ra as long as we safely store our secret key on our Server, unaccessible to other outside 
* Cách nó hoạt động giống như Password Hashing: input như nhau thì hashing out sẽ như nhau và không thể bị unhashing 

# Why using JWT - the common usecase:
* JWT is popular in the context of micro-services

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

# Disavataged of JWT
* JWT vẫn có thể cướp bởi attacker; JWT can also difficult to invalidate (vô hiệu hoá); JWT can't be used to authenticate a user in the background on the server