# System.NotSupportedException: Serialization and deserialization of 'System.Action' instances are not supported. Path: $.DataResult.MoveNextAction. ---> System.NotSupportedException: Serialization and deserialization of 'System.Action' instances are not supported.
* Access API vẫn ra dữ liệu nhưng, frontend gọi bị lỗi
* Lỗi này là do dùng không dùng **`async`**` cho Method Action và Service

# Có dữ liệu trong Database nhưng không Get được
* Do các Item có field **`IsDeleted=True`**

# Turn Off Warning: the variable may be null
* **`!`** - suppress this warning when we try to access property of nullable variable
```
string? name = GetName();
int length = name!.Length;
```

