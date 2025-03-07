> when it come to **decoupled communication** between microservices (using **Message Queue**) - **`Message Broker`** pattern is one of the most useful pattern 
> RabbitMQ is an open source distributed **`Message Broker`**

======================================================================
# Message Broker
* -> its main responsibility is to broker messages between **publisher** and (a set of) **subscribers**
* -> once a message is received by a message broker from a producer, its routes the message to a subscriber

* -> **`Producer`** (or Publisher) - an application responsible for **sending message**
* -> **`Consumer`** (or Subscriber) - an application **listening for the messages**
* -> **`Queue`** - **storage** where messages are stored by the broker

======================================================================
# RabbitMQ 
* -> one of the most widely used Message Broker - lightweight and very easy to deploy, support mulitple protocols, highly available and scalable, support multiple OS
* _written in the **Erlang** programming language, which itself is famous for powering the **Open Telecom** platform_
* => allows microservices to communicate asynchronously with variety of different protocols
* => the end result is an architecture that allows servers to both **Publish** and **Subscribe** to data thanks to the RabbitMQ middleman

## Protocols Supported
* -> the **main protocols supported directly by the RabbitMQ** is **`AMQP 0-9-1`** (_Advanced messaging queue protocol 091_) - a binary messaging protocol specification
* -> other protocols will be supported through plugins like **STOMP**, **MQTT**, **AMQP 1.0**, **HTTP and WebSocket**

## Process
* -> produces a message with the required data and **`publishes`** it to **an Exchange**
* -> the Exchange is then responsible for **`routing`** it to one or more **`Queues`**, which are linked to the Exchange with **`a binding and routing key`**
* -> the Exchange can route directly to a specific queue (**`Direct`**) or to multiple queues with a shared pattern (**`Topic`**) or to every queues it know about (**`Fanout`**) 
* -> now the message sits in the queue until it's handled by the **`Consumer`** 

===================================================================
> ta sẽ cần tạo 1 **Exchange** để **`manage multiple queues at the same time`**
> a **`Fanout`** or **`Topic`** exchange would allow multiple servers to subscribe to the same messages but consume them at different times

# Exchange in RabbitMQ
* -> **`routes messages`** from a producer to a single or multiple consumers
* -> **an Exchange** uses **`header attributes`**, **`routing keys`** and **`binding`** to route messages

* -> it's very important to notice that in **`RabbitMQ messages are never published to a queue (directly)`**, they **always goes through an Exchange**
* -> the **Exchange is going to route the message to the queue** 
* -> event when we send message to a queue, it uses **`default exchange (AMQP default)`**

## Type of Exchanges
* **`Direct Exchange`** - the Exchange uses **`exact match routing key`** in the header to **identify which queue** the message should be send to
* -> **`Routing key`** is **a header value set by the Producer**
* -> **Consumer uses the routing key** to **`bind to the queue`**

* **`Topic Exchange`** - also uses **routing key but not in exact match** instead it does a **`pattern match`**

* **`Header Exchange`** - routes messages based on **header values**

* **`Fanout Exchange`** - fans out all the messages to all the queues bound to it

# Example: Direct Exchange

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

======================================================================
> lifetime of a message 

# Prefetch count 
* -> if we have **multiple consumers connected to a queue**, then prefetch tells **`how many messages that particular consumer can prefetch and process`**
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
