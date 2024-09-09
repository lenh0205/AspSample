>https://www.reddit.com/r/csharp/comments/znij41/streams_and_buffer_size/
https://stackoverflow.com/questions/43935608/difference-between-buffer-stream-in-c-sharp
https://learn.microsoft.com/en-us/dotnet/api/system.io.bufferedstream?view=net-8.0
https://www.infoworld.com/article/2337595/how-to-use-bufferedstream-and-memorystream-in-c-sharp.html
https://stackoverflow.com/questions/63637874/does-converting-between-byte-and-memorystream-cause-overhead
===================================================================
# "input stream" and "output stream"
* -> streams for **`writing only`** are typically called **output streams**
* -> streams for **`reading only`** are called **input streams**

```cs
```

# To Read and Write with 'FileStream'

```cs
// for read file at a specific path:
var fs = New FileStream("File Path", FileMode.Open);

// for write file at a specific path:
FileStream fs = new FileStream(strFilePath, FileMode.Create);
// another:
FileStream fs = File.Create(strFilePath); 
// <=> new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.None);

// copy the files from one directory to another directory
string StartDirectory = @"c:\Users\exampleuser\start";
string EndDirectory = @"c:\Users\exampleuser\end";

foreach (string filename in Directory.EnumerateFiles(StartDirectory))
{
    using (FileStream SourceStream = File.Open(filename, FileMode.Open))
    {
        using (FileStream DestinationStream = File.Create(EndDirectory + filename.Substring(filename.LastIndexOf('\\'))))
        {
            await SourceStream.CopyToAsync(DestinationStream);
        }
    }
}
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
// when passing a string as "file path", the "StreamWriter" will try to create a "FileStream" of it
// if the file is not exist yet it'll create the file; if path to directory is not exist, it throw Exception  
using (StreamWriter writer = new StreamWriter("example.txt"))
{
    writer.Write(text);
}
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

# Convert between Stream
* -> we generally read data from one stream and write it to another - **`.CopyTo()`**

```cs - Reading from the "FileStream" and writing to a "MemoryStream"
string filePath = "example.txt";

// writing some data to a file
using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
{
    using (StreamWriter writer = new StreamWriter(fileStream))
    {
        writer.Write("Hello, World!");
    }
}

using (FileStream fileStream = new FileStream("/filePath", FileMode.Open, FileAccess.Read))
{
    using (MemoryStream memoryStream = new MemoryStream())
    {
        fileStream.CopyTo(memoryStream);

        // read from the MemoryStream
        memoryStream.Position = 0; // Reset position to the beginning
        using (StreamReader reader = new StreamReader(memoryStream))
        {
            string content = reader.ReadToEnd();
            Console.WriteLine("Content read from MemoryStream: " + content);
        }
    }
}
```

## Typical operations on a stream

```cs
byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

using (MemoryStream memoryStream = new MemoryStream(data))
{
    // 1. Read one byte - Next time we read, we'll get the next byte, and so on.
    int byteRead = memoryStream.ReadByte();
    Console.WriteLine($"Read one byte: {byteRead}");

    // 2. Read several bytes into an array
    byte[] buffer = new byte[5]; // Array to hold bytes
    int bytesRead = memoryStream.Read(buffer, 0, buffer.Length);
    Console.WriteLine($"Read {bytesRead} bytes: {string.Join(", ", buffer)}");

    // 3. Seek to a new position
    // move our current position in the stream, so that next time we read we get bytes from the new position
    memoryStream.Seek(0, SeekOrigin.Begin); // Go back to the beginning
    memoryStream.Seek(3, SeekOrigin.Begin); // Move to the 4th byte
    Console.WriteLine($"Current position after seek: {memoryStream.Position}");

    // 4. Write one byte
    memoryStream.Seek(0, SeekOrigin.End); // Move to the end to write
    memoryStream.WriteByte(11);
    Console.WriteLine("Wrote one byte: 11");

    // 5. Write several bytes from an array into the stream
    byte[] newData = { 12, 13, 14 };
    memoryStream.Write(newData, 0, newData.Length);
    Console.WriteLine("Wrote several bytes: " + string.Join(", ", newData));

    // 6. Skip bytes (using Seek) from the stream (this is like read, but you ignore the data. Or if you prefer it's like seek but can only go forwards.)
    memoryStream.Seek(3, SeekOrigin.Begin); // Move to the 4th byte
    memoryStream.Seek(2, SeekOrigin.Current); // Skip 2 bytes
    Console.WriteLine($"Current position after skipping: {memoryStream.Position}");

    // 7. Peek (using a custom method) (look at bytes without reading them, so that they're still there in the stream to be read later)
    memoryStream.Seek(0, SeekOrigin.Begin); // Reset position to start
    byte[] peekBuffer = new byte[3];
    memoryStream.Read(peekBuffer, 0, peekBuffer.Length);
    memoryStream.Seek(0, SeekOrigin.Begin); // Reset position again
    Console.WriteLine($"Peeked bytes: {string.Join(", ", peekBuffer)}");

    // 8. Push back bytes (manually)
    // into an input stream (this is like "undo" for read - you shove a few bytes back up the stream, so that next time you read that's what you'll see. It's occasionally useful for parsers, as is:
    // Push back is fairly rare, but you can always add it to a stream by wrapping the real input stream in another input stream that holds an internal buffer. Reads come from the buffer, and if you push back then data is placed in the buffer. If there's nothing in the buffer then the push back stream reads from the real stream. This is a simple example of a "stream adaptor": it sits on the "end" of an input stream, it is an input stream itself, and it does something extra that the original stream didn't.
    memoryStream.Seek(0, SeekOrigin.Begin);
    byte[] pushBackBytes = { 99, 98, 97 }; // Bytes to push back
    memoryStream.Seek(0, SeekOrigin.Begin);
    memoryStream.Write(pushBackBytes, 0, pushBackBytes.Length); // Write them at the start
    memoryStream.Seek(0, SeekOrigin.Begin); // Reset position
    byte[] pushedBackBuffer = new byte[3];
    memoryStream.Read(pushedBackBuffer, 0, pushedBackBuffer.Length);
    Console.WriteLine($"Pushed back bytes: {string.Join(", ", pushedBackBuffer)}");
}
```