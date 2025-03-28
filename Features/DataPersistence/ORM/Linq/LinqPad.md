
# Connect Database 
* -> Nhấp **Add Connection** 
* ->  chọn **`LINQ to SQL`** (tối ưu cho SQL Server) hoặc **`EF Core`** (dùng cho SQL Server, Oracle, MySQL, Postgres and SQLite) 
* -> nhập tên SQL Server instance ta muốn kết nối + Window Authen

# Point to DbContext
* -> using LINQPad, we can point to our DLL
* => so it will understand our DbContext and from there on you can write LINQ queries in the tool
* => the tool will show you the resulting data, the SQL statements, and other details

## "C# Program" query
* -> right-click on database ta muốn chọn **New Query**
* -> trong cửa sổ "Query", right-click chọn **Namespace Imports** kiểm tra xem nó có 2 packages:
```bash
System.Data.Linq
System.Data.Linq.Mapping
 ```
* -> và phần **Advanced** có check **Include LINQ-to-SQL assemblies**

```cs
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

## Expression

```sql
CREATE TABLE actor (
	actor_id INT,
	first_name NVARCHAR(100),
	last_name NVARCHAR(100)
)

CREATE TABLE film_actor (
	actor_id INT,
	film_id INT
)

-- Postgres
SELECT a.actor_id, a.first_name, a.last_name
FROM actor a
INNER JOIN film_actor fa ON fa.actor_id = a.actor_id
GROUP BY a.actor_id, a.first_name, a.last_name
ORDER BY COUNT(fa.film_id) DESC
LIMIT 1;

-- MS SQL Server
SELECT TOP 1 a.actor_id, a.first_name, a.last_name
FROM actor a
INNER JOIN film_actor fa ON a.actor_id = fa.actor_id
GROUP BY a.actor_id, a.first_name, a.last_name
ORDER BY COUNT(fa.film_id) DESC;
```

```cs - C# Expression
(from a in Actors
join fa in Film_actors on a.Actor_id equals fa.Actor_id
group a by new { a.Actor_id, a.First_name, a.Last_name } into g
orderby g.Count() descending
select new
{
    ActorId = g.Key.ActorId,
	FirstName = g.Key.FirstName,
	LastName = g.Key.LastName,
	FilmCount = g.Count()
})
.FirstOrDefault()
```
