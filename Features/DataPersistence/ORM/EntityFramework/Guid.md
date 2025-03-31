==================================================================================
# Guid vs Clusterd Index

## Guid - Globally Unique Identifier
* -> EntityFramework sẽ map kiểu **Guid** trong C# thành kiểu **UNIQUEIDENTIFIER** trong SQL Server  
* -> is suitable for distributed system where **`uniqueness`** across multiple databases, tables, server, instances is a priority

* => avoid conflict when merging, replicate record from different databases, tables, sources; easy distribution of databases across multiple servers
* => can **generate IDs anywhere**, instead of having to roundtrip to the database (_unless **partial sequentiality** is needed_)
* => ngoài ra là **security/privacy** hơn vì khó predict subsequent values

```sql
-- create a GUID in SQL Server
SELECT NEWID()

-- declare a variable of type GUID
DECLARE @UNI UNIQUEIDENTIFIER
SET @UNI = NEWID()
SELECT @UNI
```

## Clustered Index as PRIMARY KEY
* -> when we define a **`PRIMARY KEY`** in SQL Server, **`a clustered index is automatically created on that column by default`** unless we **explicitly specify it as a non-clustered index**
* => table rows are **physically stored in clustered index** order on disk

## Problem
* -> SQL Server will **`let us build a clustered index around a uniqueidentifier column`**, however it will **cause the SQL Server to do unnecessary work and cause performance slowdowns**
* -> the reason for this is that to **`insert data into the middle of a clustered index`** (out of sequential order) causes SQL Server to make room for the data by rearranging the cluster

* => this is so called **`Index Fragmentation`**
* => if we want a table with a **uniqueidentifier** data type as a **primary key** we need to change that index to a **non-clustered index**
* => non-clustered indexes don't reorder the data as rows are inserted to the table, so they don't have the **performance impact of a clustered index on inserts of non-sequential data**

## Solution 1: Mark Primary Key as 'Non-Clustered'
* -> if we want a table with a **uniqueidentifier** data type as a **primary key** we need to change that index to a **`non-clustered index`** then we need to pick a **`clustered index`** for the table also
* -> common approach is add another column of data type **`datetime`** non null
* => ensure that the data for the row is **inserted at the end of the table data**, there is **no rearranging of the cluster** - best performance for inserts

* -> another good choice is choosing **`a column that reflects the ordering of the table in the majority of the select statements`** (tức là ta có 1 câu SELECT thường dùng sắp xếp dựa trên order của 1 column cụ thể)
* => even though it hinders performance to insert a non-sequential record into the cluster you will get a performance benefit when you call your data
```r
// For example,
// -> if you have a table called categories
// -> and you have an integer column called "ordervalue"
// -> and you always call the table with an SELECT … ORDER BY [ordervalue]
// => then making ordervalue the clustered index makes sense
```

## Solution 2: Sequential GUIDs
* -> **generates unique sequential uniqueidentifier** - **`NEWSEQUENTIALID`** - GUIDs are generated partial based on the network card of the computer
* => can successfully have a uniqueidentifier as a primary key column and use that **`primary key column the required clustered index`**
* => but still if **`privacy`** is a concern, do not use this function because it is **possible to guess the value of the next generated GUID** and **access data associated with that GUID**

```cs
// using EF Core to generate sequential GUIDs by "SQL Server's NEWSEQUENTIALID()"
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()"); // Use SQL Server function for sequential GUIDs
    }
}
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Sequential GUID as PK
    public Guid Id { get; set; }  

    public string Name { get; set; }
}

// generate sequential GUIDs at the application level
public static class SequentialGuidGenerator
{
    public static Guid NewSequentialGuid()
    {
        byte[] guidArray = Guid.NewGuid().ToByteArray();
        DateTime now = DateTime.UtcNow;
        byte[] timestamp = BitConverter.GetBytes(now.Ticks);

        // Overwrite the first 8 bytes with the timestamp
        Array.Copy(timestamp, timestamp.Length - 2, guidArray, guidArray.Length - 2, 2);
        return new Guid(guidArray);
    }
}

public class Product
{
    [Key]
    public Guid Id { get; set; } = SequentialGuidGenerator.NewSequentialGuid();
}
```

## Azure SQL
* -> NEWSEQUENTIALID() function isn't support for SQL Azure
* -> if we try to use it you will get this error: _Built-in function 'NEWSEQUENTIALID' is not supported in this version of SQL Server_

==================================================================================
# 'integer' datatype vs 'IDENTITY' constraint

## integer
* -> dễ đánh index nên là query performance tốt hơn;
* -> debug và quản lý dễ hơn
* -> lưu int chỉ tốn 4 bytes (_trong khi Guid đến 16 bytes can cause serious performance and storage implications if not careful_)

## IDENTITY
* -> informs SQL Server to **`auto increment`** the **numeric value** within the specified column **anytime a new record is INSERTED**
* -> although it accepts two arguments of the numeric **seed** (where the values will begin from) and the **`increment`** (mỗi lần tăng thêm bao nhiêu đơn vị);
* -> however, these values are **`typically not specified with the IDENTITY constraint`** and instead are left as defaults - **`both default to 1`**

```sql
CREATE TABLE books (
  id              INT           NOT NULL    IDENTITY    PRIMARY KEY,
  title           VARCHAR(100)  NOT NULL,
  primary_author  VARCHAR(100),
);
```

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
