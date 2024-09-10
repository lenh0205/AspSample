# System.IO
* -> contains types that **`allow reading and writing`** to **`files and data streams, and types`** that provide basic file and directory support.

=======================================================
# Stream
* -> **a stream** is **`an abstraction`** of **`a sequence of bytes`** - such as _a file, an input/output device, an inter-process communication pipe, or a TCP/IP socket_
* -> the **Stream class** and its **derived classes** provide **`a generic view`** of **`these different types of input and output`**
* -> also help **`isolate`** the **`programmer`** from the **`specific details`** of the _operating system_ and the _underlying devices_

* _về cơ bản,  streams are utilized in method interfaces to indicate that `all data does not need to be stored in memory at once`_
```r
//  For instance, when calculating the checksum for all bytes in a file, there is no need to load all the data into memory before the calculation can begin.
```

========================================================
## Fundamental operations of streams
* _depending on the underlying data source or repository, streams might support only some of these capabilities_
* -> we can query a stream for its capabilities by properties of Stream class: **`CanRead`**, **`CanWrite`**, **`CanSeek`**
* -> some asynchronous operations like **ReadAsync**, **WriteAsync**, **CopyToAsync**, **FlushAsync** 
* => perform resource-intensive I/O operations **`without blocking the main thread`** (_can block the `UI thread`_)

* **read from streams** 
* -> reading is the **`transfer of data`** **`from a stream into a data structure`**, such as an array of bytes
* -> the **`Read`** and **`Write`** methods read and write data in _a variety of formats_

* **write to streams** 
* -> writing is the **`transfer of data`** **`from a data structure into a stream`**
* -> the **`Read`** and **`Write`** methods read and write data in _a variety of formats_

* **Seeking** 
* -> refers to **`querying and modifying`** the **`current position`** within a stream (_cho phép  thay đổi vị trí con trỏ đọc dữ liệu vào giữa luồng để đọc ở 1 khoảng nhất định_)
* -> **`Seek capability`** depends on the **`kind of backing store`** a stream has
* -> For example, network streams have no unified concept of a current position, and therefore typically **`do not support seeking`**
* -> **`Seek`** + **`SetLength`** methods + **`Position`** property + **`Length`** property = query and modify the current position and length of a stream

```c# - VD:
// Tạo một stream và lưu vào đó một số byte
var stream = new System.IO.MemoryStream();
for (int i = 0; i < 122; i++)
{
    stream.WriteByte((byte)i);
}
stream.Position = 0; // Thiết lập vị trí là điểm bắt đầu

// Phương thức "Read()"
// -> đọc 1 số lượng byte nhất định từ 1 vị trí nhất định
// -> kết quả đọc lưu vào mảng byte được chỉ định; trả về số lượng byte đọc được (trả về 0 nếu đọc hết stream)
byte[] buffer = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // tạo 1 mảng byte trong memory
int numberbyte = stream.Read(buffer, 0, 2); // bắt đầu dọc

while (numberbyte != 0) // lặp đến khi đọc hết stream
{
    Console.WriteLine(" ----->");
    Console.WriteLine($"the number of bytes got read from stream is {numberbyte}, include:");
    for (int i = 0; i < numberbyte; i++)
    {
        byte b = buffer[i]; // lấy ra 1 byte
        Console.Write(string.Format("{0, 5}", b));
    }
    Console.WriteLine();
    numberbyte = stream.Read(buffer, 0, 5); 
}
```

===========================================================
## Dispose "stream"
* the **`Stream class`** implements the **IDisposable** interface
* -> when we have **`finished using the type`**, we should **`dispose`** of it either directly or indirectly
* -> to **dispose of the type directly**, call its **`Dispose`** method in **`a try/catch block`**
* -> to **dispose of it indirectly**, use **`using`** keyword

* **`Disposing a Stream object`** **flushes any buffered data**
* -> essentially calls the **Flush** method 

* **`Dispose`** also **releases operating system resources** such as file handles, network connections, or memory used for any internal buffering. 

## type of Stream
* the **`most used streams`** that inherit from Stream are **FileStream**, and **MemoryStream**
* The **BufferedStream class** provides the capability of **`wrapping a buffered stream`** around another stream in order to **`improve read and write performance`**
* Ngoài ra, còn có **NetworkStream**, **PipeStream**, **CryptoStream**

## Implement "stream"
* _convert Between .NET Framework Streams and Windows Runtime Streams_
* -> 2 extension methods **AsInputStream** and **AsOutputStream** of **`Stream`** convert **`a Stream object to a stream`** in the **`Windows Runtime`** 
* -> and **AsStreamForRead** and **AsStreamForWrite** methods to **`convert a stream in the Windows Runtime to a Stream object`** 

* _some stream implementations **perform local buffering** of the underlying data to improve performance_
* -> we can use the **Flush** or **FlushAsync** method to **`clear any internal buffers`** and ensure that **`all data has been written`** to the underlying data source or repository.

* if we need **`a stream with no backing store`** (**a bit bucket**), use the **`Null`** field to retrieve an instance of a stream that is **`designed for this purpose`**

======================================================
# Stream Readers and Writers

## StreamReader: 
* -> **a helper class** for **reading characters from a Stream**
* -> by **`converting bytes into characters`** using an **`encoded value`** 
* -> can be used to **`read strings`** (characters) **`from different Streams`** like _FileStream, MemoryStream, etc_

## StreamWriter: 
* -> **a helper class** for **writing a string to a Stream**
* -> by **`converting characters into bytes`**
* -> can be used to **`write strings to different Streams`** such as _FileStream, MemoryStream, etc_

## BinaryReader: 
* -> **a helper class** for **reading primitive datatype from bytes**

## BinaryWriter: 
* -> **writes primitive types in binary**