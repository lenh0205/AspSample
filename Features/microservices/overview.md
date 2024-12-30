> https://www.youtube.com/watch?v=lTAcCNbJ7KE&t=22s

==================================================================
# microservices
* -> microservices are loosely coupled
* -> each service handles a dedicated function inside a large-scale application, these functional areas are sometimes called **`domains`**

==================================================================
# Communication
* _**API Gateway** là dành cho việc xử lý những external requests; còn việc giao tiếp giữa những microservices với nhau ta có thể dùng **HTTT request**_
* _đây thực sự là một nghịch lý, vì dù ta thiết kết microservice để độc lập với nhau nhưng việc một microservice gọi đến 1 microservice khác đã làm chúng coupling_

## API Gateway
* -> handles incoming requests and **`routes them to the relevant microservices`**
* -> to locate the service to route an incoming request to, the API Gateway consults a **`Service Registry & Discovery`** service
* -> relies on an **`identity provider service`** to **handle the authentication** and **put authorization of each request** coming through the API Gateway

## Between Microservices
* -> microservices communicate with each other via **well-defined interface** with **`small surface areas`**
* -> small surface areas limit the blast radius of failures and defects
* -> microservices talk to one another over a combination of **`RPC`** (**Remote Producer Calls**), **`Event Streaming`** or **`Message Brokers`**

* -> **RPC** like **`gRPC`** provides faster response, but blast radius, or the impact to other microservices would be larger when the service was to go down
* -> **`Event Streaming`** - provides better isolation between services but they take longer to process

* _về cơ bản thì Khi message cần được được xử lí ở nhiều service khác nhau không yêu cầu phản hồi kết quả ngay lập tức, hoặc message xử lí tốn rất nhiều thời gian thì người ta thường sử dụng message broker_
* _còn nếu các service giao tiếp với nhau cần phản hồi kết quả ngay thì thông thường người ta sẽ gọi trực tiếp qua REST API, GraphQL, gRPC_

==================================================================
# Database

## well-architected microservices practice: Strong Information Hiding
* -> this often means breaking up a monolithic database into its logical components and **`keeping each logical component well hidden inside its corresponding microservice`**
* -> _về cơ bản thì các microservice share data nhưng **không share database, mỗi microservices sẽ có database riêng**_
* _**logical component** này có thể là **`a seperate schema`** within a database cluster or **`an entirely seperate physical database`**_

* -> however, **one big drawback of microservices is breaking up of the database** into seperate logical units
* -> the database can **no longer maintain foreign key relationships** and **`enforce referential integrity between these units`**
* -> the burden of maintaining data integrity is now moved into application layer

==================================================================
# Citical Components
* -> some other useful components: **Monitoring and Alerting**, **DevOps Toolings** for deployment, Troubleshooting

# Tools
* _khi hệ thống microservices trở nên lớn, thì complexity lại xảy ra tiếp; vậy nên có 1 số tool được thiết kế để tackle and contain the ever-growing complexity of highly distributed microservices based systems_
* -> **`Containerization`** (Ex: **Docker**) - helps deploy microservices in minimalist self-contained runtimes
* -> **`Containers Orchestration`** (Ex: **Kubernetes**) - manage containers life cycles 
* -> **`Pipeline Automation`** (Ex: **GitLab**) - for CI/CD
* -> **`Asynchronous Messaging`** (Ex: **Kafka**) - further the couple's microservices by providing **Event Bus** or **Message Broker** and **Queue** 
* -> **`Performance Monitoring`** (Ex: **Prometheus**) - to track microservices performance
* -> **`Logging and Audit`** (Ex: **Datadog**) - keep track of everything happening within the system 

==================================================================
# Evolution of Application Design

## Monolithic
* -> design it as a single piece of code encapsulating data storage and access, business logic and user interfaces

* => limits when building complex intricate systems since everthing is tangled together, it becomes quickly difficult to maintain evolve and scale such application
* => solution: the **`multi-tier architecture`**

## Multi-tier 
* -> application components are **separated into layers** based on technical functions

* the common model is **`3 Tiers Architecture`** - consists 3 of logical layer
* -> a **`presentation layer`** - cover all code and components reponsible for the interaction with users through visual interfaces (_mantain by Frontend Developers_)
* -> a **`logical layer`** encompasses all the business logic and proccesses relative to the business function (_mantain by Backend Developers_)
* -> a **`data layer`** dealing with storing, accessing and retrieving data when needed (_mantain by Database Administrator_)

* => this still can not address quite well the chanllenges associated with complex application and systems
* => by breaking apart the logic and data layers into smaller pieces called **`microservices`**

## Microservice
* -> every microservices deals with **one business function end-to-end** independently from other microservices 
* -> microservices presents simple, easy to understand **`APIs`**
* -> communicate with each other through lightweight common protocols such as **HTTP** or **message queues**

### Purpose
* -> addresses the limitations and drawbacks of legacy applications
* -> cho phép các team làm việc tách biệt, không ảnh thưởng đến các team cũng như business functions khác
