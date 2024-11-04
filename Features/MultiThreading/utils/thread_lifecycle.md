==============================================================================
# start a thread
* -> **a C# client program** (Console, WPF, or Windows Forms) starts in **a single thread created automatically by the `CLR` and `operating system`** (the **`main thread`**); and is made multithreaded by creating additional threads

* -> threads are created using **the Thread class’s constructor**, passing in **`a ThreadStart delegate`** which indicates where execution should begin
```cs
public delegate void ThreadStart();
```

```cs - use 2 thread to run code simultaneously - one generate "x", one generate "y"
class ThreadTest
{
  static void Main()
  {
    // "Thread" constructor receive a "ThreadStart" delegate to create a new thread
    Thread t = new Thread (WriteY);  // hoặc Thread t = new Thread (new ThreadStart(WriteY)); nếu muốn rõ ràng
    t.Start(); // running WriteY()
 
    // Simultaneously, do something on the main thread
    for (int i = 0; i < 1000; i++) Console.Write ("x");
  }
 
  static void WriteY()
  {
    for (int i = 0; i < 1000; i++) Console.Write ("y");
  }
}

// Output:
// xxxxxxxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
// xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxyyyyyyyyyyyyy
// yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyxxxxxxxxxxxxxxxxxxxxxx
// xxxxxxxxxxxxxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
// yyyyyyyyyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx...
```

## Passing Data to a Thread
* -> the easiest way to **pass arguments to a thread's target method** is to **`execute a lambda expression that calls the method with the desired arguments`**

```cs
static void Main()
{
  // using "ThreadStart" delegate
  Thread t = new Thread (() => Print ("Hello from t!") );
  t.Start();
}
 
static void Print (string message) 
{
  Console.WriteLine (message);
}

// hoặc using "public delegate void ParameterizedThreadStart (object obj);"
// but the 'ParameterizedThreadStart' can accepts only one argument   
Thread t = new Thread (Print);
t.Start ("Hello from t!");
```

```cs
// wrap the entire implementation in a multi-statement lambda
new Thread (() =>
{
  Console.WriteLine ("I'm running on another thread!");
  Console.WriteLine ("This is so easy!");
}).Start();

// c# 2.0
new Thread (delegate()
{
  // ...
}).Start();
```

## problem with 'captured variables' and solution
* -> we must be careful about **`accidentally modifying captured variables`** after **starting the thread**, because these variables are shared

* -> the solution is to use **`a temporary variable`**

```cs
for (int i = 0; i < 10; i++) {
  new Thread (() => Console.Write (i)).Start();
}

// Output: 0223557799 
// -> the problem is that the i variable refers to the same memory location throughout the loop’s lifetime
```

```cs - solution
for (int i = 0; i < 10; i++)
{
  int temp = i; // the variable is now local to each loop iteration
  new Thread (() => Console.Write (temp)).Start(); // each thread captures a different memory location 
}
```

==============================================================================
# thread alives
* -> once started, a thread's **`IsAlive`** property returns **true**, until the point where the thread ends

# thread ends
* -> **`a thread ends`** when the **delegate** passed to the Thread's constructor **finishes executing**
* -> once ended, **`a thread cannot restart`**

