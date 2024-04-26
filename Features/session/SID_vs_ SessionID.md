# Session
* -> **identifies a particular client**
* -> data put in **`Server Object`** is **persists on the server**, so it is **`secure and cannot be messed with by the client`**
* -> thường thì sẽ có 1 **`State trong Session Object`** để biết User **Authentication** (_login_) hay chưa
* -> thường thì các Framework sẽ hỗ trợ ta **`phân tích cookie rồi tìm kiếm Session Object`**, nếu ta lấy ra 1 **`empty Session Object`** (_s/d Request.Session chẳng hạn_) thì ta biết đó là 1 User mới 

# SID
* -> **`SID`** được Server dùng để tìm kiếm **Session Object** bên trong **`Session Store`**; nó sẽ được bỏ trong **Cookie** để trao đổi qua lại giữa "Client" và "Server"
* -> thường thì "SID" có thể sẽ là 1 chuỗi combine của **`SessionID`** và **signature** (_use hash + secret + encrypt_) của **`SessionID`**

# SessionID
* -> sẽ là an internally **unique id** for each **`session object`**; used in the internal implementation of the session store for some purpose

=========================================================================
# In ASP.NET, just store a "state" in "session object" to show "user login" is enough or not ?

## **ASP.Net_SessionId** cookie 
* -> used to **identify the users session on the server**; the session being an area on the server which can be used to _store data in between http requests_
* -> tức là a different user will submit a different cookie; and thus **`Session["Variable"]`** will **hold a different value for that different user**

```cs - VD: ta update/main "state" trong "session"
// controller Action perform:
Session["FirstName"] = "abcxyz"; 
// the subsequent Action can retrieve it from session:
var firstName = Session["FirstName"]; // "abcxyz"
```

## "ASPXAUTH" cookie:
* -> **identify if the user is authenticated** (that is, has **`their identity been verified`**)
* -> For example, a controller action may determine if the **`user has provided the correct login credentials`** and if so **issue a authentication cookie** using:
```cs
FormsAuthentication.SetAuthCookie(username, false);
```
* -> then later we can check if the **`user is authorized`** to perform an action by using the **`[Authorize] attribute`** which **checks for the presence of the "ASPXAUTH" cookie**

## Summary
* ->  these cookies are there for 2 different purpose; one to determine the users session state and one to determine if the user is authenticated
* -> corresponding to 2 concept **Session State Management** and **Authentication Management using Form Authentication**
* -> we **`could not use ASPXAUTH cookie`** and just **`use session to identify the user`**; but it's better to have a cleaner separation of concerns 

* => by using seperately, session and authentication will **have their own time-out values set**
* => if using alone **`ASP.NET_SessionId`** alone can cause **Session Fixation**
* => using **`Forms Authentication Cookie`** alone can cause **can’t Terminate Authentication Token on the Server**

* also, **`loosely Coupled ASP.NET_SessionID and Forms Authentication Cookies`** can still Vulnerable