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

======================================================================
> basic setup

# Example 1: using Queue for message communication between a Producer application and a Consumer application

## Setup
* first, install a **Docker images of RabbitMQ** and also run start it in at a particular Port
```bash
docker run -d --hostname my-rabbit --name ecomm-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
# hostname is "my-rabbit"
# name of the instance is "ecomm-rabbit"
# expose the port which we want to access from outside: fist is 5672 (the port used by AMQP protocol), second is 15672 (port used by the Management Console)
# we will use the "rabbitmq:3-management" image

# -> khi chạy thì nó sẽ cố tìm "rabbitmq:3-management" image trên máy local nếu không tìm thấy, nó sẽ pull về
# -> rồi sau đó nó sẽ chạy RabbitMQ
# -> giờ chạy "docker logs -f e67" thì ta có thể thấy Management Console is up and running, and the RabbitMQ is ready to use

# giờ ta có thể truy cập thử "http://localhost:15672" trên Browser để xem RabbitMQ Management Console (or Administration Console); and by default RabbitMQ has username/password as "guest/guest"
# RabbitMQ is listening on port 5672 for AMQP; we have mapped the port of the Docker container to localhost:5672 nên nếu ta s/d localhost:5672, it going to call it back to Docker container 
```

* setup 1 Console App as "producer"
* -> ta cần cài NuGet package **`RabbitMQ.Client`** for RabbitMQ
```cs
using RabbitMQ.Client;

static class Program
{
    static void Main(string[] args)
    {
        // create a connection factory
        var factory = new ConnectionFactory
        {
            // pass the URI of RabbitMQ client that we created include AMQP pattern
            Uri = new Uri("amqp://guest:guest@localhost:5672");
        };

        // create a connection
        using var connection = factory.CreateConnection();

        // create a channel
        using var channel = connection.CreateModel();

        // publish message
        QueueProducer.Publish(channel);
    }
}

public static class QueueProducer
{
    public static void Publish(IModel channel)
    {
        // declare a queue: 
        channel.QueueDeclare(
            "demo-queue", // queue name
            durable: true, // keep message hang round until a consumer reads it
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // publish a message to the queue
        var message = new { Name = "Producer", Message = "Hello!" };
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        channel.BasicPublish(
            "", // we aren't sending to any specific 'exchange', just use the default Exchange
            "demo-queue", // Routing key is the name of the Queue
            null,
            body
        );
    }
}
```

* setup 1 Console App as "Consumer"
* -> ta cần cài NuGet package **`RabbitMQ.Client`** for RabbitMQ
```cs
using RabbitMQ.Client;

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

        QueueConsumer.Consume(channel);
    }
}

public static class QueueConsumer
{
    public static void Consume(IModel channel)
    {
        channel.QueueDeclare("demo-queue", durable: true, 
            exclusive: false, autoDelete: false, arguments: null);

        // create a Consumer
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) => {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };
        channel.BasicConsume("demo-queue", true, consumer);

        Console.WriteLine("Consumer started");
        Console.ReadLine(); // để chương trình không bị thoát sau khi chạy xong
    }
}
```

## Running
* -> ta sẽ chạy Consumer trước
* -> ta có thể vào **"Queues" section** của **localhost:15672** để xem queue "demo-queue" mà Consumer vừa tạo ra, ta thấy 1 Consumer is connecting to the Queue và chưa có message nào
* -> giờ ta sẽ run Producer
* -> giờ ta vào **"Queues" section**, ta sẽ thấy 1 message comming in

======================================================================
> 1 Producer with multiple Consumer - how the messages are distributed across multiple Consumers
> **Exchange** in RabbitMQ

# Example: using Queue for message communication between a Producer application and multiple Consumer applications

## Setup
```cs - Producer
public static class QueueProducer
{
    public static void Publish(IModel channel)
    {
        channel.QueueDeclare("demo-queue", durable: true, 
            exclusive: false, autoDelete: false, arguments: null);

        // publish multiple message at the same time
        var count = 0;
        while (true)
        {
            var message = new { Name = "Producer", Message = $"Hello! Count: {count}" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish("", "demo-queue", null, body);
            count++;

            // not continuously produce a message 
            Thread.Sleep(1000);
        }
    }
}
```

# Running
* -> first, ta sẽ chạy 2 instances của Console App "Consumer" (click file ".exe" 2 lần để chạy 2 chương trình)
* -> sau đó ta sẽ chạy Producer
* -> ta sẽ thấy Consumers is getting the message theo cách tuần tự; 1 Consumer sẽ chỉ nhận toàn message chẵn còn 1 Consumer sẽ chỉ nhận toàn message lẻ (_xem phần $"Hello! Count: {count}" được log ra_)

* => if we have **multiple consumers to a single queue**, **`the messages will be evenly distributed across the consumers`**
* => ensure that **`one consumer gets a unique message`** - the **`same message is not delivered to multiple consumers`** 

* => this is extremely important when it comes to scaling a service horizontally, the RabbitMQ give us exactly that option

===================================================================
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