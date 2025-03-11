* chưa hiểu ví dụ về việc câu query sử dụng index hay không là sao
* cần ví dụ về Composite indexes và If a query filters by an expression over a column 
* ý cuối của index đéo hiểu gì cả

* AsAsyncEnumerable

* The current implementation of split queries executes a roundtrip for each query. We plan to improve this in the future, and execute all queries in a single roundtrip

* Ưu nhược điểm của Streaming và Buffering ?

* a retrying execution strategy

* how to use View and Function in EntityFramework

===============================================================================
> **Efficient Querying** - making your queries faster and and pitfalls users typically encounter

# Use 'indexes' properly

## 'Index' important
* -> **`the main deciding factor in whether a query runs fast or not`** is whether it will **properly utilize `indexes` where appropriate**
* -> databases are typically used to hold large amounts of data, and queries which **`traverse entire tables`** are typically **sources of serious performance issues** 

## 'Index' in Entity Framework
* -> as a general rule, there isn't any special EF knowledge to **using indexes** or **diagnosing performance issues related to them** 
* -> general database knowledge related to indexes is just **`as relevant to EF applications as to applications not using EF`**

## Detect 'Indexing' issue
* -> **Indexing issues aren't easy to spot**, because it **isn't immediately obvious `whether a given query will use an index or not`**
* -> a good way to spot indexing issues is to **`first pinpoint a slow query`**, and then **`examine its query plan via our database's favorite tool`**
* -> the **`query plan`** displays **whether the query traverses the entire table, or uses an index**

* _For example:_
```cs
// Matches on start, so uses an index (on SQL Server)
var posts1 = await context.Posts.Where(p => p.Title.StartsWith("A")).ToListAsync();
// Matches on end, so does not use the index
var posts2 = await context.Posts.Where(p => p.Title.EndsWith("A")).ToListAsync();
```

## Best practices
* -> while indexes **`speed up queries`**, they also **`slow down updates`** since they **need to be kept up-to-date**
* => avoid defining indexes which aren't needed
* => and consider using **`index filters`** to **limit the index to a subset of the rows**, thereby reducing this overhead

* -> **`composite indexes`** can speed up queries which **filter on multiple columns**, but they can **`also speed up queries which don't filter on all the index's columns`** - depending on ordering
* => For example, an index on columns A and B speeds up queries filtering by A and B as well as queries filtering only by A, but it does not speed up queries only filtering over B

* -> if **a query filters by an expression over a column** (e.g. price / 2), **`a simple index cannot be used`**
* => however, we can define a **`stored persisted column`** for our expression, and **`create an index`** over that
* => some databases also support **`expression indexes`**, which can be **directly used to speed up queries filtering by any expression**

* -> **different databases allow indexes to be configured in various ways**, and in many cases **`EF Core providers expose these via the Fluent API`**
* => for example, the SQL Server provider allows us to configure whether an index is clustered, or set its fill factor


# 'Project' only properties we need

## Problem
* -> EF Core makes it very easy to **query out entity instances**, and then **use those instances in code**
* -> however, querying entity instances can **`frequently pull back more data than necessary from our database`** Consider the following:

```cs
// Example: Although this code only actually needs each Blog's "Url" property, the entire Blog entity is fetched 
await foreach (var blog in context.Blogs.AsAsyncEnumerable())
{
    Console.WriteLine("Blog: " + blog.Url);
}
```

```sql
-- unneeded columns are transferred from the database:
SELECT [b].[BlogId], [b].[CreationDate], [b].[Name], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
```

## Solution
* -> this can be optimized by using **`Select`** to tell EF which **columns to project out**

```cs
await foreach (var blogName in context.Blogs.Select(b => b.Url).AsAsyncEnumerable())
// if we need to project out more than one column, 
// project out to a "C# anonymous type" with the properties we want
{
    Console.WriteLine("Blog: " + blogName);
}
```

```sql
-- the resulting SQL pulls back only the needed columns:
SELECT [b].[Url]
FROM [Blogs] AS [b]
```

## Note
* -> this technique is very useful for **`read-only queries`**, but things get more complicated if we **need to update the fetched entities**, 
* -> since **`EF's change tracking only works with entity instances`**
* => however, it's possible to **perform updates without loading entire entities** by **`attaching a modified entity instance`** and telling EF which **`properties have changed`**
* => but that is a more advanced technique that **may not be worth it**


