# IFormFile
* -> .NET provides an **IFormFile** interface that represents **`transmitted files`** in _an HTTP request_
* -> it's contain properties to present file infomation like **`ContentDisposition`**, **`ContentType`**, **`FileName`**, **`Headers`**, **`Name`**, and **`Length`**
* -> also contains many methods like **`copying the request stream content`**, **`opening the request stream for reading`**, ...

# the socket exhaustion problem
* in real life, it’s **`not a good practice`** to **`create HttpClient on every request`** (https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net)

====================================================
# Send "a multipart" HTTP requests containing "files" using "HttpClient"

## Sending and Receiving "a single file" using "HttpClient"

```js - Client send Request
private static async Task UploadSampleFile()
{
    // create "MultipartFormDataContent" from "StreamContent"
    await using var stream = System.IO.File.OpenRead("./Test.txt");
    using var content = new MultipartFormDataContent
    {
        { new StreamContent(stream), "file", "Test.txt" }
    };

    // create a "request"
    using var request = new HttpRequestMessage(HttpMethod.Post, "file");
    request.Content = content;

    // create "HttpClient" instance to send the "request"
    var client = new HttpClient
    {
        BaseAddress = new("https://localhost:5001")
    };
    await client.SendAsync(request);
}
```

```c# - Server receive Request
// Server
[ApiController]
[Route("file")]
public class FileController : ControllerBase
{
    // "FromForm" expect "a multipart/form-data" content type
    [HttpPost]
    public IActionResult Upload([FromForm] IFormFile file) 
    {
        // .....code responsible for file processing
        return Ok();
    }
}
```


## Sending and Receiving "a file" with "additional data" using "HttpClient"

```c# - Client send request
// Client
private static async Task UploadSampleFile()
{
    await using var stream = System.IO.File.OpenRead("./Test.txt");
    var payload = new
    {
        Name = "payload name",
        Tags = new[] { "tag1", "tag2" },
        ChildData = new
        {
            Description = "test description"
        }
    };
    using var content = new MultipartFormDataContent // fill with "HttpContent" objects
    { 
        { new StreamContent(stream), "FileToUpload1", "Test.txt" }, 

        // defines a single property that will be mapped to "DataDto"
        { new StringContent(payload.Name), "Data.Name" },
        { new StringContent(payload.Tags[0]), "Data.Tags" },
        { new StringContent(payload.Tags[1]), "Data.Tags" },
        { new StringContent(payload.ChildData.Description), "Data.ChildData.Description" }
    };

    using var request = new HttpRequestMessage(HttpMethod.Post, "file");
    request.Content = content;

    var client = new HttpClient
    {
        BaseAddress = new("https://localhost:5001")
    };

    await client.SendAsync(request);
}
```
```r - the raw request:
POST https://localhost:5001/file HTTP/1.1
Host: localhost:5001
traceparent: 00-853f01b7762c9746912e3775865d60ba-c43a730c1a434848-00
Content-Type: multipart/form-data; boundary="949a4299-4662-4775-94e2-de82e76936a5"
Content-Length: 772

--949a4299-4662-4775-94e2-de82e76936a5
Content-Disposition: form-data; name=fileToUpload1; filename=Test.txt; filename*=utf-8''Test.txt


--949a4299-4662-4775-94e2-de82e76936a5
Content-Type: text/plain; charset=utf-8
Content-Disposition: form-data; name=Data.Name

payload name
--949a4299-4662-4775-94e2-de82e76936a5
Content-Type: text/plain; charset=utf-8
Content-Disposition: form-data; name=Data.Tags

tag1
--949a4299-4662-4775-94e2-de82e76936a5
Content-Type: text/plain; charset=utf-8
Content-Disposition: form-data; name=Data.Tags

tag2
--949a4299-4662-4775-94e2-de82e76936a5
Content-Type: text/plain; charset=utf-8
Content-Disposition: form-data; name=Data.ChildData.Description

test description
--949a4299-4662-4775-94e2-de82e76936a5--
```

```c# - Server receive Request
// Server
[ApiController]
[Route("file")]
public class FileController : ControllerBase
{
    [HttpPost]
    public IActionResult Upload([FromForm] FileDataDto fileUploadDto)
    {
        IFormFile file = fileUploadDto.FileToUpload;

        var fileDetails = new FileDetails()
        {
            ID = 0,
            FileName = file.FileName,
        };

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            fileDetails.FileData = stream.ToArray();
        }
    }
}
public class FileDataDto
{
    public IFormFile FileToUpload { get; set; }
    public DataDto Data { get; set; }
}
public class DataDto
{
    public string Name { get; set; }
    public string[] Tags { get; set; }
    public ChildDataDto ChildData { get; set; }
}
public class ChildDataDto
{
    public string Description { get; set; }
}

public class FileDetails
{
    public int ID { get; set; }
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
}
```

