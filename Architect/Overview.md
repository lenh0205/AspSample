
* Fundamentally, API can be broke down into either Request-Response APIs and Event-Driven APIs

# Request-Response APIs
* there're 3 commonly used standards:
* -> REST - Representational State Transfer
* -> RPC - Remote Procedure Call
* -> GraphQL

* REST are all about **resources**
* resources as nouns (https://foobar.com/api/v1/user) -> use verbs https://foobar.com/api/v1/getUsers is incorrect
* each resource typicall have 2 URLs: for collection (VD: users) and for entity in collection (specifying the identifier)
* consumer allowed to use these resources using CRUD operations
* REST Api typically return JSON, XML data