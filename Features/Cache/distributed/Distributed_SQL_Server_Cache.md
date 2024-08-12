
# Distributed SQL Server Cache
* ->  _the Distributed SQL Server Cache implementation (**`AddDistributedSqlServerCache`**)_
* -> **allows the distributed cache** to **`use a SQL Server database as its backing store`**

```r - create a "SQL Server cached item table" in a "SQL Server instance"
// -> we can use the "sql-cache" tool with "sql-cache create" command - the tool creates a table with the name and schema that we specify
// -> provide the "SQL Server instance" (Data Source), "database" (Initial Catalog), "schema" (for example, dbo), and "table name" (for example, TestCache)
// -> dotnet sql-cache create "Data Source=(localdb)/MSSQLLocalDB;Initial Catalog=DistCache;Integrated Security=True;" dbo TestCache
// -> the table will have these schema: "Id" (nvarchar(449)); "Value" (varbinary(MAX)); "ExpiresAtTime" (datetimeoffset(7)), "SlidingExpirationInSeconds" (bigint); "AbsoluteExpiration" (datetimeoffset(7))
```

```cs - Program.cs
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString(
        "DistCache_ConnectionString");
    options.SchemaName = "dbo";
    options.TableName = "TestCache";
});
```