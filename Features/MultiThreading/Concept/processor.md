> rồi time-slice ruốt cuộc là cái khỉ gì?
> "một luồng tương tự như tiến trình hệ điều hành mà ứng dụng của bạn chạy trong đó" là cái khỉ gì ?
> a parallel "worker" thread là cái quái gì nữa ?
> máy tính với "multicore" và "multiprocessor" có khác nhau không ?

=============================================================================
# How Threading Works ?
* -> **multithreading** is managed internally by **`a thread scheduler`**

=============================================================================
# Thread scheduler
* -> **`a function`** the **CLR typically delegates to the operating system**
* -> a thread scheduler **`ensures all active threads are allocated appropriate execution time`**, and that **`threads that are waiting or blocked do not consume CPU time`** (_for instance, on an exclusive lock or on user input_)

# Thread scheduler with processor
* -> **a thread** is said to be **`preempted`** when **`its execution is interrupted`** due to **an external factor such as time-slicing**
* -> in most situations, a thread has no control over when and where it's preempted

### on a "single-processor" computer
* -> a thread scheduler performs **time-slicing** — **`rapidly switching execution between each of the active threads`**
* -> under Windows, **`a time-slice`** is typically in **the tens-of-milliseconds region** 
* -> **`much larger than the CPU overhead`** in **actually switching context between one thread and another** (_which is typically in the `few-microseconds region`_)

### on a multi-processor computer
* -> **multithreading** is implemented with a mixture of **`time-slicing`** and **`genuine concurrency`** - where **`different threads run code simultaneously on different CPUs`**
* -> it’s almost certain there will **still be some time-slicing**, because of **`the operating system's need to service its own threads`** — as well as those of other applications

=============================================================================
# 'Threads' vs 'Processes'
* -> **`a thread`** is analogous to **`the operating system process in which our application runs`**
* -> just as **processes run in parallel on a computer**, **`threads run in parallel within a single process`**

* -> **`Processes` are `fully isolated` from each other**; **`threads` have just `a limited degree of isolation`**
* -> in particular, **`threads share (heap) memory with other threads running in the same application`**
* => this, in part, is **why threading is useful**: one thread can fetch data in the background, for instance, while another thread can display the data as it arrives

=============================================================================
# Threading’s Uses and Misuses

## Uses
* -> with technologies such as **`ASP.NET`** and **`WCF`**, we may be unaware that multithreading is even taking place — unless we **access `shared data` (perhaps via static fields) without appropriate `locking`, `running afoul of thread safety`**
* ->  _Multithreading has many uses; here are the most common:_

### Maintaining a responsive user interface
* -> by running **`time-consuming tasks`** on **a parallel "worker" thread**, the main UI thread is free to continue processing keyboard and mouse events

### Making efficient use of an otherwise blocked CPU
* -> "multithreading" is useful when **a thread is `awaiting a response` from another computer or piece of hardware**
* -> while **one thread is blocked** while performing the task, **`other threads can take advantage of the otherwise unburdened computer`**

### Parallel programming
* -> **code that performs `intensive calculations`** can **execute faster on `multicore` or `multiprocessor` computers** if the **workload is shared among multiple threads** in **`a "divide-and-conquer" strategy`**

### Speculative execution
* -> **on `multicore` machines**, we can sometimes **improve performance** by **`predicting something that might need to be done`** and then **`doing it ahead of time`**

* -> a variation is to **run a number of `different algorithms in parallel`** that **`all solve the same task`**
* -> whichever one finishes first "wins" — this is effective when we **can’t know ahead of time which algorithm will execute fastest**

### Allowing requests to be processed simultaneously
* -> on a server, **client requests can arrive concurrently** and so **`need to be handled in parallel`** (the .NET Framework creates threads for this automatically if you use ASP.NET, WCF, Web Services, or Remoting)\
* -> this can also be **`useful on a client`** (e.g., handling peer-to-peer networking — or even multiple requests from the user)

## Misuses
* _threads also come with strings attached; the biggest is that multithreading can **increase complexity**_
* -> **having lots of threads `does not` in and of itself `create much complexity`**; it’s **`the interaction between threads (typically via shared data) that does`**
* => this applies whether or not the interaction is intentional, and can cause **long development cycles** and **an ongoing susceptibility to `intermittent and nonreproducible bugs`**
* => a good strategy is to **`encapsulate multithreading logic into reusable classes`** that can be **independently examined and tested** (_the Framework itself offers many "higher-level threading constructs"_)

* _**threading** also **`incurs a resource and CPU cost`** in **scheduling** and **switching threads** (when there are more active threads than CPU cores) — and there’s also **a creation/tear-down cost**_

* _multithreading **will not always speed up our application** — it can even `slow it down` if used excessively or inappropriately_
* -> _For example, when heavy disk I/O is involved, it can be `faster to have a couple of worker threads run tasks in sequence` than to have `10 threads executing at once`_