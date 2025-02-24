=============================================================
# Overview

## 'ADO.NET' vs 'EntityFramework' vs 'Dapper'
* -> _ADO.NET_ is a **`low-level tool`** - means that it **provides fine-grained control over database operations** but **not developer-friendly** (_vì sẽ cần tự viết rất nhiều code dài dòng lặp đi lặp lại_)
* -> we can other low level layer like **`ODBC`** but **ADO.NET is the more .NET-friendly alternative**
* -> as **applications became more complex**, the need for **higher abstraction levels** led to the development of **`ORMs and micro-ORMs`** 
* -> in .NET, we have **EntityFramework (ORM)** and  **Dapper (micro-ORM)** that is **`built on top of ADO.NET`** (_nên là các .NET app hiện tại ta chỉ thấy chúng sử dụng 2 thằng này_)

## 'ADO.NET' vs 'Dapper'
* _in theory it can't be faster than well written ADO.NET code (however, most people don't write well written ADO.NET code; Dapper adds better caching for queries, connections, and object properties)_
* _if we're going to be mapping literally ANYTHING from the DB into an object dynamically more than once then Dapper is faster_
* _we can do pretty much anything in Dapper we would want with ADO.NET_

* **`Object mapping and boilerplate code`** 
* -> "ADO.NET" require us to **manually create SqlConnection, SqlCommand, SqlDataReader** and **loop through the results to map them to objects**
* -> "Dapper" simplifies this by **providing extension methods like .Query<T>() and .Execute()** (_it will automatically maps query results to C# objects with specific T type_)
```cs
// -----> ADO.NET
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    var command = new SqlCommand("SELECT Id, Name FROM Users WHERE Id = @Id", connection);
    command.Parameters.AddWithValue("@Id", 1);
    var reader = command.ExecuteReader();

    User user = null;
    if (reader.Read())
    {
        user = new User
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1)
        };
    }
}

// -----> Dapper
using (var connection = new SqlConnection(connectionString))
{
    // maps columns to properties in the "User" class 
    var user = connection.QueryFirstOrDefault<User>(
        "SELECT Id, Name FROM Users WHERE Id = @Id", new { Id = 1 });
}
```

* **`SQL Control and Performance`**
* -> both "ADO.NET" and "Dapper" allow us to **write raw SQL** so we could have a fine-grained control over database operations
* -> means that we get **`better performance and flexibility`** compare to an ORM (_because a full ORM frameworks may generate unefficient SQL query statement_)

```cs
// cơ chế của Dapper
// -> just straightforward execution of user-supplied queries and result mapping (not do many things like ORM)
// -> to optimize performance, Dapper generates and caches the materialization code (mapping query results to objects) using IL-emit
// -> it also prepares parameter-handling logic the same way
// -> it simply retrieves and invokes cached delegate instances, making execution highly efficient

// => since database latency, query execution, and network overhead are much higher than Dapper's minimal processing cost, any performance impact is negligible
```

## EF vs Dapper
* -> EntityFramework is a full-fledged ORM include work with databases in an object-oriented manner, tracking changes, code-first development, migrations, and automatic schema generation and managing the database schema, query generation, Handles Relationships automatically, ...
* -> this makes ORMs more flexible but more expensive
* => ideal for projects where **`rapid development`** and a higher level of abstraction are more critical
* => it's a great fit for applications with **`complex data models and relationships`**

* -> Dapper offers more control over the SQL queries generated; it can be beneficial when you need to optimize queries for performance or execute complex operations

=============================================================
# ADO.NET (ActiveX Data Objects for .NET)
* ->  is part of the **`.NET Framework`**, provides a set of "classes" and "interfaces" that allow .NET applications to interact with databases (or XML files)

```cs
string connectionString = "Data Source=server;Initial Catalog=database;Integrated Security=True";
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    // protect against SQL injection by using parameterized queries
    string query = "SELECT * FROM Customers WHERE Country = @Country";
    SqlCommand command = new SqlCommand(query, connection);
    command.Parameters.AddWithValue("@Country", "INDIA");
    SqlDataReader reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"{reader["CustomerID"]}, {reader["CompanyName"]}, {reader["Country"]}");
    }
}
```

=============================================================
# Entity Framework Core (EF Core)
* -> is a **`high-level ORM tool`** built **`on top of ADO.NET`**, that allows .NET applications to interact with databases
* -> provides a set of "classes" and "APIs" that **abstract the database operations** (_making it easier for developers to work with databases_)

## Features
* ->  provides several features, such as **automatic schema migration**, **query translation**, and **change tracking**
* -> also supports **LINQ**, which **`allows developers to write queries in C# instead of SQL`**

```cs
string connectionString = "Data Source=server;Initial Catalog=database;Integrated Security=True";
using (var context = new MyDbContext(connectionString))
{
    var customers = context.Customers.Where(c => c.Country == "INDIA").ToList();
    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.CustomerID}, {customer.CompanyName}, {customer.Country}");
    }
}
```

## Dapper
* -> is a **`micro-ORM`** built **`on top of ADO.NET`**, designed to be **`lightweight, fast and efficient`** to work with databases (_means it doesn't have some of the features provided by EF Core_)
* => ideal for scenarios where **`performance is critical`** and developers want **`fine-grained control over the database operations`**

```cs
string connectionString = "Data Source=server;Initial Catalog=database;Integrated Security=True";
using (var connection = new SqlConnection(connectionString))
{
    var customers = connection.Query<Customer>("SELECT * FROM Customers WHERE Country = @Country", new { Country = "INDIA" });
    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.CustomerID}, {customer.CompanyName}, {customer.Country}");
    }
}
```

## LINQ to SQL
* -> deprecated

## Compare
* **`Performance`**: 
* -> **Dapper is faster ADO.NET is faster EF**; 
* -> however, ADO.NET may offers more **`control over the performance of queries`** as it allows developers to write SQL queries directly 

* **`Ease of Use`**:
* -> **EF Core** is the clear winner - provide **`a high-level API`** that abstracts the database operations and supports LINQ to **`write queries in C# instead of SQL`**
* -> **Dapper** is also easy to use **`but requires developers to write SQL queries`**

* **`Features`**:
* -> **EF Core** is the clear winner
* -> **Dapper** require to self-implement some features
* -> **ADO.NET** doesn't provide as many features as EF Core

* **`Flexibility`**
* -> **Dapper** is the most flexible because it allows developers to **`write SQL queries`** and **`map the results to any class or structure`**
* -> **EF Core** is less flexible because it requires developers to define classes that map to database tables
* -> **ADO.NET** is also less flexible because it requires developers to write more code to map the results to classes or structures