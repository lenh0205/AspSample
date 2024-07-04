
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