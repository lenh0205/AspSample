==================================================================================
# Clustered Index
* -> when we define a **PRIMARY KEY** in SQL Server, **`a clustered index is automatically created on that column by default`** unless we **explicitly specify it as a non-clustered index**
* => table rows are physically stored in clustered index order on disk

## Guid as Primary Key
* -> EntityFramework sẽ map kiểu **Guid** trong C# thành kiểu **UNIQUEIDENTIFIER** trong SQL Server  

## Guid
* -> is suitable for distributed system where uniqueness across multiple databases, tables, server, instances is a priority
* -> avoid conflict when merging, replicate record from different databases, tables, sources
* -> allows easy distribution of databases across multiple servers
* -> can generate IDs anywhere, instead of having to roundtrip to the database (_unless **partial sequentiality** is needed_)
* -> ngoài ra là security hơn vì khó predict subsequent values
* -> vậy nên generated GUIDs should be **`partially sequential`** for best performance (eg, **`NEWSEQUENTIALID()`** on SQL Server 2005+, mặc định của Guid là **NEWID()**) and to enable use of clustered indexes

## integer
* -> dễ đánh index nên là query performance tốt hơn;
* -> debug và quản lý dễ hơn
* -> lưu int chỉ tốn 4 bytes (_trong khi Guid đến 16 bytes can cause serious performance and storage implications if not careful_)

==================================================================================
# Entity Framework
```
This unique Id is created by SQL Server on insert.

If you want to let SQL Server generate the value on insert, you have to use the following attributes in your model :

[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
[Key]
public Guid Id { get; set; }
Or if you want to manage the Id by yourself, just generate it :

var id = Guid.NewGuid();
```
