
# Memory Stack sharing 
* -> the CLR assigns **`each thread its own memory stack`** so that **`local variables` are kept separate**
* -> threads **`share data`** if they have **`a common reference to the same object instance`**

* _như VD bên dưới, với trường hợp share data giữa 2 thread thì kết quả là không đảm bảo_
* => this is the lack of **`thread-safety`**
* -> **Shared data** is the **`primary cause` of complexity and obscure errors in multithreading**; although often essential, it pays to keep it as simple as possible

```cs - method with "local" variable
static void Main() 
{
  new Thread (Go).Start();      // Call Go() on a new thread
  Go();                         // Call Go() on the main thread
}
 
static void Go()
{
  // Declare and use a local variable - 'cycles'
  for (int cycles = 0; cycles < 5; cycles++) Console.Write ('?');
}

// Output: ??????????
// => 10 "?" - chứng tỏ sự tăng lên của biến "cycles" là độc lập trong từng thread 
```

```cs - method share data
class ThreadTest
{
  bool done;
 
  static void Main()
  {
    ThreadTest tt = new ThreadTest();   // Create a new instance to run both 2 threads

    new Thread (tt.Go).Start();
    tt.Go();
  }
 
  // Note that Go is now an instance method
  void Go() 
  {
     if (!done) { Console.WriteLine ("Done"); done = true; }
  }
}
// hoặc:
class ThreadTest 
{
  static bool done;    // Static fields are shared between all threads
 
  static void Main()
  {
    new Thread (Go).Start();
    Go();
  }
 
  static void Go()
  {
    if (!done) { Console.WriteLine ("Done"); done = true; }
  }
}

// Output: với mỗi lần chạy progam, có thể "Done" in ra 1 lần nhưng cũng có lúc in ra 2 lần (không bảo đảm được)
// -> the problem is that one thread can be evaluating the if statement right as the other thread is executing the WriteLine statement — before it’s had a chance to set "done" to true
```

# Exclusive lock
* -> the remedy is to obtain **an exclusive lock** while reading and writing to the common field - C# provides the **`lock statement`** for just this purpose
* -> when **two threads simultaneously contend a lock**, **`one thread waits / blocks`**, until the lock becomes available; in this case, it ensures **`only one thread can enter the critical section of code at a time`**
* -> **a thread, while blocked**, doesn't consume **`CPU resources`**

* => code that's protected in such a manner — from indeterminacy in a multithreading context — is called **`thread-safe`**

```cs
class ThreadSafe 
{
  static bool done;
  static readonly object locker = new object();
 
  static void Main()
  {
    new Thread (Go).Start();
    Go();
  }
 
  static void Go()
  {
    lock (locker)
    {
      if (!done) { Console.WriteLine ("Done"); done = true; }
    }
  }
}
```