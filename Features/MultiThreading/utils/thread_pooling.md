===================================================================
# Thread Pooling
* -> whenever we **start a thread**, **`a few hundred microseconds are spent organizing such things`** as **a fresh private local variable stack**
* -> **each thread** also **`consumes (by default) around 1 MB of memory`**

* => the **thread pool** **`cuts these overheads by sharing and recycling threads`**, **`allowing multithreading to be applied at a very granular level without a performance penalty`**
* -> this is useful when **leveraging `multicore processors` to execute `computationally intensive code`** in **`parallel` in "divide-and-conquer" style**

* -> the thread pool also keeps **`a lid on the total number of worker threads it will run simultaneously`**
* -> **too many active threads** **`throttle the operating system`** with **administrative burden** and render **CPU caches ineffective**
* => **once a limit is reached**, **`jobs queue up and start only when another finishes`**
* => this **makes `arbitrarily concurrent applications` possible**, such as **`a web server`** 
* => _the **`asynchronous method pattern`** is an advanced technique that takes this further by making highly efficient use of the **pooled threads**_

## Enter the thread pool
* -> via the **`Task Parallel Library`** (from Framework 4.0)
* -> by calling **`ThreadPool.QueueUserWorkItem`**
* -> via **`asynchronous delegates`**
* -> via **`BackgroundWorker`**

## 'constructs' use the 'thread pool' indirectly
* -> WCF, Remoting, ASP.NET, and ASMX Web Services application servers
* -> System.Timers.Timer and System.Threading.Timer
* -> Framework methods that end in Async, such as those on WebClient (the event-based asynchronous pattern), and most BeginXXX methods (the asynchronous programming model pattern)
* -> PLINQ

* => the **`Task Parallel Library (TPL)`** and **`PLINQ`** are sufficiently powerful and high-level that we'll want to use them to **assist in multithreading even when `thread pooling is unimportant`**
* => right now, we will use the **`Task` class** as a simple means of **`running a delegate on a pooled thread`**

## Notice when using pooled threads:
* -> we can **query if we're currently executing on a pooled thread** via the property **`Thread.CurrentThread.IsThreadPoolThread`**
* -> we **cannot set the `Name` of a pooled thread**, making debugging more difficult (although you can attach a description when debugging in Visual Studio’s Threads window).
* -> **pooled threads** are always **`background threads`** (_this is usually not a problem_)
* -> **blocking a pooled thread** may **`trigger additional latency in the early life of an application`** unless we call **ThreadPool.SetMinThreads**

* _we are **`free to change the priority of a pooled thread`** — it will be **restored to normal when released back to the pool**_

===================================================================
# Entering the Thread Pool via TPL
* -> we can **`enter the thread pool`** easily using the **`Task`** classes in the **Task Parallel Library**
* _the **Task** classes were introduced in **Framework 4.0**:_
* _if we're familiar with the `older constructs`, consider the **`nongeneric Task class`** a replacement for **ThreadPool.QueueUserWorkItem**_
* _and the **`generic Task<TResult>`** a replacement for **asynchronous delegates**_
* _the newer constructs are faster, more convenient, and more flexible than the old_

## TPL - the Task Parallel Library 
* -> has many more features, and is particularly well suited to **`leveraging multicore processors`**

## Using nongeneric 'Task' class
* -> to use the nongeneric Task class, call **`Task.Factory.StartNew`**, passing in **a delegate of the target method**
* -> _Task.Factory.StartNew_ returns **a Task object**, which we can then use to **`monitor the task`** 
* _for instance, we can **wait for it to complete** by calling its **`Wait`** method_

