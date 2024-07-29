
# create "table" in database from "class" in project
```r - MVC
// xem "~/Resolver/Common/MVC/Code/Form_MVC.md" để tìm hiểu thêm
```

```xml - web.config
<configuration>
    <connectionString>
        <add name="Mydb" connectionString="server=...;username=...;password=...">
    </connectionString>
</configuration>
```

```cs - DbContext
// map custom DbContext với 1 "connectionstring" ta thêm trong web.config 
[DbConfigurationType(typeof(MySqlEFConfiguration))]
public class ApplicationDbContext : DbContext
{
    public DbSet<Users> Users { get; set; } 

    public ApplicationDbContext() : base("MyDb") {} // pass connection string

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
    }
}
```

```r - Migrations
// CLI "Enable-migrations" để tạo thư mục "Migrations" và 1 số file config
// CLI "add-migration Initial - Force" tạo lớp đại diện cho current migration commit
// CLI "update-datebase -Verbose" tạo các bảng trong migration commit, đồng thời tạo 1 bảng lưu "migration history"
```
```cs - Migration commit
CreateTable(
    "dbo.Users",
    c => new 
    {
        Id = c.Int(nullable: false, identity: true),
        UserName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
        Password = c.String(nullable: false, unicode: false)
    }
)
.PrimaryKey(t => t.Id)
```