
## Event-Driven Microservices
Use RabbitMQ as an event bus for decoupled microservices.

Services communicate asynchronously via events.

## RPC with RabbitMQ (Synchronous Communication)
Simulate request-response using RabbitMQ for inter-service communication.

Producer (Request Sender)

csharp
Copy
Edit
var replyQueue = channel.QueueDeclare().QueueName;
var correlationId = Guid.NewGuid().ToString();

var props = channel.CreateBasicProperties();
props.CorrelationId = correlationId;
props.ReplyTo = replyQueue;

channel.BasicPublish("", "rpc_queue", props, body);
Consumer (Response Handler)

csharp
Copy
Edit
channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);