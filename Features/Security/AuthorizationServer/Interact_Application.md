> 

===========================================================================
# Interactive Applications with ASP.NET Core
* -> we'll add support for **`interactive user authentication`** via the **`OpenID Connect protocol`** to our "IdentityServer"
* -> and create an **MVC application** that will **use IdentityServer for authentication**

===========================================================================

## Adding the UI for IdentityServer
* -> hiện tại **is4empty server** của đã có tất cả _protocol support cần cho `OpenID Connect`_; giờ ta sẽ cần thêm phần hỗ trợ những UI cần thiết (_login, logout, consent and error,..._) 
* -> bằng cách ta sẽ chạy lệnh **`dotnet new is4ui`** ngay trong **is4empty** template project của ta để nó build **`QuickStart UI của IS4`** template on top of our project (_nó sẽ bao gồm những folder, class, view cho phần UI_) 
* -> tiếp theo ta sẽ cần cấu hình **Startup.cs** để enable **`MVC`** cho project của ta vì **QuickStart UI** sử dụng **MVC-based**

* _trong trường hợp ta phải setup từ đầu thì ta có thể dùng template **`is4inmem`** cho nhanh - nó sẽ có cả **basic IdentityServer** including the **standard UI**_

## adding support for 'OpenID Connect Identity Scopes' to IdentityServer
* -> **`scopes`** represent **something we want to protect and that clients want to access**
* -> similar to **OAuth 2.0**, **OpenID Connect** also uses the **`scopes concept`**
* -> however in contrast to **OAuth**, **scopes in OIDC don't represent APIs**, but **`identity data like user id, name or email address`**

* -> we add support for the standard **`openid`** (_subject id_) and **`profile`** (_first name, last name ..._) **scopes** by amending the **IdentityResources** property in **Config.cs**
* -> then register resources in **Startup.cs**

* -> ta sẽ thêm 1 số user để test bằng cách gọi **.AddTestUsers** extension method mà sample UI đã cũng cấp sẵn 

## Adding the MVC Client to the IdentityServer Configuration
* -> **OpenID Connect-based clients** are very **similar** to the **OAuth 2.0 clients** we added so far
* -> but since the **`flows in OIDC are always interactive`**, we **need to add some redirect URLs to our configuration**

===========================================================================

## Creating an MVC client
* -> ta sẽ tạo thêm 1 **`ASP.NET Core MVC Web Application`** project để làm **Client** 
* -> add Nuget package **`Microsoft.AspNetCore.Authentication.OpenIdConnect`** that containing the **OpenID Connect handler** để thêm support cho **`OpenID Connect authentication`**

* -> ta sẽ cấu hình lại **`.AddAuthentication`** service 
* -> we'll use **Cookies** as **`DefaultScheme`** để locally sign-in user bằng cookies
* -> use **oidc** as **`DefaultChallengeScheme`** để khi cần login user ta sẽ dùng **OpenID Connect protocol**
* -> ta chaining thêm **`.AddCookie`** để add the **handler that can process cookies**

* -> và chaining thêm **`.AddOpenIdConnect`** để **configure the handler that performs the OpenID Connect protocol**
* -> **`Authority`** indicates where the **trusted token service is located**
* -> the **`ClientId`** and the **`ClientSecret`** **identify this client**
* -> **`SaveTokens`** is used to **persist the tokens from IdentityServer in the cookie** (to be used later)
* -> **`ResponseType = "code"`** - use **`Authorization code flow`** with **`PKCE`** to **connect to the OpenID Connect provider**
 
* -> Ngoài ra, sử dụng **`.UseAuthentication()`** to ensure the **execution of the authentication services on each request**; and **`.RequireAuthorization`** method **disables anonymous access for the entire application**
* -> h ta có thể dùng **[Authrorize] attribute** to specify authorization on a per controller or action method basis

* -> ta sẽ dùng "Home" view để hiển thị **claims of the user** cũng như **cookie properties**

* => Lưu ý, nếu giờ ta dùng Browser để navigate tới application này thì **`a redirect attempt will be made to IdentityServer`**
* => và điều này sẽ gây ra lỗi vì thằng **Mvcclient** chưa được đăng ký với IdentityServer

## add 'Sign-out' to MVC client
* -> with an **authentication service like IdentityServer**, it is **`not enough to clear the local application cookies`**
* -> in addition we also need to **make a roundtrip to the IdentityServer** to **`clear the central single sign-on session`**

* -> the **OpenID Connect handler** we add before already the implement exact protocols step for sign-out; 
* -> we just need to call **`SignOut("Cookies", "oidc")`** (_method của Microsoft.AspNetCore.Mvc.ControllerBase_)  inside an action method
* -> this will **`clear the local cookie`** and **`then redirect to the IdentityServer`**
* -> the **IdentityServer** will **`clear its cookies`** and then **`give the user a link to return back to the MVC application`**

## Getting claims from the UserInfo endpoint
* -> even though we've configured the **client** to be **allowed to retrieve the "profile" identity scope**,
* -> **`the claims associated with that scope (such as name, family_name, website, ...) don't appear in the returned token`**

* -> we need to tell the client to **pull remaining claims from the `UserInfo` endpoint** by **`specifying scopes that the client application needs to access`** and **`setting the 'GetClaimsFromUserInfoEndpoint' option`**
* -> now we just need to **restart the client app, logging out, and logging back** in we will see **additional user claims associated with the "profile" identity scope** displayed on the page

===========================================================================
# Summary
* -> Ta sẽ migrate cho **IdentityServer** cho thêm phần hỗ trợ UI

===========================================================================
## Adding Support for External Authentication
* -> we'll **`add support for external authentication`** just by adding **an ASP.NET Core compatible authentication handler**
* -> **`ASP.NET Core itself ships with support for Google, Facebook, Twitter, Microsoft Account and OpenID Connect`**
* _for other authentication providers: https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers_

## Add Google support
* -> 