# Limit the resultset size

## Problem
* -> by default, **a query returns all rows that matches its filters**
* -> since the **number of rows returned depends on actual data in our database**, it's impossible to know how much data will be loaded from the database, 
* _how much **`memory`** will be taken up by the results,_
* _and how much **`additional load`** will be generated when processing these results (e.g. by sending them to a user browser over the network)_

* => crucially, **test databases frequently contain little data**, so that everything works well while testing, but **`performance problems`** suddenly appear when the query starts running on real-world data and many rows are returned

```cs
var blogsAll = await context.Posts
    .Where(p => p.Title.StartsWith("A"))
    .ToListAsync();
```

## Solution
* -> as a result, it's usually worth giving thought to **limiting the number of results**
* => at a minimum, our **UI could show a message indicating that `more rows may exist` in the database** (and allow retrieving them in some other manner). 
* => a full-blown solution would implement **`pagination`**, where our **UI only shows a certain number of rows at a time**, and **allow users to advance to the next page** as needed

```cs
var blogs25 = await context.Posts
    .Where(p => p.Title.StartsWith("A"))
    .Take(25)
    .ToListAsync();
```

# Efficient pagination
* -> "Pagination" refers to **`retrieving results in pages, rather than all at once`**; 
* -> this is typically done for **large resultsets**, where a user interface is shown that **allows the user to navigate to the next or previous page of the results**

* => a common way to implement pagination with databases is to use the **`Skip`** and **`Take`** operators (_**OFFSET** and **LIMIT** in SQL_) - while this is an **intuitive implementation**, it's also **`quite inefficient`**

* => for pagination that allows moving one page at a time (as opposed to jumping to arbitrary pages), consider using **`keyset pagination`** instead

# Avoid 'cartesian explosion' when loading related entities

## Problem
* -> in **relational databases**, **all related entities are loaded** by introducing **`JOINs in single query`**
* -> if a typical blog has multiple related posts, rows for these posts will duplicate the blog's information
* => this **duplication** leads to the so-called **`cartesian explosion`** problem
* => as more **`one-to-many relationships`** are loaded, the amount of duplicated data may grow and adversely affect the **`performance of our application`**

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId], [p].[PostId]
```

## Solution
* -> EF allows avoiding this effect via the use of **`split queries`**, which **load the related entities via separate queries**


# Load related entities eagerly when possible

## Scenarios 1:
* -> when dealing with related entities, we **`usually know in advance what we need to load`** 
* _a typical example would be loading a certain set of Blogs, along with all their Posts_
* => in these scenarios, it is always better to use **`eager loading`**, so that EF can fetch all the required data in one roundtrip
* => the filtered **`include`** feature also allows us to **limit which related entities we'd like to load**, while **keeping the loading process eager** and therefore doable in a single roundtrip:

## Scenarios 2:
* -> in other scenarios, we may **`not know which related entity we're going to need`** before we get its principal entity
* _for example, when loading some Blog, we may need to consult some other data source - possibly a webservice - in order to know whether we're interested in that Blog's Posts_
* => in these cases, **`explicit or lazy loading`** can be used to fetch related entities separately, and populate the Blog's Posts navigation

## Beware of lazy loading
* -> **`Lazy loading`** often **seems like a very useful way to write database logic**, since EF Core automatically loads related entities from the database as they are **accessed by our code**
* => this **`avoids loading related entities that aren't needed`** (like **`explicit loading`**), and seemingly frees the programmer from having to deal with related entities altogether
* => however, lazy loading is particularly prone for **`producing unneeded extra roundtrips`** which can **`slow the application`**
* => a typical problem is so called the **`N+1 problem`**, and it can **`cause very significant performance issues`**

