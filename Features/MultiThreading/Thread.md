> https://www.albahari.com/threading/

* _thread bình thường và pooled thread ?_

====================================================
## Introduce
* tạo và start 1 thread ngoài main thread, cách 2 thread chạy song song
* mỗi thread có **`stack riêng`**, nhưng **`có thể share heap với thread khác`**
* nhưng việc share tài nguyên có thể dẫn tới **Thread safe**, để tránh việc đó ta có thể dùng **lock**
* **.join()** để chờ 1 Thread khác kết thúc; **.Sleep()** đề pause Thread 
* **Thread.Sleep(0)** sẽ đưa tài nguyên CPU cho các Thread khác; **Thead.Yield** cũng làm điều tương tự nhưng chỉ cho những Thread chạy trên cùng processor
* **Thread.Sleep** và **Thread.Yield** có thế giúp ích về performance và debug **`thread safety`**

* **Multithreading** được quản lý bởi **thread scheduler**; 
* **`thread scheduler`** giúp đảm bảo những thread đang đợi hoặc blocked **do not consume CPU time**
* với **`single-processor computer`**, thread scheduler performs **time-slicing**
* với **`multi-processor computer`**, **`multithreading`** is implemented with a mixture of **time-slicing** and **genuine concurrency**
* just as (operating system) **`processes`** (in which our application runs) run in parallel on a computer, **`threads`** **run in parallel within a single process**

* **`Process`** are **fully isolated** from each other, nhưng Thread có thể share memory với Thread khác; đây là điều làm Thread trở nên có ích 
* vậy nên sự phức tạp của **`multithreading`** không đến từ có nhiều Threads mà là **the interaction between threads (typically via shared data)**
* => điều này có thể dẫn tới những lỗi không xảy ra liên tục và không thể tái hiện lại lỗi ngay được; làm quá trình dev trở nên rất khó khăn
* => ta cần giảm tối thiểu sự tương tác giữa các Thread cũng như tuân thủ 1 số nguyên tắc thiết kế
* => good strategy is to **encapsulate multithreading logic into reusable classes**
* sử dụng MultiThreading quá mức cũng không hẳn là tốt, vì nó tiêu tốn tài nguyên và CPU cho việc scheduling, switching threads, creation/tear-down

## Creating and Starting Threads
* threads are created using the **Thread** class’s constructor, passing in a **`ThreadStart delegate`**, calling **.Start()**
* thread end when its method returns

```c# - method with no param
Thread t = new Thread (new ThreadStart (Go));
// or
Thread t = new Thread (Go);
// or 
Thread t = new Thread (() => Console.WriteLine ("Hello!"));

t.Start();   // Run Go() on the new thread
Go();        // Simultaneously run Go() in the main thread

public delegate void ThreadStart();
static void Go()
{
    Console.WriteLine ("hello!");
}
```

```c# - method with param - passing data to thread
// a lambda expression
Thread t = new Thread (() => Print ("Hello from t!"));
t.Start();
static void Print (string message) 
{
  Console.WriteLine (message);
}

//  a multi-statement lambda
new Thread (() =>
{
  Console.WriteLine ("I'm running on another thread!");
  Console.WriteLine ("This is so easy!");
}).Start();

// anonymous methods
new Thread (delegate()
{
  ...
}).Start();

// pass an argument into Thread’s Start method
// Thread’s constructor is overloaded to only accept 1 param that has "object" type 
Thread t = new Thread (Print);
t.Start ("Hello from t!");
static void Print (object messageObj)
{
  string message = (string) messageObj;   // require casting
  Console.WriteLine (message);
}
```

* avoiding accidentally modifying **captured variables** after starting the thread; the solution is to **`use a temporary variable`**
```c#
string text = "t1";
Thread t1 = new Thread ( () => Console.WriteLine (text) );
 
text = "t2";
Thread t2 = new Thread ( () => Console.WriteLine (text) );
 
t1.Start();
t2.Start();
```

* we can Naming Thread for better Debug

