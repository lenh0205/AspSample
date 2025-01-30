
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


# "JObject" type
* Đại điện cho 1 đối tượng Json

=================================================
> `System.Data`

# "DataRelation" class
* represents a **parent/child relationship** between **two DataTable objects**
* inherit **object** class

# "MarshalByValueComponent" class
* -> implements **`IComponent`** and provides the **base implementation** for **remotable components** that are `marshaled by value` (a copy of the serialized object is passed)
* -> inherit **object**

# "DataSet" class
* -> `represents` an **in-memory cache of data**
* -> inherit **MarshalByValueComponent**
* -> implement **IListSource**, **IXmlSerializable**, ...

# "DataView" class
* -> represents a **`databindable`**, **`customized`** **view** of a **DataTable** for **sorting, filtering, searching, editing, and navigation**
* -> The _DataView_ **`does not store data`**, but instead **`represents a connected view of its corresponding DataTable`**
* -> changes to the DataView's data will affect the DataTable. Changes to the DataTable's data will affect all DataViews associated with it
* -> inherit **MarshalByValueComponent**
* -> implement **ICollection**, **IEnumerable**, **IList**, **IBindingList**, **IBindingListView**,...

# "DataTable" type
* `represents` one **table** of **in-memory data**
* inherit **MarshalByValueComponent** class
```cs - create relation between datatables
// Example: creates two "DataTable" objects 
// -> use "DataColumn" to create new column -> add some table constrains -> "table.Columns.Add(column);" to add new column to table
// -> use "table.NewRow();" to create new row -> fill data to each field of row -> "table.Rows.Add(row);" to add new row to table
// -> create primary key column for the table
// Create "new DataSet()" -> Add table to a "DataSet" using "dataSet.Tables.Add(table);"
// creates a new "DataRelation" and adds it to the "DataRelationCollection" of a "DataSet"
// The tables are then displayed in a "DataGridView" control

private System.Data.DataSet dataSet;

private void MakeDataTables()
{
    MakeParentTable();
    MakeChildTable();
    MakeDataRelation();
    BindToDataGrid();
}
private void MakeParentTable()
{
    // Create a new DataTable.
    System.Data.DataTable table = new DataTable("ParentTable");
    // Declare variables for DataColumn and DataRow objects.
    DataColumn column;
    DataRow row;

    // Create new DataColumn, set DataType,
    // ColumnName and add to DataTable.
    column = new DataColumn();
    column.DataType = System.Type.GetType("System.Int32");
    column.ColumnName = "id";
    column.ReadOnly = true;
    column.Unique = true;
    // Add the Column to the DataColumnCollection.
    table.Columns.Add(column);

    // Create second column.
    column = new DataColumn();
    column.DataType = System.Type.GetType("System.String");
    column.ColumnName = "ParentItem";
    column.AutoIncrement = false;
    column.Caption = "ParentItem";
    column.ReadOnly = false;
    column.Unique = false;
    // Add the column to the table.
    table.Columns.Add(column);

    // Make the ID column the primary key column.
    DataColumn[] PrimaryKeyColumns = new DataColumn[1];
    PrimaryKeyColumns[0] = table.Columns["id"];
    table.PrimaryKey = PrimaryKeyColumns;

    // Instantiate the DataSet variable.
    dataSet = new DataSet();
    // Add the new DataTable to the DataSet.
    dataSet.Tables.Add(table);

    // Create three new DataRow objects and add
    // them to the DataTable
    for (int i = 0; i <= 2; i++)
    {
        row = table.NewRow();
        row["id"] = i;
        row["ParentItem"] = "ParentItem " + i;
        table.Rows.Add(row);
    }
}

private void MakeChildTable()
{
    // Create a new DataTable.
    DataTable table = new DataTable("childTable");
    DataColumn column;
    DataRow row;

    // Create first column and add to the DataTable.
    column = new DataColumn();
    column.DataType = System.Type.GetType("System.Int32");
    column.ColumnName = "ChildID";
    column.AutoIncrement = true;
    column.Caption = "ID";
    column.ReadOnly = true;
    column.Unique = true;

    // Add the column to the DataColumnCollection.
    table.Columns.Add(column);

    // Create second column.
    column = new DataColumn();
    column.DataType = System.Type.GetType("System.String");
    column.ColumnName = "ChildItem";
    column.AutoIncrement = false;
    column.Caption = "ChildItem";
    column.ReadOnly = false;
    column.Unique = false;
    table.Columns.Add(column);

    // Create third column.
    column = new DataColumn();
    column.DataType = System.Type.GetType("System.Int32");
    column.ColumnName = "ParentID";
    column.AutoIncrement = false;
    column.Caption = "ParentID";
    column.ReadOnly = false;
    column.Unique = false;
    table.Columns.Add(column);

    dataSet.Tables.Add(table);

    // Create three sets of DataRow objects,
    // five rows each, and add to DataTable.
    for (int i = 0; i <= 4; i++)
    {
        row = table.NewRow();
        row["childID"] = i;
        row["ChildItem"] = "Item " + i;
        row["ParentID"] = 0;
        table.Rows.Add(row);
    }
    for (int i = 0; i <= 4; i++)
    {
        row = table.NewRow();
        row["childID"] = i + 5;
        row["ChildItem"] = "Item " + i;
        row["ParentID"] = 1;
        table.Rows.Add(row);
    }
    for (int i = 0; i <= 4; i++)
    {
        row = table.NewRow();
        row["childID"] = i + 10;
        row["ChildItem"] = "Item " + i;
        row["ParentID"] = 2;
        table.Rows.Add(row);
    }
}

private void MakeDataRelation()
{
    // DataRelation requires two DataColumn
    // (parent and child) and a name.
    DataColumn parentColumn =
        dataSet.Tables["ParentTable"].Columns["id"];
    DataColumn childColumn =
        dataSet.Tables["ChildTable"].Columns["ParentID"];
    DataRelation relation = new
        DataRelation("parent2Child", parentColumn, childColumn);
    dataSet.Tables["ChildTable"].ParentRelations.Add(relation);
}

private void BindToDataGrid()
{
    // Instruct the DataGrid to bind to the DataSet, with the
    // ParentTable as the topmost DataTable.
    DataGrid1.SetDataBinding(dataSet, "ParentTable");
}
```

