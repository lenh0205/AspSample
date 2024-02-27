
# Get "MIME" type
* -> GetMimeMapping() method takes a file name or a file path as an argument
* -> depend on the file extension passed returns the corresponding MIME type

```c#
string fileName = "example.pdf";
string mimeType = MimeMapping.GetMimeMapping(fileName);

Console.WriteLine($"The MIME type for file '{fileName}' is '{mimeType}'.");
// The MIME type for file 'example.pdf' is 'application/pdf'.
```