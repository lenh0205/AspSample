
============================================================
# HTTP is stateless
* _on the internet_, there's a problem - **`the web server does not know`** _who you are or what you do_, because the **HTTP is stateless**
* -> **Session variables** solve this problem by **`storing one single user information`** (_Ex: username, favorite color, ..._) to be **`used across multiple pages`** in application
* -> _For every client, session data is **`stored separately`**_

============================================================
# Session
* -> a way to **track users** and **protect private user information** (_their preferences, actions, and permissions_) to be **`used across multiple pages/ multiple request`**
* -> **Session ID** associates **a user** (_user’s requests_) with **a specific session** (_session data_) stored on the server side, allow manage multiple sessions simultaneously

* -> the information is **`stored in server`**, rather than stored on the _users computer (like `cookie`)_
* -> includes **`contiguous actions by a visitor`** on an **`individual website`** within **`a given time frame`**
* -> include your _search engine searches, filling out a form to receive content, scrolling on a website page, adding items to a shopping cart, researching airfare, or which pages you viewed on a single website_
* -> any interaction that you have with a single website is recorded as **`a web session`** to that **`website property`**

## A Web Session
* by default, **a Session**  refer to **`a visitor’s time browsing a web site`**
* -> last from a visitor’s _first arrival at a page on the site_ and the time they _stop using the site_
* -> a session may also includes **`an expiration`**, so a single session never lasts beyond a certain duration

==========================================================
# crucial application of "session" usage

* commonly utilize web sessions to **authenticate** users on a website or web application
* -> additionally, a session is created when **`a user logs in`** to a site
* -> furthermore, it allows the user to **`access restricted content or perform actions`** only available to authenticated users.

* using web sessions to **personalize the user experience** on a website
* -> For example, a site might use a session to **`remember a user’s preferences`**, such as their `language or preferred currency`

* utilize session in **e-commerce sites to manage shopping carts**
* -> when a user adds an item to their cart, **`a session is created`** that allows the site to **`keep track of the items in the cart`**

* using session to **track user behavior** using sessions
* -> For example, a site might use a session to **`track which pages a user visits`** and **`how long they spend`** on each page

==========================================================
# Basic mechanism

## Create Session
* -> when a user logs into a web application, **`the server creates a session`** and it **`retrieves information`** related to the user and previous session activity
* -> the session receives **`a unique identifier`**, generally a session ID or token, which is often **`stored in a cookie`** on the **`user’s browser`**

## Track Sessions
* -> Each time the user logs on to the web application, the session ID **`helps the server identify the user`** 
* -> this session ID is **passed along with any HTTP requests** that the visitor makes **`while on the site`** (_Ex: clicking a link_)

## Storing Sessions
* -> stores **`session data`** (_information related to the session) in the **server's memory** or **a separate session store** (_such as a **`Redis database`**_) 
* -> when a user logs in, server **`creates a session object`** and **`assigns it a unique ID`** - **Session ID**
* -> the session ID is then **`stored in a cookie`** on the **`user's browser`**
* -> server **`uses the session ID`** to **`retrieve the session data`** from the _server_ or _session store_

## Destroy Sessions
* when a user **logs out** or the **session timeout**, we need to destroy the session (_`deletes the session data` from the server or session store_) to ensure that **`session data is not stored indefinitely`**

==============================================================
## Why is "a web session" used 
* to **`avoid storing massive amounts of information in-browser`**, developers use session IDs to **store information server-side** while **enabling user privacy**

* every time a user takes an action or makes a request on a web application, the application **`sends the session ID and cookie ID back to the server`**, along with _a description of the action itself_
* _once a web developer accrues enough information on how users traverse their site_
* -> they can start to **`create very personalized, engaging experiences`**
* -> this both good for the **`company behind the site`** (_they can convert more visitors to customers_) and the **`visitors`** themselves (_get to the information or products for which they are looking in a fast, hassle-free manner_)

* Web developers often **`cache web session information`** using fast, scalable **in-memory processing** technologies 
* -> to ensure that their web sites deliver a very **`responsive personalized experience`** for **`many visitors`** at **`the same time`**

=============================================================
## Usage
* -> **`carrying information`** as a client travels **`between pages`**
* -> one page data can be stored in session variable and that **`data can be accessed from other pages`**
* -> session can help to **`uniquely identify each client from another`**
* -> session is **`mostly used in ecommerce sites`** where there is shopping cart system
* -> it helps **`maintain user state and data`** all over the application
* -> it is **`easy to implement`** and we can **`store any kind of object`**
* -> stores **`client data separately`**
* -> session is **`secure`** and transparent from the user