```cs
foreach (var blog in await context.Blogs.ToListAsync())
{
    foreach (var post in blog.Posts)
    {
        Console.WriteLine($"Blog {blog.Url}, Post: {post.Title}");
    }
}
```
```sql
-- This seemingly innocent piece of code iterates through all the blogs and their posts, printing them out
-- Turning on EF Core's statement logging reveals the following:
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [b].[BlogId], [b].[Rating], [b].[Url]
      FROM [Blogs] AS [b]
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (5ms) [Parameters=[@__p_0='1'], CommandType='Text', CommandTimeout='30']
      SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Title]
      FROM [Post] AS [p]
      WHERE [p].[BlogId] = @__p_0
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[@__p_0='2'], CommandType='Text', CommandTimeout='30']
      SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Title]
      FROM [Post] AS [p]
      WHERE [p].[BlogId] = @__p_0
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[@__p_0='3'], CommandType='Text', CommandTimeout='30']
      SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Title]
      FROM [Post] AS [p]
      WHERE [p].[BlogId] = @__p_0

... and so on
```

## Summary
* -> depending on our specific scenario, it may be **`more efficient`** to **just always load all Posts**, rather than to **execute the additional roundtrips and selectively get only the Posts you need**
* -> also lazy loading makes it **`extremely easy to inadvertently trigger the N+1 problem`**, it is recommended to avoid it
* => **Eager or explicit loading** make it **`very clear in the source code when a database roundtrip occurs`**
* =>  in some cases, it may also be useful to **avoid cartesian explosion effects** by using **`split queries`**

## Solution Example
* -> assuming we're going to need all of the blogs' posts, it makes sense to use **eager loading** here instead; we can use the "Include" operator to perform the loading, 
* -> but since we only need the Blogs' URLs (and we should only load what's needed), so we'll use a **projection** instead
```cs
// this will make EF Core fetch all the Blogs - along with their Posts - in a single query
await foreach (var blog in context.Blogs.Select(b => new { b.Url, b.Posts }).AsAsyncEnumerable())
{
    foreach (var post in blog.Posts)
    {
        Console.WriteLine($"Blog {blog.Url}, Post: {post.Title}");
    }
}
```


# Buffering and streaming
* -> **`Buffering`** refers to **loading all our query results into memory**
* -> **`Streaming`** means that EF hands the application **a single result each time**, **never containing the entire resultset in memory**

## Mechanism
* -> in principle, the **memory requirements** of a **`streaming query`** are **`fixed`** - they are the same whether the query returns 1 row or 1000
* -> a **`buffering query`**, on the other hand, **`requires more memory the more rows are returned`**; 

* => for **queries that result large resultsets**, this can be an **`important performance factor`**

## When query is 'buffering' or 'streaming'?
* -> whether a query buffers or streams depends on **`how it is evaluated`**

```cs
// ToList and ToArray cause the entire resultset to be buffered:
var blogsList = await context.Posts.Where(p => p.Title.StartsWith("A")).ToListAsync();
var blogsArray = await context.Posts.Where(p => p.Title.StartsWith("A")).ToArrayAsync();

// Foreach streams, processing one row at a time:
await foreach (var blog in context.Posts.Where(p => p.Title.StartsWith("A")).AsAsyncEnumerable())
{
    // ...
}

// AsAsyncEnumerable also streams, allowing you to execute LINQ operators on the client-side:
var doubleFilteredBlogs = context.Posts
    .Where(p => p.Title.StartsWith("A")) // Translated to SQL and executed in the database
    .AsAsyncEnumerable()
    .Where(p => SomeDotNetMethod(p)); // Executed at the client on all database results
```

## Conclusion
* -> if our queries return **just a few results**, then we probably don't have to worry about this
* -> however, if your query might return **`large numbers of rows`**, it's worth giving thought to **`streaming instead of buffering`**

## Internal buffering by EF
* -> in **certain situations**, **`EF will itself buffer the resultset internally`**, **REGARDLESS of how you evaluate our query**
* _the two cases where this happens are:_
* -> when **`a retrying execution strategy`** is in place - this is done to make sure the **same results are returned if the query is retried later**s
* -> when **`split query`** is used, the **resultsets of all but the last query are buffered** - unless MARS (Multiple Active Result Sets) is enabled on SQL Server (_this is because it is usually impossible to have multiple query resultsets active at the same time_)

* -> note that this **`internal buffering occurs in addition to any buffering we cause via LINQ operators`**
* _For example, if we use ToList on a query and a retrying execution strategy is in place, the resultset is **loaded into memory twice**: once internally by EF, and once by ToList()_


# Tracking, no-tracking and identity resolution

