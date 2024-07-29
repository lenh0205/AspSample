====================================================================
# create 'class' in project from table in Database

```xml - web.config
<configuration>
    <connectionString>
        <add name="Mydb" connectionString="server=...;username=...;password=...">
    </connectionString>
</configuration>
```

* -> _sử dụng **ADO.NET Entity Data Model** để tạo model với model content **EF Desinger from database** với tên "ModelDB"_
* -> _nó sẽ tạo ra 1 thư mục **`ModelDB.edmx`** chứa **DbContext**, các **Model**, **DB Desinger**_

====================================================================
# use 'Partial class' to retain validation after update
* -> ta sẽ thêm validation vào các property của model (_sử dụng Databse_) và đảm bảo là khi update Database thì nó sẽ không bị mất
 
# Problem
* _mỗi lần ta update Database rồi để genrate lại model trong **ModelDB.edmx** thì những `validation (data-annotation)` mà ta thêm manual sẽ biến mất; model trở về trang thái ban đầu (chưa có validation):_
```cs - VD: ~/ModelDB.edmx/ModelDB.tt/Size.cs
public partial class Size
{
    public long id { get; set; }
    public long restaurantId { get; set; }
    public string name { get; set; } 
} 
```

# Solution

* _đầu tiên, ta tạo 1 **partial class** tương tự trong thư mục `/Models`:_
```cs - ~/Models/Size.cs

[MetadataType(typeof(SizeMetaData))]
public partial class Size
{

} 

public class SizeMetaData
{
    public long id { get; set; }

    public long restaurantId { get; set; }

    [StringLength(5)] // add validation
    [Required(ErrorMessage = "Name is required")]
    public string name { get; set; } 
}
```