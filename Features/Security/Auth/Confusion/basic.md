> https://auth0.com/docs/glossary

===========================================================  
#  Identity Glossary - các thuật ngữ identity thường dùng

## digital resource
* -> is any **`combination of applications and data`** in a computer system
* -> _examples of digital resources_ include **`web applications`**, **`APIs`**, **`platforms`**, **`devices`**, or **`databases`** 

## Identity (_the core of IAM_)
* -> someone wants access to your resource, it could be _a customer, employee, member, participant,..._
* -> in IAM, a **user account** is a **`digital identity`**; _user accounts_ can also represent **`non-humans`**, such as software, Internet of Things devices, or robotics

# Authentication and Authorization
* -> it’s common to confuse authentication and authorization because they **`seem like a single experience to users`**
* -> they are two separate processes: **authentication** proves a **`user's identity`**, while **authorization** grants or denies the **`user’s access to certain resources`**

## Authentication
* -> determines **`whether users are who they claim to be`**
* -> challenges the user to **validate credentials** (_for example: through passwords, answers to security questions, or facial recognition_)
* -> usually done **`before authorization`**
* -> generally, transmits info through an **ID Token**
* -> generally governed by the **OIDC - OpenID Connect protocol**

```r - Example:
Employees in a company are required to authenticate through the network before accessing their company email
```

## Authorization
* -> determines what users **`can and cannot access`**
* -> verifies whether **`access is allowed`** through **policies and rules**
* -> usually done **`after successful authentication`**
* -> generally, transmits info through an **Access Token**
* -> generally governed by the **OAuth 2.0 framework**

```r - Example
After an employee successfully authenticates, the system determines what information the employees are allowed to access
```

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


