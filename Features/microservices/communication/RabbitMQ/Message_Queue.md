> Load balancing ? cơ chế của load balancing là gì, nó có distribute the load over time
> Load balancing vs Rate Limiting ?
> bottleneck vs system resources overwhelmed ?
> Message model: Pub/Sub, point-to-point
> Event-Driven design vs Pub/Sub
> Event-Drivent design vs Request/Response

================================================================================
# Message Broker vs. Message Queue
* -> both facilitate communication between different applications
* -> while **`message queues`** focus on **reliable message delivery** (asynchronous communication) and **task management** (task queuing) (_Ex: ActiveMQ, RabbitMQ, Amazon SQS_)
* -> **`message brokers`** provide **additional features such as routing, protocol translation, and load balancing** (_Ex: Kafka, RabbitMQ, Amazon SNS_)
* => for complex systems requiring flexibility and scalability, a message broker is ideal
* => for simpler scenarios where task queuing and processing order are crucial, a message queue is more suitable

================================================================================
# Use cases

## Message Queue
* -> **Task Scheduling** - schedule and manage **`background tasks`**, ensuring they are **`processed in a specific order`** (_sequentially or based on priority_) and **`without overloading the system`** (_distribute the load over time_)
* -> **Load Balancing** - by distributing tasks across multiple worker instances, message queues help **`balance the load`** and **`prevent any single service from becoming a bottleneck`**
* -> **Rate Limiting** - control **`the rate at which messages are processed`**, ensuring system resources are not overwhelmed (_Example: An API gateway that queues incoming requests to limit the rate at which backend services are accessed_)
* -> **Email and Notification Services** - manage the delivery of emails and notifications, queuing messages for sending to **`ensure timely and reliable delivery`**
* -> **Data Buffering** - in data processing pipelines, message queues **`act as buffers`**, allowing data to be **`processed asynchronously`** and preventing bottlenecks when the **`processing system is slower than the data ingestion rate`**
* -> **Retry Mechanisms** - provide fault tolerance by ensuring that **`messages are not lost even if the receiving application is temporarily unavailable`**; implement retry logic for failed messages, **`ensuring that tasks are retried until they are successfully processed`**, which improves the reliability of the system.
* -> **Asynchronous Workflows** - enable asynchronous processing, allowing systems to **`continue operating without waiting for tasks to complete`** (_w  hen tasks do not need to be processed immediately_), which enhances responsiveness and performance.

## Message Broker
* -> **Microservices Communication** - in a microservices architecture, message brokers facilitate communication between different services by **`routing messages`** (base on complex rules or patterns), **`handling protocol translation`** (Ex: devices use MQTT, but the backend services use HTTP/REST), and ensuring messages reach their intended destinations
* -> **Real-Time Data Processing** - are used in **`real-time analytics and monitoring systems`** to stream data from various sources to processing engines, ensuring timely and efficient data flow 
* -> **Event-Driven Architectures** - support event-driven systems by **`distributing events to multiple consumers`**, enabling reactive and responsive applications
* -> **Decoupling Systems** - when we need to decouple services to **`reduce dependencies and improve maintainability`**, a message broker can act as **`an intermediary`**
* -> **IoT Applications** - in Internet of Things (IoT) applications, message brokers manage the **`communication between numerous devices and back-end systems`**, handling the high volume of messages and ensuring reliable delivery
* -> **Enterprise Integration** - facilitate the integration of disparate enterprise systems, **`allowing legacy systems, cloud services, and on-premises applications to communicate seamlessly`**

================================================================================
# Message Queue
* -> A message queue is a data structure, or a container - a way to hold messages for eventual consumption.

* -> Point-to-Point
* -> Pub/Sub
* -> Request/Reply
* => **`Decoupling`** - break apps apart with asynchronous communication; dedicate one particular piece of code, application to just be responsible for that one piece of work

================================================================================
# Message Broker (so called Service Bus)
 * -> A message broker is a separate component that manages queues
