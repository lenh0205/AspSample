
# Get Physical Path:
```cs
var pathToSaveSatus = HttpContext.Current.Request.PhysicalApplicationPath + "DesktopModules\\status\\";

// hoặc:
// ta có thể truyền 1 trong 2 kiểu "relative path" hoặc "virtual path" 
string subPath = HttpContext.Current.Server.MapPath("~/Uploads/KySo/"); 
```

# Get Virtual Path:
```cs
var authority = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
var resolvedUrl = VirtualPathUtility.ToAbsolute("~/DesktopModules/ViF/QLVB/QLVB/pages/ClientUpload.aspx");
var urlUpload1 = authority + resolvedUrl;
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