
===================================================================
# Read/Write with 'FileStream'

```cs
// for read file at a specific path:
var fs = New FileStream("File Path", FileMode.Open);

// for write file at a specific path:
FileStream fs = new FileStream(strFilePath, FileMode.Create);

FileStream fs = File.Create(strFilePath); 
// <=> new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.None);
```

```cs
var outStream = new MemoryStream();
//outStream.Flush();
//outStream.Close();
//outStream.Dispose();
//outStream.EndRead();
//outStream.Read();
//outStream.Write();
//outStream.WriteTo();
```

# 'using' block
* -> as soon as we leave the using block’s scope, the stream is **`disposed`**
* -> the **`Stream.Dispose()`** always call the **`Stream.Close()`** 
* -> the **`Stream.Close()`** alwasy call the **`Stream.Flush`**
* -> **`Flush`** a stream **takes any buffered data which hasn't been written yet** and **writes it out right away**
* _some streams **`use buffering internally`** to avoid making a ton of small updates to relatively **expensive resources like a disk file or a network pipe**_

* => we need to call either **Close** or **Dispose** on most streams, because the **`underlying resource won't be freed for someone else to use`** until the garbage collector comes (who knows how long that'll take.) 
* -> Dispose is preferred as a matter of course; it's expected that we'll dispose all disposable things in C#
* -> we probably don't have to **call 'Flush' explicitly** in most scenarios

```cs
using (FileStream stream = new FileStream(path))
{
}

// hoặc
FileStream stream;
try
{
    stream = new FileStream(path);
    // ...
}
finally
{
    if (stream != null) stream.Dispose();
}
```

# 'StreamReader - StreamWriter' and 'BinaryWriter - BinaryReader'

## Problem
* -> are `not streams` (_ not derived from System.IO.Stream_)
* -> with **Stream**, the functions for reading and writing are all **`byte orientated`** (_e.g. WriteByte()_)
* -> there are **`no direct functions for work with higher data types like integers, strings, ....`**
* => this makes the stream very general-purpose, but **`less simple to work with`** (_if, say, we just want to **transfer text**_)

## .NET support
* => however, **.NET provides classes** that **`convert between native types and the low-level stream interface`** and **`transfers the data to or from the stream for us`**
* -> _some notable such classes are: **StreamWriter**, **StreamReader**, **BinaryWriter**, **BinaryReader**_
* -> to use it, first we acquire our **stream**, then **`create one of the above classes`** and **`associate it with the stream`**

```cs
MemoryStream memoryStream = new MemoryStream();
StreamWriter myStreamWriter = new StreamWriter(memoryStream);
```

## 'StreamReader' and 'StreamWriter' 
* _are designed to help to **`write and read string/text`** **`from and to stream`**_ 
* -> it convert between **native types** and their **string representations** 
* -> then **transfer the strings to and from the stream as bytes**

* _so if we're dealing with `text files` (e.g. html), StreamReader and StreamWriter are the classes we would use_

```cs
myStreamWriter.Write(123);
// -> will write "123" (three characters '1', '2' then '3') to the stream
```

## 'BinaryWriter' and 'BinaryReader'
* _if we're dealing with `binary files` or `network protocols`, BinaryReader and BinaryWriter are what we might use_

```cs
myBinaryWriter.Write(123);
// -> will write 4 bytes representing the 32-bit integer value 123 (0x7B, 0x00, 0x00, 0x00)
```


```cs - write a string to a stream 
// using raw byte operations 
string text = "Hello, World!";
byte[] bytes = Encoding.UTF8.GetBytes(text);

using (FileStream fileStream = new FileStream("example.txt", FileMode.Create, FileAccess.Write))
{
    fileStream.Write(bytes, 0, bytes.Length);
}

// using the 'StreamWriter' class
string text = "Hello, World!";

using (StreamWriter writer = new StreamWriter("example.txt"))
{
    writer.Write(text);
}
```