### sending "additional data" as JSON
* -> in the case target endpoint might be prepared to accept the application/json content type for additional data
* -> we need to custom model binders that deserializes the JSON content to the target type
* -> this case, the Data property is decorated with the ModelBinder attribute that takes the type of a custom binder

```c# - Server
public class FileDataDto
{
    public IFormFile FileToUpload1 { get; set; }

    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public DataDto Data { get; set; }
}

public class FormDataJsonBinder : IModelBinder
{
    private readonly ILogger<FormDataJsonBinder> _logger;

    public FormDataJsonBinder(ILogger<FormDataJsonBinder> logger)
    {
        _logger = logger;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;

        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        try
        {
            var result = JsonSerializer.Deserialize(value, bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}
```

```c# - Client
private static async Task UploadSampleFile()
{
    var client = new HttpClient
    {
        BaseAddress = new("https://localhost:5001")
    };

    await using var stream = System.IO.File.OpenRead("./Test.txt");

    var payload = new
    {
        Name = "payload name",
        Tags = new[] { "tag1", "tag2" },
        ChildData = new
        {
            Description = "test description"
        }
    };

    using var request = new HttpRequestMessage(HttpMethod.Post, "file");

    using var content = new MultipartFormDataContent
    {
        // file
        { new StreamContent(stream), "FileToUpload1", "Test.txt" },

        // payload
        { new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"),
            "Data" },
    };

    request.Content = content;

    await client.SendAsync(request);
}
```

## Send "Multiple file" with "additional data" using "HttpClient"

```c#
[ApiController]
[Route("file")]
public class FileController : ControllerBase
{
    [HttpPost]
    public IActionResult Upload([FromForm] FileDataDto dto)
    {
        // code responsible for file processing
        return Ok();
    }
}

public class FileDataDto
{
    public List<IFormFile> FilesToUpload { get; set; }

    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public DataDto Data { get; set; }
}

// Client:
using var content = new MultipartFormDataContent
{
    // files - notice the file contents "should have the same name" FilesToUpload
    { new StreamContent(stream), "FilesToUpload", "Test.txt" },
    { new StreamContent(anotherStream), "FilesToUpload", "NewTest.txt" },

    // payload
    ...
};
```

==============================================================
# Return "files"from Web API service - System.Web.Http

## Step:

### Convert the Required "file" into "Bytes"
* -> cannot send the file from its original state
* -> to convert the file into **bytes** using **`File`** Class 

```c#
//converting Pdf file into bytes array
var dataBytes = File.ReadAllBytes(reqBook);
```

### Adding "bytes" to "MemoryStream"
* -> create an **`instance`** of **MemoryStream** by passing the **`byte form`** of the **`file`**
```c#
//adding bytes to memory stream
var dataStream = new MemoryStream(dataBytes);
```

### Adding "MemoryStream" object to the "HttpResponseMessage" Content
* -> add the **`MemoryStream object`** to the **HttpResponseMessage Content** 
* -> make the **`client understand the content of the service`** by **HttpResponseMessage header properties**: **`ContentDisposition`**, **`FileName`**, **`ContentType`**

* the **`Content Type`** is **application/octet-stream** (or **`application/pdf`**)
* => in a simple way, it means when we are **`sending a file to the REST service`** and we **`don’t know the file type`** but it is **a trusted one**

```c#
httpResponseMessage.Content = new StreamContent(bookStuff);

httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

httpResponseMessage.Content.Headers.ContentDisposition.FileName = PdfFileName;

httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
```

## Implement

