> session có tồn tại sau khi người dùng đóng Browser không; và cookie có phải mặc định sẽ bị xoá khi user đóng trình duyệt ?
> đối với authen, session-based authen sẽ sử dụng cookie, còn Token-based sẽ sử dụng Token ID

===========================================================
# IAM integration
* _we will integrate IAM with our **`application, API, device, data store, or other technology`**_

## In simple
* -> this integration can be very `simple`
```r - For example:
// our web application might "rely entirely on Facebook for authentication", and have an "all-or-nothing authorization policy"
// our app performs a simple check: if a user isn’t currently logged in to Facebook in the current browser, we direct them to do so
// once authenticated, all users can access everything in our app.
```
* -> but in practical, it need more `complex IAM solution` to meet the `needs of your users, organization, industry, or compliance standards`

## Practical complexity
* _most systems require some combination of these capabilities:_
* -> **`Seamless signup and login experiences`** - smooth and professional login and signup experiences occur within our app, **with our brand's look and language** 
* -> **`Multiple sources of user identities`** - Users expect to be able to **log in using a variety of identity providers (Google, Linkedin, ...), enterprise (Microsoft Active Directory), ....**
* -> **`Multi-factor authentication (MFA)`** - in an age when passwords are often stolen, **requiring additional proof of identity is the new standard** (_Fingerprint authentication and one-time passwords are examples of common authentication methods_)
* -> **`Step-up authentication`** - access to advanced capabilities and sensitive information require stronger proof of identity than everyday tasks and data; it requires **additional identity verification for selected areas and features**
* -> **`Attack protection`** - **preventing bots and bad actors from breaking into our system** is fundamental to cybersecurity
* -> **`Role-based access control (RBAC)`** - as the number of users grows, managing the access of each individual quickly becomes impractical. With RBAC, people who have the **same role have the same access to resources**

===========================================================
# Integrate IAM in Web Development
* -> IAM solution is a **gatekeeper to the resources** we provide to **customers** as **web applications, APIs, ...** 
* -> the gatekeeper initiates **`authorization`** as outlined in **`OAuth 2.0`**
* -> the addition of the **`OpenID Connect layer`** adds **`authentication`** to secure our users' digital identities and our product

## Choosing Identity platform  
* -> have to **`supports different application types and frameworks`** (_whether our application is **a regular web app, a mobile app, or a machine-to-machine app**_)
* -> **`supporting secure protocols`**
* -> **`allows to customize login services`** to fit our business, our technology, and our customer base
* -> **`connect our user data store, manage those users, choose an authorization grant, and establish authentication factors`** for a seamless, scalable product with an impactful user experience

## Authenticate
* -> the **vehicle of authentication** is the **`login form`**, or **`the intermediary`** to allow our users access to our application
* -> users provide **pre-determined credentials** (_such as username or password_), in the **`login form`** to verify their digital identities

### the "Login form" 
* -> **Passwordless** login with **`biometrics`**
* -> choice of **multi-factor authentication methods** from **`email, voice, or Duo`**
* -> **Single Sign-on (SSO)** capabilities
* -> **Localization** support

### Connect to "user store"
* -> _once we have a login form_, we can **`connect our user store to the Identity platform`**
* -> we can connect an **`existing database`**, or use a social, legal, or enterprise **`identity provider`** (_such as X or Azure Active Directory_)
* -> new users can sign up with the connection we have configured

### Set up Protocol
* -> _once we have a login form and user store connection_, we can set **`protocols that work behind the scenes`** when users log in to our application
* -> the **`most common protocols`** are associated with **the OAuth 2.0 and OpenID Connect (OIDC) specs**
* -> **`another protocol`** to **`securely transmit information`** during log in comes in the form of **Tokens**
* -> **`Tokens`** from the **`Authorization Server`**, the Authentication API transmit information between entities
* -> _When a user logs in and access is approved_, the Authentication API sends an **access token**, an **ID token**, or **`both`** depending on the authentication grant we are **`using to create a session`**
* -> **Access tokens** contain information about what **`scopes, or permissions, the requestor`** has in our application 
* -> while **ID tokens** have **`requestor information`** (_such as user metadata to better the user experience_)

* _Tokens from the Authentication API are **JSON Web Tokens (JWTs)** structured with:_
* -> a **`header`** that includes the signature
* -> the **`payload`** that contains statements and attributes about the requestor
* -> the **`signature`** that verifies the token is valid

* _other protocols, like **SAML** (Security Assertion Markup Language) and **WS-Fed** (Web Service Federation) are used with more specific systems_
* -> **`SAML`** works with **`some identity providers`** 
* -> **`WS-Fed`** is used with **`Microsoft products`**

## Manage users
* -> managing **`user profiles and access`** can be time-consuming
* -> we need easily automate CRUD operations and query user profiles using the Management API
* -> we're must able to categorize our users into categories to arrange our customer-base to fit our management style

### Levels of Access
* _our business model may include **levels of access** for our users_ 
* -> we may want **`a subsection of users to have read-only permissions`** and another **`subsection with the ability to edit`**
* => the Identity platform allows us to implement **role-based access control**
* => we can **create roles**, **assign roles** to users, and **define permissions**

### Limit lifetime of Session
* _if you want to **`manage access`** based on **browser behaviors**, we can **limit the lifetime of a session**_
* -> **a session**, or the **`interaction`** between the **`requesting entity`** and **`our application or resource`**, has a lifetime limit 
* -> a session can end when the **`user closes the browser or navigates away from our webpage`**
* -> we can **`extend sessions`** with **refresh tokens** that renew access tokens

### Cookie
* -> Cookies, or strings of data, **tie into the session** and **represent an authenticated user**
* -> Cookies allow **`our authenticated users to maintain a session`** and **`move between web pages without being forced to re-authenticate`**
* -> once the browser closes, the **cookie is cleared by the browser**

##  Customize
* we may adjust the text of prompts our user receives when an action needs to be completed
* we can configure prompts for our users to signup, to enroll a device for authentication, or to send a code to an email/SMS for users to enter for verification
* we can also customize email communications to welcome new users, verify enrollment, or reset passwords with email templates
* we could add metadata before the user signs up or redirect users to an external site

## Secure
* -> alicious attacks can happen anytime; there're several **attack protection options** (_such as Bot Detection in combination with Google reCAPTCHA Enterprise to prevent cyber attacks_)
* -> some **security options** we can enable : **`Breached Password Detection`**,  **`Brute-Force Protection`**, **`Suspicious IP Throttling`** 

* **Breached password detection** - is a **`security measure against malicious agents with stolen credentials`**

* **Brute-force protection** 
* -> safeguards a targeted user account by **`limiting the amount of login attempts`**
* -> automatically **block the malicious IP** and **send a notification to the flagged user account**

* **suspicious IP throttling**
* -> works where **`brute force protection leaves off`** 
* -> to block traffic from any **`IP address`** that attempts **`rapid signups or logins`**

* _depend on how we want your users to authenticate_ 
* -> enabling **`multi-factor authentication (MFA)`** 
* -> can customize MFA to trigger under certain circumstances, such as a user logging in from an **`unknown device or from a questionable IP address`**

