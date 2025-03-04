==============================================================
# Git - lỗi toàn cục
* -> không thể show các changes, xoá đi tải lại vẫn không được
* -> vào **`C:/Users/<user>/`** xoá file **`.gitconfig`**

# Git - không thể tương tác với Git remote do SSL "fatal: unable to access 'https://git.....': SSL certificate problem: unable to get local issuer certificate"
* -> https://stackoverflow.com/questions/23885449/unable-to-resolve-unable-to-get-local-issuer-certificate-using-git-on-windows
```bash
git config --global http.sslbackend schannel
```

==============================================================
# Http - Đối với việc gửi data bằng request-response sẽ có giới hạn. VD gửi 23000 records lên Client thì response sẽ không chịu nổi 
* có thể dùng Take() để giảm bớt số lượng cần lấy
* filter bớt những trường không cần thiết, chỉ để 1 trường làm identity và 1 trường để hiển thị là đủ

==============================================================
# Setting - Hosting xong trả về 404
* thư mục **bin** không có gì cả

# Setting - Publish code Debug nhưng không thể Attach to Process để Debug được
* thử xoá **`thư mục .vs và thư mục obj`** đi
* right-click vào csproj -> Properties -> Build -> uncheck **Optimize Code**
* vào **`Configuration Manager`**, đổi hết thành Debug
* vào Debug -> Options -> Debugging -> check **`Suppress JIT optim....`** và uncheck **`Enable Just My Code`**

# Setting - Connected Service: add Service Reference (add WCF lỗi) lỗi "the system can not find the path specified error"
* Trong trường hợp ta `add Service Reference`, rồi pass URL của WCF rồi bấm "Go" để tìm kiếm và tìm thấy service; nhưng khi bấm "OK" thì báo lỗi
* Rất có khả năng project đang thiếu thư mục **Connected Services** (hoặc **`Service References`** tuỳ version) để chứa các service; ta chỉ cần tạo thư mục rồi add lại là được 
* ngoài ra, ta có thể thử bỏ check **`Reuse types in referenced assemblies`** trong **Advanced** setting

# Setting - Connected Service - không thể khởi tạo instance của WCF service - Lỗi: Could not find default endpoint element that references contract 
* Lỗi này xảy ra đối với **`ASP.NET`**, là do ta đã add **`Connected Service`** nhưng **chưa thêm cấu hình vào web.config**
```xml
<client>
    <endpoint address="http://192.168.1.31:9017/QLVB_LienThong/ThongKeLienThongService.svc" binding="basicHttpBinding" bindingConfiguration="basicHttpService" contract="ThongKeLienThongService.IThongKeLienThongService" name="basicHttpService" />
    
	<endpoint address="http://192.168.1.12:9017/QLVB/QLVB_DanhMucService.svc" binding="basicHttpBinding" bindingConfiguration="basicHttpService" contract="QLVB_DanhMucService.IQLVB_DanhMucService" name="basicHttpService" />
</client>
```

# Setting - the current Visual studio version does not support targeting .NET 8.0. Either target .NET 7.0 or lower, or use Visual Studio version 17.8 or higher
* -> ta chỉ cần cài **`.NET 8.0 SDK`** (bản **`x64`**) 

# Setting - ta chạy App của ta bằng Visual Studio sử dụng 'SSL' nên bị trình duyệt chặn
* Trình duyệt sẽ báo: _`Your Connection is not private`: Attackers might be trying to steal your information from `localhost`_
* ta sẽ sửa lại cấu hình Chrome -> truy cập đường link **`chrome://flags/`** -> check vào **Allow invalid certificates for resources loaded from localhost**

# Setting - Visual Studio lúc run project báo lỗi "Couldn't process file .... due to its being in the Internet or Restricted zone or having the mark of the web on the file"
* -> if we **`downloaded the file from the internet`**, either separately or inside a .zip file or similar, it may have been **`locked`** because it is flagged as coming from the **internet zone**

* -> cách 1: mở file đó trong File Explorer -> right-click rồi chọn "properties" -> check vào **`Unblock`** option -> rồi bấm Ok

