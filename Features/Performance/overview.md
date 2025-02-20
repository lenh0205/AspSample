> https://roadmap.sh/pdfs/best-practices/backend-performance.pdf

============================================================================
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
* -> Monitoring Tools: Prometheus, Grafana, New Relic, Datadog.
* -> Load Testing Tools: JMeter, k6, Apache Bench.
* -> Logging & Tracing: OpenTelemetry, Serilog.

## Bottlenecks

============================================================================
# API Performance

## Rule
* -> optimization **should not be the first step in our process** - done it prematurely can lead to **`unnecessary complexity`**
* -> first step should always be to **`identify the actual bottlenecks`** through **load testing** and **profiling requests**
* => only begin optimization once we're confirmed that **an API endpoint has performance issues**

## Caching
* -> _one of the most effective ways to speed up our APIs_
* -> store the result of an expensive computation so that we can use it again later **`without needing to redo the computation`**

* => if we have **an endpoint that is `frequently accessed` with the `same request parameters`**, we can **avoid repeated database hits** by caching the response in **`Redis`** or **`Memcached`**
* _most caching libraries make this easy to add with just a few lines of code_
* _even a brief period of caching can make a significant difference in speed_

## Connection Pool
* -> involves maintaining a pool of connections rather than opening a new database connection for each API call 
* -> creating a new connection each time involves a lot of handshake protocols and setup which can slow down our API
* => this reuse of connections can greatly improve throughput

https://www.youtube.com/watch?v=zKim2DZw91k
https://www.youtube.com/watch?v=TbQZXayzzTg
https://www.youtube.com/watch?v=zK3jDkLBgcM
https://www.youtube.com/watch?v=nwBBd9GrcqI

### Serverless Architecture
* -> connection management can be a bit **more challenging** - because each serverless function instance typically **`opens its own database connection`**
* -> and because serverless can **scale rapidly**, this could potentially lead to a large number of open connections that can **`overwhelm the database`** 

* => solution like **`AWS RDS Proxy`** and **`Azure SQL Database serverless`** are designed to handle this situation and manage connection pooling 

## N+1 query problem
* -> _closely related to database performance_
* -> the N+1 problem is a common inefficient that can occur when accessing data of an entity and its related entities
  
```sql - Ex:
-- we're building an API endpoint to fetch blog "posts" and their "comments"; 
-- the N+1 problem would occur if we first made a query to fetch the "posts" and then for each post, we made another query to fetch its "comments"

-- 1 query for "posts"
SELECT id, title FROM posts;

-- N query for "comments"
SELECT id, content FROM comments WHERE id = 1;
SELECT id, content FROM comments WHERE id = 2;
SELECT id, content FROM comments WHERE id = 3;
```

```sql - solution
-- to avoid this, it's more efficient to fetch the data in a single query 
-- or in some cases, two queries: one to fetch the posts and one to fetch all the comments for those posts
SELECT 
    posts.id AS post_id, 
    post.title AS post_title, 
    comments.id AS comment_id,
    comments.content AS comment_content
FROM posts
LEFT JOIN comments ON comments.post_id = posts.id;
```

* => this can significantly **`reduce the number of round trips to the database`** - improve performance

## Pagination
* -> if our API response returns a large amount of data, it can slow things down
* => instead **break the response into smaller, more manageable pages** using **`limit and offset parameters`**
* => this can **`speed up data transfer`** and **`reduce load on the client side`**

## JSON serialization
* -> when returning JSON responses from our API, the **`speed of our serialization process can make a noticeable difference in response times`**
* => consider using a **`fast serialization library`** to **minimize the time spent converting our data into JSON format**

https://www.youtube.com/watch?v=HhyBaJ7uisU
https://www.youtube.com/watch?v=w7ZfEVC76ho
https://www.youtube.com/watch?v=w7ZfEVC76ho

## Compression
* -> by enabling **compression on large API response payloads**, we can **`reduce the amount of data tranferred over the network`**, the client then decompresses the data
* => nowadays, there are even more efficient algorithms like **`Brotli`** that provide **better compression ratios**
* => also many **`Content Delivery Networks (CDN) like Cloudflare`** can **handle compression for us** - **`offloading this task from our server`**

https://www.youtube.com/watch?v=g7qyEgxz9kE
https://www.youtube.com/watch?v=NLtt4S9ErIA
https://www.youtube.com/watch?v=D4Xq_9_FM1g
https://www.youtube.com/watch?v=Xb2PSCkkaaE

## Asynchronous logging
* -> in many applications, the time it takes to write logs is negligible; however, in high-throughput systems where every milisecond counts **the time taken to write logs can add up**
* => in such cases, **asynchronous logging** can help 
* -> this involves the **`main application thread quickly placing the log entry into an in-memory buffer`**, 
* -> while **`a seperate logging thread writes the log entries to the file or sends them to logging service`**
* _with asynchronous logging, there's a small chance we might lose some logs if our application crashes before the logs have been written_