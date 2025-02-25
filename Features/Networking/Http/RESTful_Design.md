> **REST APIs** are one of the most common kinds of **`web interfaces`** available today
> have to take into **`account security`**, **`performance`**, and **`ease of use`** for API consumers
> https://blog.postman.com/rest-api-examples/
> https://www.integrate.io/blog/how-to-make-a-rest-api/

===========================================================================
# REST API
* -> **`REST (representational state transfer)`** is a software architectural style commonly used for building web services such as APIs
* -> an **`API (application programming interface)`** is a collection of functions and protocols that enables two software applications or systems to communicate with each other

===========================================================================
# REST API standards 
* _to **`make an API service RESTful`**, six guiding constraints must be satisfied:_

## Uniform interface
* -> All clients should be able to interact with the REST API in the same manner, whether the client is a browser, a mobile app, or something else. The REST API is usually accessible at a single URL (uniform resource locator) — for example, “https://api.example.com.”

## Client-server architecture
* -> In REST APIs, the client and server are two separate entities. Concerns about the API interface are separate from concerns about how the underlying data is stored and retrieved

## Statelessness
* -> REST requests must be stateless; the server does not have to remember any details about the client’s state. This means that the client must include all necessary information within each API request it makes

## Cacheability
* -> REST servers can cache data by designating it as cacheable with the Cache-Control HTTP header. The cached result is ready for reuse when there is an equivalent request later on

## Layered system
* -> The REST client does not know (and does not need to know) if it is communicating with an intermediary layer in the architecture, or with the server itself

## Code on demand
* -> The client can optionally download code such as a JavaScript script or Java applet in order to extend its functionality at runtime

===========================================================================
# Best practices for REST API design
* https://stackoverflow.blog/2020/03/02/best-practices-for-rest-api-design/
