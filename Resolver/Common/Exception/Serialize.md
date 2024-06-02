https://stackoverflow.com/questions/2200241/in-c-sharp-how-do-i-define-my-own-exceptions
https://www.hanselman.com/blog/good-exception-management-rules-of-thumb
https://asp-blogs.azurewebsites.net/erobillard/129134
https://www.tutorialsteacher.com/csharp/custom-exception-csharp
https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling


=================================================================
# Make Exception Classes Serializable

## Reason
* -> it's really hard to debug a SerializationExceptions because an instance of a **`poorly written exception class`** crossed an AppDomain boundary
* _there're some exception class that's `impossible to serialize`, but this usually because of the `lazyness on the developer's part`_

## Note 
* -> `Exceptions` are **not serializable by default**; it is our responsability to ensure our **`custom exception class is serializable`**
* -> _but marking an exception class with [Serializable] is not enough_, **System.Exception** implements **`ISerializable`**, so it forces us to do so as well

## ToDo
* _**for most exception class** - since most don't actually add **`new properties`** and **`simply rely on the message`**; this is enough:_
* -> mark the exception type with the **[Serializable] attribute**
* -> add an **empty protected serialization constructor** that simply delegates to the base class:
```cs
protected MyException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
```

* _however, if our custom **exception class has custom properties**_
* -> it **MUST override ISerializable.GetObjectData()** (_and don't forget to call the base one_)
* -> and we **MUST unpack those properties** in our **`serialization constructor`** 

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


