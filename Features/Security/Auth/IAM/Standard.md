===========================================================
# Identity providers
* **The reason:**
* -> in the past, _the standard for identity and access management_ was for a system to **`create and manage its own identity information for its users`**
* -> each time a user wanted to **`use a new web application`**, they **filled in a form** to **`create an account`**
* -> the application **stored all of their information**, including **`login credentials`**, and **`performed its own authentication`** whenever a user signed in
* -> as the internet grew and more and more applications became available, most people **`amassed countless user accounts`**, each with its **own account name and password to remember**
* -> _there are many applications that continue to work this way_

* => but many others now **`rely on identity providers`** to **reduce their development and maintenance burden and their users’ effort**
* -> an identity provider **`creates, maintains, and manages identity information, and can provide authentication services to other applications`**
* -> Identity providers **don’t share your authentication credentials** with the apps that rely on them

```r 
// Google Accounts is an "identity provider"
// they store account information such as our user name, full name, job title, and email address
// a web like Slate online magazine lets us log in with Google (or another identity provider) rather than go through the steps of entering and storing our information anew 
// but Slate doesn’t ever see our Google password; Google only lets Slate know that we’ve proven our identity. 
```

* other identity providers include **`social media`** (_such as Facebook or LinkedIn_), **`enterprise`** (_such as Microsoft Active Directory_), and **`legal identity providers`** (_such as Swedish BankID_)

# Authentication factors
* -> _Authentication factors_ are **`methods for proving a user’s identity`**
* -> IAM systems require one or many authentication factors to verify identity

* _commonly fall into these basic types:_
* -> **Knowledge** (something you know): Pin, password
* -> **Possession** (something you have): Mobile phone, encryption key device
* -> **Inherence** (something you are):	Fingerprint, facial recognition, iris scan

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