```c# - Client send "file type" to get a suitable "file" from server
// Client:
// http://localhost:59217/api/Ebook/GetBookFor?format=pdf
// http://localhost:59217/api/Ebook/GetBookFor?format=doc
// http://localhost:59217/api/Ebook/GetBookFor?format=xls
// http://localhost:59217/api/Ebook/GetBookFor?format=zip


// Server:
// use both "IHttpActionResult" and "HttpResponseMessage" to return file
public class EbookController : ApiController
{
    string bookPath_Pdf = @"C:\MyWorkSpace\SelfDev\UserAPI\UserAPI\Books\sample.pdf";
    string bookPath_xls = @"C:\MyWorkSpace\SelfDev\UserAPI\UserAPI\Books\sample.xls";
    string bookPath_doc = @"C:\MyWorkSpace\SelfDev\UserAPI\UserAPI\Books\sample.doc";
    string bookPath_zip = @"C:\MyWorkSpace\SelfDev\UserAPI\UserAPI\Books\sample.zip";

    [HttpGet]
    [Route("Ebook/GetBookFor/{format}")]
    public IHttpActionResult GetbookFor(string format)
    {
        string reqBook = format.ToLower() == "pdf" ? bookPath_Pdf : 
        (
            format.ToLower() == "xls" ? bookPath_xls : 
            (
                format.ToLower() == "doc" ? bookPath_doc : bookPath_zip
            )
        );
        
        var dataBytes = File.ReadAllBytes(reqBook); //converting Pdf file into bytes array
        var dataStream = new MemoryStream(dataBytes); //adding bytes to memory stream

        string bookName = "sample." + format.ToLower();
        return new eBookResult(dataStream, Request, bookName);
    }

    [HttpGet]
    [Route("Ebook/GetBookForHRM/{format}")]
    public HttpResponseMessage GetBookForHRM(string format)
    {
        string reqBook = format.ToLower() == "pdf" ? bookPath_Pdf : 
            (format.ToLower() == "xls" ? bookPath_xls : 
                (format.ToLower() == "doc" ? bookPath_doc : bookPath_zip));

        var dataBytes = File.ReadAllBytes(reqBook);
        var dataStream = new MemoryStream(dataBytes);

        string bookName = "sample." + format.ToLower();

        HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
        httpResponseMessage.Content = new StreamContent(dataStream);
        httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        httpResponseMessage.Content.Headers.ContentDisposition.FileName = bookName;
        httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        return httpResponseMessage;
    }
}

public class eBookResult : IHttpActionResult
{
    MemoryStream bookStuff;
    string PdfFileName;
    HttpRequestMessage httpRequestMessage;
    HttpResponseMessage httpResponseMessage;

    public eBookResult(MemoryStream data, HttpRequestMessage request, string filename)
    {
        bookStuff = data;
        httpRequestMessage = request;
        PdfFileName = filename;
    }
    
    public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
    {
        httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
        httpResponseMessage.Content = new StreamContent(bookStuff);
        //httpResponseMessage.Content = new ByteArrayContent(bookStuff.ToArray());
        httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        httpResponseMessage.Content.Headers.ContentDisposition.FileName = PdfFileName;
        httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
    }
}
```


============================================================
# Return file in "ASP.Net Core" Web API

```c# - using "File" of "IActionResult"
[Route("api/[controller]")]
public class DownloadController : Controller {
    //GET api/download/12345abc
    [HttpGet("{id}")]
    public async Task<IActionResult> Download(string id) {
        Stream stream = await {{__get_stream_based_on_id_here__}}

        if(stream == null)
            return NotFound(); // returns a NotFoundResult with Status404NotFound response.

        return File(stream, "application/octet-stream", "{{filename.ext}}"); // returns a FileStreamResult
    }    
}
// The framework will dispose of the stream used in this case when the response is completed. If a using statement is used, the stream will be disposed before the response has been sent and result in an exception or corrupt response
```

```c# - using "FileStreamResult" of "IActionResult"
[HttpGet("get-file-stream/{id}")]
public async Task<FileStreamResult> DownloadAsync(string id)
{
    var fileName="myfileName.txt";
    var mimeType="application/...."; 
    Stream stream = await GetFileStreamById(id);

    return new FileStreamResult(stream, mimeType)
    {
        FileDownloadName = fileName
    };
}
```

```c# - using "FileContentResult" of "IActionResult"
[HttpGet("get-file-content/{id}")]
public async Task<FileContentResult> DownloadAsync(string id)
{
    var fileName="myfileName.txt";
    var mimeType="application/...."; 
    byte[] fileBytes = await GetFileBytesById(id);

    return new FileContentResult(fileBytes, mimeType)
    {
        FileDownloadName = fileName
    };
}
```

============================================================
# Download Files in Server

```c# - get File from DB then download it on Server Files System
public async Task DownloadFileById(int Id)
{
    try
    {
        var file =  dbContextClass.FileDetails.Where(x => x.ID == Id).FirstOrDefaultAsync();

        var content = new System.IO.MemoryStream(file.Result.FileData);
        var path = Path.Combine(
            Directory.GetCurrentDirectory(), "FileDownloaded",
            file.Result.FileName);

        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream);
        }
    }
    catch (Exception)
    {
        throw;
    }
}
```