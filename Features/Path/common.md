
# Get Physical Path:
```cs
var pathToSaveSatus = HttpContext.Current.Request.PhysicalApplicationPath + "DesktopModules\\status\\";

// hoặc:
// ta có thể truyền 1 trong 2 kiểu "relative path" hoặc "virtual path" 
string subPath = HttpContext.Current.Server.MapPath("~/Uploads/KySo/"); 
```

# Get Virtual Path:
```cs
// .NET Framework
var urlUpload = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
    + VirtualPathUtility.ToAbsolute("~/DesktopModules/ViF/QLVB/QLVB/pages/ClientUpload.aspx");

// .NET Core
public MyConstructor(IWebHostEnvironment environment)
{
    _environment = environment;
}
public IActionResult MyAction()
{
    var path = Path.Combine(_environment.WebRootPath, "DesktopModules", "VIF", "QLVB", "UPLOADS");
    var url = Url.Content("~/DesktopModules/VIF/QLVB/UPLOADS");
}
```

# Get "file name" without extension (.NET Framework)
```cs
var myPath = "Template\\Word\\thisIsAFile.xls";
var fileName1 = Path.GetFileNameWithoutExtension(myPath); // Output: thisIsAFile

var fileName2 = Path.GetFileName(myPath); // Output: thisIsAFile.xls
```

# Get "file extension" (.NET Framework)
```cs
var ext = Path.GetExtension(myPath); // Output: .xls
```

# Get path to "directory" contain the file (.NET Framework)
```cs
File.Exists(pathToFile); // Output: true
Path.GetDirectoryName(pathToFile); // Output: Template/Word

// cần check xem nó có phải là file trước, vì thằng "Path.GetDirectoryName" nó chỉ đơn giản là bỏ đi phần cuối thôi không quan tâm là nó là Directory hay File
```

# Get supported physical directory path for saving temporary file:
* -> the GetTempPath function checks for the existence of environment variables in the following order and uses the first path found: The path specified by the TMP environment variable, the path specified by the TEMP environment variable, the path specified by the USERPROFILE environment variable.
The Windows directory.
* -> **IMPORTANT NOTE**: it doesn't check whether or not the path actually exists or can be written to, so you may end up trying to write your log files to a path that doesn't exist, or one that you cannot access.

```cs
Path.GetTempPath() // typically return 
```