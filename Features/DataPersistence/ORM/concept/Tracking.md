===============================================================================
# Tracking queries
* -> by default, **queries that return entity types** are **`tracking`**;
* -> **`a tracking query`** means any **changes to entity instances are persisted by "SaveChanges"**
```cs
var blog = await context.Blogs.SingleOrDefaultAsync(b => b.BlogId == 1);
blog.Rating = 5;
await context.SaveChangesAsync();
```

* -> when **`the results are returned in a tracking query`**, EF Core **checks if the entity is already in the context**
* -> if EF Core finds an **existing entity**, **`then the same instance is returned`** (_if the entity isn't found in the context, EF Core creates a new entity instance and attaches it to the context_)
* => which can potentially use **less memory** and **`be faster than a no-tracking query`**
```cs
using (var context = new AppDbContext())
{
    // First query - tracking enabled by default
    var emp1 = context.Employees.FirstOrDefault(e => e.Id == 1);
    
    // Second query for the same entity
    var emp2 = context.Employees.FirstOrDefault(e => e.Id == 1);

    Console.WriteLine(object.ReferenceEquals(emp1, emp2)); // Output: True => same instance 
}
```

* -> EF Core **`doesn't overwrite current and original values of the entity's properties in the entry with the database values`**
```cs
using (var context = new AppDbContext())
{
    // First, fetch an entity and modify its property
    var emp = context.Employees.FirstOrDefault(e => e.Id == 1);
    emp.Name = "Updated Name"; // Modify the entity

    // Fetch again (tracking mode)
    var empAgain = context.Employees.FirstOrDefault(e => e.Id == 1);

    Console.WriteLine(empAgain.Name); // Output: "Updated Name", not the database value
}
```

* -> **`query results don't contain any entity which is added to the context but not yet saved to the database`**
```cs
using (var context = new AppDbContext())
{
    var newEmp = new Employee { Id = 999, Name = "New Employee" };
    context.Employees.Add(newEmp);

    // Query for all employees
    var employees = context.Employees.ToList();

    Console.WriteLine(employees.Any(e => e.Id == 999)); // Output: False
}
```

===============================================================================
# No-tracking queries
* -> **no-tracking queries*8 are useful when the results are used in a **`read-only scenario`**
* -> they're **`generally quicker to execute`** because there's **no need to set up the change tracking information**
* => if the **entities retrieved from the database don't need to be updated**, then a no-tracking query should be used
* => a no-tracking query also give **`results based on what's in the database disregarding any local changes or added entities`**

## Apply
```cs
//  set no-tracking on individual query
var blogs = await context.Blogs
    .AsNoTracking()
    .ToListAsync();
```

```cs
// or the default tracking behavior can be changed at the context instance level:
context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

// usage:
var blogs = await context.Blogs.ToListAsync();
```

```cs
// configuring the default tracking behavior (if we find yourself changing the tracking behavior for many queries)
// => this makes all your queries no-tracking by default
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFQuerying.Tracking;Trusted_Connection=True;ConnectRetryCount=0")
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // this one
}

// but still, we can enable tracking for specific queries by using ".AsTracking()"
using (var context = new AppDbContext())
{
    // This query will use tracking (overrides the default NoTracking behavior)
    var trackedEmp = context.Employees.AsTracking().FirstOrDefault(e => e.Id == 1);

    // This query will still use NoTracking (default behavior)
    var untrackedEmp = context.Employees.FirstOrDefault(e => e.Id == 1);
}
```

===============================================================================
# Identity resolution

## When a 'no-tracking' query is less efficient than a 'tracking' query
* -> since a tracking query uses the change tracker, EF Core does **`identity resolution`** in a tracking query
* -> when **materializing (retrieves raw data from the database then populating an C# entity object)** an entity, EF Core returns the same entity instance from the change tracker if it's already being tracked
* => if the **result contains the same entity multiple times**, **`the same instance is returned for each occurrence`**
```cs
using (var context = new MyDbContext())
{
    var orderItems = context.OrderItems
        .Include(oi => oi.Order) // Include the Order for each item
        .ToList();

    var firstItemOrder = orderItems[0].Order;
    var secondItemOrder = orderItems[1].Order;

    Console.WriteLine(ReferenceEquals(firstItemOrder, secondItemOrder)); // True
}
```

* -> **no-tracking queries** **`don't use the change tracker`** and **`don't do identity resolution`**
* => return a new instance of the entity even when the same entity is contained in the result multiple times

## AsNoTrackingWithIdentityResolution - combine 'tracking' and 'no-tracking' in the same query
* -> that is, we can have a **`no-tracking query`**, which **`does identity resolution`** in the results
* -> just like "AsNoTracking" queryable operator, we've added another operator **`AsNoTrackingWithIdentityResolution<TEntity>(IQueryable<TEntity>)`** (_there's also associated entry added in the **QueryTrackingBehavior** enum_)
* => when the query to use identity resolution is configured with no tracking, **`a stand-alone change tracker is used in the background`** when generating query results so **`each instance is materialized only once`**
* => since this change tracker is different from the one in the context, **the results are not tracked by the context**
* => after the query is enumerated fully, the change tracker goes out of scope and garbage collected as required.

```cs
var blogs = await context.Blogs
    .AsNoTrackingWithIdentityResolution()
    .ToListAsync();
```
