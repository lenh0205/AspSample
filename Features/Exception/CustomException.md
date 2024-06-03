https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0
https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-8.0

=================================================================
# Custom Exception 
* -> usually, we often like to raise an exception when the **`business rule of our application gets violated`**; for this, we can can create a custom exception class

=================================================================
# Inherit: "System.Exception" or "System.ApplicationException"
https://asp-blogs.azurewebsites.net/erobillard/129134s

* -> The .Net framework includes ApplicationException class since .Net v1.0. It was designed to use as a base class for the custom exception class. However, Microsoft now recommends Exception class to create a custom exception class. You should not throw an ApplicationException exception in your code, and you should not catch an ApplicationException exception unless you intend to re-throw the original exception.

=================================================================
# Make Exception Classes Serializable

## Reason
* -> it's really hard to debug a SerializationExceptions because an instance of a **`poorly written exception class`** **crossed an AppDomain boundary**
* _tức là ta cần khiến custom Exception serializable nếu ta muốn `throw nó accross AppDomain`_
* _there're some exception class that's `impossible to serialize`, but this usually because of the `lazyness on the developer's part`_

## Knowlegde 
* -> `Exceptions` are **not serializable by default**; it is our responsability to ensure our **`custom exception class is serializable`**
* -> _but marking an exception class with [Serializable] is not enough_, **System.Exception** implements **`ISerializable`**, so it forces us to do so as well

## Serialize is necessary
* -> not just about **remoting** only
* -> but also, the underlying **remoting and serialization mechanisms in .NET** (_not means for remote object invokation_) are **`the basis`**
* -> for all **cross AppDomain calls**, even when they are **`inside the same process boundary`**

* -> more and more we see **`applications and environments`** where **multiple AppDomains** are the norm
* -> _rather than the on AppDomain: VSTO, MMC 3.0, BizTalk, ASP.NET and everything in between (including anything with COM interop in it)_
* -> and this will grow in the future because **`using multiple AppDomains`** is the preffered way of **hosting plugins safely**
* -> and even applications will support extensibility in this fashion in the future

* => in each of those cases there's the possibility that our **exception might cross an AppDomain boundary**; so we must **`make sure it does so gracefully`**
* _if we're a library writer (open source or commercial), must be aware that the chance that a `user might use our library in one of those contexts` is always there_

==========================================================
## Mark

### For most exception class
* _since most don't actually add **`new properties`** and **`simply rely on the message`**; this is enough:_
* -> mark the exception type with the **[Serializable] attribute**
* -> add an **empty protected serialization constructor** that simply delegates to the base class:
```cs
protected MyException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
```

