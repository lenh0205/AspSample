======================================================================
## Database performance
* -> Indexing
* -> Database Replication
* -> Database Sharding


======================================================================
## SQL Performance Tuning
* -> is the process of **optimizing SQL queries** to improve the **`speed and efficiency of database operations`**
* -> involves various techniques to **optimize the execution of queries**, **manage system resources more effectively**, and **ensure that the database responds quickly to user requests**

### SQL Process
* https://docs.oracle.com/en/database/oracle/oracle-database/21/tgsql/sql-processing.html#GUID-14D902DA-5CAC-4892-BE8B-880A4F8A6914
* https://www.geeksforgeeks.org/sql-query-processing/

### Major Factors Affecting SQL Speed (computation and execution time in SQL)
* -> **`Table Size`**: Larger tables with millions of rows can slow down query performance if the query hits a large number of rows.
* -> **`Joins`**: The use of complex joins, especially when joining multiple tables, can significantly affect query execution time.
* -> **`Aggregations`**: Queries that aggregate large datasets require more processing time and resources.
* -> **`Concurrency`**: Simultaneous queries from multiple users can overwhelm the database, leading to slow performance.
* -> **`Indexes`**: Proper indexing speeds up data retrieval but, when misused, can lead to inefficiencies

### SQL query performance
* -> **`SELECT fields instead of using SELECT *`**
- chỉ SELECT những cột cần thiết, không nên SELECT *
- Selecting unnecessary columns increases memory usage and network traffic, slowing performance
```sql
SELECT * FROM Employees;

-- replace with:
SELECT EmployeeID, FirstName, LastName FROM Employees;
```

* -> **`Avoid SELECT DISTINCT`**
- DISTINCT forces sorting or hashing, which can be expensive
- a better approach is using GROUP BY, JOIN conditions, or subqueries to refine results
```sql
SELECT DISTINCT FirstName, LastName,
State FROM GeeksTable;

-- replace with:
SELECT  FirstName, LastName,
State FROM GeeksTable WHERE State IS NOT NULL;
```

* -> **`Use INNER JOIN Instead of WHERE for Joins`**
```sql
SELECT GFG1.CustomerID, GFG1.Name, GFG1.LastSaleDate
FROM GFG1, GFG2
WHERE GFG1.CustomerID = GFG2.CustomerID

-- replace with:
SELECT GFG1.CustomerID, GFG1.Name, GFG1.LastSaleDate
FROM GFG1 
INNER JOIN GFG2
ON GFG1.CustomerID = GFG2.CustomerID
```

* -> **`Use WHERE Instead of HAVING`**
- HAVING is applied after aggregation, so filtering earlier using WHERE minimizes data before aggregation
```sql
SELECT GFG1.CustomerID, GFG1.Name, GFG1.LastSaleDate
 FROM GFG1 INNER JOIN GFG2
ON GFG1.CustomerID = GFG2.CustomerID
GROUP BY GFG1.CustomerID, GFG1.Name
HAVING GFG2.LastSaleDate BETWEEN "1/1/2019" AND "12/31/2019"

-- replace with:
SELECT GFG1.CustomerID, GFG1.Name, GFG1.LastSaleDate
FROM GFG1 INNER JOIN GFG2
ON GFG1.CustomerID = GFG2.CustomerID
WHERE GFG2.LastSaleDate BETWEEN "1/1/2019" AND "12/31/2019"
GROUP BY GFG1.CustomerID, GFG1.Name
```

* -> **`Limit Wildcards to the End of a Search Term`**
- Indexes work left to right, so using % at the beginning forces a full table scan.
- If you must search within strings (%No%), consider full-text search (FTS) or indexing solutions
```sql
SELECT City FROM GeekTable WHERE City LIKE ‘%No%’

-- replace with:
SELECT City FROM GeekTable WHERE City LIKE ‘No%’ 
```

* -> **`Use LIMIT for Sampling Query Results`**
- Good practice for debugging/testing queries.
- Be aware that in some databases (SQL Server), LIMIT is replaced by TOP, and in Oracle, it's FETCH FIRST N ROWS
```sql
SELECT GFG1.CustomerID, GFG1.Name, GFG1.LastSaleDate
FROM GFG1
INNER JOIN GFG2
ON GFG1.CustomerID = GFG2.CustomerID
WHERE GFG2.LastSaleDate BETWEEN "1/1/2019" AND "12/31/2019"
GROUP BY GFG1.CustomerID, GFG1.Name
LIMIT 10
```

* -> **`Run Queries During Off-Peak Hours`**
- Scheduling heavy queries reduces impact on production.
- If real-time analytics is needed, consider read replicas, indexed views, or caching mechanisms.

* -> **`Create Small Batches of Data for Deletion and Updation`**
- in case if there will be a rollback, you will avoid losing or killing your data
- also enhances concurrency, other operations can continue processing unaffected data while small batches are being modified
- deleting or updating large amounts of data in bulk can cause performance issues and long locks
```sql
DELETE FROM Orders WHERE OrderDate < '2020-01-01';

-- replace with small batches to prevents long-running transactions and excessive locking 
WHILE 1=1
BEGIN
    DELETE TOP (1000) FROM Orders WHERE OrderDate < '2020-01-01';
    IF @@ROWCOUNT = 0 BREAK;
END
```

