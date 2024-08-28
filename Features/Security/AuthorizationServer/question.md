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

============================================================================
# Quick starts 

## ToDo
* _this is the most scenario for protecting APIs using IdentityServer_
* -> **`adding IdentityServer`** to an ASP.NET Core application
* -> **`configuring IdentityServer`**
* -> **`issuing tokens`** for various clients
* -> securing **`web applications`** and **`APIs`**
* -> adding support for **`EntityFramework`** based configuration
* -> adding support for **`ASP.NET Identity`**

## Overview
* -> we'll define **an API** and **a Clients** with which to access it
* -> the **client** will request an **`access token`** from the **Identity Server** using its **`client ID`** and **`secret`**
* -> then use the token to gain access to the API

## Preparation
* -> install the **`IdentityServer templates`**
```r
dotnet new -i IdentityServer4.Templates
```

# Quickstack #1: Securing an API using Client Credentials

* create an **`ASP.NET Core`** application include **`basic Identity setup`** 
```r
dotnet new is4empty -n IdentityServer
```
* -> **`Program.cs`** and **`Startup.cs`** - **the main application entry point**
* -> **`Config.cs`** - **IdentityServer resources** and **`clients configuration file**
* -> Output: app running on **https** protocol and the port is set to **5001** when running on **Kestrel** or a random one on **IISExpress**

## Defining an API Scope
* -> ta sẽ load API resource thông qua file **`Config.cs`** vì Template này sử dụng **'code as configuration' approach**

## Defining the client
* -> define **`a client application to access our new API`**
* -> the client will **authenticate** using the **`ClientId`** and **`ClientSecret`** with **IdentityServer** (_as the **login** and **password** for our application itself_)
* -> it **`identifies our application to the identity server`** so that it knows which application is trying to connect to it

## Configuring IdentityServer
* -> we'll **load resource and client definitions** in **Startup.cs** - add services using **`.AddIdentityServer()`**, ....
* -> giờ ta có thể run the server and navigate to "https://localhost:5001/.well-known/openid-configuration" (_xem `~\Features\Security\Auth\Protocol\OpenID_Connect phần discovery document` để hiểu_)
* -> also in first startup, IdentityServer sẽ tạo 1 **`developer signing key`** (_a file called **``**tempkey.jwk`**_) cho ta

## Create an Web API 
* -> create a project with "ASP.NET Core Web API template" running on "https://localhost:6001" 
* -> to **`test the authorization requirement`**, as well as **`visualize the claims identity through the eyes of the API`**

* -> ta sẽ cần add the **authentication services to DI** and the **authentication middleware to the pipeline**
* -> ta sẽ cần install NuGet package: **Microsoft.AspNetCore.Authentication.JwtBearer**
* -> từ đó ta có thể **`validate the incoming token to make sure it is coming from a trusted issuer`** and **`validate that the token is valid to be used with this api (aka audience)`**

* _hiện tại khi ta access vào "https://localhost:6001/identity" thì response sẽ trả về **401 status code** - chứng tỏ nó đang được **`protected by IdentityServer`** và **`require a credential to access`**_

## Creating the client
* -> ta sẽ tạo 1 **a client** (_ta sẽ tạo 1 project với template là **Console App**_) that **`requests an access token`**, and **`then uses this token to access the API`**

* -> we will use a client library called **IdentityModel** that encapsulates the protocol interaction in an easy to access the **`token endpoint of IdentityServer that implement OAuth 2.0 protocol`**
* _but we can always use raw HTTP to access it_
* -> also, **IdentityModel** includes a client library to use with the **discovery endpoint**
* => this way we only need to know the **base-address of IdentityServer** - the actual endpoint addresses can be read from the metadata

* -> h thì ta sẽ **add code để get token từ IdentityServer**
* -> ta sẽ start 2 project là Client và IdentityServer lên (_nếu gặp lỗi **SSL certificate** thì xem `~/Resolver/experient/Debug_Error.md` để fix_)
* -> nếu lấy được **Access Token** thì ta thửu pass nó vào "jwt.ms" để xem **`raw token`**, nó sẽ trông như sau:
```json
{
  "alg": "RS256",
  "kid": "B2F02363B9CF44F9E88FD16E946C7D65",
  "typ": "at+jwt"
}.{
  "nbf": 1724841020,
  "exp": 1724844620,
  "iss": "https://localhost:5001",
  "client_id": "client",
  "jti": "BBCFED9EC5B28F7F9B0F7385935E5EA5",
  "iat": 1724841020,
  "scope": [
    "api1"
  ]
}.[Signature]
```

## Calling the API