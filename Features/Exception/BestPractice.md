
# Best practices in handling exceptions
* -> an exception is **a runtime error** occur when the **`execution of a program is disrupted`**
* -> **`violates a system or application constraint`**, or **`a condition that is not expected to occur`** during the normal execution of the program

=========================================================
# Khi nào nên ném 'exception' ?

## throw exception
* -> if our functions are named well, using verbs (actions) and nouns (stuff to take action on) then **`throw an exception if our method can't do what it says it can`**
* _Ex: SaveBook() - if it can't do what it promised save the book, then throw an exception_
* -> it should also **`throw if you supplied it with crap input`**

## Common failure conditions should be handled without throwing exceptions
* -> for **`conditions that are likely to occur and trigger an exception`**, consider **handling them in a way that will avoid the exception**
* _nếu ta ném những ngoài lệ này thường xuyên thì nó đã thành `ordinaries` thay vì `exception`_
* _nếu ta có thể làm gì với nó (VD: validate) thì nó không nên để nó rơi vào `exception`, vậy nên đừng catch nó trong khi ta không thể xử lý nó_

* _some **`common example:`**_
* -> **close the connection only** after checking if it's **`not already closed`**
* -> before **`opening a file`**, **check if it exists** using the **`File.Exists(path)`** method
* -> **use fail-safe methods** like - **`CreateTableIfNotExists`** while dealing with **`databases and tables`**
* -> before **dividing**, **`ensure the divisor is not 0`**
* -> **check null** before **`assigning value inside a null object`**
* -> while dealing with **`parsing`**, consider using the **TryParse** methods

```cs
public void ProcessData(List<string> data)
{
    if (data == null) throw new ArgumentNullException(nameof(data));
    if (data.Count == 0) throw new ArgumentException("Data cannot be empty", nameof(data));

    // Proceed with processing
}
```

## Throw exceptions instead of returning an "error code"
* sometimes **`instead of throwing exceptions`** developers **`return error codes to the calling function`**
* -> this might lead to **`exceptions going unnoticed`** as **the calling functions might not always check for the return code (might be an error codes)**

```cs
// Bad code
public int Method2()
{
    try 
    {
        // Code that might throw exception
    }
    catch (Exception) 
    {
        LogError(ex);
        // this is bad approach as the caller function  might miss to check the error code.
        return -1; // error code
    }
}
```

=========================================================
# Định nghĩa 'custom exception' như thế nào ?

## name and inherit in "custom Exception"
* -> when a custom exception is necessary, name it appropriately and **`end the exception name with the word Exception`**
* -> ensure that the **custom exception derives from the "Exception" class** 
* -> and includes **`at least`** the **three common constructors**: **`Parameterless constructor`**, **`Constructor that takes a string message`**, **`Constructor that takes a string message and an inner exception`**

```cs
// -----> Declare
[Serializable] // make sure the class is serializable
public class MyException : Exception
{
    public MyException () {}

    public MyException (string message) : base(message) {}

    public MyException (string message, Exception innerException) : base (message, innerException) {}    
}

// -----> Usage
throw new MyException("My message here!");

// -----> Handle
try {
    // try block
}
catch (Exception ex) {
    if (ex is MyException) { 
        string message = ex.InnerException != null ? ex.InnerException?.Message : ex.Message;
    }
}
// or:
try {
    // try block
}
catch (MyException ex) { // only fall into this catch block in this case
    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
}
catch (Exception ex) {
    // do something
}
```

## Use grammatically correct and meaningful "error messages"
* -> ensure that the error messages are **`clear and end with a period`**, it should not be abrupt and open-ended
* => clear and meaningful messages give the developer a good idea of **`what the issue could have been while trying to replicate and fix the issue`**

```cs
// Bad code
catch (FileNotFoundException ex)
{
    this.logger.LogError(ex, "Something went wrong");
}

// Good code
catch (FileNotFoundException ex)
{
    this.logger.LogError(ex, "Could not find the requested file.");
}
```

=========================================================
# Ném 'exception' như thế nào ?

## Rethrowing Exceptions with "throw" instead of "throw ex"
* -> using **`throw ex`** inside a catch block **doesn't preserve the origin stack trace** and **the original offender would be lost**, and also good bits of **context** 
* -> instead use **`throw`** as it **preserves the stack track** and **the original offender would be available**