* -> **`Use CASE instead of UPDATE`**
- Using CASE inside an UPDATE can help apply conditional changes in a single scan rather than running multiple UPDATE statements
```sql
UPDATE Employees SET Status = 'Active' WHERE LastLogin >= '2023-01-01';
UPDATE Employees SET Status = 'Inactive' WHERE LastLogin < '2023-01-01';

-- replace with:
UPDATE Employees
SET Status = CASE 
    WHEN LastLogin >= '2023-01-01' THEN 'Active'
    ELSE 'Inactive'
END;
```

* -> **`Use Temp Tables`** 
- thay vì join 1 bảng lớn, ta sẽ extract thành 1 bảng tạm nhỏ as frequently accessed data để reducing repeated table scans, computational load when performning complex joins on large datasets
```sql
SELECT o.OrderID, c.CustomerName  
FROM Orders o  
JOIN Customers c ON o.CustomerID = c.CustomerID  
WHERE c.Region = 'West';

-- replace with:
SELECT CustomerID INTO #TempCustomers FROM Customers WHERE Region = 'West';

SELECT o.OrderID, c.CustomerName  
FROM Orders o  
JOIN #TempCustomers c ON o.CustomerID = c.CustomerID;
```

* -> **`Avoid Negative Searches`**
- Queries that use **NOT IN**, **NOT LIKE**, or **<>** slow down our DB performance a lot
```sql
SELECT * FROM Customers WHERE NOT Country = 'USA';

-- replace with:
SELECT * FROM Customers WHERE Country IN ('Canada', 'UK', 'Germany');
--or
SELECT c.CustomerID FROM Customers c  
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID  
WHERE o.CustomerID IS NULL;
```

* -> **`No Need to Count Everything in the Table`**
- EXISTS(SELECT 1 FROM dbo.T1) is faster than COUNT(*), as it stops searching after finding the first match
```sql
DECLARE @CT INT;
SET @CT = (SELECT COUNT(*) FROM Orders WHERE Status = 'Pending');
IF @CT > 0
BEGIN
    PRINT 'Orders Pending';
END

-- replace with:
IF EXISTS (SELECT 1 FROM Orders WHERE Status = 'Pending')
BEGIN
    PRINT 'Orders Pending';
END
```

* -> **`Avoid Using Globally Unique Identifiers (GUIDs)`**
- GUIDs (NEWID()) are large and unordered, which can fragment indexes and slow performance.
- Use IDENTITY (auto-increment) or SEQUENTIAL GUIDs (NEWSEQUENTIALID()) when unique values are needed in an ordered manner
```sql
CREATE TABLE Orders (
    OrderID UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    OrderDate DATETIME
);

-- replace with:
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME
);
```

* -> **`Avoid Using Triggers`**
- using triggers results in lock many tables until the trigger completes the cycle. 
- we can split the data into several transactions to lock up certain resources; this will help us in making our transaction faster
```sql
CREATE TRIGGER trg_UpdateStock  
ON Orders AFTER INSERT  
AS  
BEGIN  
    UPDATE Products SET Stock = Stock - 1  
    FROM Products p JOIN inserted i ON p.ProductID = i.ProductID;  
END

-- replace with:
CREATE PROCEDURE ProcessOrder(@ProductID INT)  
AS  
BEGIN  
    UPDATE Products SET Stock = Stock - 1 WHERE ProductID = @ProductID;  
END
```

* -> **`Avoid Using ORM`**
- ORMs is useful for rapid development and maintaining business logic; however, it may generate inefficient queries that my cause a bad performance in our daily encounters
- instead of completely avoiding ORMs, optimize ORM-generated queries by using **stored procedures**, **tuning queries**, and **leveraging lazy/eager loading** wisely

* -> **`Avoid Using DISTINCT If Not Necessary`**
- instead of using DISTINCT, ensure that duplicates are eliminated at the source by properly structuring joins and conditions
```sql
SELECT DISTINCT CustomerID FROM Orders;

-- replace with:
SELECT CustomerID FROM Orders GROUP BY CustomerID;
```

* -> **`Use Fewer Cursors`**
- Cursors process one row at a time, leading to slow performance and locking issues
- Use set-based operations (JOIN, CASE, MERGE, CTE) instead of cursors whenever possible
```sql
DECLARE cur CURSOR FOR SELECT OrderID FROM Orders;
OPEN cur;
FETCH NEXT FROM cur INTO @OrderID;
WHILE @@FETCH_STATUS = 0  
BEGIN  
    -- Process each order  
    FETCH NEXT FROM cur INTO @OrderID;  
END  
CLOSE cur;
DEALLOCATE cur;

-- replace with:
UPDATE Orders SET Status = 'Processed' WHERE OrderDate < '2023-01-01';
```

* -> **`Avoid using 'subqueries'`**
```sql
SELECT * FROM customers WHERE customer_id IN (SELECT customer_id FROM orders WHERE order_date >= DATEADD(day, -30, GETDATE()));

-- this query will be faster than the previous query:
SELECT DISTINCT c.* FROM customers c JOIN orders o ON c.customer_id = o.customer_id WHERE o.order_date >= DATEADD(day, -30, GETDATE());
```

* -> **`using stored procedure`**
- khi ta gửi 1 raw SQL queries from the application, câu query dài có thể ảnh hưởng tới traffic
- và đồng thời database sẽ phải đọc câu query này để đưa ra execution plan rồi mới thực thi đc
- stored procedure thì khác, khi được lưu DB sẽ biết nó đc parsed and optimized thế nào, nên có thể thực thi ngay lập tức

* -> **`'Index' tuning`**