* **Foreground Threads** and **Background Threads**: 
* **`Foreground threads`** **keep the application alive** for as long as any one of them is running
* đây sẽ là 1 vấn đề vì những **`finally block`** hoặc **`using block`** được program dùng để releasing resources hoặc xoá những file tạm sẽ bị huỷ khi tất cả Foreground Threads end
* có 2 cách là call **Join** on the thread hoặc s/d **event wait handle**; và ta nên chỉ định **`timeout`** như là 1 phương án backup để close application
* we can change a thread to background status using its **IsBackground** property
```c#
// the worker thread as a foreground thread
// application is alive until user press a keyboard
Thread worker = new Thread (() => Console.ReadLine());
worker.Start();

// the worker is assigned background status
// the program exits almost immediately as the main thread ends (terminating the ReadLine)
Thread worker = new Thread ( () => Console.ReadLine() );
worker.IsBackground = true;
worker.Start();
```
* Nếu ta s/d **Task Manager** để **`force end 1 .NET process`**, rất có thể **all threads "drop dead"** như thể chúng là background threads
* ta cũng nên take care Foreground threads to avoid bugs that could cause the **`thread not to end`**; a common cause for applications failing to exit properly is the presence of active foreground threads

* when multiple threads are **`simultaneously active`**, **thread’s Priority** property determines how much execution time it gets relative to other active threads in the operating system
* but this can lead to problems such as **`resource starvation`** for other threads
* elevating a thread’s priority doesn’t make it capable of performing real-time work, because it’s still throttled by the **`application’s process priority`**

* to **`perform real-time work`**, we also need to elevate the **process priority** using the Process class in System.Diagnostics 
* -> **ProcessPriorityClass.High** is usually the best choice for real-time applications
* -> but actually, the highest priority is **Realtime**, this instructs the OS that the process **`never yield CPU time`** to another process
* có 1 vấn đề là nếu real-time application cần update UI thì việc này lại tiêu tốn quá mức **`CPU time`**, làm chậm toàn bộ computer
* giải pháp là ta nên chạy real-time worker và user interface **`trên những app tách biệt nhau với process priority khác nhau`**, và giao tiếp thông qua **Remoting** or **memory-mapped files** 
* nhưng nói chung vấn đề về real-time vẫn sẽ bị giới hạn về the suitability of the managed environment, vậy nên ta cần **`dedicated hardware`** or **`a specialized real-time platform`**

* any **try/catch/finally blocks** in scope when a thread is created are of **`no relevance to the thread`** when it starts executing
* -> cũng dễ hiểu vì **`each thread has an independent execution path`**
* -> need an exception handler on all thread entry methods in production applications
* -> the **"global" exception handling events** for WPF and Windows Forms applications fire only for exceptions thrown on the **`main UI thread`**
* -> we still must **handle exceptions on worker threads manually**

* **Thread Pooling** 
* -> thread pool **`cuts some overheads`** when starting a thread by sharing and recycling threads; make better performance
* -> thread pool also **`limit the total number of worker threads`** it will run simultaneously
* -> to enter the thread pool: **Task Parallel Library**, **ThreadPool.QueueUserWorkItem**, **asynchronous delegates**, **BackgroundWorker**

* **Entering the Thread Pool via TPL - Task Parallel Library**
* Framework 4.0 introduce **Task** classes (_in the Task Parallel Library_)
* -> ta có thể xem **`nongeneric Task`** class như replacement cho **ThreadPool.QueueUserWorkItem**, và **`generic Task<TResult>`** class như replacement cho **asynchronous delegates** ở những phiên bản cũ hơn
* -> **Task.Factory.StartNew** returns a **Task object** - use to **`monitor the task`** (_Ex: we can wait for it to complete by calling its **`Wait`** method_)

* **Entering the Thread Pool without TPL**
* -> if we're targeting an earlier version < .NET Framework 4.0; we must use older constructs for entering the thread pool: **`ThreadPool.QueueUserWorkItem`** and **`asynchronous delegates`**
* -> 

