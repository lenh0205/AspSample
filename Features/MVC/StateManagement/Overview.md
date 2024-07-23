
# State management
* -> HTTP is a **stateless protocol** - by default, HTTP requests are independent messages that **`don't retain user values`**
* -> **`state`** can be stored using several approaches: **Cookies**, **Session state**, **TempData**, **Query string**, **Hidden fields**, **HttpContext.Items**, **Cache**

========================================================================
# Cookies
* -> we should **store only an identifier in a cookie** with **the data stored by the app (`Ex: using session`)**
* -> because **`cookies are sent with every request`**, their **`size should be kept to a minimum`** (_most browsers restrict cookie size to `4096 bytes` for each domain_)

* -> Cookies are often used for **`personalization`** - the user is **only identified and not authenticated** in most cases
* -> Cookies can store the **`user's name, account name, or unique user ID`**; then use it access the **`user's personalized settings`** (_such as their preferred website background color_)

========================================================================
# 'Session state' in ASP.NET 
* -> **storage of user data** while the **`user browses a web app`** 
* -> **`critical application data`** should be **stored in the user database** and **`cached in session`** only as **a performance optimization**
* -> ASP.NET Core **`maintains session state`** by **providing a `cookie` to the client that contains a `session ID`**; the cookie session ID is used by the app to **fetch the session data**

* -> **`session state`** uses **a store maintained by the app** to persist data across requests from a client; **`session data`** is backed by **a cache** and considered ephemeral data
* _the site should continue to function without the session data_

## Session behavior
* -> the **session cookie is specific to the browser**; sessions aren't shared across browsers
* -> Session cookies are deleted when the browser session ends
* -> If a cookie is received for an expired session, a new session is created that uses the same session cookie.
* -> Empty sessions aren't retained. The session must have at least one value set to persist the session across requests. When a session isn't retained, a new session ID is generated for each new request.
* -> The app retains a session for a limited time after the last request. The app either sets the session timeout or uses the default value of 20 minutes. Session state is ideal for storing user data:
That's specific to a particular session.
Where the data doesn't require permanent storage across sessions.
* -> Session data is deleted either when the ISession.Clear implementation is called or when the session expires.
* -> There's no default mechanism to inform app code that a client browser has been closed or when the session cookie is deleted or expired on the client.
* -> Session state cookies aren't marked essential by default. Session state isn't functional unless tracking is permitted by the site visitor. For more information, see General Data Protection Regulation (GDPR) support in ASP.NET Core.


