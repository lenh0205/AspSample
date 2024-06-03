# AggregateException
* -> when **working with tasks and parallel operations**, **`multiple exceptions can be thrown`** 
* -> **`AggregateException`** is used to **bundle these into a single exception**

```cs
try
{
    Task.WaitAll(task1, task2, task3);
}
catch (AggregateException ex)
{
    foreach (var innerEx in ex.InnerExceptions)
    {
        LogError(innerEx);
    }
}
```

# Conditional Catch Blocks - "when" keyword
```cs
try
{
    // Operation that might throw exceptions
}
catch (CustomException ex) when (ex.ErrorCode == 404)
{
    // Handle not found errors specifically
}
catch (Exception ex) when (ex is IOException)
{
    // General error handling
}
```

# Utilizing "ExceptionDispatchInfo"
* -> allows for **capturing an exception and throwing it from another context*** while **`preserving the stack trace`**
* -> particularly **`useful in asynchronous programming`** where **exceptions need to be rethrown on a different thread**

```cs
ExceptionDispatchInfo capturedException = null;

try
{
    // Async operation that might fail
}
catch (Exception ex)
{
    capturedException = ExceptionDispatchInfo.Capture(ex);
}

if (capturedException != null)
{
    capturedException.Throw(); // Rethrows with the original stack trace intact
}
```

# Leveraging 'Task.Exception' for "Asynchronous Error Handling"
* -> when **`working with asynchronous tasks`**, **exceptions are encapsulated in the task itself**
* -> **`accessing the Exception property`** allows for handling these errors gracefully
```cs
var task = Task.Run(() => { throw new CustomException("Error"); });

try
{
    await task;
}
catch (CustomException ex)
{
    // Handle specific custom exceptions
}
```