```cs - sử dụng trong dự án
// Để Tranfer data có dạng "DataTable" trong "request-response" ta sẽ cần Serialize nó
// -> nếu Client-Server:
var dt = new DataTable();
var dtForTransfer = JsonConvert.SerializeObject(dt);
var result = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dt); // nếu ta cần trả về 1 Array gồm nhiều object

// -> nếu Server-Server:
return new {Dt = JsonConvert.SerializeObject(dt), DtReplace = JsonConvert.SerializeObject(dtreplace)}; 

// xử lý response:
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

# "DataColumn" class
* `represents` the **schema of a column** in **`a DataTable`**

=================================================
> _`System.IO`_

# "MarshalByRefObject" type
* Enables access to **`objects across application domain boundaries`** in **`applications that support remoting`**
* inherit **object**

# "Stream" abstract class
* Provides **`a generic view of a sequence of bytes`**
* -> inherit **MarshalByRefObject**
* -> implement **IDisposable**, **IAsyncDisposable**

# "MemoryStream" class
* Creates a **`stream`** whose **backing store is memory**
* inherit **Stream**
* implement **IDisposable** interface, but doesn't actually have any resources to dispose (_calling Dispose() is not neccessary_)

* `_backing store`_ refer to secondary storage of data 
* -> where data is stored but not executed
* -> that typically has greater capacity than the primary store but is slower to access
* -> thường là nói về disk, hard drive, SSD

```cs - Stream vs byte[]
// If you have to hold all the data in memory, then in many ways the choice is arbitrary. If you have existing code that operates on Stream, then MemoryStream may be more convenient, but if you return a byte[] you can always just wrap that in a new MemoryStream(blob) anyway.

// It might also depend on how big it is and how long you are holding it for; MemoryStream can be oversized, which has advantages and disadvantages. Forcing it to a byte[] may be useful if you are holding the data for a while, since it will trim off any excess; however, if you are only keeping it briefly, it may be counter-productive, since it will force you to duplicate most (at an absolute minimum: half) of the data while you create the new copy.

// So; it depends a lot on context, usage and intent. In most scenarios, "whichever works, and is clear and simple" may suffice. If the data is particularly large or held for a prolonged period, you may want to deliberately tweak it a bit.

