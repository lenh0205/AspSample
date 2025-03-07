================================================================================
# Message Broker vs. Message Queue
* -> both facilitate communication between different applications
* -> while **`message queues`** focus on **reliable message delivery** and **task management**
* -> **`message brokers`** provide **additional features such as routing, protocol translation, and load balancing**
* => for complex systems requiring flexibility and scalability, a message broker is ideal
* => for simpler scenarios where task queuing and processing order are crucial, a message queue is more suitable

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
