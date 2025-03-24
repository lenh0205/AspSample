
===================================================================
> ta sẽ cần tạo 1 **Exchange** để **`manage multiple queues at the same time`**
> a **`Fanout`** or **`Topic`** exchange would allow multiple servers to subscribe to the same messages but consume them at different times

# Exchange in RabbitMQ
* -> **`routes messages`** from **a producer** to **a single or multiple consumers**
* -> **an Exchange** uses **`header attributes`**, **`routing keys`** and **`binding`** to route messages

## Type of Exchanges

### Default
* the **`Default (nameless) Exchange (AMQP default)`** compares the **routing key** with the **queue name** (_not "binding key"_), 
* -> if matched then **forward message to the queue** 
* (_therefore makes it seemingly possible to send a message directly to a queue but under the hood each message goes through an Exchange_)

### Direct
* _the Exchange can route directly to a specific queue (exact routing key)_
* -> the Exchange uses **`exact match routing key`** in the header to **identify which queue** the message should be send to
* -> **`Routing key`** is **a header value set by the Producer**
* -> **Consumer uses the routing key** to **`bind to the queue`**

### Topic
* _to multiple queues with a shared pattern (có phần giống nhau là đc)_
* -> also uses **`routing key` but not in exact match** instead it does a **`pattern match`**

### Header
* _using message header instead of routing key_
* -> routes messages based on **header values**

### Fanout
* _to every queues it know about (ignore routing key)_
* -> fans out all the messages to all the queues bound to it

===================================================================
# Example: 

## 'Direct' Exchange

```cs - create a Direct Exchange Publisher
static class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672");
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        DirectExchangePublisher.Publish(channel);
    }
}

public static class DirectExchangePublisher 
{
    public static void Publish(IModel channel)
    {
        // declare exchange with: exchange name, type of exchange
        channel.ExchangeDeclare("demo-direct-exchange", ExchangeType.Direct);

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

* Đầu tiên, ta sẽ chạy "Consumer"
* -> trong Management Console, ta sẽ thấy "demo-direct-queue" được tạo và Consumer connected to it trong **Queues** section; 
* -> và "demo-direct-exchange" được tạo và bind to "demo-direct-queue" trong **Exchanges** section
```cs - Consumer
static class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672");
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        DirectExchangeConsumer.Consume(channel);
    }
}

public static class DirectExchangeConsumer
{
    public static void Consume(IModel channel)
    {
        // declare Exchange
        channel.ExchangeDeclare("demo-direct-exchange", ExchangeType.Direct);

        // declare Queue
        channel.QueueDeclare("demo-direct-queue", durable: true, 
            exclusive: false, autoDelete: false, arguments: null);

        // make a mapping between the Queue and the Exchange - told the Exchange to bind to specific Queue
        channel.QueueBind("demo-direct-queue", "demo-direct-exchange", "account.init")

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) => {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };
        channel.BasicConsume("demo-direct-queue", true, consumer);

        Console.WriteLine("Consumer started");
        Console.ReadLine(); // để chương trình không bị thoát sau khi chạy xong
    }
}
```
