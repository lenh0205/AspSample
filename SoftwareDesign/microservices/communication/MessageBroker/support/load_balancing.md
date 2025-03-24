
# Prefetching & Fair Dispatch
By default, RabbitMQ sends messages in a round-robin fashion. To ensure that a single consumer does not get overwhelmed:

Use Basic QoS (Quality of Service) to limit the number of unacknowledged messages a consumer can handle at a time.

```cs
channel.BasicQos(0, 1, false); // Prefetch only 1 message per consumer
```

## Message Prefetching (Avoid Overloading Consumers)
RabbitMQ distributes messages evenly by default, but it might overload fast consumers.

Use QoS (Quality of Service) to limit the number of unprocessed messages.

```cs
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
```

##  Multiple Consumers for One Queue
Scale horizontally by adding multiple consumers to process messages faster.

RabbitMQ will distribute messages among consumers

======================================================================
> lifetime of a message 

# Prefetch count 
* -> if we have **multiple consumers connected to a queue**, then "prefetch" tells **`how many messages that particular consumer can prefetch and process`**
* -> this is really important feature when building **a realtime RabbitMQ application with multiple Queues and Exchanges**
* _Ex: if the prefetch count is 2 and there're like 10 messages came to the queue immediately, 2 messages will be deliverd per consumer_

## Setup
```cs - Producer
// to do that we declare an argument attribute in the Exchange
// declare a dictionary of string and object, in that we pass the argument attribute that needed by RabbitMQ to identify the time to live

public static class DirectExchangePublisher 
{
    public static void Publish(IModel channel)
    {
        var ttl = new Dictionary<string, object> // this line
        {
            { "x-message-ttl", 30000 }
        }
        channel.ExchangeDeclare("demo-direct-exchange", ExchangeType.Direct, arguments: ttl);

        var count = 0;
        while (true)
        {
            var message = new { Name = "Producer", Message = $"Hello! Count: {count}" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish("demo-direct-exchange", "account.init", null, body);
            // value của "routing key" là tuỳ ý

            count++;
            Thread.Sleep(1000);
        }
    }
}
```

```cs - Consumer
public static class DirectExchangeConsumer
{
    public static void Consume(IModel channel)
    {
        channel.ExchangeDeclare("demo-direct-exchange", ExchangeType.Direct);
        channel.QueueDeclare("demo-direct-queue", durable: true, 
            exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind("demo-direct-queue", "demo-direct-exchange", "account.init");

        // make the Consumer to fetch 10 messages at a time
        channel.BasicQos(0, 10, false); // this line

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) => {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };
        channel.BasicConsume("demo-direct-queue", true, consumer);

        Console.WriteLine("Consumer started");
        Console.ReadLine(); 
    }
}
```

## Running
* -> trước tiên ta sẽ vào Management Console để delete the Queue and Exchange
* -> h ta sẽ start Producer và Consumer, thì trong Management Console ta sẽ thấy "demo-direct-exchange" have "x-message-ttl" attribute and the "demo-direct-queue" has a prefetch count of 10
