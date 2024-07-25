==================================================================
> https://learning.postman.com/docs/sending-requests/authorization/authorization-types/
# Authorization Header
* -> The Authorization header is a part of the HTTP request headers used in client-server communications. Its primary function is to authenticate a user-agent with a server, typically by carrying credentials in the form of a token or a set of credentials like username and password. This header is fundamental in implementing security measures for web applications and APIs.

* -> structure:
```r
Authorization: <type> <credentials>
```
* -> Type: This is the authentication scheme, such as Basic, Bearer, Digest, etc. It indicates the method used for encoding or handling the credentials.
* -> Credentials: These are the actual authentication tokens or encoded user credentials. The format and content depend on the authentication scheme.

## Common Authorization Schemes
* -> Basic Authentication, Bearer Authentication, Digest Authentication, OAuth, API Keys, JWT (JSON Web Tokens)

<table>
    <thead>
        <tr>
            <th>Authorization Scheme</th>
            <th>Security Level</th>
            <th>Typical Usage</th>
            <th>Specific Considerations</th>
            <th>Limitations</th>
            <th>Example</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Basic Authentication</td>
            <td>Low</td>
            <td>Simple internal applications, testing</td>
            <td>Base64 encoding of credentials; requires HTTPS for security</td>
            <td>Easily decoded; not suitable for sensitive data</td>
            <td><code>Authorization: Basic QWxhZGRpbjpPcGVuU2VzYW1l</code></td>
            <td>Uses a username and password encoded in Base64. It's simple but less secure, making HTTPS essential.
            </td>
        </tr>
        <tr>
            <td>Bearer Authentication</td>
            <td>Medium to High</td>
            <td>Modern web applications, OAuth 2.0</td>
            <td>Secure token storage and management; use of HTTPS essential</td>
            <td>Token theft risk; requires robust token management</td>
            <td><code>Authorization: Bearer &lt;token&gt;</code></td>
            <td>Utilizes a bearer token, often provided after an initial login. This method is stateless and secure with
                HTTPS.
            </td>
        </tr>
        <tr>
            <td>Digest Authentication</td>
            <td>Medium</td>
            <td>More secure alternative to Basic Auth</td>
            <td>Uses MD5 hashing and nonce values; more secure than Basic</td>
            <td>Vulnerable to certain attacks; less secure than modern token-based systems</td>
            <td><code>Authorization: Digest username="Mufasa", ...</code></td>
            <td>Enhances Basic Authentication by using MD5 hashing of the credentials. It's more secure than Basic but
                still less preferred than token-based methods.
            </td>
        </tr>
        <tr>
            <td>OAuth</td>
            <td>High</td>
            <td>Third-party resource access</td>
            <td>Complex implementation; secure management of tokens and redirect URIs</td>
            <td>Can be complex to implement; potential security pitfalls if not correctly configured</td>
            <td><code>Authorization: Bearer &lt;access_token&gt;</code></td>
            <td>A framework for token-based access to resources on behalf of a resource owner. It allows for secure
                delegated access.
            </td>
        </tr>
        <tr>
            <td>API Keys</td>
            <td>Medium</td>
            <td>API client identification</td>
            <td>Should be kept confidential; often included in HTTP headers</td>
            <td>Broad access if compromised; not user-specific</td>
            <td><code>Authorization: Apikey 123456789abcdef</code></td>
            <td>Unique identifiers used to authenticate a user, developer, or calling program to an API. Simpler than
                OAuth, but should be protected as carefully as passwords.
            </td>
        </tr>
        <tr>
            <td>JWT (JSON Web Tokens)</td>
            <td>Medium to High</td>
            <td>Single sign-on (SSO), information exchange</td>
            <td>Use of strong signing algorithms; validation of token integrity; secure transmission</td>
            <td>Susceptible to attacks if not properly validated; requires secure transmission methods</td>
            <td><code>Authorization: Bearer &lt;JWT_token&gt;</code></td>
            <td>A compact, URL-safe means of representing claims to be transferred between two parties. It allows for
                stateless authentication and secure data exchange. A JWT is composed of three parts, separated by dots
                (.), which are: <strong>Header</strong>, <strong>Payload</strong> and <strong>Signature</strong>. A JWT
                contains all the necessary information about the user, eliminating the need to query the database more
                than once.
            </td>
        </tr>
    </tbody>
</table>

## Security Consideration During Implementation
Implementing authorization schemes securely is crucial to protect sensitive data and maintain the integrity of web applications. Here are the top five implementation considerations for the common authorization schemes:

Use HTTPS for Secure Transmission: Regardless of the authorization scheme used, always ensure that communications occur over HTTPS (Hypertext Transfer Protocol Secure).

Token Security and Management: For schemes involving tokens (like Bearer Authentication and JWT), it's essential to implement secure token generation, storage, and management. This includes using strong, unpredictable tokens, short-lived, and implementing proper expiration and revocation mechanisms.

Robust Validation and Error Handling: Implement strong validation on the server side for any incoming credentials or tokens. This includes verifying the integrity and authenticity of tokens, ensuring that credentials are not tampered with, and checking for expiration or revocation.

Secure Storage of Credentials: If using Basic or Digest Authentication, where credentials are involved, ensure that these credentials are stored securely on the server side. This typically involves hashing passwords using a strong, one-way hashing algorithm and implementing salt to guard against rainbow table attacks. Never store plain-text passwords.

Regular Security Audits and Updates: Regularly audit the security of your authentication implementation. This includes keeping up with the latest security advisories and updates for any frameworks or libraries used in the authentication process.

## HTTP Status Codes
The standard HTTP communication (or Rest APIs) use HTTP status codes for communicating the authentication status. Here is a list of common HTTP status codes related to the authentication process.

200 OK: The request is validated, and found to have right access rights, and the server has responded with the requested resource.
401 Unauthorized: The client request lacks valid authentication credentials. The server requires authentication, typically prompting the user to log in, and these should be sent in the Authorization header.
403 Forbidden: The client is authenticated but does not have permission to access the requested resource. This indicates a lack of sufficient authorization by the client/user. In short, he needs additional permissions to access the resource.
429 Too Many Requests: When the client sends too many requests in a short period, the server may may indicate rate limiting. The 429 status code is used for this purpose. The client should wait for the suggest period to make further API or HTTP calls.
