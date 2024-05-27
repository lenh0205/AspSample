https://stackoverflow.com/questions/2200241/in-c-sharp-how-do-i-define-my-own-exceptions
https://winterdom.com/2007/01/16/makeexceptionclassesserializable
https://www.hanselman.com/blog/good-exception-management-rules-of-thumb
https://asp-blogs.azurewebsites.net/erobillard/129134
https://www.tutorialsteacher.com/csharp/custom-exception-csharp
https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling

=================================================================

# Custom Exception
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

=================================================================
# Make Exception Classes Serializable
* -> it's really hard to debug a SerializationExceptions because an instance of a **`poorly written exception class`** crossed an AppDomain boundary
* _there're some exception class that's `impossible to serialize`, but this usually because of the `lazyness on the developer's part`_

## Note - for 
* -> `Exceptions` are **not serializable by default**; it is our responsability to ensure our **`custom exception class is serializable`**
* -> by mark the exception type with the **[Serializable] attribute**

* -> _but marking an exception class with [Serializable] is not enough_, **System.Exception** implements **`ISerializable`**, so it forces us to do so as well
* -> we can add an **empty protected serialization constructor** that simply delegates to the base class:
```cs
protected MyException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
```



=================================================================
```cs - create a managed exception to handle action in ASP.NET Core WebApi
/// <summary>
/// Define
/// </summary>
[Serializable]
public class ManagedException : Exception {

    public ManagedException() {
    }

    public ManagedException(string message) : base(message) {
    }

    public ManagedException(string message, Exception? innerException) : base(message, innerException) {
    }

    protected ManagedException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }

    public static void ThrowIf([DoesNotReturnIf(true)] bool when, string message) {
        if (when) Throw(message);
    }

    public static void ThrowIfFalse([DoesNotReturnIf(false)] bool when, string message) {
        if (!when) Throw(message);
    }

    public static void ThrowIfNull([NotNull] object? obj, string message) {
        if (obj is null) Throw(message);
    }

    public static T ThrowIfNull<T>([NotNull] T? obj, string message) {
        if (obj is null) Throw(message);
        return obj;
    }

    [DoesNotReturn]
    public static void Throw(string message) {
        throw new ManagedException(message);
    }
}
```

