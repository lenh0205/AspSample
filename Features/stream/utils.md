
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
