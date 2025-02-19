> https://roadmap.sh/pdfs/best-practices/backend-performance.pdf

# Backend performance
* -> Caching
* -> Optimize API Response
* -> Databases
* -> Asynchronism
* -> Load Balancing & Scaling
* -> Code Optimization
* -> Security
* -> Monitoring and Logging
* -> Performance Testing
* -> Network

## what is 'backend performance'?
* -> Backend performance refers to how efficiently (system's **`speed, scalability, and reliability`** under different loads) a backend system (server, database, APIs, etc.) processes requests and delivers responses

* _there're some key metrics to measure backend performance:_
* -> **`Response Time (Latency)`** - how long the backend takes to process a request and return a response
* -> **`Throughput (Requests per Second)`** - number of requests a backend can handle per second
* -> **`CPU & Memory Usage`** - how efficiently the backend uses system resources (_Ex: CPU usage = 70%, Memory usage = 5GB_)
* -> **`Database Query Performance`** - includes query execution time, index efficiency, and number of queries per request
* -> **`Error Rate`** - percentage of failed requests due to server errors
* -> **`Concurrency & Scalability`** - how many simultaneous requests the backend can handle before performance drops; determine if horizontal or vertical scaling is needed
* -> **`Cache Hit Ratio`** - percentage of requests served from the cache instead of making a database query (_Ex: 80% cache hit ratio means 80% of requests don’t hit the database => faster performance_)
* -> **`Network Latency & Bandwidth`** -  the time taken for data transfer between client and server
* -> **`Uptime & Availability`** - percentage of time the backend is up and running
* -> **`Queue Length (Background Jobs)`** - if the backend uses background jobs (e.g., message queues like RabbitMQ), monitoring queue length is important

## Tools
Monitoring Tools: Prometheus, Grafana, New Relic, Datadog.
Load Testing Tools: JMeter, k6, Apache Bench.
Logging & Tracing: OpenTelemetry, Serilog.

## Bottlenecks
