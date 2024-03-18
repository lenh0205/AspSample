# Response Header

## Content-Type: 

### application/octet-stream
* -> commonly used for binary data that does not have a specific file format
* -> to indicating to the client that the data being transferred is in the form of a stream
* -> _nó chung là để chỉ dẫn cho Client biết cần interpret the data as a stream_

## Content-Disposition


## Location
* -> the _Location response header_ indicates **the URL to redirect a page to**
* -> it only provides a meaning when served with **`a 3xx (redirection) or 201 (created) status response`**: **201, 301, 302, 303, 307, 308**

### 3xx - redirection
* _make the new request to fetch the page pointed to by **`Location`** depends on the original method and the kind of redirection:_
* -> **303** - **`See Other`** responses always lead to the use of a GET method.
* -> **307** - **`Temporary Redirect`** and 308 **`Permanent Redirect`** don't change the method used in the original request.
* -> **301** - **`Moved Permanently`** and 302 **`Found`** don't change the method most of the time, though older user-agents may (so you basically don't know)

### 201 - resource creation
* -> _indicates the URL to the newly created resource_

========================================================
# Request Header