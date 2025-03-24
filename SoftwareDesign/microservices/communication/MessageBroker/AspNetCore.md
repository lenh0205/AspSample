======================================================================
# Integrating RabbitMQ with ASP.NET Core
* -> Using RabbitMQ.Client NuGet package for connecting
* -> Setting up a ConnectionFactory and channels
* -> Publishing messages to exchanges
* -> Consuming messages with background workers (IHostedService or BackgroundService)
* -> Handling failures, retries, and dead-letter queues (DLQ)

======================================================================
> basic setup

# Example 1: using Queue for message communication between a Producer application and a Consumer application

## Setup
* first, install a **Docker images of RabbitMQ** and also run start it in at a particular Port
```bash
$ docker run -d --hostname my-rabbit --name ecomm-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
# hostname is "my-rabbit"
# name of the instance is "ecomm-rabbit"
# expose the port which we want to access from outside: fist is 5672 (the port used by AMQP protocol), second is 15672 (port used by the Management Console)
# we will use the "rabbitmq:3-management" image

# -> khi chạy thì nó sẽ cố tìm "rabbitmq:3-management" image trên máy local nếu không tìm thấy, nó sẽ pull về
# -> rồi sau đó nó sẽ chạy RabbitMQ
# -> giờ chạy "docker logs -f e67" thì ta có thể thấy Management Console is up and running, and the RabbitMQ is ready to use

# giờ ta có thể truy cập thử "http://localhost:15672" trên Browser để xem RabbitMQ Management Console (or Administration Console); and by default RabbitMQ has username/password as "guest/guest"
# RabbitMQ is listening on port 5672 for AMQP; because we have mapped the port of the Docker container to localhost:5672 nên, khi ta truy cập localhost:5672 it going to call it back to Docker container 
```
```bash
# the installation also contain a CLI tool to manage and inspect or Broker "rabbitmqctl"
$ rabbitmqctl list_queues
```

* setup 1 Console App as "producer"
* -> ta cần cài NuGet package (_implements a messaging protocol like "AMQP 0-9-1"_) **`RabbitMQ.Client`** for RabbitMQ 
```cs
using RabbitMQ.Client;

static class Program
{
    static void Main(string[] args)
    {
        // create a connection factory
        var factory = new ConnectionFactory
        {
            // pass the URI of RabbitMQ client that we created include AMQP pattern (amqp://...)
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
        channel.QueueDeclare(
            "demo-queue",
            durable: true, 
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

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


# Example 2: using Queue for message communication between a Producer application and multiple Consumer applications
* -> 1 Producer with multiple Consumer - how the messages are distributed across multiple Consumers
* -> **Exchange** in RabbitMQ

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
* -> ta sẽ thấy Consumers is getting the message theo cách tuần tự - 1 Consumer sẽ chỉ nhận toàn message chẵn còn 1 Consumer sẽ chỉ nhận toàn message lẻ (_xem phần $"Hello! Count: {count}" được log ra_)

* => if we have **multiple consumers to a single queue**, **`the messages will be evenly distributed across the consumers`**
* => ensure that **`one consumer gets a unique message`** - the **`same message is not delivered to multiple consumers`** 

* => this is extremely important when it comes to scaling a service horizontally, the RabbitMQ give us exactly that option
