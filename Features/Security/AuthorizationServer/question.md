============================================================================
> trong `https://identityserver4.readthedocs.io/en/latest/intro/big_picture.html`, Applications have two fundamental ways with which they communicate with APIs – using the application identity, or delegating the user’s identity ?

# The Big Picture
* -> cho ta thấy **`interaction`**tổng quan giữa 1 hệ thống gồm các tầng, lớp clients và servers khác nhau (**Browser**, **Web API**, **Web App**, **Native App**, **Server App**) 
* -> để thêm security mà tránh duplicate giữa các chúng ta sẽ dùng 1 **`Security Token Service`**
* -> và hệ thống sau khi support bởi **Security Token Service** sẽ cần follow **`architect và một số protocol`**
* -> design này sẽ chia security concern thành 2 phần **`Authentication`** và **`API Access`**

* -> có nhiều **Authentication protocol** nhưng **`OpenID Connect`** is future - vì nó được designed theo kiểu **API friendly**

* -> **OAuth2** is a **protocol** that **`allows applications to request access tokens from a security token service`** and **use them to communicate with APIs**
* => this delegation reduces complexity in both the client applications as well as the APIs since **`authentication and authorization can be centralized`**

* => the **combination of OpenID Connect and OAuth 2.0** (_i **`OpenID Connect is an extension on top of OAuth 2.0`**_) is the best approach to secure modern applications
* -> **2 fundamental security concerns** (_authentication and API access_) are combined into **a single protocol** - often with **`a single round trip to the security token service`**

* => **`IdentityServer4`** is **an implementation of these two protocols** and is highly optimized to solve the typical security problems of today's **mobile, native and web applications**
* -> is **`middleware`** that **`adds the spec compliant OpenID Connect and OAuth 2.0 endpoints`** to **an arbitrary ASP.NET Core application**
* -> nói đơn giản dù ta có 1 hosting application phức tạp, nhưng ta chỉ sử dụng middlware này để add **`necessary protocol heads`** cho **authentication related UI** (_login/logout page, consent..._) 
* -> nhờ đó mà **client applications can talk to our application** **`using those standard protocols`**

============================================================================
# Terminology

## IdentityServer
* hay **Security Token Service** / **Identity Provider** / **Authorization Server** / **OpenID Connect Provider** / **IP-STS** là tương tự nhau - đơn giản là **`a piece of software that issues security tokens to clients`**
* -> **`protect our resources`**
* -> **`authenticate users using a local account store or via an external identity provider`**
* -> **`provide session management and single sign-on`**
* -> **`manage and authenticate clients`**
* -> **`issue identity and access tokens to clients`**
* -> **`validate tokens`**

## User
* -> a human that is using **a registered client** to access resources

## Client
* -> is _a piece of software_ that **`requests tokens from IdentityServer`** - either for **authenticating a user** (requesting an **`identity token`**) or for **accessing a resource** (requesting an **`access token`**)
* -> **`a client must be first registered with IdentityServer before it can request tokens`**

* _Examples for clients are web applications, native mobile or desktop applications, SPAs, server processes, ..._

## Resources
* -> are something we want to **`protect with IdentityServer`** - either **`identity data`** of our users, or **`APIs`**
* -> **Identity data** - **`claims`** / **identity information** about a user (_e.g. name or email address_)
* -> **APIs** - **APIs resources** represent functionality a client wants to invoke (_typically modelled as **`Web APIs`**, but not necessarily_)

* _every resource has a unique name - and clients use this name to specify to which resources they want to get access to_

## Identity Token
* -> _an Identity Token_ represents the **`outcome of an authentication process`**
* -> contains at **a bare minimum an identifier for the user** (_called the **`sub`** aka **`subject claim`**_) and information about **how and when the user authenticated**; also can contain additional identity data

## Access Token
* -> **`allows access to an API resource`**
* -> **clients request access tokens** and **forward them to the API**
* -> Access tokens contain **`information about the client and the user (if present)`** - APIs use that information to authorize access to their data.





