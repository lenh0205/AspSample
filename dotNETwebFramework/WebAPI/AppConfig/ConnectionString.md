
# Local Connect

* _with **`Connection String`**_
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
}
```

* _or we can put it in program.cs like this:
```cs
var dbHost = "localhost";
var dbName= "Demo";
var dbPassword = "12345";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connectionString))
```

* _with **`SQL Server Management Studio`**_: **Server name** is **`(localdb)\MSSQLLocalDB`**, using **Window Authentication**