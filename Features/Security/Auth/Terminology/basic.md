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

## Authen/Author with Client or Server ?
* **Authentication and Authorization happen in "Server"** , **`not Client`**
* -> Lý do là **can not trust Client** 
```cs - VD: Auth on client 
// như kiểu ta có 1 user, what button can they click ?
``` 

* nhưng nó **vẫn hoàn toàn OK to show and hide UI base on actual user datas or user information** để nói cho user what they can do (_VD: ta s/d 1 Boolean variable like "isAdmin" to able to show and hide different part of UI_) (_nhưng verification và check phải ở server_)
```cs - VD:
// Nếu User là "Admin", ta sẽ show "delete" and "edit" button while "normal user" don't have that
// Kể cả khi ta change UI/ change Client để "delete" button hiện ra cho "normal user" 
// => Server vẫn không cho phép ta thực hiện hành động đó
```

* ta có thể vào DevTool/Application để view Session trong Cookie, nó chỉ là 1 đoạn text **it doesn't matter what that is, it's just a unique identifier**
* -> nếu ta xoá nó thì giống như ta logout

* và ta cần đảm bảo ta **`securely store sessionID`** vì nếu **someone else has access to sessionID, they can logged in as user without knowing "user name" and "password"**
```cs - VD: Ta có được sessionID của user
// Ta vào DevTool/Application/Cookies của 1 trang web ta đang login copy lại value của "session"
// Ta mở tab ẩn danh -> mở trang web -> DevTool/Application -> paste lại cặp key/value vừa copy vào "Cookies"
// Refresh lại trang và ta sẽ thấy trang được login
```

* Vậy nên it a good idea to **periodically remove current sessions** from our session map
* -> if a user have **`inactive for a certain period of time, we'll remove that session`**, means that user is `logout`

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


