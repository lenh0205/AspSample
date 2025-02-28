================================================================================
## Dispose
* ->

## 'Using' vs 'Dispose'

### Example: Class Without IDisposable That Leaks "File Handle" Resources
* -> When FileWriter is no longer in use, the file is still locked.
* -> GC won't release the file handle immediately, causing potential issues.
* -> If the program crashes, the file remains locked until the OS reclaims the handle
*  _ta cần lưu ý **File Hanlde Resource** của ta là "FileWriter" chỉ khởi tạo instance của FileStream một lần và sử dụng cho những lần sau đó, vậy nên nếu bất cẩn Dispose() nó ngay khi sử dụng thì nếu gọi Write() method lần nữa thì nó sẽ throw Exception ngay_
*  _Ngoài ra, it doesn't follow **Standard Resource Management**:The caller of FileWriter has no control over when the file is disposed; Normally, a class that uses a resource should own the responsibility of disposing it at the right time, not inside an unrelated method._
```cs
public class FileWriter
{
    private FileStream _fileStream;

    public FileWriter(string path)
    {
        _fileStream = new FileStream(path, FileMode.Create);
    }

    public void Write(string content)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
        _fileStream.Write(data, 0, data.Length);
    }
}
```

* -> Fix: Implement IDisposable to ensure proper cleanup
```cs
public class FileWriter : IDisposable
{
    private FileStream _fileStream;

    public FileWriter(string path)
    {
        _fileStream = new FileStream(path, FileMode.Create);
    }

    public void Write(string content)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
        _fileStream.Write(data, 0, data.Length);
    }

    public void Dispose()
    {
        _fileStream?.Dispose(); // Releases the file handle
        GC.SuppressFinalize(this);
    }
}
```

================================================================================
# Finalize

## Unmanage Resource
* -> Unmanaged resources are resources that are not directly managed by the .NET runtime (CLR); these include things that interact with the operating system, such as:
* File handles (e.g., working with raw file descriptors)
* Database connections (e.g., using native database drivers)
* Network sockets (e.g., raw TCP/UDP connections)
* Native memory allocations (e.g., allocated using Marshal.AllocHGlobal())
* COM objects (e.g., using Microsoft Office Interop)

## Why Doesn't .NET Manage These Automatically?
* .NET automatically manages memory for objects created in managed heap (like string, List<T>, etc.).
* However, unmanaged resources come from outside the .NET runtime (e.g., OS handles, external DLLs).
* Garbage Collector (GC) only cleans up managed objects, so unmanaged resources must be released manually.

## Finalize vs Dispose
* https://www.scholarhat.com/tutorial/net/difference-between-finalize-and-dispose-method
* https://codetosolutions.com/blog/36/difference-between-finally,-finalize-and-dispose-in-c%23
* https://learn.microsoft.com/en-us/dotnet/api/system.object.finalize?view=net-9.0
* https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers
* https://stackoverflow.com/questions/46691339/why-net-object-has-method-finalize
* https://www.bytehide.com/blog/finalize-objects-csharp
* https://dev.to/bytehide/dispose-or-finalize-in-c-discover-which-to-use-150h
* https://pvs-studio.com/en/blog/posts/csharp/0437/

## Finalize best practices
Is It Necessary?
🔹 Yes, if the class directly owns unmanaged memory (e.g., allocated via Marshal.AllocHGlobal()).
🔹 No, if the class only contains managed objects that implement IDisposable (e.g., a FileStream or SqlConnection).

Why?
If the developer forgets to call Dispose(), the finalizer ensures that unmanaged memory is eventually freed.
However, finalizers are expensive because they make objects survive longer in memory (GC needs two cycles to reclaim them).
When to Use a Finalizer
✅ Use it when dealing with unmanaged memory or OS handles (IntPtr, native allocations, etc.).
❌ Avoid it for managed objects (like FileStream or SqlConnection) since their own Dispose() method is enough.