* -> cách 2: if we got the file from an archive, trước khi unzip ta sẽ **`Unblock`** the archive first (_if the file is flagged as coming from the internet zone, and you unzip it, that flag might propagate to many of the files you just unarchived_)
* -> cách 3: sử dụng code
```bash
> Get-ChildItem -Path '<YOUR-SOLUTION-PATH>' -Recurse | Unblock-File
```

==============================================================
# BE - C# - Optional parameters must appear after all required parameters
```cs
public void GetIAccountInfo(string VanThuDonVi = "1", int? donviId, int? phongbanId);
// sửa thành:
public void GetIAccountInfo(int? donviId, int? phongbanId, string VanThuDonVi = "1");
```
* Lỗi này là do để sai thứ tự param; vì "VanThuDonVi" là 1 **`Optional parameters`**, nó cần được để sau "donviId" và "phongbanId"

# BE - C# - Object reference not set to an instance of an object
* -> trong trường hợp là bind data vào tham số của 1 Method không được, thì kiểm tra lại trong function nó có truy cập giá trị của các biến bằng **`.Value`**, có thì bỏ nó đi
* -> có thể thử viết lại Method đó tối giản hơn, Ví dụ: gọi nó trực tiếp thay vì thông qua 1 Service hay 1 instance nào đó, đổi parameter sang dạng đơn giản hơn 

# BE - Run Debug - Gọi API với 1 Action Method nhưng khi đặt break point trong action method thì nó không chạy vô được
* Kiểm tra lại Endpoint xem ta đặt break point đúng controller/action method chưa
* Khả năng cao lỗi này là do **`không map được data ta truyền trong Request với Parameter của action method`**
* -> VD: DaDoc=1 nhưng ta lại truyền DaDoc="1" ; hoặc truyền 1 field null cho unullable value

# BE - Dependencies - không thể load được các 'Analyzers' của project
* -> rất có thể là do 1 trong các projects của ta đang target đến 1 phiên bản cao của .NET, ta sẽ cần kiểm tra xem máy tính của ta có cài phiên bản .NET này chưa
* -> nếu cài rồi thì rất có thể Visual Studio của ta chưa hỗ trợ phiên bản .NET này, ta sẽ cần vào **Visual Studio Installer** để update Visual Studio lên phiên bản mới nhất

