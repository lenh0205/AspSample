> while waiting on a **Sleep** or **Join**, `a thread is blocked` and so `does not consume CPU resources`

# Join
* -> **`wait for another thread to end`** (_hoặc dừng execute main thread tới khi thread được gọi .Join() chạy xong_)
* -> it return **true** if the **`thread ended`**; or **false** if **`it timed out`**

```cs
static void Main()
{
  Thread t = new Thread (Go);
  t.Start();
  t.Join();
  Console.WriteLine ("Thread t has ended!");
}
 
static void Go()
{
  for (int i = 0; i < 1000; i++) Console.Write ("y");
}

// Output: prints "y" 1000 times, followed by "Thread t has ended!" immediately afterward (với mọi lần chạy)
```

# Thread.Sleep
* -> **`pauses the current thread for a specified period`**

* -> **Thread.Sleep(0)** relinquishes the thread's current time slice immediately, **`voluntarily handing over the CPU to other threads`**
* -> Framework 4.0's new **Thread.Yield()** method does the same thing — except that it **`relinquishes only to threads running on the same processor`**
* => "Sleep(0)" or "Yield" is occasionally useful in **production code for `advanced performance tweaks`**
* => it’s also an excellent **diagnostic tool for helping to `uncover thread safety issues`**: if inserting Thread.Yield() anywhere in our code makes or breaks the program, we almost **`certainly have a bug`**

```cs
Thread.Sleep (TimeSpan.FromHours (1));  // sleep for 1 hour
Thread.Sleep (500);                     // sleep for 500 milliseconds
```