https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries

==============================================================================
# Performance issues with single queries
* -> when working against **relational databases**, EF **`loads related entities`** by introducing **`JOINs into a single query`**
* -> while JOINs are quite standard when using SQL, they can create **significant performance issues if used improperly**
* -> note **`one-to-one related entities`** are always loaded via JOINs in the same query, as it **has no performance impact**

## Cartesian explosion

### Example 1:
* -> in this example, since both "Posts" and "Contributors" are **`collection navigations`** of "Blog" - they're at the **`same level`**
* -> relational databases return a cross product: each row from "Posts" is joined with each row from "Contributors"
* -> this means that if a given blog has 10 posts and 10 contributors, the database returns 100 rows for that single blog
* => this phenomenon - sometimes called **`cartesian explosion`**
* => can cause **`huge amounts of data to unintentionally get transferred to the client`**, especially as **`more sibling JOINs are added to the query`**;
* => this can be a major **performance issue** in database applications

```cs
var blogs = await ctx.Blogs
    .Include(b => b.Posts)
    .Include(b => b.Contributors)
    .ToListAsync();
```
```sql
SELECT [b].[Id], [b].[Name], [p].[Id], [p].[BlogId], [p].[Title], [c].[Id], [c].[BlogId], [c].[FirstName], [c].[LastName]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
LEFT JOIN [Contributors] AS [c] ON [b].[Id] = [c].[BlogId]
ORDER BY [b].[Id], [p].[Id]
```

### Example 2:
* -> note that cartesian explosion **`does not occur when the two JOINs aren't at the same level`**

* _In this case, a single row is returned for each comment that a blog has (through its posts), and a cross product does not occur_
```cs
var blogs = await ctx.Blogs
    .Include(b => b.Posts)
    .ThenInclude(p => p.Comments)
    .ToListAsync();
```
```sql
SELECT [b].[Id], [b].[Name], [t].[Id], [t].[BlogId], [t].[Title], [t].[Id0], [t].[Content], [t].[PostId]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
LEFT JOIN [Comment] AS [c] ON [p].[Id] = [c].[PostId]
ORDER BY [b].[Id], [t].[Id]
```

## Data duplication
* -> **JOINs** can create another type of **performance issue** - data duplication
* -> it's worth noting that unlike cartesian explosion, **`the data duplication caused by JOINs isn't typically significant`**, as the **duplicated data size is negligible**;
* -> this typically is something to worry about only if we have **`big columns`** (e.g. binary data, or a huge text) in our principal table

### Example:
```cs
// only loads a single collection navigation
var blogs = await ctx.Blogs
    .Include(b => b.Posts)
    .ToListAsync();
```
```sql
SELECT [b].[Id], [b].[Name], [b].[HugeColumn], [p].[Id], [p].[BlogId], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
ORDER BY [b].[Id]
```

* -> Examining at the projected columns, each row returned by this query contains properties from both the Blogs and Posts tables;
* -> this means that the **blog properties are duplicated for each post that the blog has**
* -> while this is usually normal and causes no issues, if the Blogs **table happens to have a very big column**, that **column would get duplicated** and **sent back to the client multiple times**
* -> this can significantly increase network traffic and adversely affect our application's performance

### Solution (but not completed):
* -> if we don't actually need the huge column, it's easy to simply not query for it; by using a **`projection`** to explicitly choose which columns we want, we can **omit big columns and improve performance**
* -> note that this is **a good idea regardless of data duplication**, so consider doing it **even when not loading a collection navigation**
* -> however, since this projects the blog to **an anonymous type**, the "blog" **`isn't tracked by EF`** and changes to it can't be saved back as usual.

```cs
var blogs = await ctx.Blogs
    .Select(b => new
    {
        b.Id,
        b.Name,
        b.Posts
    })
    .ToListAsync();
```

==============================================================================
# Split queries
* -> to work around the performance issues described above, EF allows us to **specify that a given LINQ query should be split into `multiple SQL queries`**
* -> instead of JOINs, split queries **`generate an additional SQL query for each included collection navigation`**
* _tức là với mỗi Include() nó sẽ tạo 1 câu SQL riêng biệt JOIN bảng Related với bảng gốc, thay vì chỉ một câu query sử dụng multiple JOIN_

```cs
using (var context = new BloggingContext())
{
    var blogs = await context.Blogs
        .Include(blog => blog.Posts)
        .AsSplitQuery()
        .ToListAsync();
}
```
```cs
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
ORDER BY [b].[BlogId]

SELECT [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title], [b].[BlogId]
FROM [Blogs] AS [b]
INNER JOIN [Posts] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId]
```

## Enabling split queries globally

### Default
* -> EF Core **`uses 'single query' mode`** by default in the absence of any configuration
* -> since this default behavior **may cause performance issues**, EF Core generates a warning whenever following conditions are met:
* -> EF Core **detects that the query loads multiple collections**
* -> User **hasn't configured 'query splitting' mode globally**
* -> User **hasn't used AsSingleQuery/AsSplitQuery operator on the query**

### Enable split query
* _to turn off the warning, `configure query splitting mode globally or at the query level` to an appropriate value_

```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFQuerying;Trusted_Connection=True;ConnectRetryCount=0",
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
}
```

```sql
using (var context = new SplitQueriesBloggingContext())
{
    var blogs = await context.Blogs
        .Include(blog => blog.Posts)
        .AsSingleQuery()
        .ToListAsync();
}
```

## Drawbacks
* -> while most databases guarantee **`data consistency`** for single queries, **no such guarantees exist for multiple queries**
* => if the **database is updated concurrently when executing our queries**, resulting data may not be consistent
* => we can mitigate it by **wrapping the queries in a serializable or snapshot transaction**, although doing so may create **performance issues** of its own

* -> each query currently implies an **`additional network roundtrip`** to our database
* => multiple network roundtrips can **degrade performance**, **especially where latency to the database is high** (for example, cloud services).

* -> while some databases allow consuming the results of multiple queries at the same time (SQL Server with MARS, Sqlite), **most allow only a single query to be active at any given point**
* => so all **results from earlier queries must be buffered in our application's memory** before executing later queries, which leads to **`increased memory requirements`**

* -> when including reference navigations as well as collection navigations, each one of the split queries will include joins to the reference navigations
* => this can **degrade performance**, particularly if there are **`many reference navigations`**

## Warning: using split queries with Skip/Take
When using split queries with Skip/Take on EF versions prior to 10, pay special attention to making your query ordering fully unique; not doing so could cause incorrect data to be returned. For example, if results are ordered only by date, but there can be multiple results with the same date, then each one of the split queries could each get different results from the database. Ordering by both date and ID (or any other unique property or combination of properties) makes the ordering fully unique and avoids this problem. Note that relational databases do not apply any ordering by default, even on the primary key.
