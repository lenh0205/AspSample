
# "var" keyword
```cs
// -> compiler infers the type of the variable from its initialization expression
var test = 10; // "test" integer type
test = test + 10; // No error
test = "hello"; // Compile time error as test is an integer type
```

# "Boxing" vs "UnBoxing"
## Boxing 
```cs
// -> process of converting a "value type" to the "type object" or to any interface type implemented by this value type. 
// -> When CLR boxes a value type, it wraps the value inside a "System.Object" instance and stores it on the managed heap. 
// -> Boxing is implicit
```
## Unboxing
```cs
// -> extracts the value type from the object
// -> Unboxing is explicit
```

# "object" type
* Supports **`all classes in the .NET class hierarchy`** and provides **`low-level services`** to derived classes. 
* -> This is the ultimate base class of all .NET classes; 
* -> it is the root of the type hierarchy
```cs
// -> All types in C# inherit directly or indirectly from "object" type (alias for System.Object);
// => include reference types and value types; reference types and value types 
// =>  can assign values of any type to variables of type "object"
// ->  require boxing and unboxing for conversion (it makes it slow)
// -> the C# compiler checks types during compile-time
// -> has only a few operation/members (Equals, GetHashCode, ToString) and doesn’t provide much flexibility.
object test = "hello"; // Boxing no error
test = 10; // Boxing no error
test = test + 10; // Compile time error; "object" not contain "+" operation
var myInt = (int) test // Unboxing
```

# "dynamic" type
```cs
// -> functions like "object" type in most cases
// -> compiler assumes a "dynamic" element supports any operation (whether the object gets its value from a COM API, from a dynamic language such as IronPython, from the HTML Document Object Model (DOM), from reflection, or from somewhere else in the program,...)
// -> is a static type, but an object of type dynamic bypasses static type checking
// -> not require boxing and unboxing
// -> checks types during runtime (All errors on dynamic variables are discovered at runtime only)
dynamic dyn = "Hello, World!";
int len = dyn.Length; // No error at compile time, runtime  
dyn = 123;
int len2 = dyn.Length; // not an error at compile time, but runtime error 
```

# "DataTable" type
```cs
// Để Tranfer data có dạng "DataTable" trong "request-response" ta sẽ cần Serialize nó
// -> nếu Client-Server:
var dt = new DataTable();
var dtForTransfer = JsonConvert.SerializeObject(dt);
var result = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dt); // nếu ta cần trả về 1 Array gồm nhiều object
// -> nếu Server-Server:
return new {Dt = JsonConvert.SerializeObject(dt), DtReplace = JsonConvert.SerializeObject(dtreplace)}; // response
var resultDataApi = commonApi.CallAPI_common(url, "POST", requestData);
var dataTableApi = JsonConvert.DeserializeObject<JObject>(resultDataApi.DataResult.ToString());
var dt = JsonConvert.DeserializeObject<DataTable>(dataTableApi["dt"].ToString());
var dtReplace = JsonConvert.DeserializeObject<DataTable>(dataTableApi["dtReplace"].ToString());

// ta cũng có thể viết 1 hàm để convert từ "DataTable" thành "Dictionary" :
public List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
{
    var rows = new List<Dictionary<string, object>>();
    Dictionary<string, object> row;
    foreach (DataRow dr in dt.Rows)
    {
        row = new Dictionary<string, object>();
        foreach (DataColumn col in dt.Columns)
        {
            row.Add(col.ColumnName, dr[col]);
        }
        rows.Add(row);
    }
    return rows;
}
```

# "JObject" type
* Đại điện cho 1 đối tượng Json

=================================================
> _System.IO_

# "MarshalByRefObject" type
* Enables access to **`objects across application domain boundaries`** in **`applications that support remoting`**
* inherit **object**

# "Stream" abstract class
* Provides **`a generic view of a sequence of bytes`**
* -> inherit **MarshalByRefObject**
* -> implement **IDisposable**, **IAsyncDisposable**

# "FileStream" class
* Provides **a Stream** for **a file**, supporting both synchronous and asynchronous read and write operations
* inherit **Stream** abstract class

