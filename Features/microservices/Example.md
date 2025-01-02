
# Design
* microservice 1 của ta sẽ có 
* -> 1 **`REST API Controller`** (synchronous - in) cho external contract, nhận HTTP Request và trả về HTTP Response, kết nối với lớp **Repository**
* => đây sẽ là external interface duy nhất của ta còn lại bên dưới chỉ dùng cho microservices domain
* -> 1 **`HTTP Client`** (synchronous - out) để make HTTP request tới microservice khác và nhận về HTTP Response
* -> 1 **`Message Publisher`** (asynchronous - out) để publish event onto out **`Message Bus`**
* -> 1 **`gRPC Service`** (synchronous - in) để cho những client khác có thể sử dụng gRPC để request tới microservice của ta, kết nối với lớp **Repository**

* microservice 2 của ta sẽ có
* -> 1 **`REST API Controller`** (synchronous - in) (the only external API)
* -> 1 **`Message Subscriber`** (asynchronous - in) để receive messages from the **Message Bus**
* -> 1 **`gRPC Client`** (synchronous - out) để make requests to **gRPC Service** của microservice khác 

```cs - PlatformsController
// as REST best practice, whenever we create a resource we should return a HTTP 201 along with the resource we created and also a URI to the resource location
```