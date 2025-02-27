> thường thì ta sẽ triển khai Index ngay trong giai đoạn phát triển, thiết kế database dựa vào sự đo lường, kinh nghiệm, tầm nhìn
> database đã có dữ liệu, thì đánh index cần phải xem xét rất kỹ impact so với lợi ích

===============================================================================
# SQL Indexes
* -> an **`index`** in SQL is **`a schema object that improves the speed of data retrieval operations on a table`**
* => **speed up SELECT queries** especially on **`large datasets`**, they can **slow down data manipulation operations (INSERT, UPDATE, DELETE)**

## Mechanism
* -> works by creating **`a separate data structure that provides pointers to the rows in a table`**
* => making it faster to look up rows based on specific column values by reducing the need for full table scans

## Benifit
* -> Large Data Tables: SQL queries on tables with millions of rows can significantly slow down due to full table scans. Indexes provide a faster alternative by allowing quick access to relevant rows.
* -> Join Optimization: Indexes on columns used for joining tables (such as foreign keys) improve the performance of complex joins.
* -> Search Operations: Queries that search for specific values in a column can be sped up with indexes, reducing the time required to perform lookups.
* -> However, it is essential to be mindful of the storage cost and performance tradeoffs associated with indexes. Over-indexing can lead to unnecessary overhead, while under-indexing may slow down data retrieval

## Use Cases
* -> Wide Range of Values: Indexes are helpful when a column has a wide range of values, such as product IDs or customer names, as they speed up search operations.
* -> Non-NULL Values: Columns that don’t contain many NULL values are ideal candidates for indexing, as NULLs complicate the indexing process.
* -> Frequent Query Conditions: Indexes should be created on columns frequently used in WHERE clauses or as part of a join condition.
* -> Small Tables: Indexes are not needed for small tables as queries will likely perform well without them.
* -> Infrequent Query Use: If a column is rarely used in queries, indexing it will only add overhead.
* -> Frequently Updated Columns: Avoid indexing columns that are frequently updated, as the index will need to be updated with each change, adding overhead.

===============================================================================
> thường thì biết 3 thằng Clustered Index, Non-Clustered Index và Unique Index là oke rồi

# Creating an 'Index' in SQL Sever
* -> SQL indexes can be applied to **one or more columns** and can be either **unique or non-unique**
* -> _**unique indexes** ensure that **no duplicate values** are entered in the indexed columns, while **non-unique indexes** simply speed up queries without enforcing uniqueness_

## Clustered Index
* -> Determines the physical order of data in a table.
* -> There can be only one clustered index per table because data can be stored in only one order.
* -> Primary keys automatically create a clustered index.

```sql
CREATE CLUSTERED INDEX IX_Employees_ID ON Employees(ID);
```

## Non-Clustered Index
* -> Stores pointers to the actual data instead of sorting the table.
* -> You can have multiple non-clustered indexes per table.
* -> Best for frequently searched columns but not for every column (too many indexes slow down inserts/updates).

```sql
CREATE NONCLUSTERED INDEX IX_Employees_Department
ON Employees(Department);
```

## Unique Index
* -> Ensures values in a column are unique.
* -> Created automatically when defining a UNIQUE constraint

```sql
CREATE UNIQUE INDEX IX_Employees_Email ON Employees(Email);
```
## Composite Index
* -> index on multiple columns, useful for queries filtering by multiple conditions.
* -> order of columns matters for query optimization.
```sql
CREATE NONCLUSTERED INDEX IX_Employees_Department_Name ON Employees(Department, Name);
```

## Filtered Index
* -> improves performance for queries filtering specific values
```sql
CREATE NONCLUSTERED INDEX IX_Employees_Active ON Employees(Status) WHERE Status = 'Active';
```

## Covering Index
* -> includes additional columns to avoid extra lookups
```sql
CREATE NONCLUSTERED INDEX IX_Orders_CustomerId ON Orders(CustomerID) INCLUDE (OrderDate, TotalAmount);
```

# Removing an Index
https://www.geeksforgeeks.org/sql-indexes/

# Altering an Index

# Confirming and Viewing Indexes

# Renaming an Index

===============================================================================
> đọc thêm: https://medium.com/@murataslan1/database-indexing-855a249fd9df

# Apply Indexes in ASP.NET Core Web API Project

## Identify Performance Issues
* -> use SQL Profiler or Execution Plan (CTRL + M in SSMS) to find slow queries.
* -> check missing index recommendations in SSMS (Missing Index Details in execution plans).

## Optimize Queries Using Indexes
* -> Index WHERE and JOIN conditions, not every column.
* -> Prefer non-clustered indexes for frequently searched columns.
* -> Use composite indexes when filtering by multiple columns.
* -> Index Usage in Entity Framework & Dapper

## Example of Index Usage
```sql
# suppose your application frequently runs this query to fetch active IT employees:
SELECT EmployeeID, Name, Email
FROM Employees
WHERE Department = 'IT'
  AND Status = 'Active';
```

* -> Analyze the Query
```bash
# Filtering Columns: The query filters on Department and Status. These columns are good candidates for key columns in a composite index.
# Columns in the SELECT: The query retrieves EmployeeID, Name, and Email.
# If these columns are not part of the key, you can include them in the index as "included columns" to create a covering index.
# This means the database engine can satisfy the query entirely from the index without needing to access the full table.
```

* -> Creating the Index
```sql
# Based on the analysis, we could create the following non-clustered, covering index:
CREATE NONCLUSTERED INDEX IX_Employees_Department_Status
ON Employees (Department, Status)
INCLUDE (EmployeeID, Name, Email);

# Key Columns: Department and Status are used in the WHERE clause, so the index is built around them.
# Included Columns: EmployeeID, Name, and Email are added to the index to cover the query. With this index, SQL Server can retrieve all requested data directly from the index, bypassing the need for additional lookups.
```

* -> Applying with Entity Framework Core (Fluent API)
```cs
// configuration
// -> ensures that SQL Server creates an index when we run a database migration
modelBuilder.Entity<Employee>()
    .HasIndex(e => new { e.Department, e.Status })
    .HasDatabaseName("IX_Employees_Department_Status")
    .IncludeProperties(e => new { e.EmployeeID, e.Name, e.Email });

// usage
// -> to use the index in a LINQ query, we don’t need to reference the index explicitly
// -> the database engine automatically uses it when the query matches the indexed columns
var employees = dbContext.Employees
    .Where(e => e.Department == "IT" && e.Status == "Active")
    .Select(e => new { e.EmployeeID, e.Name, e.Email })
    .ToList();

// to Check If Index Is Used
// -> Copy the generated SQL from using .ToQueryString() in EF Core into SQL Server Management Studio (SSMS)
// -> and enable "Include Actual Execution Plan" (CTRL + M) to see if SQL Server is using your index
```

* -> Applying with Dapper
```cs
using (var connection = new SqlConnection(connectionString))
{
    var employees = connection.Query<Employee>(@"
        SELECT EmployeeID, Name, Email
        FROM Employees WITH (INDEX(IX_Employees_Department_Status))
        WHERE Department = @dept AND Status = @status;",
        new { dept = "IT", status = "Active" });
}
```
