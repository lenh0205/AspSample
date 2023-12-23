# In ASP.NET 
* by Default, all the file in ASP.NET project was accessible through the browser by the right path
* -> less secure - malicious client can access application code files, which can contain sensitive information

=============================================
# In ASP.NET Core
* ASP.NET Core application **`doesn't serve static files By Default`**
* -> use **.UseStaticFiles() Middleware** to enable ASP.NET Core application to server static files
* -> put _`all static files, subdirectories`_ in **webroot directory** (in APS.NET Core is **`www.root`** folder)

* => server static by access **`URL contain the file path`**
```cs
// VD:  http://localhost:44307/lee.png  <=>  wwwroot/lee.png 
```

## use other folder for "webroot directory" instead of "wwwroot" folder
```cs
var builder = WebApplication.CreateBuilder(new WebApplicationOptions() 
{
    // VD ta chọn "staticfiles" folder làm webroot:
    WebRootPath = "staticfiles" // as Default Webroot Directory (instead of "wwwroot")
});
var app = builder.Build();
app.UseStaticFiles();

// config to add multiple "webroot" folder:
app.UseStaticFiles(new StaticFilesOptions() 
{
    // thêm "mywebroot" folder as "webroot":
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Enviroment.ContentRootPath, "mywebroot")); 
    // <=> C:\aspnetcore\ServerStaticFiles\ServerStaticFiles\ + mywebroot
    // "ContentRootPath" is basically our project folder (folder chứa file .csproj)
});


// H khi ta try cập http://localhost:44307/sample.txt 
// -> đầu tiên nó sẽ tìm trong Default webroot Directory Directory ("staticfiles" folder)
// -> if not found, look for next webroot folder ("mywebroot" folder)
```
* if having _multiple-webroot directories_ in project, one of them will act as a **`default webroot folder`**

