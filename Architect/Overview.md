> Fundamentally, API can be broke down into either **`Request-Response APIs`** and **`Event-Driven APIs`**

================================================================================
# Request-Response APIs
* there're 3 commonly used standards:
* -> REST - Representational State Transfer
* -> RPC - Remote Procedure Call
* -> GraphQL

* REST are all about **resources**
* resources as nouns (https://foobar.com/api/v1/user) -> use verbs https://foobar.com/api/v1/getUsers is incorrect
* each resource typicall have 2 URLs: for collection (VD: users) and for entity in collection (specifying the identifier)
* consumer allowed to use these resources using CRUD operations
* REST Api typically return JSON, XML data

================================================================================
# Event-Driven Design
In web development, several technologies and tools help implement Event-Driven Design, enabling decoupled, scalable, and real-time applications. These technologies can be categorized into message brokers, event streaming platforms, real-time communication libraries, and frameworks.

## Message Brokers (Pub/Sub & Queue-Based Messaging)
These handle event distribution asynchronously. They are commonly used in microservices, distributed systems, and serverless architectures.

RabbitMQ – A message broker using AMQP (Advanced Message Queuing Protocol), often used for Pub/Sub and message queues.
Apache Kafka – A distributed event streaming platform, ideal for high-throughput event-driven applications and microservices.
Redis Pub/Sub – Lightweight Pub/Sub messaging for real-time communication and event propagation.
Azure Service Bus – A cloud-based messaging service that supports queues and topics for event-driven applications.
Amazon SNS (Simple Notification Service) & SQS (Simple Queue Service) – AWS-native services for event notifications and message queuing.

## Event Streaming Platforms
These technologies are used to process and analyze event data in real time.

Apache Kafka – Supports event streaming, real-time analytics, and event sourcing.
Apache Pulsar – A cloud-native, distributed messaging system similar to Kafka.
Kinesis (AWS) – A managed real-time data streaming service for large-scale event processing.

## Real-Time Communication Libraries
For instant event-based updates in web applications.

WebSockets – Used for real-time two-way communication (e.g., chat apps, live notifications).
SignalR (ASP.NET Core) – Microsoft’s WebSocket-based library for real-time communication in .NET applications.
Socket.IO – A JavaScript library for real-time bidirectional event-based communication (used in Node.js and frontend apps).

## Serverless & Event-Driven Cloud Services
Serverless platforms that execute functions in response to events.

AWS Lambda – Executes code in response to events from S3, DynamoDB, API Gateway, etc.
Azure Functions – Event-driven compute service for serverless event handling.
Google Cloud Functions – Similar to AWS Lambda, handling events from various cloud services.

## Event-Driven Frameworks & Libraries
Frameworks that help manage event-driven architecture in web applications.

MassTransit – A .NET distributed application framework for event-driven messaging using RabbitMQ, Azure Service Bus, Kafka, etc..
MediatR – A .NET library for implementing in-memory event-driven communication (CQRS, Domain Events).
NATS – A high-performance messaging system for microservices.
EventStoreDB – A database for event sourcing, allowing you to store events as a source of truth.
Common Use Cases in Web Development

## Use Case	Technologies
Microservices communication	Kafka, RabbitMQ, Azure Service Bus
Real-time updates (chat, notifications)	SignalR, Socket.IO, WebSockets
Serverless event-driven workflows	AWS Lambda, Azure Functions, Google Cloud Functions
Event streaming (analytics, logging)	Kafka, Kinesis, Pulsar
In-memory event-driven design (CQRS, domain events)	MediatR, MassTransit, EventStoreDB

## Which One Should You Use?
If you're working on microservices, use Kafka, RabbitMQ, or MassTransit.
If you need real-time WebSockets, use SignalR or Socket.IO.
If you're building a serverless event-driven system, use AWS Lambda or Azure Functions.
If you need high-speed event streaming, use Kafka or Pulsar.
If you need simple in-memory event handling, use MediatR.