# BE - calling API - System.NotSupportedException: Serialization and deserialization of 'System.Action' instances are not supported. Path: $.DataResult.MoveNextAction. ---> System.NotSupportedException: Serialization and deserialization of 'System.Action' instances are not supported.
* Access API Swagger vẫn ra dữ liệu nhưng, frontend gọi bị lỗi
* Lỗi này là do không dùng **`async`**` cho Method Action và Service

# BE - DB - Có dữ liệu trong Database nhưng không Get được
* Do các Item có field **`IsDeleted=True`**

# BE - Turn Off Warning: the variable may be null
* **`!`** - suppress this warning when we try to access property of nullable variable
```
string? name = GetName();
int length = name!.Length;
```

# BE - Exception: could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'
* Lỗi này là do `Entity Framework` không thể chuyển 1 số cú pháp `LINQ` sang `SQL`

* Rất có thể là do trong câu `.Where()` chứa những **`Extension method`** không thể tranfer
* -> như: **`Custom Extension method`**, `.Any()`, `.ToString()`, `DateTime.Add()`, ...
* -> **Giải pháp:** trường hợp này ta nên đổi `lstMucMuc` thành 1 list Id, rồi dùng `Contain` thì sẽ chuyển đc sang `SQL`
```
var matchEntities = _context.MucLucs.ToList().Where(mucluc => lstMucLuc.Any(x => x.Id == mucluc.Id));
```

* Rất có thể việc `.Contain()` không thể chuyển thành `IN` SQL Clause cũng gây ra lỗi này
* -> **Nguyên nhân** có thể là do expression used for `Contains` not a simple IEnumerable<T> variable 
* -> tức là `T` param không phải `primitive type` 
* -> có thể dẫn tới việc nó vẫn kéo data theo condition của Where(), nhưng Contain() sẽ xử lý trên memory gây bad perfomance

# BE - The type or namespace name 'System' could not be found
* Ta có thể thử Unload rồi Reload `project`

# BE - Lỗi:"Unexpected character encountered while parsing value: <. Path '', line 0, position 0."
* **reason**: có thể xảy ra khi SER chết , HelperCommon gọi SER sẽ trả về response như này
* **Solution**: publish lại SER

# BE - Khi Debug 1 method ta không thể "Step in" vào method đó mắc dù public đúng method cũng như truyền đúng hết tham số - báo lỗi "Could not load file or assembly ... Version=..."
* **Confustion**: ta đặt break point ngay dòng đầu 1 dòng chắc không thể lỗi nhưng nó vẫn không nhảy vô được
* **reason**: rất có thể đâu đó trong phần thân hàm, đang chứa 1 **`Assembly`** (1 class mà ta reference đến) không thuộc **`.NET Core`** mà thuộc của **`.NET Framework`**
* -> vậy nên Assembly này sẽ load những Assembly con target đến **`.NET Framework 3.5`**, trong khi những Assembly con đó trong **`ASP.NET Core`** lại target đến **`.NET Framework 4.7`** chẳng hạn
* **Solution**: đầu tiên ta nên xoá Assembly đó trong ASP.NET Core project đi, ta có thể đưa logic lên HelperCommon để xử lý

# BE - Server Error in '/' Application - An error occurred during the processing of a configuration file required to service this request
* Rất có thể ta đang chạy sai project, right-click vào project cần chạy -> chọn **`Set as Startup project`**

# BE - MVC - InvalidOperationException: The view 'Index' was not found. The following locations were searched: /Views/Home/Index.cshtml
* -> Lỗi này là do program không tìm thấy file view nó cần ở đường dẫn **`/Views/Home/Index.cshtml`**, mặc dù ta thấy đường dẫn này có tồn tại trong project
* -> trước tiên ta cần install package **`Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`**
* -> nếu là MVC project thì ta thêm **`.AddRazorRuntimeCompilation()`** vào sau **services.AddControllersWithViews()** (_tức là services.AddControllersWithViews().AddRazorRuntimeCompilation();_)
* -> còn nếu là Razor project thì ta thêm **`.AddRazorRuntimeCompilation()`** vào sau **services.AddRazorPages()**

* _nếu vẫn không được thì ta có thể thử:_
* -> vô file .csproj để bỏ  dòng <RazorCompileOnBuild>false</RazorCompileOnBuild> và dòng<Content Remove="Views\Home\Index.cshtml" />
* -> vào properties của file view, chuyển "Build Action" thành **`Content`**
* -> include lại file view vô project; 
* -> vào properties của file view, chuyển "Build Action" thành "Content"

# BE - MVC - chỉnh sửa nội dung 1 số file như ".cshtml" nhưng lúc chạy project thì lại không được cập nhật mà vẫn sử dụng bản cũ
* -> lỗi này rất có thể là do **`caching`** của file "/obj/Debug/net7.0/.AssemblyInfoInputs.cache"
* -> ta có thể thử unload rồi reload project

# BE - MVC - InvalidOperationException: The following sections have been defined but have not been rendered by the page at '/myPath' : 'Scripts'. To ignore an unrendered section call IgnoreSection('sectionName')
* -> Lỗi này là do "myPath.cs" đang thiếu **`script section`**
* -> rất có thể do "view" có định nghĩa "section" nhưng layout của chúng lại chưa có

```cs - VD:
@section Scripts {
    @await RenderSectionAsync("Scripts", required: false)
}
```

# BE - MVC - start project (MVC hay Razor page) nhưng chỉ có nội dung hiện lên không có CSS
* -> ta thử xoá thư mục **.vs**, clean solution rồi rebuild lại project

# BE - ASP.NET - Cannot access a disposed object
* -> rất có thể là trong 1 Scope request-reponse, logic trước đó đã **`.Dispose()`** hoặc **`.Close()`** đi 1 connection hoặc instance; nên logic sau cần sử dụng **`instance`** đó thì không có
* -> ta có thể **`DI kiểu Scope`**; hoặc với mỗi logic ta lại tạo instance sau đó dispose nó 

# BE - Build - build xong thì báo lỗi "The type or namespace name '...' could not be found"
* -> ta cần kiểm tra xem **`target đến đúng folder ta muốn build chưa`** (right-click .csproj -> properties -> Build -> check đường dẫn của cả Build và Release)

# BE - Attach Process - Có lỗi nhưng không rõ ràng
* -> Xảy ra lỗi khi 1 service bên trong project khi attach process IIS, nhưng **`không biết lỗi gì vì exception không cụ thể`**
* -> ta có thể **`tạo 1 project Web App .NET framework mới`** rồi bỏ service vào chạy thử , nó sẽ cho ta lỗi rõ ràng hơn

# BE - Tranfer data - missing some fields in object
* -> khả nằng là do **`tạo 1 class inherit 1 class từ WCF và thêm 1 số field`**, rồi dùng nó làm response thì trả về bị thiếu những field ta tự thêm vào
* -> đây là do quá trình **`serialize dữ liệu để response thì những trường này bị mất`** (_ta có thể dùng JsonConvert.SerializeObject() để kiểm chứng_)
-> ta nên **`tạo ra 1 class hoàn toàn mới`**

# BE - System.IO - 'File.Exist' trả về false mặc dù đường dẫn có tồn tại
* -> nếu hiện tại code này đang nằm trên **`.NET Framework`**, thì ta thử đưa nó sang **`.NET Core`** xem có chạy không 
* -> có thể vấn đề nằm ở sự khác biệt ở **`32-bit`** và **`64-bit`** architect, ta có thể vào _properties -> Build -> đổi thành **anyCPU**_
* -> hoặc cũng có thể là do vấn đề về **permission**, ta chỉ vần **`chạy Visual Studio ở Administration`** là được

# BE - TypeDescriptor.GetProperties(typeof(T)) trả về 1 list với Count = 0 hoặc nhỏ hơn thực tế
* -> rất có thể khi định nghĩa class, ta quên pass **{ get; set; }** cho **`property`**

# BE - Lỗi xảy ra trên production mà local chạy bình thường
* -> ta sẽ cần so sánh **`các tham số đầu vào cũng như đầu ra`** của câu lệnh SQL được thực thi trên production với local bằng **SQL Server Profiler**

# BE - API - # Error: Actions require unique method/path combination for Swagger
* -> trong controller thay [Route("api/[controller]")] bằng [Route("api/[controller]/[action]")] ???
* -> hoặc define explicit route for action: [Route("GetById")]

# BE - System.IO - bị lỗi khi gặp tên file quá dài
* -> cụ thể là với **`path length > 259 character`** và sử dụng **.NET Framework** để xử lý
* -> đối với **version > .NET 4.6.2**, solution là ta có thể dùng path với **extended-length path syntax**: **`\\?\D:\very long path`**
* -> đối với **version < .NET 4.6.2**, solution là use **`extended-length path syntax`** and **`the Unicode version of the Win32 API function with P/Invoke`**
* -> với **.NET Core** thì không cần vị nó sẽ tự add cho ta

* -> nhưng lưu ý sử dụng **extended-length path syntax** sẽ **`turns off file name normalization`** (_removing trailing spaces, expanding '.' and '..', converting relative paths into full paths,..._) performed by Windows APIs
* -> can be used in most of the file-related Windows APIs, but **`not all Windows APIs`** (Ex: LoadLibrary<...> fails if the file name is longer than MAX_PATH)
* -> compatibility with other Windows-based applications and the Windows shell itself 

## BE - Https hoặc SSL certificate có vấn đề
* -> thứ nhất để chạy project web của ta trên 1 URL ta chỉ định thì ta có thể vô **`~/Properties/lauchSettings.json`**
* -> check xem URL mà web ta đang chạy có giống với URL trong file "launchSettings.json", nếu chưa thì chỉnh lại cho đúng

* -> if we get an **error connecting** it may be that we are running **https** and the **`development certificate`** for **localhost** is not trusted
* -> we can run **`dotnet dev-certs https --trust`** in order to trust the development certificate (_this only needs to be done once_)

## BE - MVC - trang không load được CSS 
* -> ta cần kiểm tra lại thẻ link đến các external resource trong trang **_Layout.cshtml**, xem thực sự
 các **external resource** này có thực sự tồn tại không
* -> rất có thể các file **`bootstrap`** cũng như **`jquery`** đã mất đi
* -> ta có thể start 1 project MVC rồi copy các file đã mất qua

## BE - IIS - Lỗi double escape sequence
* -> khi ta truy cập 1 virtual path đến 1 file vật lý server bởi IIS với tên có ký tự đặc biệt
* -> theo RFC 2396, **`reserved = ";" | "/" | "?" | ":" | "@" | "&" | "=" | "+" | "$" | ","`** 
* (_https://stackoverflow.com/questions/7739233/double-escape-sequence-inside-a-url-the-request-filtering-module-is-configured_)
* (_https://www.rfc-editor.org/rfc/rfc2396#section-2.2_)


==============================================================

# DB - Exception The database operation was expected to affect 1 row(s), but actually affected 0 row(s);
* Xảy ra khi `SaveChanges()`
* lỗi này là do ta đánh dấu 1 phần tử đang được track bởi context là "Modified"
* nhưng khi tìm `Primary key` của phần tử này trong Database để update thì lại không thấy

# DB - Không "Add-Migration" được
* **Thiếu thư viện**: Entity Framework Core (`Design, SQL Server, Tool`)
* `Tất cả Project trong Solution` đểu phải **build success** hết (nếu có project bị lỗi có thể Remove nó)
* Kiểm tra **ConnectionString**

# DB - Migration bị lỗi do không đồng nhất với database
```
Ví dụ Migration cần drop 1 Table nhưng Table đó không tồn tại trong Database
```

* Đầu tiên check xem `migration` trong **`thư mục Migration`** và **`dbo._EFMigrationsHistory`** có đồng nhất với nhau không

* Cần thiết thì xoá Migration trong **thư mục Migration** đi

* **`Rollback Migration`** EntityFramework Core: **update-database -Migration <migration ta muốn>**
(_với EntityFramework là: `update-database -TargetMigration <migration ta muốn>`_)

# DB - SQL - An expression of non-boolean type specified in a context where a condition is expected
* -> lỗi này có thể là do dùng **isnull() chung với AND**; ta nên dùng **`IS NULL / IS NOT NULL`** để trả về boolean (Ex: assume `phont IS NULL` then `Isnull(phont, 1)` will return 1, so the statement will look like `cont.contactid = '29' AND 1 AND ...`)

* -> ngoài ra có thể lỗi này còn có thể do  forgot to add **"ON" condition** when specifying **`join clause`**; *
* -> lưu ý nếu có **multiple joins** thì **`the last join will get highlighted with the error message`** (_mặc dù có thể không phải chính nó gây ra lỗi_)

* -> hoặc cũng có thể là ta đang **`mixing relational expressions with scalar operators (OR)`**

# DB - SQL - lỗi 'Microsoft.Data.SqlClient.SqlException (0x80131904): A network-related or instance-specific error occured while establishing a connection to SQL Server' khi 'update-database'
* -> lỗi này rất có thể là do **`ConnectionString`** của ta đang không đúng

===============================================
# FE - React can't access an object before it gets initialize
* **Lý do**: 
* -> viết **`2 class phụ thuộc lẫn nhau`** dẫn đến vòng lặp
* -> class import lẫn nhau để sử dụng properties, method của nhau

* -> hoặc là do bị **block scope** vì sử dụng 1 biến trước khi được khai báo với **`let, const`**

* **Bản chất**:
* -> do viết sai SOLID - 1 class chứa quá nhiều logic khác nhau trong đó

* **Giải pháp**:
* -> thay vì lấy property, method từ class khác; ta lấy tạo thẳng property, method trong class này với logic y chang
* -> cài thư viện Dependency Injection: tự động quản lý việc tạo đối tượng

# FE - Network Error; AxiosError: Network Error at XMLHttpRequest.handleError hiện ngay trên UI
* do **`try/catch`** sai cách
```cs
const DataBinding = () => {
    try {
        axiosServices.guiGet("TraCuuVanBanDen/GetCboData").then((response: any) => console.log(response));
    }
    catch(ex) {
        appCommon.toast(t("message.getDataError"), 'error');
    }
}
```
* **Bản chất**: 
* -> `try/catch` block won’t catch any errors that occur **`within the Promise`** returned ajax call
* -> the **catch block** has already executed by the time the **`Promise rejects`**

* **Giải pháp**:
* -> **`async/await`**
* -> thêm **`.catch()`**