* -> any **unhandled exceptions** are conveniently **`rethrown onto the host thread`** when we call a task's **Wait** method
* (_if we don't call **Wait** and instead abandon the task, **`an unhandled exception will shut down the process as with an ordinary thread`**_)

```cs
static void Main()    // The Task class is in System.Threading.Tasks
{
  Task.Factory.StartNew(Go);
}
 
static void Go()
{
  Console.WriteLine ("Hello from the thread pool!");
}
```

## Using generic 'Task' class
* _the **generic Task<TResult> class** is **`a subclass` of the nongeneric Task**_
* -> it lets us **`get a return value back from the task`** after it **finishes executing**

* -> any **unhandled exceptions** are **`automatically rethrown when we query the task's 'Result' property`**, wrapped in an **AggregateException**
* -> however, **if we `fail to query` its 'Result' property (and don’t call `Wait`)** any unhandled exception will **`take the process down`**

```cs - we download a web page using Task<TResult>:
static void Main()
{
  // Start the task executing:
  Task<string> task = Task.Factory.StartNew<string>(() => DownloadString("http://www.linqpad.net"));
 
  // We can do other work here and it will execute in parallel:
  RunSomeOtherMethod();
 
  // When we need the task's return value, we query its Result property:
  // If it's still executing, the current thread will now block (wait)
  // until the task finishes:
  string result = task.Result;
}
 
static string DownloadString (string uri)
{
  using (var wc = new System.Net.WebClient())
    return wc.DownloadString (uri);
}
```

===================================================================
# Entering the Thread Pool Without TPL
* -> we **can't use the Task Parallel Library** if we're targeting an **`earlier version of the .NET Framework (prior to 4.0)`**
* -> instead, we must use one of the older constructs for entering the thread pool: **`ThreadPool.QueueUserWorkItem`** and **`asynchronous delegates`**
* -> the difference between the two is that **asynchronous delegates** let us **`return data from the thread`**; **asynchronous delegates** also **`marshal any exception back to the caller`**

## QueueUserWorkItem
* -> to use "QueueUserWorkItem", simply **`call this method with a delegate`** that we want to run on a **pooled thread**
* -> our **target method** **`must accept a single object argument`** (to satisfy the **`WaitCallback`** delegate)
* _this provides a convenient way of **`passing data to the method`**, just like with "ParameterizedThreadStart"_

* -> unlike with **Task**, **`QueueUserWorkItem doesn't return an object`** to help us **subsequently manage execution**
* -> also, we **`must explicitly deal with exceptions in the target code`** — **unhandled exceptions will take down the program**

## Asynchronous delegates
* -> **ThreadPool.QueueUserWorkItem** doesn't provide an easy mechanism for **`getting return values back from a thread`** after it has finished executing
* -> **asynchronous delegate invocations** (_asynchronous delegates for short_) solve this, allowing **`any number of typed arguments to be passed in both directions`**

### Handle exception
* -> furthermore, **unhandled exceptions on asynchronous delegates** are conveniently **`rethrown on the original thread`** (_or more accurately, the thread that calls **EndInvoke**_)
* _and so they don’t need explicit handling_

### start a 'worker task' via an 'asynchronous delegate'
* -> **`instantiate a delegate` targeting the method** we want to **`run in parallel`** (_typically one of the predefined Func delegates_)
* -> call **`BeginInvoke`** on the delegate, saving its **`IAsyncResult`** return value
* _**BeginInvoke** **`returns immediately to the caller`**; we can then **`perform other activities while the pooled thread is working`**_
* -> when we **need the results**, call **`EndInvoke`** on the delegate, passing in the saved **`IAsyncResult`** object

```cs - use an asynchronous delegate invocation 
// to execute concurrently with the main thread
// a simple method that returns a string’s length

static void Main()
{
  Func<string, int> method = Work;
  IAsyncResult cookie = method.BeginInvoke ("test", null, null);
  //
  // ... here's where we can do other work in parallel...
  //

  int result = method.EndInvoke (cookie);
  Console.WriteLine ("String length is: " + result);
}
 
static int Work (string s) { return s.Length; }
```

### EndInvoke 
* _EndInvoke does three things:_
* -> first, it **`waits for the asynchronous delegate to finish executing`**, if **it hasn't already**
* -> second, it **`receives the return value`** (_as well as any **ref** or **out** parameters_)
* -> third, it **`throws any unhandled worker exception back to the calling thread`**

* -> if the method we're calling with **an asynchronous delegate has no return value**, we are **`still (technically) obliged to call 'EndInvoke'`**
* -> in practice, this is open to debate; there are **no EndInvoke police to administer punishment to noncompliers!**
* -> however, if we **choose not to call EndInvoke** we'll need to **`consider exception handling on the worker method to avoid silent failures`**

### BeginInvoke
* -> the **final argument to BeginInvoke** is **`a user state object`** that populates the **`AsyncState` property of `IAsyncResult`** (_it can contain anything we like_)

* -> we can also **`specify a callback delegate`** when calling BeginInvoke — **a method accepting an 'IAsyncResult' object** that's **automatically called upon completion**
* -> this allows the **`instigating thread to "forget" about the asynchronous delegate`**, but it **requires a bit of extra work at the callback end**:

```cs
static void Main()
{
  Func<string, int> method = Work;

  // the final argument - pass the method delegate to the completion callback, so we can call EndInvoke on it
  method.BeginInvoke ("test", Done, method);
  // ...
  //
}
 
static int Work (string s) { return s.Length; }
 
static void Done (IAsyncResult cookie)
{
  var target = (Func<string, int>) cookie.AsyncState;
  int result = target.EndInvoke (cookie);
  Console.WriteLine ("String length is: " + result);
}
```

===================================================================
# Optimizing the Thread Pool
* -> the "thread pool" **`starts out with one thread in its pool`**
* -> as **tasks are assigned**, **`the pool manager "injects" new threads`** to **cope with the extra concurrent workload, up to a maximum limit**
* -> after **a sufficient period of inactivity**, **`the pool manager may "retire" threads`** if it suspects that **doing so will lead to better throughput**

## set the upper limit of threads that the pool will create
* -> by calling **`ThreadPool.SetMaxThreads`**
* -> the defaults are: **`1023`** in **Framework 4.0 in a 32-bit environment**; **`32768`** in **Framework 4.0 in a 64-bit environment**; **`250 per core`** in **Framework 3.5**; **`25 per core`** in **Framework 2.0**

* -> these figures may vary according to the hardware and operating system
* -> the reason there are that many is to **`ensure progress should some threads be blocked`** (**idling** while **awaiting some condition**, such as a response from a remote computer)

## set a lower limit
* -> we can also set a lower limit by calling **`ThreadPool.SetMinThreads`**
* -> the role of the lower limit is subtler: it's **an advanced optimization technique** that **`instructs the pool manager not to delay in the allocation of threads until reaching the lower limit`**
* -> **raising the minimum thread count** **`improves concurrency when there are blocked threads`**

* -> **the default lower limit** is **`one thread per processor core`** — the minimum that **`allows full CPU utilization`**
* -> on server environments, though (such ASP.NET under IIS), **`the lower limit is typically much higher — as much as 50 or more`**

===================================================================
# How Does the Minimum Thread Count Work?
* -> **increasing the thread pool’s minimum thread count** to x doesn't actually force x threads to be created right away — **`threads are created only on demand`**
* -> rather, it **instructs the pool manager** to **`create up to x threads the instant they are required`**

* _the question, then, is why would the thread pool otherwise delay in creating a thread when it's needed?_
* -> the answer is to **prevent `a brief burst of short-lived activity` from causing `a full allocation of threads`**, **`suddenly swelling an application’s memory footprint`**

## Case 1
* -> to illustrate, consider a quad-core computer running a client application that **enqueues 40 tasks at once**
* -> if each task performs a 10 ms calculation, the whole thing will be over in 100 ms, assuming the work is divided among the four cores
* -> ideally, we'd want the 40 tasks to run on exactly four threads: 
* -> **`any less and we'd not be making maximum use of all four cores`**; **`any more and we'd be wasting memory and CPU time creating unnecessary threads`**

* => and this is **exactly how the thread pool works** - **`matching the thread count to the core count`** 
* => allows **a program** to **`retain a small memory footprint without hurting performance`** — as long as the threads are efficiently used (which in this case they are)

## Case 2:
* -> but now suppose that instead of working for 10 ms, each task queries the Internet, **waiting half a second for a response while the local CPU is idle**
* -> the **`pool manager's thread-economy strategy breaks down`**; it would now do better to **create more threads**, so all the Internet queries could happen simultaneously

* => fortunately, the pool manager has a backup plan
* => **if its `queue` remains stationary for `more than half a second`**, it responds by **`creating more threads — one every half-second — up to the capacity of the thread pool`**

* -> **`the half-second delay is a two-edged sword`**
* -> on the one hand, it means that **a one-off burst of brief activity doesn't make a program suddenly consume an extra unnecessary 40 MB (or more) of memory**
* -> on the other hand, it can **needlessly delay things when a pooled thread blocks**, _such as when `querying a database` or calling `WebClient.DownloadFile`_
* => for this reason, we can **`tell the pool manager not to delay in the allocation of the first x threads`**, by calling **SetMinThreads**, for instance:
```cs
ThreadPool.SetMinThreads (50, 50);
// The second value indicates how many threads to assign to I/O completion ports, which are used by the APM
// The default value is one thread per core
```
