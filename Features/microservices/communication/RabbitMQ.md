> when it come to **decoupled communication** between microservices (using **Message Queue**) - **`Message Broker`** pattern is one of the most useful pattern 
> RabbitMQ is an open source **`Message Broker`**

======================================================================
# Message Broker
* -> its main responsibility is to broker messages between **publisher** and (a set of) **subscribers**
* -> once a message is received by a message broker from a producer, its routes the message to a subscriber

* -> **`Producer`** (or Publisher) - an application responsible for **sending message**
* -> **`Consumer`** (or Subscriber) - an application listening for the messages
* -> **`Queue`** - **storage** where messages are stored by the broker

======================================================================
# RabbitMQ 
* -> one of the most widely used Message Broker - lightweight and very easy to deploy, support mulitple protocols, highly available and scalable, support multiple OS

## Protocols Supported
* -> the **main protocols supported directly by the RabbitMQ** is **`AMQP 0-9-1`** - a binary messaging protocol specification
* -> other protocols will be supported through plugins like **STOMP**, **MQTT**, **AMQP 1.0**, **HTTP and WebSocket**