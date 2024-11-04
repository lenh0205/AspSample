> this section cover: **lock**, **Mutex, semaphores (for nonexclusive locking), reader/writer locks, SpinLock struct (From Framework 4.0 for high-concurrency scenarios)

====================================================================
# Locking
* -> **exclusive locking** is used to ensure that **`only one thread can enter particular sections of code at a time`**
* -> the **two main exclusive locking constructs** are **`lock`** and **`Mutex`**

* -> of the two, the **`lock`** construct is **faster and more convenient**
* -> **`Mutex`**, though, has a niche in that **its lock can span applications in different processes on the computer**

## 'lock' construct
* -> **`only one thread can lock the synchronizing object at a time`** (in the example, _locker), and **`any contending threads are blocked until the lock is released`**
* -> if **more than one thread contends the lock**, they are **`queued on a "ready queue"`** and **`granted the lock on a first-come - first-served basis`** 
* (_a caveat is that **nuances in the behavior of Windows and the CLR** mean that **`the fairness of the queue can sometimes be violated`**_)

* -> **exclusive locks** are sometimes said to **`enforce serialized access to whatever’s protected by the lock`**, because **one thread’s access cannot overlap with that of another**
* -> in this case, we're protecting the logic inside the Go method, as well as the fields _val1 and _val2.

```cs
// the example class is not thread-safe:
// if `Go` was called by two threads simultaneously, it would be possible to get a division-by-zero error
// because `_val2` could be set to zero in one thread right as the other thread was in between executing the if statement and Console.WriteLine

class ThreadUnsafe
{
  static int _val1 = 1, _val2 = 1;
 
  static void Go()
  {
    if (_val2 != 0) Console.WriteLine (_val1 / _val2);
    _val2 = 0;
  }
}
```

```cs - use "lock" can fix the problem
class ThreadSafe
{
  static readonly object _locker = new object();
  static int _val1, _val2;
 
  static void Go()
  {
    lock (_locker)
    {
      if (_val2 != 0) Console.WriteLine (_val1 / _val2);
      _val2 = 0;
    }
  }
}
```

## Thread blocked
* -> **a thread blocked while awaiting a contended lock** has a "ThreadState" of **`WaitSleepJoin`**
* -> _in **`Interrupt`** and **`Abort`**, we describe how **a blocked thread can be forcibly released via another thread**; this is a fairly heavy-duty technique that might be used in ending a thread_

<div class="sidebar">
    <p class="sidebartitle">A Comparison of Locking Constructs</p>
    <table border="1" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <th valign="top">Construct</th>
                <th valign="top">Purpose</th>
                <th valign="top">Cross-process?</th>
                <th valign="top">Overhead*</th>
            </tr>
            <tr>
                <td valign="top">
                    <a href="#_Locking">lock</a> (<code>Monitor.Enter</code> / <code>Monitor.Exit</code>)</td>
                <td valign="middle" rowspan="2">Ensures just one thread can access a resource, or section of code at a time</td>
                <td valign="top">-</td>
                <td valign="top">20ns</td>
            </tr>
            <tr>
                <td valign="top">
                    <a href="#_Mutex">Mutex</a>
                </td>
                <td valign="top">Yes</td>
                <td valign="top">1000ns</td>
            </tr>
            <tr>
                <td valign="top">
                    <a href="#_Semaphore">SemaphoreSlim</a> (introduced in Framework 4.0)</td>
                <td valign="middle" rowspan="2">Ensures not more than a specified number of concurrent threads can access a resource, or section of code</td>
                <td valign="top">-</td>
                <td valign="top">200ns</td>
            </tr>
            <tr>
                <td valign="top">
                    <a href="#_Semaphore">Semaphore</a>
                </td>
                <td valign="top">Yes</td>
                <td valign="top">1000ns</td>
            </tr>
            <tr>
                <td valign="top">
                    <a href="part4.aspx#_Reader_Writer_Locks">ReaderWriterLockSlim</a> (introduced in Framework 3.5)</td>
                <td valign="middle" rowspan="2">Allows multiple readers to coexist with a single writer</td>
                <td valign="top">-</td>
                <td valign="top">40ns</td>
            </tr>
            <tr>
                <td valign="top">
                    <a href="part4.aspx#_Reader_Writer_Locks">ReaderWriterLock</a> (effectively deprecated)</td>
                <td valign="top">-</td>
                <td valign="top">100ns</td>
            </tr>
        </tbody>
    </table>
    <p>*Time taken to lock and unlock the construct once on the
    same thread (assuming no blocking), as measured on an Intel Core i7 860. </p>
</div>

====================================================================
# 'Monitor.Enter' and 'Monitor.Exit'
* -> **`C#’s lock statement`** is in fact **a syntactic shortcut for a call to the methods** **`Monitor.Enter`** and **`Monitor.Exit`**, with **`a try/finally block`**
* -> **calling Monitor.Exit `without first` calling Monitor.Enter on the `same object`** **`throws an exception`**

* _here's (a simplified version of) what’s actually happening within the Go method of the preceding example:
```cs
Monitor.Enter (_locker);
try
{
  if (_val2 != 0) Console.WriteLine (_val1 / _val2);
  _val2 = 0;
}
finally { Monitor.Exit (_locker); }
```

## the lockTaken overloads

### Problem
* -> _the code that we just demonstrated is **exactly what the C# 1.0, 2.0, and 3.0 compilers produce in translating a lock statement**; however, there's **`a subtle vulnerability`** in this code_

* -> consider the (unlikely) event of **`an exception being thrown`** **within the implementation of Monitor.Enter**, or **between the call to Monitor.Enter and the try block** 
* (_perhaps, due to **`Abort`** being called on that thread — or an **`OutOfMemoryException`** being thrown_)
* => in such a scenario, **the lock may or may not be taken**
* => if **the lock is taken**, **`it won't be released`** — because we'll never enter the try/finally block
* => this will result in **`a leaked lock`**

### Solution
* -> to avoid this danger, **CLR 4.0's designers** added the following overload to **Monitor.Enter** (**`lockTaken` overload**):
```cs
public static void Enter (object obj, ref bool lockTaken);
```