https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy

===============================================================
# Same-origin policy
* -> the same-origin policy is **`a critical security mechanism`** that **restricts how a document or script loaded by one origin** can **`interact`** with **a resource from another origin`**
* -> helps isolate potentially malicious documents, **`reducing possible attack vectors`**

```r - For example
// -> it prevents a malicious website on the Internet from running JS in a browser to read data from a third-party webmail service (which the user is signed into) 
// -> or a company intranet (which is protected from direct access by the attacker by not having a public IP address) 
// -> and relaying that data to the attacker
```

===============================================================
# Definition of an "Origin"
* -> **`2 URLs have the same origin`** if the **protocol**, **port** (_if specified_), and **host** are the same for both

```r - Example: http://store.company.com/dir/page.html
// Same origin
// -> "http://store.company.com/dir2/other.html" # only the path differs
// -> "http://store.company.com/dir/inner/another.html" # only the path differs

// Failure
// -> "https://store.company.com/page.html" # different protocol
// -> "http://store.company.com:81/dir/page.html" # different port (http:// is port 80 by default)
// -> "http://news.company.com/dir/page.html" # different host
```

## Inherited origins
* -> **scripts** executed from **`pages`** with _an **about:blank** or **javascript:** URL_ **inherit the origin of the document containing that URL**
* -> since these types of URLs **`do not contain information about an origin server`**

* -> _For example, **about:blank** is often used as **`a URL of new, empty popup windows into which the parent script writes content`** (_Ex: via the **`window.open()`** mechanism_)
* -> _if this **`popup also contains JavaScript`**, that **script would inherit the same origin as the script that created it**_

## File origins
* -> **`Modern browsers`** usually **`treat the origin of files loaded using`** the **file:/// schema** as **opaque origins**
* -> means if **`a file`** includes **`other files from the same folder (say)`**, they are **not assumed to come from the same origin**, and may trigger **CORS errors**

* _nhưng lưu ý `origin of files` vẫn là phụ thuộc vào cách implementation, nên 1 số browser có thể sẽ khác_

===============================================================
# Cross-origin network access
* _the same-origin policy **`controls interactions`** between 2 different origins(_such as using `fetch()` or an `<img> element`_); typically placed into **3 categories**:_

## Cross-origin reads
* ->  are typically **disallowed**
* -> but **read access is often leaked by embedding**
* -> _Ex: we can read the `dimensions of an embedded image`, the `actions of an embedded script`, or the `availability of an embedded resource`_

## Cross-origin writes
* -> are typically **allowed** (_Ex: are **links**, **redirects**, and **form submissions**_)
* -> **`some`** HTTP requests require **preflight**

## **Cross-origin embedding** 
* -> is typically **allowed**
* -> `Cross-origin embedding` example:

* **JavaScript with `<script src="…"></script>`**
* -> Error details for syntax errors are only available for same-origin scripts_

* **CSS applied with `<link rel="stylesheet" href="…">`**
* -> due to the relaxed syntax rules of CSS, cross-origin CSS **requires a correct 'Content-Type' header**
* -> Browsers **block stylesheet loads** if it is a cross-origin load where the **`MIME type is incorrect`** and **`the resource does not start with a valid CSS construct`**

* **Images displayed by `<img>`**

* **Media played by `<video>` and `<audio>`**

* **External resources embedded with `<object>` and `<embed>`**

* **Fonts applied with `@font-face`**
* -> some browsers **`allow cross-origin fonts`**, **`others require same-origin`**

* **anything embedded by `<iframe>`**
* -> sites can use the **X-Frame-Options** header to prevent cross-origin framing

===============================================================
# Allow cross-origin access - CORS
* -> use **CORS** to **`allow cross-origin access`**
* -> _CORS_ is **`a part of HTTP`** that **lets servers specify any other hosts** from which **`a browser should permit loading of content`**

# Block cross-origin access 

## prevent "Cross-origin writes"
* -> **`check an unguessable token`** in the request — known as **a Cross-Site Request Forgery (CSRF) token**
* -> we must prevent **`cross-origin reads`** of pages that require this token

## prevent "Cross-origin reads"
* -> ensure that it is **not embeddable**
* -> it is **`often necessary to prevent embedding`** because **embedding a resource always leaks some information about it**

## prevent "Cross-origin embeds"
* -> ensure that our resource **cannot be interpreted as one of the embeddable formats listed above**
* -> Browsers may not respect the **`'Content-Type' header`**

* _For example, if we point a <script/> tag at an HTML document, the browser will try to parse the HTML as JavaScript_

* -> when **`our resource is not an entry point to our site`**, we can also **use a CSRF token to prevent embedding**

===============================================================
# Cross-origin script API access

## Communicate between documents from different "origins"
* -> use **window.postMessage**

## Reference between documents from different "origins"
* -> **javaScript APIs** like **`iframe.contentWindow`**, **`window.parent`**, **`window.open`**, and **`window.opener`** allow **documents to directly reference each other** 
* -> when **`2 documents do not have the same origin`**, **`these references`** provide **very limited access to "Window" and "Location" objects**

### Window
* -> cross-origin access to **`Window properties`** is allowed: **window.blur**, **window.close**, **window.focus**, **window.postMessage**
* -> attribute: **window.closed**, **window.frames**, **window.length**, **window.location**, **window.opener**, **window.parent**, **window.self**, **window.top**, **window.window**

### Location
* -> cross-origin access to **`Location properties`** is allowed: **location.replace**, **location.href**

===============================================================
# Cross-origin data storage access

* _access to **data stored** in the browser (such as **`Web Storage`** and **`IndexedDB`**)_
* -> are **separated by origin**
* -> **`each origin gets its own separate storage`**, and JavaScript in one origin cannot read from or write to the storage belonging to another origin

* **Cookies** use a **`separate definition of origins`**
* -> a page can **`set a cookie for its own domain or any parent domain`**, as long as the parent domain is not a public suffix
* -> _Firefox and Chrome use the **`Public Suffix List`** to determine if a domain is a public suffix_
* -> when we set a cookie, we can **`limit its availability`** using the **Domain**, **Path**, **Secure**, and **HttpOnly** flags
* -> when we read a cookie, we **cannot see from where it was set**; 
* -> even if we **`use only secure 'https' connections`**, **`any cookie we see`** may have been set **using an insecure connection**