## Default
* -> **`EF tracks entity instances by default`**, so that changes on them are **detected and persisted when SaveChanges is called**
* -> **`identity resolution`** - another effect of tracking queries is that EF **detects if an instance has already been loaded for our data** and **will automatically return that tracked instance rather than returning a new one**

## Performance perspective of "changing tracking"
* -> **`EF internally maintains a dictionary of tracked instances`**
* => when **new data is loaded**, EF checks the dictionary to see **if an instance is already tracked for that entity's key (`identity resolution`)**
* => the dictionary maintenance and lookups **take up some time when loading the query's results**

* -> **before handing a loaded instance to the application**, **`EF snapshots that instance`** and keeps the snapshot internally
* => when **SaveChanges** is called, the **`application's instance is compared with the snapshot`** to discover the changes to be persisted
* => the **`snapshot takes up more memory`**, and the **`snapshotting process itself takes time`**; it's sometimes possible to specify different, possibly more efficient snapshotting behavior via **`value comparers`**
* => or to use **`change-tracking proxies`** to **bypass the snapshotting process altogether** (though that comes with its own set of **`disadvantages`**)

## No Tracking
* -> in **`read-only scenarios`** where **changes aren't saved back to the database**, the above overheads can be avoided by using **`no-tracking queries`**
* -> however, since no-tracking queries **`do not perform identity resolution`** - means **a database row which is referenced by multiple other loaded rows will be materialized as different instances**

```cs
// To illustrate, assume we are loading a large number of "Posts" from the database, as well as the "Blog" referenced by each "Post"
// -> If 100 Posts happen to reference the same Blog, a tracking query detects this via "identity resolution", and all Post instances will refer the same de-duplicated Blog instance
// -> A "no-tracking query", in contrast, duplicates the same Blog 100 times - and application code must be written accordingly
```

## Efficient "updating" with 'change tracking'
* -> Finally, it is possible to **`perform updates without the overhead of change tracking`**, 
* -> by utilizing a **`no-tracking query`** and then **`attaching the returned instance to the context`**, **`specifying which changes are to be made`**
* => this **transfers the burden of change tracking `from EF to the user`** - should only be attempted **`if the change tracking overhead has been shown to be unacceptable`** via profiling or benchmarking.


# Using SQL queries

## Why using it
* -> in some cases, **more optimized SQL exists for our query, which EF does not generate**
* -> this can happen when the **`SQL construct is an extension specific to our database that's unsupported`**
* -> or simply because **`EF does not translate to it yet`**

* => in these cases, writing SQL by hand can provide a **`substantial performance boost`**

## Note before using
* -> **Raw SQL** should generally be **`used as a last resort`**
* -> after making sure that **`EF can't generate the SQL we want`**, and when **`performance is important enough for the given query to justify it`**
* => using raw SQL brings **considerable maintenance disadvantages**

## Implement
* -> **`use SQL queries directly in our query`**, e.g. via **`FromSqlRaw`**
* => EF even lets us **compose over the SQL with regular LINQ queries**, allowing us to **`express only a part of the query in SQL`**
* => this is a good technique when the SQL only needs to be used in **a single query in our codebase**

* -> define a **`user-defined function (UDF)`**, and then **call that from our queries**
* => note that **EF allows UDFs to return full resultsets** - these are known as **`table-valued functions (TVFs)`** 
* => and also allows mapping a DbSet to a function, making it look just like just another table

* -> define a **`database view`** and **query from it in our queries**
* => note that unlike functions, views cannot accept parameters


# Asynchronous programming
* -> as a **general rule**, in order for **our application to be scalable**, it's important to **`always use asynchronous APIs rather than synchronous one`** (_e.g. SaveChangesAsync rather than SaveChanges_)
* -> **`synchronous APIs block the thread for the duration of database I/O`**, **increasing the need for threads and the number of thread context switches that must occur**

===============================================================================
> Efficient Updating

# Batching
* -> EF Core helps **minimize roundtrips** by **`automatically batching together all updates in a single roundtrip`**

## How EF handle Batching
* -> **`the number of statements that EF batches in a single roundtrip`** depends on the **`database provider`** being used
* -> _For example, performance analysis has shown batching to be generally less efficient for **SQL Server** when **less than 4 statements are involved**_
* -> _similarly, the benefits of batching degrade **after around 40 statements** for **SQL Server**_
* => so EF Core will by **`default only execute up to 42 statements in a single batch`**, and **`execute additional statements in separate roundtrips`**

