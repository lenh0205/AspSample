
# Channel in RabbitMQ
* -> a **`channel`** is **a logical communication path** inside **a single TCP connection** between our application and RabbitMQ

* => a TCP connection is established between our application and RabbitMQ; a channel is created within that TCP connection
* => all interactions (publishing, consuming, acknowledging messages) happen over channels

## Why use 'channels' instead of multiple TCP connections
* -> **Performance** - opening a new TCP connection for each message is expensive; instead, multiple channels can be created over a single connection, reducing overhead.
* -> **Concurrency** - each thread (or worker) can have its own channel while sharing the same TCP connection.
* -> **Isolation** - each channel can work independently without affecting others

## Channels Exist in the RabbitMQ Process   
* Producer Side:
* -> When a producer wants to send a message to an exchange, it first creates a channel over an existing TCP connection.
* -> The message is then published to the exchange through that channel.

* Consumer Side:
* -> A consumer also creates a channel to listen for messages from the queue.
* -> When a message is delivered to the consumer, it is received via the channel.
* -> The consumer sends an acknowledgment (ACK) for the message over the same channel