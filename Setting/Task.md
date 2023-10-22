# Overview
## Trong phạm vi kernel
* `the kernel` is allowed to manage the **hardware interrupts** used to switch between things that are running on CPUs.
* to decide when and where things should run, the kernel implements a **scheduler**
* -> `Linux scheduler implements` a **"task" structure** which contains CPU masks and other things that are important for `deciding priorities`
* -> However, because the every computer architecture handles running code slightly differently
* => Linux maps that `Task` onto a **`schedulable entity`** called a **Thread**

* Linux schedules a task and its associated thread is run. `All threads` operate within the confines of a **process**

## Ở phạm vi user-Thread
* is basically some state and your program code
*  features provided by user-threads are ultimately defined by the **`programming language`**

# Process
*  a Process are basically **a program** that are dispatched from the ready state and are **scheduled in the CPU for execution**
* A single process can contain several threads for executing multiple tasks
* A process **`can create other processes`** - _Child Processes_
* does **`not share the memory`** with any `other process`
* **state**: new, ready, running, waiting, terminated, and suspended. 
* **Process switching** must uses an interface in an operating system
* If one `process is blocked` then it will **not affect the execution of other processes**

# Thread
* is the smallest unit of execution 
* a process can `have multiple threads` and these multiple threads **`are contained within a process`**
* Threads `uses address spaces of the process`; and do **`not isolate`** (_thread share memory_)
* **state**: Running, Ready, and Blocked
* **Thread switching** does not require calling an operating system and causes an interrupt to the kernel
* If `a user-level thread is blocked`, then **all other user-level threads are blocked**
* OS use a `thread` to **contain everything needed for code execution** - include: CPU flags, timers, counters, a stack, ...


# Task
* operating system needs to `execute a set of instructions` -> An **execution of thread** results in a Task
* A task is simply **a set of instructions loaded into the memory**
* The operating system `schedules a thread to run`. And when it runs, the executable code is considered a task
* **`Task`** is mapped to a thread to determine what must be executed
* More than one task can be executed at the same time so that you can run multiple programs at the same time and perform different tasks simultaneously