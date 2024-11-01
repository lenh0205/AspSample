
# Foreground and Background Threads
* -> by default, **threads we create explicitly** are **`foreground threads`**
* -> "foreground threads" **`keep the application alive for as long as any one of them is running`**, whereas **background threads do not**
* -> **once all foreground threads finish**, the **application ends**, and **`any background threads still running abruptly terminate`**
* _**a thread's foreground/background status** has **`no relation to its priority or allocation of execution time`**_

## Functional
* -> we can **query or change a thread's background status** using its **`IsBackground`** property

```cs
class PriorityTest
{
  static void Main (string[] args)
  {
    Thread worker = new Thread ( () => Console.ReadLine() );
    if (args.Length > 0) worker.IsBackground = true;
    worker.Start();
  }
}

// if this program is called with no arguments
// -> the application will keep running, even though the main thread exits, until the "ReadLine" statement fullfill when the user to press Enter
// -> because "worker" thread in this case is a 'foreground thread'

// if an argument is passed to Main()
// -> the "worker" is assigned 'background' status
// -> the program exits almost immediately as the main thread ends (terminating the ReadLine)
```

## Problem with background thread
* -> when a process terminates in this manner, any **`finally` blocks in the execution stack of `background threads`** are **`circumvented`**
* => this is a problem if our program employs **`finally` (or `using`) blocks** to perform cleanup work such as releasing resources or deleting temporary files

## handle 'Background thread' when application exit
* -> to avoid this, we can explicitly **wait out such background threads upon exiting an application**; there are 2 ways to accomplish this:
* => if we've **created the thread ourself**, call **`Join`** on the thread; if we're **on a pooled thread**, use an **`event wait handle`**

* -> in either case, we should **`specify a timeout`**, so we can **abandon a renegade thread should it refuse to finish for some reason**
* _this is our backup exit strategy: in the end, we want `our application to close — without the user having to enlist help from the Task Manager`_
* -> if **a user uses the Task Manager to forcibly end a .NET process**, **`all threads "drop dead"`** as though they were **`background threads`**
* _this is observed rather than documented behavior, and it could vary depending on the CLR and operating system version_

## handle 'Foreground thread' when application exit
* -> **Foreground threads** don't require this treatment, but we **`must take care to avoid bugs that could cause the thread not to end`**
* -> a common cause for **applications failing to exit properly** is **`the presence of active foreground threads`**

