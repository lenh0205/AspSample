> bài viết này đề cập đến những API để server của ta tương tác với Auth server
> khi nói về "confidential applications" thì ta cần hiểu là những yếu tố được đề cập liên quan đến việc giao tiếp giữa server của ta với Auth server (nơi mà securely store secrets), chứ không phải user với server 

======================================================================
# Authentication API
* -> enables us to **manage all aspects of user identity** when we use Auth Server; it offers **endpoints** so our users can **`log in, sign up, log out, access APIs,...`**
* => used by **`front-ends`** and **`untrusted parties`**
* => supported various **`identity protocols`** including **OpenID Connect**, **OAuth**, and **SAML**
* => if we are building our authentication UI manually, we will need to **`call the Authentication API directly`** or we can **`consume this API through some library integrated with the Auth Server`**

* _some of these **`tasks`** include:_
* -> get **tokens** during authentication
* -> request a **user's profile** using an Access Token
* -> exchange **Refresh Tokens** for new Access Tokens
* -> request a challenge for **MFA** - **`multi-factor authentication`**

## Base URL
* the Authentication API is served over **HTTPS**. All URLs have the following base: **`https://{yourDomain}`**

## Authentication methods

### OAuth2 Access Token 
* -> send a **`valid Access Token`** in the **Authorization header**, using the **`Bearer authentication scheme`**

* _an example is the "Get User Info" endpoint_
* -> we get an **Access Token** when we **`authenticate a user`**
* -> then we can **`make a request`** to the "Get User Info" endpoint, using that **`token in the Authorization header`**, in order to retrieve the **`user's profile`**

### Client ID and Client Assertion (confidential applications)
* _cơ bản là để request Access Token thì Client sẽ generate 1 cái JWT được sign bởi private key_
* -> generate a **client assertion** containing **`a signed JSON Web Token (JWT)`** to authenticate
* -> in the body of the request, include our **`Client ID`**, a **`client_assertion_type`** parameter with the value **`urn:ietf:params:oauth:client-assertion-type:jwt-bearer`**, and a **`client_assertion`** parameter with your signed assertion

```json
# Example JWT in Client Assertion
iss (Issuer) → Your Client ID.
sub (Subject) → Also your Client ID.
aud (Audience) → The Token Endpoint URL.
exp (Expiration time)

# Request:
POST https://login.microsoftonline.com/{tenant}/oauth2/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=YOUR_CLIENT_ID
&client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer
&client_assertion=YOUR_SIGNED_JWT
&resource=https://graph.microsoft.com/
```

### Client ID and Client Secret (confidential applications)
* -> Send the Client ID and Client Secret. The method you can use to send this data is determined by the Token Endpoint Authentication Method configured for your application.
* -> If you are using Post, you must send this data in the JSON body of your request.
* -> If you are using Basic, you must send this data in the Authorization header, using the Basic authentication scheme. To generate your credential value, concatenate your Client ID and Client Secret, separated by a colon (:), and encode it in Base64.
* -> An example is the Revoke Refresh Token endpoint. This option is available only for confidential applications (such as applications that are able to hold credentials in a secure way without exposing them to unauthorized parties).

### Client ID (public applications)
* -> Send the Client ID. For public applications (applications that cannot hold credentials securely, such as SPAs or mobile apps), we offer some endpoints that can be accessed using only the Client ID.
* -> An example is the Implicit Grant.

### mTLS Authentication (confidential applications)
* -> Generate a certificate, either self-signed or certificate authority signed. Then, set up the customer edge network that performs the mTLS handshake.
* -> Once your edge network verifies the certificate, forward the request to the Auth0 edge network with the following headers:
* -> The Custom Domain API key as the cname-api-key header.
* -> The client certificate as the client-certificate header.
* -> The client certificate CA verification status as the client-certificate-ca-verified header. For more information, see Forward the Request.

===========================================================
# Management API
* -> is a collection of endpoints to **complete administrative tasks** programmatically (_allows us to manage our Auth Server_), so we can automate configuration of our environment 
* -> should be used by **`back-end servers`** or **`trusted parties`**

* _some of these **`tasks`** include:_
* -> **register our applications and APIs** with Auth Server
* -> set up **connections** with which our users can authenticate
* -> **manage users**
* -> **link user accounts**

## Authentication
* -> use of the Management API requires **`a Management API access token`**
* -> the Management API uses **`JSON Web Tokens`** (JWTs) to authenticate requests
* -> the Management API access token’s **scopes** claim indicates which **`request methods`** can be performed when calling this API
* -> trying to perform any _request method_ **`not permitted within the set scopes`** will result in a **`403 Forbidden response`**

```json - the deserialized token grants read-only access to users, and read/write access to connections
{
 "aud": "m8DAxghyfE0KdpzogfXgMSxrkCSdKVEF",
 "scopes": { 
   "connections": { 
     "actions": ["read", "update"] 
   } 
 },
 "iat": "1446056652",
 "jti": "7e9c6a991f5a227fb7ebaa522536ae4c"
}
```

* -> to **`make calls to the API`**, send the **`API token`** in **the Authorization HTTP header** using the **`Bearer authentication scheme`**
```r
curl -H "Authorization: Bearer eyJhb..." https://@@TENANT@@.auth0.com/api/v2/users
```

## Request Correlation
* -> a **Correlation ID** is a **`unique identifier`** (_up to 64 characters_) of a **`single Management API operation`** and allows for **tracking such operations in tenant logs**
* -> the API accepts a **`client-provided Correlation ID`** if sent with the **X-Correlation-ID HTTP** header within **`POST, PUT, PATCH, and DELETE`** methods
* _If an X-Correlation-ID header value longer than 64 characters is provided, only the first 64 characters will be shown in the logs_

```r
curl -H "Authorization: Bearer eyJhb..." -H "x-correlation-id: client1_xyz" https://@@TENANT@@.auth0.com/api/v2/users
```

## Management API Access Tokens
https://auth0.com/docs/secure/tokens/access-tokens/management-api-access-tokens 
