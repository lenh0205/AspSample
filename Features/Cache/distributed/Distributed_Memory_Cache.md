# Distributed Memory Cache
* -> _the Distributed Memory Cache (**`AddDistributedMemoryCache`**)_ is a **framework-provided implementation** of **`IDistributedCache`** that stores items in memory

* -> **`isn't an actual distributed cache`** - cached items are **stored by the app instance** on the server where the app is running
* -> lý do là ta dùng nó là vì it allows for **`implementing a true distributed caching solution in the future`** if multiple nodes or fault tolerance become necessary

* -> vậy nên thường thì ta sẽ dùng nó in **`development`** and **`testing`** scenarios
* -> và nếu là in production, thì khi có **`a single server`** is used and **`memory consumption isn't an issue`**

```cs - program.cs
builder.Services.AddDistributedMemoryCache();
```