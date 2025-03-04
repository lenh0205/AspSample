
# Response Compression
* -> reduce the **size of responses sent to clients** (có khi cả 10 lần), **`improving performance`** and **`reducing bandwidth usage`**
* -> using the **`Response Compression Middleware`** provided by ASP.NET Core
* -> **`Brotli`** compression is newer and generally offer **better compression ratios** than **`Gzip`**

## Best practice
* -> enable compression **globally**, but **`limit it to specific MIME types (e.g., JSON, HTML)`**
* -> not all endpoints need compression, **`disable compression for specific endpoints`** if they don't benefit from it
* -> note that **`the binary data (e.g., images, PDFs, ZIP files) are already compressed`**, so don't compress it again
* -> **small responses (e.g., a JSON response under 1 KB) compression** adds **`unnecessary CPU processing overhead`** that outweigh the benefit
* -> use it for **`APIs that return large text-based responses`** (e.g., JSON, XML, HTML, CSV) or **`public API with high traffic`** where bandwidth reduction can improve performance
* -> also useful for **reports or logs** that contain a lot of redundant text data

## Step
* -> install required **package**
```bash
$ dotnet add package Microsoft.AspNetCore.ResponseCompression
```

* -> configure response compression in **program.cs**:
```cs
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add response compression services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Enable compression for HTTPS responses

    options.Providers.Add<BrotliCompressionProvider>(); // use Brotli compression - better compression than Gzip
    options.Providers.Add<GzipCompressionProvider>(); // use Gzip compression as fallback

    // By default, only certain MIME types (e.g., text/plain, text/html) are compressed

    // -> we can specify additional MIME types:
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "application/xml" }
    );

    // -> or Limit compression to specific MIME types
    options.MimeTypes = new[] 
    {
        "application/json",
        "application/xml",
        "text/html",
        "text/plain",
        "text/css",
        "application/javascript"
    };
});

// Optional: Configure Brotli compression level
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    // level options: Fastest, Optimal, SmallestSize, NoCompression
    // "default" is 'Fastest'
    // -> Fastest - complete as quickly as possible, even if the resulting output isn't optimally compressed
    // -> Optimal - optimally compressed even if the compression take more time to complete
    options.Level = CompressionLevel.Optimal;
});

// Optional: Configure Gzip compression level (similar to Brotli)
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest; // Other options: Optimal, NoCompression
});

var app = builder.Build();

// Use response compression middleware
app.UseResponseCompression();

app.MapControllers();
app.Run();
```

* -> **Exclude certain endpoints** from Compression
```cs
[HttpGet("uncompressed")]
public IActionResult GetUncompressedData()
{
    Response.Headers.Append("Content-Encoding", "identity"); // Disable compression
    return Ok("This response is not compressed.");
}
```

* -> check if "Response Compression" succeed by using **Postman** to send request
* _ensure **`Content-Encoding: gzip`** or **Content-Encoding: br``** header in the response_
* _if an endpoint returns something like an image (image/png), it should not be compressed_

* -> **Client setup**
* _**`by default, Axios sends an "Accept-Encoding" header`** that allows compressed responses_ 
* _i **`Browsers automatically handle decompression`**; if we use SSR app (like NextJS), we may need to configure Axios to support decompression_
* _data inside compression usually large, so we should **pagination** or **lazy loading** before actually rendering it and **cache response** (Redux, React Query, or local storage) to avoid unnecessary re-fetching_
```js
// however, in some cases (like when using Node.js), we might need to explicitly set 'Accept-Encoding' header
axios.get('/api/data', {
  headers: {
    'Accept-Encoding': 'gzip, deflate, br' // Ensure the client requests compressed responses
  }
})
.then(response => console.log(response.data))
.catch(error => console.error(error));
```

* -> **ASP.NET Core checking**
```bash
# Example the compression support of a client look like this:
Accept-Encoding: br, gzip, deflate
```
* _checks the **"Accept-Encoding" header** from the client_
* _picks the **first compression format** that matches what the server supports_
* _in this case, the client supports Brotli (br), it will be used_
* _however, if in case the client does not support Brotli but supports Gzip (gzip), then Gzip is used instead_
* _and if neither is supported, the response is sent **`uncompressed`**_

