
==============================================================================
# Using Stored Procedures
Step 1: Create the Stored Procedure
sql
Copy
Edit
CREATE PROCEDURE GetUsersByRole
    @role NVARCHAR(50)
AS
BEGIN
    SELECT Id, Name FROM Users WHERE Role = @role
END
Step 2: Call the Stored Procedure from EF Core
EF Core allows executing stored procedures using FromSqlRaw().

Example Usage
csharp
Copy
Edit
public async Task<List<User>> GetUsersByRole(string role)
{
    return await _context.Users
        .FromSqlRaw("EXEC GetUsersByRole @p0", role)
        .ToListAsync();
}
💡 Note: If the stored procedure does not match the DbSet<User>, create a separate model and use FromSqlRaw() with a DbSet<> that is not mapped to a table.

==============================================================================
# Using User-Defined Functions (UDFs)
There are two types of UDFs in SQL Server:

Table-Valued Functions (TVFs)
Scalar Functions
Step 1: Register a Table-Valued Function in EF Core
You need to create a method in DbContext and map it to the UDF.

Example of a Table-Valued Function
sql
Copy
Edit
CREATE FUNCTION dbo.GetUsersByStatus(@status INT)
RETURNS TABLE
AS
RETURN
(
    SELECT Id, Name FROM Users WHERE Status = @status
);
Register the Function in DbContext
csharp
Copy
Edit
public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    [DbFunction("GetUsersByStatus", "dbo")]
    public IQueryable<User> GetUsersByStatus(int status)
    {
        return FromExpression(() => GetUsersByStatus(status));
    }
}
Use It in Your Service
csharp
Copy
Edit
public async Task<List<User>> GetActiveUsers()
{
    return await _context.GetUsersByStatus(1).ToListAsync();
}

==============================================================================
# Using Views in Entity Framework Core
Step 1: Create a Model for the View
Since EF Core does not automatically map views, you need to create a model that matches the columns returned by the view.

csharp
Copy
Edit
public class MyViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}
Step 2: Configure the Model in DbContext
Add the view to your DbContext using the HasNoKey() method.

csharp
Copy
Edit
public class MyDbContext : DbContext
{
    public DbSet<MyViewModel> MyView { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyViewModel>().ToView("MyView").HasNoKey();
    }
}
Step 3: Query the View in Your Repository or Service
csharp
Copy
Edit
public class MyService
{
    private readonly MyDbContext _context;

    public MyService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<MyViewModel>> GetViewData()
    {
        return await _context.MyView.ToListAsync();
    }
}
