> defined in OAuth 2.0 RFC 6749, section 4.1

# Authorization Code Flow
* -> involves **`exchanging an 'Authorization Code' for a 'Token'`**
* -> this flow **`can only be used for "confidential applications"`** (such as **`Regular Web Applications`**) 
* _because the **application's authentication methods are included in the exchange** and **must be kept secure**_

## Steps in "Authorization Code Flow"
* -> User selects Login within application.
* -> Auth0's SDK redirects user to Auth0 Authorization Server (/authorize endpoint)
* -> Auth0 Authorization Server redirects user to login and authorization prompt.
* -> User authenticates using one of the configured login options, and may see a consent prompt listing the permissions Auth0 will give to the application.
* -> Auth0 Authorization Server redirects user back to application with single-use authorization code.
* -> Auth0's SDK sends authorization code, application's client ID, and application's credentials, such as client secret or Private Key JWT, to Auth0 Authorization Server (
/oauth/token
endpoint).
* -> Auth0 Authorization Server verifies authorization code, application's client ID, and application's credentials.
* -> Auth0 Authorization Server responds with an ID token and access token (and optionally, a refresh token).
* -> Application can use the access token to call an API to access information about the user.
* -> API responds with requested data