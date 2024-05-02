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
> https://gist.github.com/ncoblentzsps/284306cbc1b91d1a11c9a61f876ec47d

# In ASP.NET, just store a "state" in "session object" to show "user login" is enough or not ?

## Session Fixation & Forms Authentication Token Termination in ASP.NET
* -> ASP.NET applications commonly have one or more vulnerabilities associated with the use of **`ASP.NET_SessionId cookies`** and **`forms authentication cookies`**
* -> **`ASP.NET_SessionId cookies`** and **`forms authentication cookies`** can be **used alone or together to maintain state with a user’s browser**

* -> The forms authentication cookie, named .ASPXAUTH by default, . W.

## ASP.Net_SessionId cookie 
* -> is **an identifier** used to **`look up session variables stored on the server-side`**; the **cookie itself does not contain any data**
* -> used to **identify the users session on the server**; the session being an area on the server which can be used to _store data in between http requests_
* -> tức là a different user will submit a different cookie; and thus **`Session["Variable"]`** will **hold a different value for that different user**

```cs - VD: ta update/main "state" trong "session"
// controller Action perform:
Session["FirstName"] = "abcxyz"; 
// the subsequent Action can retrieve it from session:
var firstName = Session["FirstName"]; // "abcxyz"
```

## Forms Authentication Cookies - "ASPXAUTH" cookie:
* -> **identify if the user is authenticated** (that is, has **`their identity been verified`**)
* -> contains **encrypted data, stored only on the client-side**
* -> when it is **`submitted in a request`** to the server, it is **`decrypted and used by custom application code`** to make **authorization** decisions
* -> For example, a controller action may determine if the **`user has provided the correct login credentials`** and if so **issue a authentication cookie** using:
```cs
FormsAuthentication.SetAuthCookie(username, false);
```
* -> then later we can check if the **`user is authorized`** to perform an action by using the **`[Authorize] attribute`** which **checks for the presence of the "ASPXAUTH" cookie**

## Summary
* ->  these cookies are there for 2 different purpose; one to determine the users session state and one to determine if the user is authenticated
* -> corresponding to 2 concept **Session State Management** and **Authentication Management using Form Authentication**
* -> we **`could not using ASPXAUTH cookie`** and just **`use session to identify the user`**; but it's better to have a cleaner separation of concerns 

* => by using seperately, session and authentication will **have their own time-out values set**
* => if using alone **`ASP.NET_SessionId`** alone can cause **Session Fixation**
* => using **`Forms Authentication Cookie`** alone can cause **can’t Terminate Authentication Token on the Server**

* also, **`loosely Coupled ASP.NET_SessionID and Forms Authentication Cookies`** can still Vulnerable

=========================================================================
# Problem
## ASP.NET_SessionId Alone: Session Fixation
* -> the root cause of this vulnerability is that the **ASP.NET_SessionId cookie value isn’t changed or regenerated after users log in** (_or cross any kind of authentication boundary_)
* -> in fact, **`Session IDs are intentionally reused in ASP.NET`**
* -> if an **attacker steals an ASP.NET_SessionId prior to a victim authenticating**, then the attacker can use the cookie value to **`impersonate the victim after he or she logs in`**
* => this gives the attacker unauthorized access to the victim’s account

## Forms Authentication Cookie Alone: Can’t Terminate Authentication Token on the Server
* -> when a forms authentication cookie is used alone, applications **`give users (and potentially attackers) control over when to end a session`**
* -> this occurs because the forms authentication ticket is an encrypted set of fields **`stored only on the client-side`**
* -> the **server can only request that users stop using the value when they log out**
* -> the ASP.NET framework **`does not have a built-in feature to invalidate the cookie on the server-side`**
* => that means, **clients (or attackers) can continue using a forms authentication ticket even after logged out**
* => allows an attacker to **`continue using a stolen forms authentication token`** despite **`a user logging out`** to protect him or herself

## Loosely Coupled ASP.NET_SessionID and Forms Authentication Cookies: Still Vulnerable
* -> applications can combine both strategies and use forms authentication and sessions
* -> in this arrangement, the **`forms authentication cookie`** is **`used for authentication and authorization decisions`**, and the **`session cookie`** is used to **`store additional state information`**
* -> but commonly, the ASP.NET framework does not **explicitly couple a specific forms authentication cookie to an ASP.NET_SessionId**
* => any **`valid forms authentication cookie`** can be used with any other **`valid session cookie`**
* => depending on the implementation, this results in **`a session fixation vulnerability`** (for the ASP.NET_SessionId cookie), the **`inability to terminate authenticated sessions on the server side`** (for the forms authentication cookie), or both vulnerabilities