## Example
```cs
var blog = await context.Blogs.SingleAsync(b => b.Url == "http://someblog.microsoft.com");
blog.Url = "http://someotherblog.microsoft.com";
context.Add(new Blog { Url = "http://newblog1.microsoft.com" });
context.Add(new Blog { Url = "http://newblog2.microsoft.com" });
await context.SaveChangesAsync();

// -> the above loads a blog from the database, changes its URL, and then adds two new blogs;
// -> to apply this, two SQL INSERT statements and one UPDATE statement are sent to the database
// -> Rather than sending them one by one, as Blog instances are added, EF Core tracks these changes internally,
// -> and executes them in a single roundtrip when SaveChanges is called
```

## Options for user 
* -> users can also **`tweak these thresholds to achieve potentially higher performance`**
* -> but **`benchmark carefully before modifying these`**
```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        @"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True",
        o => o
            .MinBatchSize(1)
            .MaxBatchSize(100));
}
```

# Use 'ExecuteUpdate' and 'ExecuteDelete' when relevant (EF Core 7)

## Update performance issue

```cs
// assume we want to give all our employees a raise. A typical implementation for this in EF Core would look like the following:

foreach (var employee in context.Employees)
{
    employee.Salary += 1000;
}
await context.SaveChangesAsync();
```

* _while this is perfectly valid code, let's analyze what it does from a performance perspective:_

* -> a database roundtrip is performed, to load all the relevant employees; note that this **brings all the Employees'row data to the client, even if only the salary will be needed**
* -> EF Core's change tracking creates **snapshots when loading the entities**, and then compares those snapshots to the instances to find out which properties changed
* -> typically, a second database roundtrip is performed to save all the changes (note that some database providers split the changes into multiples roundtrips)
* -> although this batching behavior is far better than doing a roundtrip for each update, EF Core still **sends an UPDATE statement per employee, and the database must execute each statement separately**

## Solution
* -> the **UPDATE** by using "ExecuteUpdateAsync" and "ExecuteDeleteAsync" methods **`performs the entire operation in a single roundtrip`**
* -> **`without loading or sending any actual data to the database`**, and **`without making use of EF's change tracking machinery`**, which imposes an additional overhead
```cs
await context.Employees.ExecuteUpdateAsync(s => s.SetProperty(e => e.Salary, e => e.Salary + 1000));
```
```sql
-- This sends the following SQL statement to the database:
UPDATE [Employees] SET [Salary] = [Salary] + 1000;
```

## Older version
* _if we're using an older version of EF Core which doesn't yet support ExecuteUpdate and ExecuteDelete,_
* _or want to execute a complex SQL statement which isn't supported by these methods, we can still use a SQL query to perform the operation_
```sql
context.Database.ExecuteSql($"UPDATE [Employees] SET [Salary] = [Salary] + 1000");
```

===============================================================================
> Advanced Performance
> https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cexpression-api-with-constant

# DbContext pooling

## Performance perspective
* -> A DbContext is generally a light object: creating and disposing one doesn't involve a database operation, and most applications can do so without any noticeable impact on performance. However, each context instance does set up various internal services and objects necessary for performing its duties, and the overhead of continuously doing so may be significant in high-performance scenarios. For these cases, EF Core can pool your context instances: when you dispose your context, EF Core resets its state and stores it in an internal pool; when a new instance is next requested, that pooled instance is returned instead of setting up a new one. Context pooling allows you to pay context setup costs only once at program startup, rather than continuously

## DbContext pooling vs Database Connection pooling
* -> note that **context pooling** is orthogonal to **database connection pooling**, which is managed at a lower level in the database driver

* => _tức là 2 thằng này là 2 separate/independent mechanism that work at **different levels** in our application (Even if we dispose of a DbContext, the underlying database connection may still be pooled and reused by another DbContext)_
* -> **DbContext Pooling** - is reusing **`DbContext instances`** that managed by **`Entity Framework Core`**
* -> **Database Connection Pooling*** - **`actual database connections (e.g., TCP connections to SQL Server)`** that managed by the **`database driver (e.g., ADO.NET for SQL Server)`**
