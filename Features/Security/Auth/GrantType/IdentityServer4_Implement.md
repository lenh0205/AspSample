================================================================
# Implement 'Grant Types' in IdentityServer4
* -> _Grant types_ specify **how a client can interact with the token service**

* -> specify which **grant types a client can use** via the **`AllowedGrantTypes`** property on the **Client configuration**
* -> the **`GrantTypes`** class can be used to **pick from typical grant type combinations**

* -> this allows **locking down the protocol interactions** that are allowed for a given client
* -> a client can be configured to use **more than a single grant type** (_e.g. Authorization Code flow for user centric operations and client credentials for server to server communication_)

```cs
Client.AllowedGrantTypes = GrantTypes.CodeAndClientCredentials;

Client.AllowedGrantTypes =
{
    GrantType.Code,
    GrantType.ClientCredentials,
    "my_custom_grant_type"
};
```

## Covering
* -> while IdentityServer **`supports all standard grant types`**
* -> we really **`only need to know 2`** of them for **common application scenarios**

================================================================
# Machine to Machine Communication
* -> this is the **`simplest type of communication`**
* -> **`tokens`** are always requested on behalf of a **`client`**, **`no interactive user is present`**

* => in this scenario, client **send a token request to the token endpoint** using the **`client credentials`** grant type
* -> the client typically has to **authenticate with the token endpoint** using its **`client ID`** and **`secret`**

================================================================
# Interactive Clients
* -> this is **`the most common type of client scenario`**: **web applications**, **SPAs** or **native/mobile apps** with **`interactive users`**
* => for this type of clients, the **`authorization code flow`** was designed

## Step
* _that flow consists of two physical operations:_
* -> **a front-channel step** via **`the browser where all "interactive" things happen`**, e.g. login page, consent etc
* _this step **results in an authorization code** that represents the outcome of the front-channel operation_
* -> **a back-channel step** where the **`authorization code from step 1 gets exchanged with the requested tokens`**. Confidential clients need to authenticate at this point

## Security properties
* _this flow has the following security properties:_
* -> **`no data gets leaked over the browser channel`** (_besides the **authorization code** which is basically a random string_)
* -> authorization codes **`can only be used once`**
* -> the **`authorization code can only be turned into tokens`** when the **client secret is know** (_for confidential clients - more on that later_)

* => this sounds all very good; however, still there is one problem called **`code substitution attack`**
* => there are two modern mitigation techniques for this: **`OpenID Connect Hybrid Flow`**, **`RFC 7636 - Proof Key for Code Exchange (PKCE)`**

## cut and pasted code attack / Frankenstein Monster Attack - code substitution attack
* -> is _an attack_ that the adversary **`swaps the 'code' in the authorization response with the victim's 'code' that the adversary has gotten hold of somehow`**
* -> it can be through the **Code Phishing attack**, or some other attacks

* -> then, he uses the 'code' in his browser session to feed to the client so that the **`client uses it to get the access token with victim user's privilege`**
* -> so, it is a kind of **privilege escalation attack**

* -> the 'code' is a string that binds the front end session through the browsers and the back end session between the client and the authorization server. 
* -> the adversary **`swaps this 'binding string' and swaps the session`**
* -> so, it is a kind of **session swap attack** from a technical point of view

* _https://nat.sakimura.org/2016/01/25/cut-and-pasted-code-attack-in-oauth-2-0-rfc6749/_

## OpenID Connect Hybrid Flow
* -> this uses a **response type** of **`code id_token`** to **`add an additional identity token to the response`**
* -> this token is **`signed`** and **`protected against substitution`**
* -> in addition it contains the **hash of the code** via the **`c_hash`** claim
* => this allows checking that we indeed got the right code (experts call this **`a detached signature`**)

* _this solves the problem but has the following down-sides:_
* -> the **`id_token`** gets transmitted over the front-channel and **`might leak additional (personal identifiable) data`**
* -> all the mitigation steps (e.g. crypto) need to be **implemented by the client** - this results in **`more complicated client library implementations`**

## RFC 7636 - Proof Key for Code Exchange (PKCE)
* -> this essentially introduces **`a per-request secret for code flow`**
* -> **`all the client`** has to implement for this, is creating **`a random string`** and **`hashing it using SHA256`**

* => this also solves the **substitution problem**, because the **`client can prove that it is the same client on front and back-channel`**

* _this also has the following additional advantages:_
* -> the **client implementation is very simple** compared to hybrid flow
* -> it also solves the problem of the **absence of a static secret for public clients**
* -> **no additional front-channel response artifacts are needed**

# Interactive clients without browsers or with constrained input devices (RFC 8628)
* -> this flow **`outsources user authentication and consent`** to an **external device** (e.g. a smart phone)
* -> it is typically used by **devices that don't have proper keyboards** (e.g. TVs, gaming consoles…) and **`can request both identity and API resources`**