=========================================================================
# Solution

## Possible Solution to mitigate the risk: Tightly Couple ASP.NET_SessionIDs to Forms Authentication Identities
* -> use both the **`ASP.NET_SessionId cookie`** and **`a forms authentication cookie`**, and to **`tightly couple them`** **using the user’s identity as the link**
* => means: _in an application that uses `forms authentication`_, the **identity of the user is stored** in **`session variables`** (_do this manually_) and **`the forms authentication ticket`** (_normal use of forms authentication_)
* => then, **on each request** to the application, the **identity associated with each cookie should be compared**. 
* => if they do not match, **`invalidate the user’s session`** and **`log them out`**

* -> this prevents session fixation by ensuring that **`an ASP.NET_SessionId cookie`** (vulnerable to session fixation) **must be coupled the user’s own forms authentication token** (not vulnerable to session fixation)
* -> rather than just **`any individual’s forms authentication token`**
* -> additionally, it allows **`forms authentication tokens`** to **`be indirectly invalidated on the server-side`** by **destroying the session that is associated with it**

* => since **`both cookies`** have to be **`present and linked by the user’s identity`**, **each protects against the weaknesses of the other**

## Solution Implementation in an ASP.NET MVC 4 Application
* -> create a global function that would **`execute for every controller and action`** to **`ensure the user identity referenced in session variables matched those stored by the forms authentication ticket`**
* -> for unauthenticated users, the session should not reference a user identity at all
* -> if either of these two conditions are violated, the user is logged out, their session is destroyed, and they are redirected back to the login page

```cs - MVC Action Filter Attribute for validation
// CoupleSessionAndFormsAuth.cs:

public class CoupleSessionAndFormsAuth : ActionFilterAttribute
{
    /* Occurs before the controller action is executed
        * Verifies one of two sitations:
        *   1. If the user is authenticated, the username in the session matches the username in the forms authentication token
        *   2. If the user does not have a forms authentication token, their session should not include any identity information, like a username
        * If any of these cases are violated, then the user will be logged out, their session will be destoryed, and they will be redirected to the login page
        * The following conditions will allow the user to reach the controller action:
        *   1. They do not have a forms auth token, and their session does not contain identity information
        *   2. They have a forms auth token, their session contains an identity, and the usernames match in both the forms auth token and the session
        */
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //Grab the username from the session. returns null or the username
        String username = (String)filterContext.HttpContext.Session["UserName"];
        
        if (!WebSecurity.Initialized)
        {
            //clear the session
            filterContext.HttpContext.Session.Abandon();
            //redirect to the login page if not already going there
            if (!(filterContext.Controller is AccountController && filterContext.ActionDescriptor.ActionName.ToLower() == "login"))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } });
            }
        }
        //If the user is authenticated, compare the usernames in the session and forms auth cookie
        //WebSecurity.Initialized is true
        else if (WebSecurity.IsAuthenticated)
        {
            //Do the usernames match?
            if (username == null || username != WebSecurity.CurrentUserName)
            {
                //If not, log the user out and clear their session
                WebSecurity.Logout();
                filterContext.HttpContext.Session.Abandon();
                //redirect to the login page
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } });
            }
        }
        //If the user is not authenticated, but the session contains a username
        //WebSecurity.Initialized is true
        //WebSecurity.IsAuthenticated is false
        else if (username != null)
        {
            //log the user out (just in case) and clear the session
            WebSecurity.Logout();
            filterContext.HttpContext.Session.Abandon();
            //redirect to the login page
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } });
        }

        base.OnActionExecuting(filterContext);
    }
}
```

```cs - added the filter to the global filters list "Global.asax.cs" for executed for every controller and action
// Global.asax.cs:
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();

        //register CoupleSessionAndFormsAuth Filter for all controllers
        GlobalFilters.Filters.Add(new CoupleSessionAndFormsAuth());

        WebApiConfig.Register(GlobalConfiguration.Configuration);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        AuthConfig.RegisterAuth();
    }
}
```

```cs - added one line of code to the AccountController to ensure the user’s identity was added to session variables when the user logs in
[Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
      
      /* skipped quite a few actions here */
      
      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public ActionResult Login(LoginModel model, string returnUrl)
      {
          if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
          {
              //store the user's identity in the session
              Session["UserName"] = model.UserName;                
              return RedirectToLocal(returnUrl);
          }

          ModelState.AddModelError("", "The user name or password provided is incorrect.");
          return View(model);
      }
      
      /* skipped quite a few actions here */
        
    }
```