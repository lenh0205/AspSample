
# Priority Queues
RabbitMQ allows priority levels to be set for messages.

Higher priority messages are processed before lower-priority messages.
```cs
var args = new Dictionary<string, object> { { "x-max-priority", 10 } };
channel.QueueDeclare("priority_queue", durable: true, exclusive: false, autoDelete: false, arguments: args);

var properties = channel.CreateBasicProperties();
properties.Priority = 5; // Higher priority (0-9)

channel.BasicPublish("", "priority_queue", properties, body);

```