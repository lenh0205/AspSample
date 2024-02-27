
# Session state 
* -> **`Session state`** was originally meant for **user provided data** (_like a shopping cart_)
* -> one thing **`session is not meant for`** is **caching of user profile data**

# caching per-user data
* -> in **`classic ASP`** there were not a lot of other caching per-user data, so **`session was the obvious choice`**
* -> but in **`.NET`** there are so many better options; one of them is **data cache**

# "data cache" in ASP.NET
* -> data cache is very useful for **`a place to store data`** when we want to **`avoid round trips to the database`**
* -> with the data cache, we specifically code to **`deal with failure`** (the cache item might not be present) 
* -> and we can also set very specific conditions per cache item **`designating how long we'd like the cache`** the item

# why "data cache" instead of "session"
* -> **`data cache`** and **`session state`** (_by default_) is stored **in-memory**
* -> session state is a horrible option to remember data for your users (_i.e. shopping cart_)
* -> because in ASP.NET, the **hosting application pool** **`frequently recycles`** and this is typically out of our control
* -> this means that **`all that in-memory state is lost`**, which is why using **session state in-proc** is a bad choice

# Out-of-proc session
* -> to keep data for the users, we simply **`can’t rely upon in-memory state`**
* -> as an alternative, we can store our **session out-of-proc** (in the **`ASP.NET State Service`** which is a windows service, or in **`SQL Server`**, or in **`a custom store`**)
* -> notice that **`out-of-proc`** will **defeats the purpose of caching**

* => this certainly makes the **`state more resilient`**, but **there is a cost** to this resiliency

# Fallback of Out-of-proc session
* -> **`each HTTP request`** into the application means that our application must make **2 extra network requests** to the **`out-of-proc store`**
* -> one to **`load session`** before the request is processed and another to **`store the session back`** after the request is done processing
* -> this is a fairly heavy-weight request as well, because the state being loaded is the **`entire state for the user for the entire application`**

* -> in other words if you have ten pages in our application and _`each one puts a little bit of data into session`_, when you visit “page1” then you’re _`loading all of the session data`_ for “page1”, “page2”, “page3”, ... 
* -> by the way, these extra network requests happen on every page even if you’re not using session on the page

* _There are **`some optimizations`**_
* -> **EnableSessionState on the Page** and **SessionStateAttribute for MVC**
* -> but they don’t solve the problem entirely because the network request must happen **`to update the store to let it know the user is still active`** so that it doesn’t cleanup inactive sessions

* => this is why we prefer to **`use data cache`** for **caching** and **`not session`**
* => The data cache is for read-only data that can be reloaded from the original data source when the cache expires and session is meant for user driven data like shopping carts (_except it has all the problems above_)

========================================================
#  what to do for our shopping cart data? 
1. explicitly disable session in your application’s web.config (it’s enabled by default in the machine-wide web.config), 
2. keep the user data elsewhere. Elsewhere might mean in a cookie, in a hidden field, in the query string, in web storage or in the database (relational or NoSQL)

* For shopping cart like data, we could use the database
* -> create a custom "shopping cart" table 
* -> when users add items then you make network call to update the database
* -> once the user places an order then you’d clear out rows for the user’s shopping cart
* -> As for the key in the shopping cart table, we can simply use **`the user’s unique authentication username`** from **User.Identity.Name**

## Problem

* network calls
* abandoned shopping carts
* anonymous users

* effort
* ->  modern ORMs we have of if you’re using a NoSQL database then it’s even easier. Also, this database doesn’t have to be your main production database — it could just be a database that’s for the webserver.

2) What about all these network calls? Well, if you were using session then in-proc was a non-starter and you were going to have to keep this data out-of-proc. So you were already going to incur heavy-weight network calls. With this approach only the pages that need the shopping cart data incur the network calls and the pages that don’t need the shopping cart data won’t have this burden imposed upon them. This is already better than out-of-proc session.

3) With this approach you will need to have some way to periodically cleanup abandoned shopping carts. Sure, but the application is still better off for the effort. Also, this assumes that you do want to get rid of the data, but think about amazon — they don’t ever want to get rid of shopping cart data. That data is potential sales (they remind you on every visit that you forgot to give them your money) and it’s probably useful for data mining. So maybe keeping this data is a good thing.

4) I suggest that you should use the authenticated user’s username for the key in the shopping cart table. Well, what about anonymous users? Part of the Session feature is that there is a “Session ID” associated with the user and this is sometimes quite useful. It is, but not at the expense of at of the other parts of session. So what to do? Well, there’s a little known feature in ASP.NET called the Anonymous Identification Module. This is a HttpModule whose sole purpose is to track anonymous users with an ID. If you enable this feature, then it issues a cookie with a unique ID per anonymous users. You can then access that ID in your code via Request.AnonymousID.