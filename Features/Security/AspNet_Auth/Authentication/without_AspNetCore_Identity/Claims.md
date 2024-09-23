
# Mapping, customizing, and transforming claims in ASP.NET Core
* -> **Claims** can be **`created from any user or identity data`** which can be **`issued using a trusted identity provider`** or **ASP.NET Core identity**

* -> **a claim** is **`a name value pair`** that **`represents what the subject is`**, **not what the subject can do**

===================================================================
# Mapping claims using OpenID Connect authentication
* -> the **`profile claims can be returned in the 'id_token'`**, which is returned after **a successful authentication**
* -> **the ASP.NET Core client app** **`only requires the profile scope`**
* -> when **using the `id_token` for `claims`**, **`no extra claims mapping is required`**