## Dispose pattern
* -> is **`a standard way of implementing IDisposable in C# to ensure proper resource cleanup, especially for 'unmanaged resources'`**
* -> involves: implementing **`IDisposable.Dispose()`**, providing a **`Dispose(bool disposing)`** method; optionally using a **`finalizer (~ClassName())`** to clean up unmanaged resources in case Dispose() was not called
* _a finalizer is only necessary if your class directly handles unmanaged resources_
* _if your class only holds managed objects that implement IDisposable, you don't need a finalizer because you can rely on those objects’ Dispose() methods_
* _the finalizer is a backup mechanism in case the developer forgets to call Dispose()_

```cs
// Example:
public class MyResource : IDisposable
{
    private bool _disposed = false;

    // Example of an unmanaged resource (e.g., a file handle)
    private IntPtr _unmanagedResource;

    public MyResource()
    {
        // Allocate unmanaged resource (for example)
        _unmanagedResource = SomeNativeMethodToAllocateResource();
    }

    // Dispose method to clean up resources
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Prevents finalizer call if Dispose() is already called
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Free managed resources here
            }

            // Free unmanaged resources here
            if (_unmanagedResource != IntPtr.Zero)
            {
                SomeNativeMethodToFreeResource(_unmanagedResource);
                _unmanagedResource = IntPtr.Zero;
            }

            _disposed = true;
        }
    }

    // Finalizer ('destructor' of "MyResource" class) to clean up unmanaged resources (only if Dispose() was not called)
    ~MyResource()
    {
        Dispose(false);
    }
}
```

## Example:

* -> **`File Handle Cleanup (FileStream)`** - Files should be closed to avoid file lock issues
```cs
public class FileHandler : IDisposable
{
    private FileStream _fileStream;
    
    public FileHandler(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
    }

    public void Dispose()
    {
        _fileStream?.Dispose(); // Release file handle
        GC.SuppressFinalize(this);
    }

    ~FileHandler()
    {
        Dispose();
    }
}
```

* -> **`Database Connection Cleanup (SqlConnection)`** - Leaving database connections open can exhaust connection pools.
```cs
public class DatabaseHandler : IDisposable
{
    private SqlConnection _connection;

    public DatabaseHandler(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
    }

    public void Dispose()
    {
        _connection?.Dispose(); // Close connection
        GC.SuppressFinalize(this);
    }

    ~DatabaseHandler()
    {
        Dispose();
    }
}
```

* -> **`Socket Cleanup (Socket)`** - Open sockets should be closed to free network resources
```cs
public class SocketHandler : IDisposable
{
    private Socket _socket;

    public SocketHandler()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void Dispose()
    {
        _socket?.Close();
        _socket?.Dispose();
        GC.SuppressFinalize(this);
    }

    ~SocketHandler()
    {
        Dispose();
    }
}
```

* -> **`Native API Calls (Using IntPtr)`** - External libraries (like C++ DLLs) may allocate memory that needs to be explicitly freed
```cs
public class NativeResourceHandler : IDisposable
{
    private IntPtr _nativeResource;

    public NativeResourceHandler()
    {
        _nativeResource = AllocateNativeResource(); // Assume some external allocation
    }

    public void Dispose()
    {
        ReleaseNativeResource(_nativeResource);
        GC.SuppressFinalize(this);
    }

    ~NativeResourceHandler()
    {
        Dispose();
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr AllocateNativeResource();

    [DllImport("kernel32.dll")]
    private static extern void ReleaseNativeResource(IntPtr resource);
}
```

* -> **`COM Object Cleanup (Marshal.ReleaseComObject)`** - COM objects need explicit cleanup to prevent memory leaks
```cs
public class ComObjectHandler : IDisposable
{
    private dynamic _comObject;

    public ComObjectHandler()
    {
        _comObject = new SomeCOMClass(); // Assume COM object usage
    }

    public void Dispose()
    {
        if (_comObject != null)
        {
            Marshal.ReleaseComObject(_comObject);
            _comObject = null;
        }
        GC.SuppressFinalize(this);
    }

    ~ComObjectHandler()
    {
        Dispose();
    }
}
```
