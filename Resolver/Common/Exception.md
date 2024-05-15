=================================================================
https://stackoverflow.com/questions/2200241/in-c-sharp-how-do-i-define-my-own-exceptions
https://winterdom.com/2007/01/16/makeexceptionclassesserializable
https://asp-blogs.azurewebsites.net/erobillard/129134
https://www.tutorialsteacher.com/csharp/custom-exception-csharp

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

