======================================================================
# the ASP.NET session state mechanism 
* -> uses a **session identifier** (**`session ID`**) 
* -> to **`distinguish between different user sessions`** and **`associate the correct session data with each user's requests`**

* _the session lifetime is about 20 minutes by default, but we can modify web.config to change it_
* _if the session is expired, the cookie contain sessionID will be invalid; this may affect user experiences (Ex: re-authentication)_

## Session Initiation: 
* -> when the **`user first accesses the application`**, the ASP.NET runtime **generates a unique session ID**
* -> and **`sends it to the user's browser`** as **a cookie** (_or uses another session identification mechanism, such as `URL-based session IDs`_)

## Session Identification:
* -> for **`each subsequent request made by the user`**, the browser **includes the session ID cookie** (_or other session identifier_) in the **request headers**
* => this allows the ASP.NET runtime to **`identify the correct session`** and associate the request with the **user's session state**

## Session Data Retrieval: 
* -> when we retrieve the value of Session["myProperty"], the ASP.NET runtime **`uses the session ID`** to **look up the correct session data**
* -> then return the value that we **`had previously stored`**

    