// One additional advantage of the byte[] approach: if needed, multiple threads can access it safely at once (as long as they are reading) - this is not true of MemoryStream. However, that may be a false advantage: most code won't need to access the byte[] from multiple threads.
```

# "FileStream" class
* Provides **a Stream** for **a file**, supporting both **`synchronous`** and **`asynchronous`** `read` and write operations
* inherit **Stream** abstract class

# "FileSystemInfo" class
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
> _`Microsoft.CodeAnalysis`_

# "TextDocument" type

# "Document" type
* Represents a **source code document** that is **`part of a project`**; **`provides access`** to the `source text, parsed syntax tree and the corresponding semantic model`

================================================
> _`Microsoft.Office.Interop.Excel`_

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
> _`System`_

# "Uri" class
* `provides` an **object representation** of a **`uniform resource identifier (URI)`** and **easy access to the parts of the URI**

```cs - take part of a typical URI
Uri uri = new Uri("https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName");

uri.PathAndQuery; // PathAndQuery: /Home/Index.htm?q1=v1&q2=v2
uri.Authority // Authority: www.contoso.com:80
uri.Port; // Port: 80
uri.Query; // Query: ?q1=v1&q2=v2
uri.Scheme; // Scheme: https
uri.Fragment; // Fragment: #FragmentName
uri.Host; // Host: www.contoso.com
uri.HostNameType; // HostNameType: Dns
uri.AbsolutePath; // AbsolutePath: /Home/Index.htm
uri.AbsoluteUri; // AbsoluteUri: https:/*/user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
uri.DnsSafeHost; // DnsSafeHost: www.contoso.com
uri.IdnHost; // IdnHost: www.contoso.com
uri.IsAbsoluteUri; // IsAbsoluteUri: True
uri.IsDefaultPort; // IsDefaultPort: False
uri.IsFile; // IsFile: False
uri.IsLoopback; // IsLoopback: False
uri.IsUnc; // IsUnc: False
uri.LocalPath; // LocalPath: /Home/Index.htm
uri.OriginalString; // OriginalString: https:/*/user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
string.Join(", ", uri.Segments); // Segments: /, Home/, Index.htm
uri.UserEscaped; // UserEscaped: False
uri.UserInfo; // UserInfo: user:password
``

```cs - perform a GET request with "HttpClient"
Uri siteUri = new Uri("http://www.contoso.com/");

HttpClient client = new HttpClient();
HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, siteUri);
HttpResponseMessage response = client.Send(request);
```

## "Uri.GetLeftPart(UriPartial)" method
* -> returns a string containing the **leftmost portion of the URI string, ending with** the portion specified by **part**
* -> `GetLeftPart` includes **`delimiters`** in the following cases: **Scheme**, **Authority**, **Path**, **Query**

```cs
Uri uriAddress = new Uri("https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName");

// "Path" của URI này là: "/Home/Index.htm", thì
uriAddress.GetLeftPart(UriPartial.Path) // https://user:password@www.contoso.com:80/Home/Index.htm
```

===================================================
> _`System.Web`_

# "HttpContext" class
* -> **Encapsulates all HTTP-specific information** about an **individual HTTP request**
* -> This object is ready for **`garbage collection`** when the `HttpRequest is completed` (_Its usage after the request completes could lead to undefined behavior, such as a NullReferenceException_)
* -> inherit **object** class

* -> có thể dùng nó để lấy `Session, Request, Response , Server, User, ...`

# "HttpRequest" class
* -> Enables `ASP.NET` to read the **HTTP values sent by a client** during **`a Web request`**
* -> **`methods`** and **`properties`** of the **`HttpRequest`** class are exposed through the **"Request" properties** of the **HttpApplication**, **HttpContext**, **Page**, and **UserControl** classes
* -> able to accessing data from the **`QueryString`**, **`Form`**, **`Cookies`**, **`ServerVariables`** collections (_HTTP request collections_)
* -> inherit **object** class

```cs
// Vì ta đang ở trong 1 trang ".ascx" vậy nên class của ta đang kế thừa "UserControl" class
// => có khả năng access các property, method của "UserControl" như: "Request", "ResolveUrl(), ..."

Request.Url.GetLeftPart(UriPartial.Authority)
// "Request" là property của "UserControl" class có giá trị là "Page.Request"
// "Page.Request" trả về "_request" có type "HttpRequest"
// method "SetIntrinsics" của "System.Web.UI.Page" class sẽ set giá trị cho "_request", "_context", "_reponse", "_application", "_cache"
// "SetIntrinsics" sẽ được gọi bởi các method như "AspCompatBeginProcessRequest", "ProcessRequestAsync" , "ProcessRequestWithNoAssert", "LegacyAsyncPageBeginProcessRequest"

HttpContext.Current.Request.Url.ToString();
// "Current" là static property của "HttpContext" có giá trị là "ContextBase.Current"
// "Request" là property của "HttpContext" có giá trị là "_request" có type "HttpRequest" 
// method "Init" sẽ khởi tạo giá trị cho "_request"
// "Init" sẽ được gọi trong các constructor của "HttpContext" class
// => tức là nếu khởi tạo 1 instance của "HttpContext", ta sẽ có 1 instance của "HttpRequest" 
```