# "FileSystemInfo" type
* Provides the **`base class`** for both **FileInfo** and **DirectoryInfo** objects
* inherit **MarshalByRefObject** class

# "DirectoryInfo" type
* Exposes **`instance methods`** for **`creating, moving, and enumerating`** through **`directories and subdirectories`** 
* inherit **FileSystemInfo** class

* **Directory** type:
* -> Exposes **`static methods`** for `creating, moving, and enumerating` through `directories and subdirectories`
* -> inherit **object** class

* **`Những method thường s/d`**`:
```cs
DirectoryInfo di = new DirectoryInfo(@"c:\MyDir"); // Init
if (!di.Exists) di.Create(); // Kiêm tra directory tồn tại chưa; nếu chưa thì tạo
// hoặc:
if (Directory.Exists(di.FullName) == false) Directory.CreateDirectory(di.FullName);
di.Delete(); // Delete the directory

//  Copy each file from "source" DirectoryInfo into "target" DirectoryInfo:
foreach (FileInfo fi in source.GetFiles())
{
    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
}

// Copy each subdirectory from "source" directory into "target" directory:
foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
{
    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name); // create empty subDirec
    CopyAll(diSourceSubDir, nextTargetSubDir); // using recursion to copy
}
```

================================================
> _Microsoft.CodeAnalysis_

# "TextDocument" type

# "Document" type
* Represents a **source code document** that is **`part of a project`**; **`provides access`** to the `source text, parsed syntax tree and the corresponding semantic model`

================================================
> _Microsoft.Office.Interop.Excel_

_mỗi Workbook là một tập tin / 1 file Excel; có phần mở rộng là .xlsx hoặc xlsm_
_Workbook: Tập hợp chứa nhiều Worksheet, giống như 1 quyển sách và Worksheet là trang sách_

# "_Application" Interface
* **a primary interface in a COM coclass** that is required by **`managed code`** for **`interoperability`** with the **`corresponding COM object`**

# "_Workbook" Interface
* **a primary interface in a COM coclass** that is required by `managed code` for `interoperability` with the `corresponding COM object`

# "WorkbookEvents_Event" Interface
* **`Events interface`** for **ExcelWorkbook** **`object events`**

# "Workbook" interface
* -> Represents **`a Microsoft Excel workbook`**
* -> properties for **returning a Workbook object**: Workbooks, ActiveWorkbook, ThisWorkbook
* -> Implement **_Workbook**, **WorkbookEvents_Event** interface

* -> `a .NET interface` derived from **`a COM coclass`** that is required by **`managed code`** for **`interoperability`** with the **`corresponding COM object`**
* -> Use this derived interface to **`access all method, property, and event members`** of the **COM object**

# "Workbooks" interface
* -> **A collection of all the Workbook objects** that are **`currently open`** in the **`Microsoft Excel application`**
* -> Use the **`Workbooks property`** of **_Application** interface to **return the Workbooks collection**
* -> Implements **IEnumerable** interface

# "WorkbookClass" Class
* -> inherit **object** class
* -> implement **Workbook** interface

===================================================
> _System.Web.UI_

# "Control" type
* -> Defines the **`properties, methods, and events`** that are shared by all **`ASP.NET server controls`**
* -> inherit **object** class

# ScriptManager
* Manages **ASP.NET Ajax script libraries and script files, partial-page rendering, and client proxy class generation** for Web and application services
* inherit **Control** class

## "ScriptManager.RegisterStartupScript" Method
* Registers **`a startup script block`** with the **`ScriptManager control`** and **adds the script block to the page**
* _`Script block` là một đoạn mã JavaScript; thêm vào trang bằng cách sử dụng thẻ script_

* _RegisterStartupScript(`Control`, Type, String, String, Boolean)_ -> registers _a startup script block_ for a control that is inside an **UpdatePanel** by using the `ScriptManager` control
* _RegisterStartupScript(`Page`, Type, String, String, Boolean)_ -> registers _a startup script block_ for **`every asynchronous postback`** with the `ScriptManager` control