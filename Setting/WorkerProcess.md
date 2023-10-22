# Worker Processes
* **separate instances** of a program that run **concurrently** to perform **`tasks`** in parallel

* used in **server applications** to **handle incoming client requests** more efficiently
* -> By **`distributing the workload across multiple worker processes`**,
* -> the server can better utilize the available system resources, such as CPU cores and memory
* => leading to **`improved performance and higher throughput`**

## Mechanism
* In the context of `web servers` or `application servers`, _each worker process_ typically _listens for incoming connections and processes client requests_ **independently** of the other worker processes

* -> When `a request` comes in, it is handled by **`one of the worker processes`**
* -> While this worker process is _busy processing the request_, the **`other worker processes can continue to listen for and process`** additional incoming requests

* => This parallelism helps ensure that the **server can handle multiple simultaneous connections** without becoming overwhelmed or unresponsive

## Implement Worker Process
* Worker processes can be implemented using various techniques

### Multi-process model
* the `server application` **spawns multiple child processes**; each running as **a separate instance of the program**
* -> Each child process `can run on a separate CPU core`
* ->  allowing the server to take advantage of **`multi-core systems`**

### Multi-threaded model
* the `server application` creates **multiple threads within a single process** (_instead of creating separate processes_)
* -> `Each thread` can handle incoming requests concurrently, similar to the multi-process model
* -> However, since **`threads share the same memory space`** `within the process`
* -> must be careful to **`manage shared resources`** and avoid issues like `data corruption` or `race conditions`

### Asynchronous or event-driven model
* the `server application` uses **asynchronous I/O** and **an event-driven architecture** to **`handle multiple simultaneous connections within a single process or thread`**    

* **Ưu điểm**: This model can provide high levels of concurrency without the need for multiple worker processes or threads
* **Nhược điểm**: requires a more complex programming model and is not suitable for all types of workloads

## Usage Example:
* In the context of Gunicorn (a popular WSGI server for Python applications)
* worker processes are used to serve multiple concurrent client requests
* can specify the number of worker processes when starting Gunicorn with the -w option followed by the desired number of workers (_eg: gunicorn -w 4 app:app_)
* Gunicorn would `spawn four worker processes` to handle incoming requests


