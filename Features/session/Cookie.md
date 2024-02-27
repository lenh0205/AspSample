> There are 2 main ways of **`creating and storing sessions`**: **using cookies** or **using tokens**

===============================================================
## Cookies
* -> **a cookie** is a **`small piece of data sent from a website`** that is **`stored on a visitor’s browser`** to help the **`website track the visitor’s activity on the web site`**
* -> a cookie **`identifies`** **a specific visitor or a specific computer** (**`often anonymously`**)
* -> cookies can be used for **authentication, storing site preferences, saving shopping carts, and server session identification**
* -> when the user re-accesses the website, their browser **`sends the cookie back to the server`**

* By knowing who is visiting a site and what they’ve done before, 
* -> web developers **`can customize pages`** to create **a personalized web experience**
* -> _For example_, a cookie may `store information` such as your name and preferences that it `gathered` when you filled out a form, then **`use that information to populate pages you visit throughout one or multiple web sessions`**

## Cookies and Sessions
* _we can simply say that the **`session manage user information in server side`** as **`cookie hold front end data`**_

* -> sessions and cookies are **`sometimes conflated`** **`creating confusion`**; more specifically, **session IDs** and **cookie IDs** are confused
* -> _Server logs typically contain `both the session ID and cookie ID of a visitor`_
* -> **a web session ID** is **`unique to a specific visit`**, while **a cookie** is **`unique to a specific visitor`** and thus (developers hope) **`remains the same through multiple web sessions`**
* -> By **`mapping a single cookie ID to multiple session IDs`**, developers and analysts can get a clearer picture of how visitors interact with their web applications

## Pros and Cons
* -> are **`easy to implement and compatible`** with most browsers and servers
* -> but can be **`vulnerable to XSS and CSRF attacks`**
* -> also _size and number limitations_ that can **`affect performance and bandwidth`**

================================================================
# Tokens
* -> similar to cookies, but they are **`generated and stored on the client-side`** (_usually using JavaScript)
* -> tokens can contain **encrypted** or **signed** **data** that can be **`verified by the server`** without storing them in a database

## Pros and Cons
* -> are more secure and flexible than cookies
* -> but **`require more logic and storage on the client-side`**
* -> along with **`expiration and revocation issues`** that can affect user experience