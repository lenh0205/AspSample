
# Delayed Messaging (Scheduled Messages)
RabbitMQ does not support scheduling natively, but Delayed Message Exchange Plugin allows messages to be delayed.
```cs
var args = new Dictionary<string, object>
{
    { "x-delayed-type", "direct" }
};
channel.ExchangeDeclare("delayed_exchange", "x-delayed-message", durable: true, arguments: args);

var props = channel.CreateBasicProperties();
props.Headers = new Dictionary<string, object> { { "x-delay", 60000 } }; // Delay of 60 seconds

channel.BasicPublish("delayed_exchange", "task", props, body);
```