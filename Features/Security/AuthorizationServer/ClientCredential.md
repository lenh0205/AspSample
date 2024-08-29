
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
* -> the **client** will not have an **`interactive user`**; it'll request an **`access token`** from the **Identity Server** using its **`client ID`** and **`secret`**
* -> then use the token to gain access to the API

## Preparation
* -> install the **`IdentityServer templates`**
```r
dotnet new -i IdentityServer4.Templates
```

============================================================================

# Securing an API using 'Client Credentials' grant type

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
* -> ta sẽ dùng **`HTTP Authorization header`** để send **Access Token** to the **API** 
* _using **`SetBearerToken`** extension method of **HttpClient** object được định nghĩa trong thư viện **IdentityModel**_

* -> ta sẽ chạy select **Multiple Startup Projects** trong Visual Studio để có thể start đồng thời "Api" và "IdentityServer"; sau đó **Start New Instance** cho "Client" 
* -> ta sẽ thấy giờ thì API đã **`accepts any access token issued by our identity server`**

## Authorization at the API - ASP.NET Core authorization policy system
* -> ta sẽ cần thêm đoạn code để kiểm tra presence of the **`scope`** in the **access token** that the **client asked for (and got granted)**
* -> và ta sẽ sử dụng **`ASP.NET Core authorization policy system`** để làm điều đó

* _ta có thể áp 1 policy ở nhiều cấp độ: **`globally`**, **`for all API endpoints`**, **`for specific controllers/actions`**_

## Summary
* -> **Client** sẽ sử dụng 1 method cho phép gửi request sử dụng **`Client Credential Grant`** với nội dung bao gồm:
* -> **client Id** là "client", **client secret** là "secret", **scope** là "api1", **address** (_`token endpoint` lấy `OIDC discovery`_)
* -> request này sẽ được gửi tới **address** (_i **`/connect/token`** endpoint của **IdentityServer**_) để lấy về **`token`** (_ở đây là **`Access Token`**_)
* -> sau đó **Client** sẽ gửi 1 request đến **protected endpoint 'Get'** (_trả về 1 list **claim**_) của **resource server** - bằng cách bỏ **`Access Token`** vào **`Authorization header bearer`** 

* -> **IdentityServer** sẽ cần định nghĩa list những **`Client`** và list những **`ApiScope`** mà nó sẽ hỗ trợ
* -> ở đây ta sẽ định nghĩa 1 **ApiScope** với tên "api1"
* -> ta cũng sẽ định nghĩa 1 **Client** với **`ClientId`** là "client", **`ClientSecrets`** là "secret" và **`AllowedScopes`** là "api1"
* => vậy nên nếu 1 máy "client" gửi request đến "IdentityServer" để lấy "Access Token" thì sẽ cần cung cấp đầy đủ những thông tin này 
* -> ta sẽ s/d **`.AddIdentityServer()`** để load những definition này

* -> **resource server** sẽ add **`Authentication service`**, những việc mà service này sẽ làm là:
* -> đầu tiên nó sẽ validate **`token có đến từ trusted issuer`** bằng cách so sánh **iss** lấy được từ "access token" với giá trị của **expected hosting url của IdentityServer** mà ta truyền vô, 
* -> thứ hai nó sẽ validate **`token is valid to be used with this api`** bằng cách kiểm tra **aud**
* -> **resource server** cũng add **`Authorization service`**, nó sẽ:
* -> tạo sẽ thêm 1 **`Policy`** gọi là "ApiScope" để kiêm tra xem **scope** trong "access token" có bao gồm "api1"
* -> sau đó khi khởi tạo API endpoint, ta sẽ thêm **policy** này vào endpoint ta cần để nó kiểm tra

============================================================================
