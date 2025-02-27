https://stackoverflow.com/questions/5244126/net-connection-pooling
https://stackoverflow.blog/2020/10/14/improve-database-performance-with-connection-pooling/
https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-connection-pooling
https://www.progress.com/tutorials/net/net-connection-pooling
https://viblo.asia/p/tai-sao-chung-ta-can-database-connection-pool-BQyJKj074Me
https://help.sap.com/docs/SAP_SQL_Anywhere/98ad9ec940e2465695685d98e308dff5/3bd05f3c6c5f101497b7e1acc5dcd6ef.html?version=17.0
https://dotnettutorials.net/lesson/ado-net-connection-pooling/
https://www.microsoftpressstore.com/articles/article.aspx?p=2231011&seqNum=4

============================================================================
# Connection Pooling with 'EntityFramework Core' and 'ADO.NET'

## Connection Pooling
* -> Checks if a connection (with the same connection string) already exists in the pool (_if yes, it reuses an idle connection from the pool; if no, it creates a new physical connection (if the pool isn't full)_)
* -> if a connection is closed, it is returned to the pool instead of being destroyed.
* -> the connection stays open and available for reuse
* -> if the pool reaches its limit (Max Pool Size), new connection requests wait until an existing connection is available
* -> idle connections are removed after a timeout (Connection Lifetime or Load Balancing Timeout)

* => this mechanism avoids the **`overhead of opening/closing connections repeatedly`**, **`improving performance`**

## ADO.NET
* -> ADO.NET **`automatically manages connection pooling`** when using **SqlClient (SQL Server)**
* -> means whenever we create a **new SqlConnection** (from System.Data.SqlClient) to request a database connection, ADO.NET reuses an existing connection from the pool instead of creating a new one
* -> when **connection.Close()** or **Dispose()** is called, **`the connection is returned to the pool instead of being closed permanently`**

## Entity Framework Core
* -> EF Core **internally uses ADO.NET (SqlClient for SQL Server)**, which **`already has connection pooling`**
* _however, we should follow best practices to ensure it works efficiently by register **DbContext** properly as a **`Scoped`** Service in DI container:_
```cs
// register DbContext as Scoped, meaning each request gets its own instance
// when request ended, it's always disposed to release connections back to the pool
builder.Services.AddDbContext<AppDbContext>(
  options => options.UseSqlServer(connectionString));
```

## Default ADO.NET pools connections
* _By default, ADO.NET pools connections when using SqlClient with_
* -> **default pool size`**: 100 connections
* -> **`idle timeout`**: 4 minutes

```bash
Server=myServer;Database=myDb;User Id=myUser;Password=myPass;Max Pool Size=200;Min Pool Size=10;
# configure pooling via the connection string
# to increases max connections in the pool to 200
# and keeps at least 10 connections ready
```
