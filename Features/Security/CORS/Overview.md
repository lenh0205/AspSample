https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy


===============================================================================
# Cross-Origin Resource Sharing (CORS)

## Definition
* -> _Cross-Origin Resource Sharing (CORS)_ is **an HTTP-header based mechanism**
* -> **allows a server to indicate any origins** (**`domain`**, **`scheme`**, or **`port`**) **`other than its own`** from which **a browser should permit loading resources**

## "Preflight" request mechanism
* -> CORS **also relies on a mechanism** by which **`browsers make a "preflight" request`** to **the server hosting the cross-origin resource**
* -> in order to check that the server will permit the actual request. In that preflight, the browser sends headers that indicate the HTTP method and headers that will be used in the actual request

===============================================================================
# In web dev
* -> for **`security reasons`**, **`browsers restrict cross-origin HTTP requests`** **initiated from scripts** (_for example, **fetch()** and **XMLHttpRequest** follow **`the same-origin policy`**_)
* -> this means that **`a web application using those APIs`** can **only request resources from the same origin the application was loaded** from **`unless the response from other origins`** includes **the right CORS headers**

## What requests use CORS ?