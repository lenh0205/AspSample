
# Exception Handling
* -> **any `try/catch/finally blocks` in scope when `a thread is created`** are of **`no relevance to the thread when it starts executing`**
* -> we need **`an exception handler on all thread entry methods in production applications`** — usually at a **higher level**, in the **execution stack**

* => **an unhandled exception** causes the **`whole application to shut down`**, with an ugly dialog!
* => there are, however, some cases where we **don't need to handle exceptions on a worker thread**, because **`the .NET Framework does it for us`**

```cs - the newly created thread will be encumbered with an unhandled NullReferenceException
// the try/catch statement in this example is ineffective

public static void Main()
{
  try
  {
    new Thread (Go).Start();
  }
  catch (Exception ex)
  {
    // We'll never get here!
    Console.WriteLine ("Exception!");
  }
}
 
static void Go() { throw null; }   // Throws a NullReferenceException
```

```cs - remedy
public static void Main()
{
   new Thread (Go).Start();
}

//  move the exception handler into the Go method
static void Go()
{
  try
  {
    // ...
    throw null;    // The NullReferenceException will get caught below
    // ...
  }
  catch (Exception ex)
  {
    // Typically log the exception, and/or signal another thread
    // that we've come unstuck
    // ...
  }
}
```

## works to handle
* -> in writing such exception handling blocks, **rarely would we ignore the error**: 
* -> typically, we'd **`log the details of the exception`**, and then perhaps **`display a dialog allowing the user to automatically submit those details to our web server`**
* -> we then might **`shut down the application`** — because it’s possible that **the error corrupted the program's state**
* -> however, the **cost of doing so** is that **`the user will lose his recent work`** — open documents, for instance

## Global Exception Handling
* -> **the `global` exception handling events for `WPF` and `Windows Forms` applications** (_Application.DispatcherUnhandledException and Application.ThreadException_) 
* -> fire only for **`exceptions thrown on the main UI thread`**
* => we still **`must handle exceptions on worker threads manually`**

* -> **AppDomain.CurrentDomain.UnhandledException** fires on any **unhandled exception**, but provides **`no means of preventing the application from shutting down afterward`**