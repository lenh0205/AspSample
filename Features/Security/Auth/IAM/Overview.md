> ta cần suy nghĩ liệu có thể gom Authentication và Authorization thành duy nhất chỉ 1 quá trình là Authorization được không ? tức là chỉ cần đơn giản bảo vệ quyền truy cập đến resource
> nếu vậy mỗi lần truy cập protected resource ta sẽ cần nhập password, đó là lý do cần có thuật ngữ Authen để tách riêng về phần user experience
> vì đơn giản quá trình Authen chỉ cần thực hiện duy nhất 1 lần, còn quá trình Author thì phải thực hiện nhiều lần tương ứng với mỗi lần truy cập protected resource
> vậy liệu có khả năng thực hiện việc Author duy nhất 1 lần như Authen không ?
> ta cần hiểu đúng: thực chất Authen cũng phải được thực hiện nhiều lần như Author, ta chỉ đơn giản là không nhập password nhiều lần thôi; ta sẽ sử dụng thứ khác (Token/SessionID) thay cho password
> mỗi lần truy cập protected resource, ta vẫn phải Authen trước Author sau dựa trên Token/SessionID

> https://auth0.com/docs/get-started/identity-fundamentals/identity-and-access-management 
> Auth0 chính là 1 IAM platform

===========================================================
# Identity Glossary - các thuật ngữ identity thường dùng
* https://auth0.com/docs/glossary  

===========================================================

# IAM - Identity and Access Management 
* -> provides control over **user validation** and **resource access**
* -> this technology ensures that the **`right people`** access the **`right digital resources`** at the **`right time`** and for the **`right reasons`**

## Fundamental Concept

* a **digital resource**
* -> is any **`combination of applications and data`** in a computer system
* -> _examples of digital resources_ include **`web applications`**, **`APIs`**, **`platforms`**, **`devices`**, or **`databases`** 

* **Identity** (_the core of IAM_)
* -> someone wants access to your resource, it could be _a customer, employee, member, participant,..._
* -> in IAM, a **user account** is a **`digital identity`**; _user accounts_ can also represent **`non-humans`**, such as software, Internet of Things devices, or robotics

* **Authentication**
* -> is the **`verification of a digital identity`**
* -> someone (or something) authenticates to **`prove that they're the user they claim to be`** 

* **Authorization**
* -> is the process of determining **`what resources a user can access`**  

## Authentication and Authorization
* _it’s common to confuse authentication and authorization because they **`seem like a single experience to users`**_
* -> they are two separate processes: **authentication** proves a **`user’s identity`**, while **authorization** grants or denies the **`user’s access to certain resources`** 

## What does "IAM" do?
* _Identity and access management gives us control over user validation and resource access:_ 
* -> **How users become a part of your system ?**
* -> **What user information to store ?**
* -> **How users can prove their identity ?**
* -> **When and how often users must prove their identity ?**
* -> **The experience of proving identity ?**
* -> **Who can and cannot access different resources ?**

## IAM itegration
* we integrate IAM with our **`application, API, device, data store, or other technology`**

* _this integration can be very simple_
```r - For example:
// our web application might "rely entirely on Facebook for authentication", and have an "all-or-nothing authorization policy"
// our app performs a simple check: if a user isn’t currently logged in to Facebook in the current browser, we direct them to do so. Once authenticated, all users can access everything in our app.
```

* _but in practical, it need more complex IAM solution to meet the needs of your users, organization, industry, or compliance standards; most systems require some combination of these capabilities:_
* -> **Seamless signup and login experiences** - smooth and professional login and signup experiences occur **`within your app, with our brand’s look and language`** 
* -> **Multiple sources of user identities** - Users expect to be able to **`log in using a variety of identity providers`** like social (such as Google or Linkedin), enterprise (such as Microsoft Active Directory), ....
* -> **Multi-factor authentication (MFA)** - in an age when **`passwords are often stolen`**, requiring **`additional proof of identity`** is the new standard (_Fingerprint authentication and one-time passwords are examples of common authentication methods_)
* -> **Step-up authentication** - access to **`advanced capabilities and sensitive information`** require stronger proof of identity than everyday tasks and data; it requires additional identity verification for selected areas and features
* -> **Attack protection** - rreventing **`bots and bad actors`** from breaking into our system is fundamental to cybersecurity
* -> **Role-based access control (RBAC)** - as **`the number of users grows`**, managing the access of each individual quickly becomes impractical. With RBAC, people who have the **`same role have the same access to resources`**

## How does IAM work? 
* -> IAM is **`not one clearly defined system`**; IAM is **`a discipline and a type of framework`** for **solving the challenge of secure access to digital resources**
* -> there’s no limit to the different approaches for implementing an IAM system

* _there're elements and practices in common implementations:_

## Identity providers
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

## Authentication factors
* -> Authentication factors are **`methods for proving a user’s identity`**
* -> IAM systems require one or many authentication factors to verify identity

* _commonly fall into these basic types:_
* -> **Knowledge** (something you know): Pin, password
* -> **Possession** (something you have): Mobile phone, encryption key device
* -> **Inherence** (something you are):	Fingerprint, facial recognition, iris scan

## Authentication and authorization standards
* **`Authentication and authorization standards`** are **open specifications and protocols** that provide guidance on how to:
* -> **`Design IAM systems to manage identity`**
* -> **`Move personal data securely`**
* -> **`Decide who can access resources`**

* _these **`IAM industry standards`** are considered the most secure, reliable, and practical to implement:_

### OAuth 2.0
* -> OAuth 2.0 is **`a delegation protocol`** for **`accessing APIs`** and is the **industry-standard protocol for IAM** 
* -> **`an open authorization protocol`**, OAuth 2.0 lets an app **access resources hosted by other web apps** on behalf of a user **without ever sharing the user’s credentials**
* -> it’s the standard that allows **`third-party developers`** to **`rely on large social platforms`** (_like Facebook, Google, and Twitter_) for **login**

### Open ID Connect
* -> a simple **identity layer** that sits **`on top of OAuth 2.0`**
* -> OpenID Connect - **OIDC** makes it easy to **`verify a user’s identity`** and **`obtain basic profile information`** from the **identity provider**
* -> OIDC is another **open standard protocol**

### JSON web tokens
* -> JSON web tokens (JWTs) are **an open standard** that defines **`a compact and self-contained`** way for **`securely transmitting information`** between parties as a JSON object 
* -> JWTs can be **`verified and trusted`** because they’re **digitally signed**
* -> they can be used to **`pass the identity of authenticated users`** **between the identity provider and the service requesting the authentication**
* -> they also can be **`authenticated`** and **`encrypted`**

### Security Assertion Markup Language (SAML)
* -> is an **open-standard, XML-based data format** 
* -> lets **`businesses`** communicate **`user authentication and authorization information`** to **`partner companies and enterprise applications that their employees use`**

### Web Services Federation (WS-Fed)
* -> Developed by Microsoft and used extensively in their applications
* -> this standard defines the way **`security tokens`** can be **`transported between different entities`** to **`exchange identity and authorization information`**

## Why use an IAM platform?
* -> User expectations, customer requirements, and compliance standards introduce significant technical challenges
* -> with multiple user sources, authentication factors, and open industry standards, the amount of knowledge and work required to build a typical IAM system can be enormous
* -> built-in support for all identity providers and authentication factors, offers APIs for easy integration with our software
* -> relies on the most secure industry standards for authentication and authorization

* => we should **`build on an identity and access management platform`** (_Ex: Auth0_) instead of building our own solution from the ground up


