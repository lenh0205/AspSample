
# Naming Thread
* -> each thread has a **`Name` property** that we can set for the benefit of **`debugging`**
* -> we can **`set a thread's name just once`**; attempts to **change it later will throw an exception**
* -> the static **`Thread.CurrentThread`** property will gives us **the currently executing thread**

* _this is particularly useful in Visual Studio, since the thread's name is displayed in the Threads Window and Debug Location toolbar_

```cs - set the main thread's name
class ThreadNaming
{
  static void Main()
  {
    Thread.CurrentThread.Name = "main";
    Thread worker = new Thread (Go);
    worker.Name = "worker";
    worker.Start();
    Go();
  }
 
  static void Go()
  {
    Console.WriteLine ("Hello from " + Thread.CurrentThread.Name);
  }
}
```