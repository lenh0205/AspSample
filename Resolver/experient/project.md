# Dnn Authentication

## Authentication config
```xml - web.config
<authentication mode="Forms">
    <forms name=".DOTNETNUKE" protection="All" timeout="60" cookieless="UseCookies" />
</authentication>
```
* -> vậy nên khi viết API trong HelperCommon, nó sẽ mặc định cần **`authentication`** 
* -> đòi hỏi request đến API đó cần có **`Cookie`** tên **`.DOTNETNUKE`**
* -> để viết 1 **`Anonymous API`** không cần Authentication, ta chỉ cần gắn **[AllowAnonymous]** attribute:
```cs
[AllowAnonymous]
[HttpPost]
public void ClientUpload() {}
```

## Cookie
* trong Dnn ngoài cookie **`.DOTNETNUKE`** để Authen, còn 1 số cookies khác để 

### Maintain State:
* -> **`.ASPXANONYMOUS`** - để quản lý Anonymous User
```xml
<anonymousIdentification enabled="true" cookieName=".ASPXANONYMOUS" cookieTimeout="100000" cookiePath="/" cookieRequireSSL="false" cookieSlidingExpiration="true" cookieProtection="None" domain="" />
```
* -> **`ASP.NET_SessionId`** - a cookie used to **identify the users' session on the server**; the session used to store data in between off HTTP requests on server

### Security
* **`__RequestVerificationToken_L3RwdGh1ZHVj0`** 
* -> this is called the **Request Verification** mechanism in ASP.NET Razor Pages 
* -> anti forgery token (**prevent CSRF attack**) guarantees that the poster is the one who gets the form
* -> prevents from anybody to **`forge a link and have it activated by a powered user`**

## Session in Dnn
* DNN doesn't use **`session`**
* -> there is NO attack risk since DNN **does not use the session id for its authentication**
* -> the **`Session fixation`** may be reported by a number of different test and site check systems; but this's **not true**

* **Session fixation** - allows an attacker to impersonate a user by abusing an authenticated session ID (SID)
* -> this attack can occur when a web application: `Fails to supply a new, unique SID to a user following a successful authentication` or `Allows a user to provide the SID to be used after authenticating`
* -> this is because DNN does **not null out the sessionid on logout**

* nhưng ta cần hiểu rằng nó vẫn có tác động lên some **`shopping cart type systems`** s/d SessionId
* -> vậy nên 1 best practice là **luôn clear sessionid when logging out**
* -> ta có thể làm nó như sau bằng cách chỉnh sửa **`/DesktopModules/Admin/Authentication/logoff.aspx.cs`** file
```cs
private void DoLogoff()
{
    try
    {
        //Remove user from cache
        if (User != null)
        {
            DataCache.ClearUserCache(PortalSettings.PortalId, Context.User.Identity.Name);				
        }

        Session.Clear();             ///  add these two lines
        Session.Abandon();      ///  add these two lines

        var objPortalSecurity = new PortalSecurity();
        objPortalSecurity.SignOut();
    }
    catch (Exception exc)	//Page failed to load
    {
        Exceptions.ProcessPageLoadException(exc);
    }
}
```

## Get Current User in Dnn
```cs
Response.Write("user " & DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().Username); 
```
