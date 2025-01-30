 
* -> Nhấp **Add Connection** ->  chọn **`LINQ to SQL`** (tối ưu cho SQL Server) hoặc **`EF Core`** (dùng cho SQL Server, Oracle, MySQL, Postgres and SQLite) -> nhập tên SQL Server instance ta muốn kết nối
* -> right-click on database ta muốn chọn **New Query**
* -> trong cửa sổ "Query", right-click chọn **Namespace Imports** kiểm tra xem nó có 2 packages:
```bash
System.Data.Linq
System.Data.Linq.Mapping
 ```
* -> và phần **Advanced** có check **Include LINQ-to-SQL assemblies**

```cs - ta chọn "C# Program"
void Main()
{
	DataContext dataContext = new DataContext(@"Server=DESKTOP-62P3LMJ;Database=QLVB_dnn;Trusted_Connection=True;TrustServerCertificate=True;");
	Table<Customer> customers = dataContext.GetTable<Customer>();
	
	IQueryable<string> query = from c in customers select c.Name;
	
	foreach(string name in query)
	{
		Console.WriteLine(name);
	}
}

[Table]
public class Customer 
{
	[Column(IsPrimaryKey=true)]
	public int ID;
	[Column]
	public string Name;
}
```

