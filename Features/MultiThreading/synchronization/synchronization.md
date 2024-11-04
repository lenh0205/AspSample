
# synchronization
* -> **coordinating the actions of threads** for **`a predictable outcome`**
* -> "synchronization" is **particularly important** when **`threads access the same data`**
* _it’s surprisingly easy to run aground in this area_

## 4 categories
* _`synchronization constructs` can be divided into four categories_
* _`Blocking` is essential to all but the last category_

### Simple blocking methods
* -> these **`wait` for `another thread to finish` or for `a period of time to elapse`**
* -> **`Sleep`**, **`Join`**, and **`Task.Wait`** are simple **blocking methods**

### Locking constructs
* -> **`these limit the number of threads`** that can **perform some activity** or **execute a section of code** **`at a time`**
* -> **`exclusive locking constructs`** are most common — these **allow just one thread in at a time**, and **allow competing threads to access common data without interfering with each other**

* -> **the standard exclusive locking constructs** are **`lock`** (_Monitor.Enter/Monitor.Exit_), **`Mutex`**, and **`SpinLock`**
* -> **the nonexclusive locking constructs** are **`Semaphore`**, **`SemaphoreSlim`**, and **`the reader/writer locks`**

### Signaling constructs
* -> these allow **`a thread to pause until receiving a notification from another`**, **avoiding the need for inefficient polling**
* -> there are two commonly used signaling devices: **`event wait handles`** and **`Monitor’s Wait/Pulse methods`**
* -> **Framework 4.0** introduces the **`CountdownEvent`** and **`Barrier`** classes

### Nonblocking synchronization constructs
* -> these **`protect access to a common field`** by calling upon **`processor primitives`**
* -> the **CLR and C#** provide the following **nonblocking constructs**: **`Thread.MemoryBarrier`**, **`Thread.VolatileRead`**, **`Thread.VolatileWrite`**, the **`volatile`** keyword, and the **`Interlocked`** class
