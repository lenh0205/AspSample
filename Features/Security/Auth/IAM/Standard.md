===========================================================
# Authentication and authorization standards
* **`Authentication and authorization standards`** are **open specifications and protocols** that provide guidance on how to:
* -> **`Design IAM systems to manage identity`**
* -> **`Move personal data securely`**
* -> **`Decide who can access resources`**

* _these **`IAM industry standards`** are considered the most secure, reliable, and practical to implement:_

## OAuth 2.0
* -> OAuth 2.0 is **a delegation protocol** for **`accessing APIs`** and is the **industry-standard protocol for IAM** 
* -> **`an open authorization protocol`**, OAuth 2.0 lets an app **access resources hosted by other web apps** on behalf of a user **without ever sharing the user’s credentials**
* -> it’s the standard that allows **`third-party developers`** to **`rely on large social platforms`** (_like Facebook, Google, and Twitter_) for **login**

## Open ID Connect
* -> a simple **identity layer** that sits **`on top of OAuth 2.0`**
* -> OpenID Connect - **OIDC** makes it easy to **`verify a user’s identity`** and **`obtain basic profile information`** from the **identity provider**
* -> OIDC is another **open standard protocol**

## JSON web tokens
* -> JSON web tokens (JWTs) are **an open standard** that defines **`a compact and self-contained`** way for **`securely transmitting information`** between parties as a JSON object 
* -> JWTs can be **`verified and trusted`** because they’re **digitally signed**
* -> they can be used to **`pass the identity of authenticated users`** **between the identity provider and the service requesting the authentication**
* -> they also can be **`authenticated`** and **`encrypted`**

## Security Assertion Markup Language (SAML)
* -> is an **open-standard, XML-based data format** 
* -> lets **`businesses`** communicate **`user authentication and authorization information`** to **`partner companies and enterprise applications that their employees use`**

## Web Services Federation (WS-Fed)
* -> Developed by Microsoft and used extensively in their applications
* -> this standard defines the way **security tokens** can be **transported between different entities** to **`exchange identity and authorization information`**
