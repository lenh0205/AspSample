# ASP Request Object
* a browser asks for a page from a server -> called a request -> _Request object_ is used to **`get information from a visitor`**

* -> contains `collecttion`: **ClientCertificate**, **Cookies**, **Form**, **QueryString**, **ServerVariables**
* -> contains `Properties`: **TotalBytes**
* -> contains `Method`: **BinaryRead**

========================================
# Get Request Info:
## Gets the specified portion of a Uri instance - Uri.GetLeftPart(UriPartial)
*

========================================
# ASP ServerVariables Collection
* is used to **`retrieve the server variable values`**
* -> để lấy: **Request.ServerVariables (server_variable_name)**
* -> `server_variable_name`: ALL_HTTP, HTTP_<HeaderName>, HTTP_COOKIE, ...

* **IIS Server Variables** provide information about the `server`, the `connection with the client`, and the `current request` on the connection
* -> **URL rewrite rules** can be used to set custom server variables


==========================================
# Use "Proxy header" to obtain Client's "IP Address"
* the `proxy headers` (**Forwarded** or **X-Forwarded-For**) are the right way to get your client IP **`only when we are sure they come to us via a proxy`**
* If there is `no proxy header or no usable value in`, we should default to the **REMOTE_ADDR** server variable
 
*  X-FORWARDED-FOR, X_FORWARDED_FOR and HTTP_X_FORWARDED_FOR are the same

## "X-Forwarded-For" header 
* -> way of `identifying the originating IP address` of the user connecting to the **`web server`** coming from either **a HTTP proxy, load balancer**

* -> will have the address of the end client

* -> this header is **non-standard** - not part of RFC
* -> the way it is handled `can differ on the proxy implementation`
* => should not rely on that value since it can be spoofed or simply not sent by proxy

* **.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]**
* [https://pmichaels.net/2021/01/02/viewing-server-variables-in-asp-net-core/#:~:text=In%20Asp.Net%20Core%2C%20you,the%20request%20call%20and%20security.&text=You'll%20need%20to%20be,go%20in%20the%20routing%20zone.]

```cs
// NET 6
var remoteIpAddress = request.HttpContext.Connection.RemoteIpAddress;
```
```cs
var serverVariables = httpContext.Features.Get();
var value = serverVariables["AUTH_USER"];
```

## "Forwarded" header
* -> a **standard header** definition `for proxy` approved by IETF
* -> should be **`use instead of X-Forwarded headers`** to get **originating IP** in case request is handled by a proxy

## "REMOTE_ADDR" header
* will have the **`address of the proxy server`**; If there is no proxy, REMOTE_ADDR will have the address of the end client

## "X-Forwarded-IP" 
* is the conventional way of identifying the originating IP address of the user connecting to the **`email server`** through an **HTTP mail service**