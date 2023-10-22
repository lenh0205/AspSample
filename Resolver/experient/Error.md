=============================================
# Http - Đối với việc gửi data bằng request-response sẽ có giới hạn. VD gửi 23000 records lên Client thì response sẽ không chịu nổi 
* có thể dùng Take() để giảm bớt số lượng cần lấy

=============================================
# Hosting - Hosting xong trả về 404
* thư mục **bin** không có gì cả

=============================================
# BE - System.NotSupportedException: Serialization and deserialization of 'System.Action' instances are not supported. Path: $.DataResult.MoveNextAction. ---> System.NotSupportedException: Serialization and deserialization of 'System.Action' instances are not supported.
* Access API vẫn ra dữ liệu nhưng, frontend gọi bị lỗi
* Lỗi này là do dùng không dùng **`async`**` cho Method Action và Service

# BE - Có dữ liệu trong Database nhưng không Get được
* Do các Item có field **`IsDeleted=True`**

# BE - Turn Off Warning: the variable may be null
* **`!`** - suppress this warning when we try to access property of nullable variable
```
string? name = GetName();
int length = name!.Length;
```

# DB - Exception The database operation was expected to affect 1 row(s), but actually affected 0 row(s);
* Xảy ra khi `SaveChanges()`
* lỗi này là do ta đánh dấu 1 phần tử đang được track bởi context là "Modified"
* nhưng khi tìm `Primary key` của phần tử này trong Database để update thì lại không thấy

# BE - Exception: could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'
* Lỗi này là do `Entity Framework` không thể chuyển số cú pháp `LINQ` sang `SQL`

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
* Ta có thể thử Reload `project`

# BE - Lỗi:"Unexpected character encountered while parsing value: <. Path '', line 0, position 0."
* có thể xảy ra khi SER chết , HelperCommon gọi SER sẽ trả về response như này
* publish lại SER

=========================================
# FE - React can't access an object before it gets initialize
* **Lý do**: 
* -> viết **`2 class phụ thuộc lẫn nhau`** dẫn đến vòng lặp
* -> class import lẫn nhau để sử dụng properties, method của nhau

* **Bản chất**:
* -> do viết sai SOLID - 1 class chứa quá nhiều logic khác nhau trong đó

* **Giải pháp**:
* -> thay vì lấy property, method từ class khác; ta lấy tạo thẳng property, method trong class này với logic y chang
* -> cài thư viện Dependency Injection: tự động quản lý việc tạo đối tượng

# FE - Network Error; AxiosError: Network Error at XMLHttpRequest.handleError hiện ngay trên UI
* do **`try/catch`** sai cách
```
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