```cs
// Bad code
catch(Exception ex)
{
    throw ex; // Stack trace will show only this line as the original offender
}

// Good code
catch(Exception ex)
{
    throw; // The original offender would be rightly pointed (both origin and this line)
}
```

## Use the predefined exception types
* -> there are many exceptions already predefined in .NET and they are **sufficient in most of the cases**
* -> hence introduce **`a new custom exception class`** only when **a predefined one doesn't apply** 
* -> or we would like to **have some additional business analytics** **`based on some custom exception`** (_Ex: a custom TransferFundsException keep track of fund transfer exceptions_)

* _some of them being:_
* -> **DivideByZeroException** is thrown if **`the divisor is 0`**
* -> **ArgumentException**, **ArgumentNullException**, or **ArgumentOutOfRangeException** is thrown if invalid parameters are passed.
* -> **InvalidOperationException** is thrown if **`a method call`** or **`property set`** is **`not appropriate in the object's current state`**
* -> **FileNotFoundException** is thrown if **`the file is not present`**
* -> **IndexOutOfRangeException** is thrown if **`the item being accessed from an array/collection is outside its bounds`**

## "ApplicationException" should never be thrown from code
* -> in the **`initial design of .NET`**, it was planned that the **framework will throw 'SystemException'** while **user applications will throw 'ApplicationException'**
* -> however, **`a lot of exception classes didn't follow this pattern`** and **the ApplicationException class lost all the meaning**, and in practice, this found to add no significant value

* -> hence it's advised that we **should not throw an ApplicationException** from our code and we also **should not catch an ApplicationException** too 
* -> unless we intend to **`re-throw the original exception`**
* -> also **`custom exceptions`** **should not be derived from ApplicationException**

=========================================================
# Gặp 'exception' thì xử lý sao ?

## Create a global error handler that logs everything

## Log the 'exception object' instead of 'exception message' while logging exceptions
* -> often developers **`log just the exception message`** to the logging system **`without the exception object`**
* -> however, the **`exception object`** contains **crucial information** such as **exception type**, **stack trace**,... and it's very important to log it

* -> the **`ILogger`** provide extension methods to **`LogError`**, **`LogCritical`**, **`Log`**, ... that **accepts exception object as a parameter**
* -> so use those **`instead of just the message ones`**
```cs
LogCritical(Exception exception, string message, params object[] args)
LogError(Exception exception, string message, params object[] args)
Log(LogLevel logLevel, Exception exception, string message, params object[] args)
```

```cs - Example:
// Bad code
catch (DivideByZeroException ex)
{
    // The exception object ex is not logged, thus crucial info is lost
    // this'll resulting in loss of stack trace
    this.logger.LogError("Divide By Zero Exception occurred");
}

// Good code
catch (DivideByZeroException ex)
{
    this.logger.LogError(ex, "Divide By Zero Exception occurred");
}
```

## Avoid catch block that just rethrows it
* -> don't simply **`catch an exception and just re-throw it`**
* -> if the **`catch block has no other purpose`**, **remove the try-catch block** and **let the calling function catch the exception** and do something with it

```cs
// Good code
public void Method1()
{
    try
    {
        Method2();
    }
    catch (Exception ex) {
        // Code to handle the exception
        // Log the exception
        this.logger.LogError(ex);
        throw;
    }
}

public void Method2()
{
    // Code that will throw exception

    // No try-catch block here  
    // as this method doesn't know how to handle the exception
}
```

## Do not swallow the exception
* -> one of the **`worst things in exception handling`** is **swallowing the exception without doing anything**
* -> if the exception is swallowed without even logging there **`won't be any trace of the issue that occurred`**
* -> if we're **`not sure of what to do`** with the exception, **don't catch it or at least re-throw it**

```cs
// Bad code
try
{
    // Code that will throw exception
}
catch (Exception ex)
{ // don't do anything
}
```

=========================================================
## User experience
* -> a user shouldn't ever see an exception dialog or ASP.NET Yellow Screen of Death, but if they do, **`let them know that we've been notified`**
* -> **{smartassembly}** is an easy way to make this happen; so is **ELMAH for ASP.NET**

* -> the **Response.Redirect** in ASP.NET offers a straightforward way to **`redirect users`**, it does so by **internally triggering an exception**
* -> because it was **`an easy way to stop execution`**; but if we don't like it, call its overload and stop page execution ourself