### For "custom exception" class has "custom properties"
* -> it **MUST override ISerializable.GetObjectData()** (_and don't forget to call the base one_)
* -> and we **MUST unpack those properties** in our **`serialization constructor`** 

==========================================================
# Implementation in .NET

## Base implementation - without "custom properties"
* -> **Important: the [Serializable] attribute is NOT inherited from Exception** and **MUST be specified** 
* -> otherwise **`serialization will fail with a SerializationException`** stating that _"Type X in Assembly Y is not marked as serializable."_
```cs
[Serializable] // Important: This attribute is NOT inherited from Exception, and MUST be specified 
public class SerializableExceptionWithoutCustomProperties : Exception
{
    // 3 common "standard" constructor - make a good starting point for cut-and-paste 
    public SerializableExceptionWithoutCustomProperties()
    {
    }

    public SerializableExceptionWithoutCustomProperties(string message) 
        : base(message)
    {
    }

    public SerializableExceptionWithoutCustomProperties(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected SerializableExceptionWithoutCustomProperties(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}
```

## Full implementation - with "custom properties"
* _include `a complete implementation of custom serializable exception` and `a derived sealed exception`_

### must decorate each derived class with the [Serializable] attribute
* -> this attribute **`will not automatically inherited from the base class`**
* -> and **`if it is not specified`**, **serialization will fail with a SerializationException** stating that _"Type X in Assembly Y is not marked as serializable."_

### must implement custom serialization
* -> **`the [Serializable] attribute alone is not enough`**
* -> **Exception implements ISerializable** which means our **`derived classes must also implement custom serialization`** (_this involves two steps_)
* -> first, **provide a serialization constructor** - this constructor should be **private** if our class is **sealed**, otherwise it should be **protected** to **`allow access to derived classes`**
* -> second, **override GetObjectData()** and make sure we **`call through to base.GetObjectData(info, context) at the end`**, in order to **let the base class save its own state**

```cs - SerializableExceptionWithCustomProperties.cs:
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

[Serializable] // Important: This attribute is NOT inherited from Exception, and MUST be specified 
public class SerializableExceptionWithCustomProperties : Exception
{
    private readonly string resourceName;
    private readonly IList<string> validationErrors;

    public SerializableExceptionWithCustomProperties()
    {
    }

    public SerializableExceptionWithCustomProperties(string message) 
        : base(message)
    {
    }

    public SerializableExceptionWithCustomProperties(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public SerializableExceptionWithCustomProperties(string message, string resourceName, IList<string> validationErrors)
        : base(message)
    {
        this.resourceName = resourceName;
        this.validationErrors = validationErrors;
    }

    public SerializableExceptionWithCustomProperties(string message, string resourceName, IList<string> validationErrors, Exception innerException)
        : base(message, innerException)
    {
        this.resourceName = resourceName;
        this.validationErrors = validationErrors;
    }

    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    // Constructor should be protected for unsealed classes, private for sealed classes.
    // (The Serializer invokes this constructor through reflection, so it can be private)
    protected SerializableExceptionWithCustomProperties(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        this.resourceName = info.GetString("ResourceName");
        this.validationErrors = (IList<string>)info.GetValue("ValidationErrors", typeof(IList<string>));
    }

    public string ResourceName
    {
        get { return this.resourceName; }
    }

    public IList<string> ValidationErrors
    {
        get { return this.validationErrors; }
    }

    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException("info");
        }

        info.AddValue("ResourceName", this.ResourceName);

        // Note: if "List<T>" isn't serializable you may need to work out another
        //       method of adding your list, this is just for show...
        info.AddValue("ValidationErrors", this.ValidationErrors, typeof(IList<string>));

        // MUST call through to the base class to let it save its own state
        base.GetObjectData(info, context);
    }
}
```

```cs - DerivedSerializableExceptionWithAdditionalCustomProperties.cs:
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

[Serializable]
public sealed class DerivedSerializableExceptionWithAdditionalCustomProperty : SerializableExceptionWithCustomProperties
{
    private readonly string username;

    public DerivedSerializableExceptionWithAdditionalCustomProperty()
    {
    }

    public DerivedSerializableExceptionWithAdditionalCustomProperty(string message)
        : base(message)
    {
    }

    public DerivedSerializableExceptionWithAdditionalCustomProperty(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public DerivedSerializableExceptionWithAdditionalCustomProperty(string message, string username, string resourceName, IList<string> validationErrors) 
        : base(message, resourceName, validationErrors)
    {
        this.username = username;
    }

    public DerivedSerializableExceptionWithAdditionalCustomProperty(string message, string username, string resourceName, IList<string> validationErrors, Exception innerException) 
        : base(message, resourceName, validationErrors, innerException)
    {
        this.username = username;
    }

    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    // Serialization constructor is private, as this class is sealed
    private DerivedSerializableExceptionWithAdditionalCustomProperty(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        this.username = info.GetString("Username");
    }

    public string Username
    {
        get { return this.username; }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException("info");
        }
        info.AddValue("Username", this.username);
        base.GetObjectData(info, context);
    }
}
```

## Unit Test
```cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UnitTests
{
    private const string Message = "The widget has unavoidably blooped out.";
    private const string ResourceName = "Resource-A";
    private const string ValidationError1 = "You forgot to set the whizz bang flag.";
    private const string ValidationError2 = "Wally cannot operate in zero gravity.";
    private readonly List<string> validationErrors = new List<string>();
    private const string Username = "Barry";

    public UnitTests()
    {
        validationErrors.Add(ValidationError1);
        validationErrors.Add(ValidationError2);
    }

    [TestMethod]
    public void TestSerializableExceptionWithoutCustomProperties()
    {
        Exception ex =
            new SerializableExceptionWithoutCustomProperties(
                "Message", new Exception("Inner exception."));

        // Save the full ToString() value, including the exception message and stack trace.
        string exceptionToString = ex.ToString();

        // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            // "Save" object state
            bf.Serialize(ms, ex);

            // Re-use the same stream for de-serialization
            ms.Seek(0, 0);

            // Replace the original exception with de-serialized one
            ex = (SerializableExceptionWithoutCustomProperties)bf.Deserialize(ms);
        }

        // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
        Assert.AreEqual(exceptionToString, ex.ToString(), "ex.ToString()");
    }

    [TestMethod]
    public void TestSerializableExceptionWithCustomProperties()
    {
        SerializableExceptionWithCustomProperties ex = 
            new SerializableExceptionWithCustomProperties(Message, ResourceName, validationErrors);

        // Sanity check: Make sure custom properties are set before serialization
        Assert.AreEqual(Message, ex.Message, "Message");
        Assert.AreEqual(ResourceName, ex.ResourceName, "ex.ResourceName");
        Assert.AreEqual(2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count");
        Assert.AreEqual(ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]");
        Assert.AreEqual(ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]");

        // Save the full ToString() value, including the exception message and stack trace.
        string exceptionToString = ex.ToString();

        // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            // "Save" object state
            bf.Serialize(ms, ex);

            // Re-use the same stream for de-serialization
            ms.Seek(0, 0);

            // Replace the original exception with de-serialized one
            ex = (SerializableExceptionWithCustomProperties)bf.Deserialize(ms);
        }

        // Make sure custom properties are preserved after serialization
        Assert.AreEqual(Message, ex.Message, "Message");
        Assert.AreEqual(ResourceName, ex.ResourceName, "ex.ResourceName");
        Assert.AreEqual(2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count");
        Assert.AreEqual(ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]");
        Assert.AreEqual(ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]");

        // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
        Assert.AreEqual(exceptionToString, ex.ToString(), "ex.ToString()");
    }

    [TestMethod]
    public void TestDerivedSerializableExceptionWithAdditionalCustomProperty()
    {
        DerivedSerializableExceptionWithAdditionalCustomProperty ex = 
            new DerivedSerializableExceptionWithAdditionalCustomProperty(Message, Username, ResourceName, validationErrors);

        // Sanity check: Make sure custom properties are set before serialization
        Assert.AreEqual(Message, ex.Message, "Message");
        Assert.AreEqual(ResourceName, ex.ResourceName, "ex.ResourceName");
        Assert.AreEqual(2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count");
        Assert.AreEqual(ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]");
        Assert.AreEqual(ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]");
        Assert.AreEqual(Username, ex.Username);

        // Save the full ToString() value, including the exception message and stack trace.
        string exceptionToString = ex.ToString();

        // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            // "Save" object state
            bf.Serialize(ms, ex);

            // Re-use the same stream for de-serialization
            ms.Seek(0, 0);

            // Replace the original exception with de-serialized one
            ex = (DerivedSerializableExceptionWithAdditionalCustomProperty)bf.Deserialize(ms);
        }

        // Make sure custom properties are preserved after serialization
        Assert.AreEqual(Message, ex.Message, "Message");
        Assert.AreEqual(ResourceName, ex.ResourceName, "ex.ResourceName");
        Assert.AreEqual(2, ex.ValidationErrors.Count, "ex.ValidationErrors.Count");
        Assert.AreEqual(ValidationError1, ex.ValidationErrors[0], "ex.ValidationErrors[0]");
        Assert.AreEqual(ValidationError2, ex.ValidationErrors[1], "ex.ValidationErrors[1]");
        Assert.AreEqual(Username, ex.Username);

        // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
        Assert.AreEqual(exceptionToString, ex.ToString(), "ex.ToString()");
    }
}
```

==========================================================
# ASP.NET Core Web API exception handling
https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling