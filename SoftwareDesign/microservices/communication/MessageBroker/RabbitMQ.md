> when it come to **decoupled communication** between microservices (using **Message Queue**) - **`Message Broker`** pattern is one of the most useful pattern 
> RabbitMQ is an open source distributed **`Message Broker`**

======================================================================
# Why RabbitMQ

```cs
// Ví dụ user gửi request cho 1 WebAPI, WebAPI này phải gửi 5 HTTP request đến 5 services khác để hoàn thành business logic
// thì nó sẽ cần chờ tất cả 5 Service khác response thì WebAPI này mới có thể response cho user
// và trường hợp này sẽ tệ hơn nếu 1 Service nó gọi tới bị chết, nó sẽ phải request over and over again và chờ cho Service này ok thì mới phản hồi cho user được
// hoặc trường hợp WebAPI gửi một lượng request đến nỗi Service kia không chịu nổi, nó sẽ làm sập Service
```

## Asynchronous
* -> với những hành động không cần thực hiện và phản hổi kết quả ngay thời điểm đó (TCP request-response) thì ta cứ cho nó vô queue cho các Consumer tự lấy, 1 service không cần phải chờ thực hiện xong tất cả hành động để mà cứ phản hổi đã làm thành công về cho user luôn cho nhanh
* -> Ví dụ: chương trình sale một sản phẩm bắt đầu, số lượng request trên giây tăng đột biến lên 5000; nhưng MySQL chỉ có thể xử lý 2000 request per second Database thì nó sẽ sập ngay); Lúc này ta cứ ném hết 5000 request cho RabbitMQ nó sẽ chỉ gửi 2000 request per second; còn 3000 request còn lại cứ từ từ xử lý sau

## Decoupling
* -> thay vì 1 server phải quản lý việc giao tiếp với nhiều server khác, thì nó chỉ cần giao tiếp với RabbitMQ rồi RabbitMQ sẽ quản lý việc giao tiếp với những thằng còn lại

## Scalibility
* -> nếu có hàng tá tin nhắn mà 1 Consumer không thể xử lý hết được làm queue bị đầy, ta có thể nhân đôi Consume lên lấy message

## Một số vấn đề trong RabbitMQ 
* -> nhưng RabbitMQ không đảm bảo 100% là không bị miss message;
* -> một vấn đề nữa là tuy RabbitMQ đã gửi message thành công và báo về những việc xử lý Data Persitance có thành công không thì chưa chắc

# RabbitMQ, Kafka, ActiveMQ, RocketMQ
* -> thông lượng của Kafka cao hơn RabbitMQ
* -> Kafka ít mất dữ liệu hơn
* -> độ tin cậy của tin nhắn thì RabbitMQ là toàn vẹn; còn Kafka cao nhưng vẫn có thể bị mất
* -> Lantency của RabbitMQ rất thấp dưới mức milisecond; còn Kafka độ trễ trong vòng milisecond 

======================================================================
# Message Broker
* -> its main responsibility is to broker messages between **publisher** and (a set of) **subscribers**
* -> once a message is received by a message broker from a producer, its routes the message to a subscriber

* -> **`Producer`** (or Publisher) - an application responsible for **sending message**
* -> **`Consumer`** (or Subscriber) - an application **listening for the messages**
* -> **`Queue`** - **storage** where messages are stored by the broker

======================================================================
# RabbitMQ 
* -> one of the most widely used **`Message Broker`** - lightweight and very easy to deploy, support mulitple protocols, highly available and scalable, support multiple OS
* (_written in the **Erlang** programming language, which itself is famous for powering the **Open Telecom** platform_)

* => allows microservices to communicate asynchronously with variety of different protocols
* => the end result is an architecture that allows servers to both **Publish** and **Subscribe** to data thanks to the RabbitMQ middleman

## Protocols Supported
* -> the **main protocols supported directly by the RabbitMQ** is **`AMQP 0-9-1`** (_Advanced messaging queue protocol 091_) - a binary messaging protocol specification
* -> other protocols will be supported through plugins like **STOMP**, **MQTT**, **AMQP 1.0**, **HTTP and WebSocket**

## Process
* -> **Producer** publish a message with the required data (_Ex: **`routing key`** in "direct" or "topic" exchange_) into **an Exchange**
* -> the **`Exchange`** is then responsible for **`routing`** (_Ex: compare **routing key** and **binding key**) it to one or more **`Queues`**
* -> **`Binding`** connects an **Exchange** with a **Queue** using **`binding key`** (_defining the routing rules_)
* -> now the **`message sits in the queue`** until it's **handled by the Consumer** 


