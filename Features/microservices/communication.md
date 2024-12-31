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
> https://viblo.asia/p/grpc-la-gi-tai-sao-nen-dung-grpc-protocol-buffers-RnB5pOmblPG

# gRPC
* -> is **an open-source remote procedure call framework** created by Google, is **`a rewrite of their internal RPC infrastructure`** that they used for years 
* -> gRPC uses **`HTTP/2`** under the covers as its **underlying transport protocol**, **`but HTTP is not exposed to the API designer`**
* -> **gRPC-generated stubs and skeletons** **`hide HTTP` from the client and server too**, so nobody has to worry how the RPC concepts are mapped to HTTP - they just have to learn gRPC

## 3 steps for client to use gRPC
* -> decide which procedure to call
* -> calculate the parameter values to use (if any)
* -> use a code-generated stub to make the call, passing the parameter values

## RPC vs HTTP
* -> **`RPC (Remote Procedure Call)`** - the **addressable entities** are **`procedures`**, and **the data is hidden behind the procedures**

* ->  **HTTP** works the inverse way, the addressable entities are **`data entities`** (called **`resources`** in the HTTP specifications)
* -> and the behaviors are hidden behind the data - the behavior of the system results from creating, modifying, and deleting resources 

## 'HTTP/2' vs 'HTTP 1.1'
* -> 

## Protocol Buffer
* ->

## Hạn chế
* -> các browser hiện tại chưa hỗ trợ gọi trực tiếp tới HTTP/2 frame, nên không thể sử dụng trực tiếp gRPC trên browser
* -> dữ liệu trong gRPC được nén lại dạng binary không thân thiện cho developer; để debug, phân tích payload hay tạo request thủ công, developer cần phải sử dụng thêm các công cụ hỗ trợ

## Features
* -> **`.proto`** - gRPC có thể tự động generate code sang nhiều ngôn ngữ lập trình khác nhau chỉ cần dựa vào file .proto
* -> **`small payload, high performance`**
* -> **`call cancellation`** - gRPC client có khả năng hủy việc gọi gRPC khi nó không cần nhận phản hồi từ server

## Architect
* _REST chỉ tập trung vào kiến trúc request-response,  gRPC hỗ trợ truyền dữ liệu theo kiến trúc **`event-driven architecture`**_
* -> **`Server Streaming RPC`** - client gửi một request tới server, sau đó server gửi lại nhiều response trên cùng một TCP connection
* -> **`Client Streaming RPC`** - client gửi nhiều request tới server, sau đó server gửi lại chỉ một response về client trên cùng một TCP connection
* -> **`Bidirectional Streaming RPC`** - client gửi nhiều request tới server, sau đó server gửi lại nhiều response trên cùng một TCP connection mà không cần chờ thời gian phản hồi
* -> Ngoài ra, gRCP còn hỗ trợ kiểu Unary. Với Unary, client gửi một request, sau đó server gửi trả một response. Unary giống với kiểu giao tiếp truyền thống

======================================================================
> https://cloud.google.com/blog/products/api-management/understanding-grpc-openapi-and-rest-and-when-to-use-them

# API Design
* -> As most software developers no doubt know, there are **two primary models for API design**: **`RPC`** and **`REST`**
* -> regardless of model, most modern APIs are implemented by **`mapping them in one way or another to the same HTTP protocol`**

## Use HTTP for API Design
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

## REST
* -> the **`least-commonly used API model`** is REST - only a small minority of APIs are designed this way, even though **`the word REST is used (or abused) more broadly`**
* -> **a signature characteristic** of this style of API is that **`clients do not construct URLs from other information`** - they just use the URLs that are passed out by the server as-is

* _this is how the browser works - it does not construct the URLs it uses from piece parts, and it does not understand the website-specific formats of the URLs it uses;_
* -> _it just **blindly follows the URLs that it finds in the current page received from the server**, or that were **bookmarked from previous pages** or are **entered by the user**_

* -> the only parsing of a URL that a browser does is to extract the information required to send an HTTP request, and the only construction of URLs that a browser does is to form an absolute URL from relative and base URLs 
* -> if our API is a REST API, then your clients never have to understand the format of our URLs and those formats are not part of the API specification given to clients

## OpenAPI
* -> probably the **`most popular way of designing RPC APIs that use HTTP`** is to **use specification languages like OpenAPI** (formerly known as the **Swagger specification**)
* -> **a signature characteristic** of the OpenAPI style of API is that **`clients use the API by constructing URLs from other information`**

* _the way a client uses an OpenAPI API is by following these three steps:_
* -> decide which OpenAPI URL path template to use
* -> calculate the parameter values to use (if any)
* -> plug the parameter values into the URL path template and send an HTTP request