```cs - get data from HTTP request
// VD: get query string variable "fullname" from URL "http://www.contoso.com/default.aspx?fullname=Fadi%20Fakhouri"

// looks for the key "fullname" only in the query string:
string fullname1 = Request.QueryString["fullname"]; // Fadi Fakhouri

// looks for the key "fullname" in all of the HTTP request collections
string fullname2 = Request["fullname"]; // Fadi Fakhouri
```

```cs - Access the HttpRequest instance for the current request 
// using the "Request" property of the "Page class 
protected void Page_Load(object sender, EventArgs e)
{
    string rawId = Request["ProductID"];
    int productId;
    if (!String.IsNullOrEmpty(rawId) && int.TryParse(rawId, out productId))
    {
        using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions())
        {
            usersShoppingCart.AddToCart(productId);
        }
    }
    else
    {
        throw new Exception("Tried to call AddToCart.aspx without setting a ProductId.");
    }
    Response.Redirect("ShoppingCart.aspx");
}
```

```cs - check if the request is authenticated; if not retrieve the raw URL and redirect to login
protected void Page_Load(object sender, EventArgs e)
{
    if (!Request.IsAuthenticated)
    {
        var rawUrl = Request.RawUrl;
        Response.Redirect("/Account/Login?ru=" + Server.HtmlEncode(rawUrl));
    }
}
```

# "HttpApplication" class
* -> defines the **`methods, properties, and events`** that are common to **all application objects** in an `ASP.NET application`. 
* -> This class is the **base class** for **`applications`** that are **`defined by the user`** in the **Global.asax** file
* -> inherit **object** class

===================================================
> _`System.Web.UI`_

# "Control" class
* -> Defines the **`properties, methods, and events`** that are shared by all **`ASP.NET server controls`**
* -> inherit **object** class

## "Control.ResolveUrl(string relativeUrl)" method
* **converts a URL** into one that is **usable on the requesting client**
* -> If the **`relativeUrl parameter`** contains an **absolute URL**, the URL is **returned unchanged**
* -> If the **`relativeUrl parameter`** contains a **relative URL**, that URL is **changed to a relative URL** that is **`correct for the current request path`**, so that the browser **`can resolve the URL`**

```cs - 
// A client has requested an ASP.NET page that contains a user control that has an image associated with it.
// The ASP.NET page is located at "/Store/page1.aspx"
// The user control is located at "/Store/UserControls/UC1.ascx"
// The image file is located at "/UserControls/Images/Image1.jpg"

// If the user control passes the relative path to the image (that is, /Store/UserControls/Images/Image1.jpg) to the "ResolveUrl" method, the method will return the value "/Images/Image1.jpg"
```

# "TemplateControl" abstract class
* -> `provides` the **Page** class and the **UserControl** class with a base set of functionality
* -> inherit **Control** class

#  "ScriptManager" class
* Manages **ASP.NET Ajax script libraries and script files, partial-page rendering, and client proxy class generation** for Web and application services
* inherit **Control** class

## "ScriptManager.RegisterStartupScript" Method
* Registers **`a startup script block`** with the **`ScriptManager control`** and **adds the script block to the page**
* _`Script block` là một đoạn mã JavaScript; thêm vào trang bằng cách sử dụng thẻ script_

* _RegisterStartupScript(`Control`, Type, String, String, Boolean)_ -> registers _a startup script block_ for a control that is inside an **UpdatePanel** by using the `ScriptManager` control
* _RegisterStartupScript(`Page`, Type, String, String, Boolean)_ -> registers _a startup script block_ for **`every asynchronous postback`** with the `ScriptManager` control

# "Page" class
* -> **`represents`** an **.aspx file**, also known as **a Web Forms page**, requested from a `server` that `hosts an ASP.NET Web application`
* -> inherit **TemplateControl**
* -> implements **IHttpHandler**

# "UserControl" class
* -> **`represents`** an **.ascx file**, also known as **a user control**, requested from a server that hosts an ASP.NET Web application. 
* -> The file **`must be called from a Web Forms page`** or a parser error will occur
* -> inherit **TemplateControl**

* UserControl có 1 property như Page (type **Page**), Request (type **HttpRequest**)
