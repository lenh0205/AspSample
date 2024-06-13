https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy

>
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
* -> this means that **`a web application using those APIs`** can **only request resources from the same origin the application was loaded from**
* -> **`unless the response from other origins`** includes **the right CORS headers**

## What requests use CORS ?
* _this `cross-origin sharing standard` can enable cross-origin HTTP requests for:_
* -> Invocations of fetch() or XMLHttpRequest, as discussed above.
* -> Web Fonts (for cross-domain font usage in @font-face within CSS), so that servers can deploy TrueType fonts that can only be loaded cross-origin and used by websites that are permitted to do so.
* -> WebGL textures.
* -> Images/video frames drawn to a canvas using drawImage().
* -> CSS Shapes from images

===============================================================================
# Functional overview
* -> the **`Cross-Origin Resource Sharing standard works`** by **adding new HTTP headers** that let **`servers describe which origins`** are permitted to read that information from a web browser
* -> additionally, for **`HTTP request methods`** that can **cause side-effects on server data** (_in particular, HTTP methods `other than GET, or POST` with `certain MIME types`_)
* -> the specification **mandates that browsers "preflight" the request**, **`soliciting supported methods from the server`** with the **HTTP OPTIONS request method**
* -> and then, **upon "approval" from the server**, **`sending the actual request`**
* -> Servers can also **inform clients whether "credentials"** (_such as `Cookies` and `HTTP Authentication`_) **`should be sent with requests`**

* -> **CORS failures** result in **`errors but for security reasons`**, specifics about the **`error are not available to JavaScript`**
* -> **all the code knows is that an error occurred**; the only way to determine what specifically went wrong is to look at the **`browser's console for details`**

===============================================================================
# Examples of access control scenarios
* _All these examples use `fetch()`, which can make cross-origin requests in `any supporting browser`_

## Simple requests
* -> some **requests don't trigger a CORS preflight** - those are called **`simple requests`** (_from the `obsolete CORS spec`, though the `Fetch spec` (which now defines CORS) doesn't use that term_)
* -> the motivation is that the **<form> element from HTML 4.0** (_which `predates` cross-site fetch() and XMLHttpRequest_) can **submit simple requests to any origin**
* => so **anyone writing a server must already be protecting against cross-site request forgery (CSRF)**

* -> _under this assumption_, the **`server doesn't have to opt-in`** (by responding to a preflight request) to receive any **request that looks like a form submission**
* -> since the **threat of CSRF is no worse than that of form submission**
* -> however, the server still **must opt-in using 'Access-Control-Allow-Origin'** to **`share the response with the script`**

### what is a "simple request"

#### Methods
* -> one of the allowed **`methods`**: **GET**, **HEAD**, **POST**

#### Headers
* -> apart from **the headers automatically set by the user agent** (_Example: **`Connection`**, **`User-Agent`**, or the other headers defined in the Fetch spec as **`a forbidden header name`**_); 
* -> the only headers which are allowed to be manually set are those which the **`Fetch spec defines`** as **a CORS-safelisted request-header**, which are:
* -> **`Accept`**, **`Accept-Language`**, **`Content-Language`**, **`Content-Type`** (_with some requirements_), **`Range`** (_only with a simple range header value. Ex: bytes=256- or bytes=127-255_)

#### Content type
* -> the **only type/subtype combinations allowed** for the **`media type`** specified in the **`Content-Type`** header are:
* -> **application/x-www-form-urlencoded**, **multipart/form-data**, **text/plain**
 
#### What made the request
* -> if the request is made using an XMLHttpRequest object, **no event listeners are registered on** the **`object returned by the 'XMLHttpRequest.upload' property`** used in the request
* -> _if given `an XMLHttpRequest instance xhr`, no code has called `xhr.upload.addEventListener()` to add an event listener to monitor the upload_

#### Others
* -> no **ReadableStream** object is used in the request

## Preflighted requests
* -> 

## Requests with credentials

===============================================================================
# The HTTP response headers

## Access-Control-Allow-Origin

## Access-Control-Expose-Headers

## Access-Control-Max-Age

## Access-Control-Allow-Credentials

## Access-Control-Allow-Methods

## Access-Control-Allow-Headers

===============================================================================
# The HTTP request headers

## Origin

## Access-Control-Request-Method

## Access-Control-Request-Headers

## 

