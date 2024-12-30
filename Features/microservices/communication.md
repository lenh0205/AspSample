https://topdev.vn/blog/message-queue-vs-message-bus/
https://stackoverflow.com/questions/7793927/message-queue-vs-message-bus-what-are-the-differences
https://stackoverflow.com/questions/3144788/difference-between-message-bus-and-message-broker
https://medium.com/@ahmedfaruk888/service-bus-and-message-broker-the-difference-2cdc2162e1d6
https://www.svix.com/resources/faq/event-bus-vs-message-queue/
https://www.linkedin.com/pulse/event-bus-vs-message-queue-broker-sanjeev-rai-m2tjc
https://pandaquests.medium.com/difference-between-event-bus-message-queue-and-message-broker-a8630a8823f7

# RabbitMQ Message Bus
* -> Event Driven Design
* -> an microservice will publish an event onto Message Bus, and there will be another microservice subscribe to the Message Bus and receive those published events

======================================================================
# gRPC
* -> gRPC is synchronous 
* -> is **an open-source remote procedure call framework** created by Google, is **`a rewrite of their internal RPC infrastructure`** that they used for years 
* -> gRPC uses **`HTTP/2`** under the covers as its **underlying transport protocol**, **`but HTTP is not exposed to the API designer`**
* -> **gRPC-generated stubs and skeletons** **`hide HTTP` from the client and server too**, so nobody has to worry how the RPC concepts are mapped to HTTP—they just have to learn gRPC

## 3 steps for client to use gRPC
* -> decide which procedure to call
* -> calculate the parameter values to use (if any)
* -> use a code-generated stub to make the call, passing the parameter values

======================================================================
> https://cloud.google.com/blog/products/api-management/understanding-grpc-openapi-and-rest-and-when-to-use-them

# Use HTTP for API Design
* -> there are 3 significant and distinct approaches for building APIs that use HTTP: **`REST`**, **`gRPC`** (and Apache Thrift and others), **`OpenAPI`** (and its competitors)

## OpenAPI and gRPC
* -> that the **client model for using an OpenAPI API** is different detail but **`very similar in overall`** to the **client model for using a gRPC API**
```cs
+---------------------------------------------------+--------------------------------------------------+
|             gRPC                                  |                   OpenAPI                        |
|---------------------------------------------------|--------------------------------------------------|
| where a gRPC client chooses a procedure to call   | an OpenAPI client chooses a URL path template to | 
|                                                   | use                                              |
|---------------------------------------------------|--------------------------------------------------|
| calculate parameter values                        | calculate parameter values                       |
|---------------------------------------------------|--------------------------------------------------|
| a gRPC client uses a stub procedure to combine    | an OpenAPI client inserts the parameter values   |
| the parameters with the procedure signature and   | into the URL path template and issues an HTTP    |
| make the call                                     | request                                          |
|---------------------------------------------------|--------------------------------------------------|
|                                                   | OpenAPI also includes tools that will optionally |
|                                                   | generate a client stub procedure in the client   |
|                                                   | programming language that hides these details,   |
|                                                   | making the client experience of the two even     |
|                                                   | more similar                                     |
+---------------------------------------------------+--------------------------------------------------+
```

## OpenAPI
* -> probably the **`most popular way of designing RPC APIs that use HTTP`** is to **use specification languages like OpenAPI** (formerly known as the **Swagger specification**)
* -> a signature characteristic of the OpenAPI style of API is that **`clients use the API by constructing URLs from other information`**

The way a client uses an OpenAPI API is by following these three steps:

Decide which OpenAPI URL path template to use

Calculate the parameter values to use (if any)

Plug the parameter values into the URL path template and send an HTTP request